﻿using ChatAI.Model;
using ChatAI.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OpenAI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Xps.Serialization;
using Wpf.Ui.Contracts;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using RestSharp;

namespace ChatAI
{
    public partial class MainViewModel : ObservableObject, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private string _applicationTitle = "ChatGPT - UIT"; // Rename Application Here

        [ObservableProperty]
        private ChatMessageViewModel _prompt = new ChatMessageViewModel();

        [ObservableProperty]
        private ObservableCollection<ChatViewModel> _chatList;

        [ObservableProperty]
        public ChatViewModel _selectedChat;

        [ObservableProperty]
        private ObservableCollection<ChatMessageViewModel> _chatMessageList;

        [ObservableProperty]
        private ChatMessageViewModel _currentChatMessage;

        [ObservableProperty]
        private bool _isEnabled;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private bool _isAdvance;

        public bool isAdvance
        {
            get { return _isAdvance; }
            set
            {
                if (_isAdvance != value)
                {
                    _isAdvance = value;
                    OnPropertyChanged(nameof(isAdvance));
                }
            }
        }

        [ObservableProperty]
        private String _getTopic;

        public String getTopic
        {
            get { return _getTopic; }
            set
            {
                if (_getTopic != value)
                {
                    _getTopic = value;
                    OnPropertyChanged(nameof(getTopic));
                }
            }
        }

        [ObservableProperty]
        private String _getSkill;

        public String getSkill
        {
            get { return _getSkill; }
            set
            {
                if (_getSkill != value)
                {
                    _getSkill = value;
                    OnPropertyChanged(nameof(getSkill));
                }
            }
        }

        [ObservableProperty]
        private String _getBand;

        public String getBand
        {
            get { return _getBand; }
            set
            {
                if (_getBand != value)
                {
                    _getBand = value;
                    OnPropertyChanged(nameof(getBand));
                }
            }
        }



        public Config SysConfig => _serviceProvider.GetService<Config>();

        private readonly IDialogService _dialogService;

        public MainViewModel(IServiceProvider serviceProvider, IDialogService dialogService) 
        {
            _serviceProvider = serviceProvider;
            _dialogService = dialogService;
            ChatList = new ObservableCollection<ChatViewModel>();
            ChatMessageList = new ObservableCollection<ChatMessageViewModel>();
            LoadHistory();
            LoadConfig();
            NewChat();
            this.PropertyChanged += MainViewModel_PropertyChanged;
            this.MainViewModel_PropertyChanged(null, new PropertyChangedEventArgs(nameof(SelectedChat)));
        }

        private void MainViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(SelectedChat))
            {
                if (SelectedChat != null && SelectedChat.Name == "New Chat")
                {
                    ChatMessageList = new ObservableCollection<ChatMessageViewModel>();
                    return;
                }
                if (SelectedChat != null && SelectedChat.ChatMessageList != null)
                {
                    //ChatMessageList = new ObservableCollection<ChatMessageViewModel>(SelectedChat.ChatMessageList);
                    using (var db = new LiteDatabase(@".\store.db"))
                    {
                        var chats = db.GetCollection<ChatDo>("chat");
                        ChatDo chatDo = chats.FindById(SelectedChat.Id);
                        if (chatDo != null)
                        {
                            var allMessages = chatDo.messages.Select(p =>
                                new ChatMessageViewModel
                                {
                                    Id = p.id,
                                    IsUser = p.isUser,
                                    Message = p.message,
                                    Time = p.time,
                                    ParentId = p.parentId
                                }
                            );
                            foreach (var message in allMessages)
                            {
                                if (message.IsUser)
                                {
                                    message.Result = allMessages.FirstOrDefault(p => p.ParentId == message.Id);
                                }
                            }
                            ChatMessageList = new ObservableCollection<ChatMessageViewModel>(allMessages.OrderBy(p => p.Time).ToList());
                        }
                    }
                }
                else
                {
                    String selectChatId = SelectedChat?.Id;
                    if (selectChatId == null)
                    {
                        selectChatId = "null";
                    }
                    // Show history here!
                    using (var db = new LiteDatabase(@".\store.db"))
                    {
                        var chats = db.GetCollection<ChatDo>("chat");

                        ChatDo chatDo = chats.FindById(selectChatId);
                        if (chatDo != null)
                        {
                            var allMessages = chatDo.messages.Select(p =>
                                new ChatMessageViewModel
                                {
                                    Id = p.id,
                                    IsUser = p.isUser,
                                    Message = p.message,
                                    Time = p.time,
                                    ParentId = p.parentId
                                }
                            );
                            foreach (var message in allMessages)
                            {
                                if (message.IsUser)
                                {
                                    message.Result = allMessages.FirstOrDefault(p => p.ParentId == message.Id);
                                }
                            }
                            ChatMessageList = new ObservableCollection<ChatMessageViewModel>(allMessages.OrderBy(p => p.Time).ToList());
                        }
                    }
                    //ChatMessageList = new ObservableCollection<ChatMessageViewModel>();
                }
            }
        }

        private static ChatMessage[] CreateChatPrompt(
            ChatMessageViewModel sendMessage,
            ObservableCollection<ChatMessageViewModel> messages)
        {
            var chatMessages = new List<ChatMessage>();
            foreach (var message in messages)
            {
                if (!string.IsNullOrEmpty(message.Message) && message.Result is { })
                {
                    chatMessages.Add(new ChatMessage
                    {
                        Role = "user",
                        Content = message.Message
                    });
                    chatMessages.Add(new ChatMessage
                    {
                        Role = "assistant",
                        Content = message.Result.Message
                    });
                }
            }

            chatMessages.Add(new ChatMessage
            {
                Role = "user",
                Content = sendMessage.Prompt
            }); ;

            return chatMessages.ToArray();
        }

        public void NewChat()
        {
            var newChat = new ChatViewModel
            {
                Id = Guid.NewGuid().ToString().Replace("-", ""),
                Name = "New Chat",
                Time = DateTime.Now
            };
            ChatList.Insert(0,newChat);
            SelectedChat = newChat;

            Prompt = new ChatMessageViewModel
            {
                Id = Guid.NewGuid().ToString().Replace("-", ""),
                Prompt = string.Empty,
                Send = this.Send
            };
        }

        private async Task NewAction()
        {
            NewChat();
            await Task.Yield();
        }

        private async Task Send(ChatMessageViewModel sendMessage)
        {
            //System.Windows.MessageBox.Show(getTopic + " " + getSkill + " " + getBand, "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            String userRequest = sendMessage.Prompt;
            if (isAdvance == true)
            {
                if (!string.IsNullOrEmpty(sendMessage.Prompt))
                {
                    MessageBoxCustom mb = new MessageBoxCustom("Notification", "Please turn off Advanced mode!", MessageType.Error, MessageButtons.OK);
                    mb.ShowDialog();
                    return;
                }
                if (String.IsNullOrEmpty(getTopic) || String.IsNullOrEmpty(GetSkill) || String.IsNullOrEmpty(getBand))
                {
                    MessageBoxCustom mb = new MessageBoxCustom("Notification", "Please completely fill in the fields: Topic, Skills, Band!", MessageType.Warning, MessageButtons.OK);
                    mb.ShowDialog();
                    return;
                }
                if (getSkill == "📖 Reading")
                {
                    sendMessage.Prompt = "I focus on design Academic Reading part for IELTS exams [exams].\n" +
                                        "• You act as IELTS examiner to provide a reading passages [passages] with topic [topic], including various types of questions. For training purpose, please provide the passages with different bands in IELTS [bands], from band 3 to band 9.\n" +
                                        "• Please provide one question by one for the questions following the each reading passages. " +
                                        "Then wait for my answer your question, give the feedback and clarify why other choices are incorrect, and then provide next question until finish.\n" +
                                        "• After finish reading each passages, please provide the list of academic words with short definition corresponding to the reading context."
                                        + "Let start with the follow variable values:\n"
                                        + "[exam] = \"IELTS\";"
                                        + "[topics] = " + getTopic + "\n"
                                        + "[bands] =" + getBand + "\n"
                                        + "[passages] = 1";
                }
                if (getSkill == "✎ Writing")
                {
                    sendMessage.Prompt = "I focus on scoring IELTS writing test exams [exam]. Please provide the information for IELTS writing part.\n"
                                        + "• I'll provide the information for the IELTS writing, including writing task [task], topic [topics], my taget band [band], and the writing answer [answer]\n"
                                        + "• You act as IELTS examiner to score the writing task. And then give the feedbacks and suggestions to improve the writing for the following points:\n"
                                        + "+) Task achievement;\n"
                                        + "+) Vocabulary and collocations;\n"
                                        + "+) Grammar and sentence structure;\n"
                                        + "+) Coherence and Cohesion;\n"
                                        + "+) Give me band score writing in IELTS"
                                        + "Let start with the following variable values:"
                                        + "[exam] = \"IELTS\";"
                                        + "[task] = \"1\";"
                                        + "[topics] = " + getTopic + "\n"
                                        + "[band] = " + getBand + "\n"
                                        + "[answer]: Please request me to give you an answer";
                }
                if (getSkill == "🗣️ Speaking")
                {
                    sendMessage.Prompt = "I focus on scoring IELTS Speaking test exams [exam]. Please provide the information for IELTS writing part.\n"
                                        + "• I'll provide the information for the IELTS Speaking, including Speaking task [task], topic [topics], my taget band [band]"
                                        + "• You act as IELTS examiner to score the Speaking task. Please give me a speaking related to the topic and ensure the following points:\n"
                                        + "+) Task achievement;\n"
                                        + "+) Vocabulary and collocations;\n"
                                        + "+) Grammar and sentence structure;\n"
                                        + "+) Coherence and Cohesion;\n"
                                        + "+) Give me band score writing in IELTS"
                                        + "Let start with the following variable values:"
                                        + "[exam] = \"IELTS\";"
                                        + "[task] = \"1\";"
                                        + "[topics] = " + getTopic + "\n"
                                        + "[band] = " + getBand + "\n";
                }
                if (getSkill == "👂 Listening")
                {
                    sendMessage.Prompt = "I focus on design Academic Listening part for IELTS exams [exams].\n" +
                                        "• You act as IELTS examiner to provide a Listening passages [passages] with topic [topic], including various types of questions. For training purpose, please provide the passages with different bands in IELTS [bands], from band 3 to band 9.\n" +
                                        "• Please provide five questions following the each Listening passages. " +
                                        "Then wait for my answer your question, give the feedback and clarify why other choices are incorrect\n" +
                                        "• After finish Listening each passages, please provide the list of academic words with short definition corresponding to the Listening context."
                                        + "Let start with the follow variable values:\n"
                                        + "[exam] = \"IELTS\";"
                                        + "[topics] = " + getTopic + "\n"
                                        + "[bands] =" + getBand + "\n"
                                        + "[passages] = 1";
                }
            }

            if(string.IsNullOrEmpty(sendMessage.Prompt))
            {
                return;
            }

            if(sendMessage.ParentId == null)
            {
                var last = ChatMessageList.LastOrDefault();
                if(last != null)
                {
                    sendMessage.ParentId = last.Id;
                }
            }

            var chatPrompts = CreateChatPrompt(sendMessage, ChatMessageList);
            IsEnabled = false;
            sendMessage.IsUser = true;
            sendMessage.IsSent = true;
            ChatMessageViewModel? promptMessage;
            ChatMessageViewModel? resultMessage = null;
            if (sendMessage.Result is { })
            {
                promptMessage = sendMessage;
                resultMessage = sendMessage.Result;
            }
            else
            {
                promptMessage = new ChatMessageViewModel();
                promptMessage.Id = Guid.NewGuid().ToString().Replace("-", "");
                promptMessage.ParentId = sendMessage.ParentId;
                promptMessage.Send = this.Send;
                ChatMessageList.Add(promptMessage);
            }

            var prompt = sendMessage.Prompt;
            promptMessage.Message = prompt;
            if (isAdvance == true)
            {
                promptMessage.Message = "\r\nTopic: " + getTopic + "," +
                                        "\r\nSkill: " + getSkill + "," +
                                        "\r\nBand: " + getBand + " || " +
                                        "This is an automated message from the system. Please don't request too quickly to avoid crashing while processing the request. Thank you!";

            }
            promptMessage.Prompt = "";
            promptMessage.IsSent = true;
            promptMessage.IsUser = true;
            promptMessage.Time = DateTime.Now;
            sendMessage.Prompt = "";
            CurrentChatMessage = promptMessage;
            var chatServiceSettings = new ChatServiceSettings
            {
                Model = "gpt-3.5-turbo",
                Messages = chatPrompts,
                Suffix = null,
                Temperature = 0.7m,
                MaxTokens = 512,
                TopP = 1.0m,
                Stop = null,
                ApiKey = SysConfig.ApiKey,
                Proxy = SysConfig.Proxy,
            };
            IChatStreamService chatService = _serviceProvider.GetService<IChatStreamService>();
            bool first = true;
            ChatMessageViewModel? lastMessage = null;
            resultMessage = new ChatMessageViewModel
            {
                Id = Guid.NewGuid().ToString().Replace("-", ""),
                IsSent = false,
            };
            resultMessage.Send = this.Send;
            resultMessage.Time = DateTime.Now;
            await chatService.GetResponseDataAsync(chatServiceSettings,
                (responseStr) => 
                {
                    if (first)
                    {
                        ChatMessageList.Add(resultMessage);
                    }
                    if ("[DONE]".Equals(responseStr))
                    {
                        resultMessage.IsSent = true;
                        resultMessage.IsUser = false;
                        resultMessage.Prompt = prompt;
                        resultMessage.ParentId = promptMessage.Id;
                    }
                    else
                    {
                        resultMessage.Message += responseStr;
                    }
                    if (ChatMessageList.LastOrDefault() == resultMessage)
                    {
                        resultMessage.IsSent = false;
                    }
                    lastMessage = resultMessage;
                    first = false;
                });
            if (SelectedChat != null)
            {
                if (SelectedChat.Name == "New Chat")
                {
                    if (isAdvance == true)
                    {
                        if (getSkill == "📖 Reading")
                        {
                            SelectedChat.Name = "📖";
                        }
                        if (getSkill == "✎ Writing")
                        {
                            SelectedChat.Name = "✎";
                        }
                        if (getSkill == "🗣️ Speaking")
                        {
                            SelectedChat.Name = "🗣️";
                        }
                        if (getSkill == "👂 Listening")
                        {
                            SelectedChat.Name = "👂";
                        }
                        SelectedChat.Name += GetBand + " - " + GetTopic;
                    } else
                    {
                        var YOUR_API_KEY = "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";
                        var userInput = "What is the main content of \"Conversation Content\"? Please only answer in the form: \"The main content is...\" Conversation content: " + userRequest;
                        var client = new RestClient("https://api.openai.com/v1");
                        var request = new RestRequest("engines/text-davinci-003/completions", Method.Post);
                        request.AddHeader("Content-Type", "application/json");
                        request.AddHeader("Authorization", $"Bearer {YOUR_API_KEY}");
                        request.AddJsonBody(new { prompt = userInput, max_tokens = 4000, temperature = 0 });
                        var response = client.Execute(request);
                        var responseData = JObject.Parse(response.Content);
                        string output = responseData["choices"][0]["text"].ToString();
                        string searchPhrase = "The main content is";
                        int index = output.IndexOf(searchPhrase);
                        if (index != -1)
                        {
                            // Adding the length of the search phrase to get the substring after it
                            string result = output.Substring(index + searchPhrase.Length).Trim();
                            result = result.TrimStart();
                            string name = char.ToUpper(result[0]) + result.Substring(1);
                            // System.Windows.Forms.MessageBox.Show(name);
                            SelectedChat.Name = name;
                        }
                        //System.Windows.Forms.MessageBox.Show(output);
                    }
                    //SelectedChat.Name = resultMessage.Message; // Replace this with the desired new name
                }
            }
            //set resultMessage to current
            CurrentChatMessage = lastMessage;
            //set resultMessage to promptMessage's result
            promptMessage.Result = lastMessage;
            Save(SelectedChat, ChatMessageList);
            IsEnabled = true;
        }

        private async Task Save(ChatViewModel chatViewModel, ObservableCollection<ChatMessageViewModel> chatMessageViewModels)
        {
            using(var db = new LiteDatabase(@".\store.db"))
            {
                var chats = db.GetCollection<ChatDo>("chat");

                ChatDo exists = chats.FindOne(p => p.Id.Equals(chatViewModel.Id));
                if (exists != null) 
                {
                    var messages = new List<MessageDo>();
                    foreach (var messageViewModel in chatMessageViewModels)
                    {
                        MessageDo message = new MessageDo();
                        message.id = messageViewModel.Id;
                        message.parentId = messageViewModel.ParentId;
                        message.chatId = chatViewModel.Id;
                        message.isUser = messageViewModel.IsUser;
                        message.role = messageViewModel.IsUser ? "user" : "assistant";
                        message.message = messageViewModel.Message;
                        message.time = messageViewModel.Time;
                        messages.Add(message);
                    }
                    exists.messages = messages;
                    chats.Update(exists);
                }
                else
                {
                    var newChat = new ChatDo
                    {
                        Id = chatViewModel.Id,
                        name = chatViewModel.Name,
                        time = DateTime.Now
                    };
                    var messages = new List<MessageDo>();
                    foreach (var messageViewModel in chatMessageViewModels)
                    {
                        MessageDo message = new MessageDo();
                        message.id = messageViewModel.Id;
                        message.parentId = messageViewModel.ParentId;
                        message.chatId = chatViewModel.Id;
                        message.isUser = messageViewModel.IsUser;
                        message.role = messageViewModel.IsUser ? "user" : "assistant";
                        message.message = messageViewModel.Message;
                        message.time = messageViewModel.Time;
                        messages.Add(message);
                    }
                    newChat.messages = messages;
                    chats.Insert(newChat);
                }
            }
        }

        private void SaveConfig()
        {
            using (var db = new LiteDatabase(@".\store.db"))
            {
                var config = db.GetCollection<Config>("config");
                Config exists = config.FindOne(p => p.Id.Equals(SysConfig.Id));
                if(exists != null)
                {
                    config.Update(SysConfig);
                }
                else
                {
                    config.Insert(SysConfig);
                }
            }
        }

        private void LoadConfig()
        {
            using (var db = new LiteDatabase(@".\store.db"))
            {
                var config = db.GetCollection<Config>("config");
                Config exists = config.FindAll().FirstOrDefault();
                if (exists != null)
                {
                    SysConfig.Id = Guid.NewGuid().ToString().Replace("-","");
                    SysConfig.ApiKey = exists.ApiKey;
                    SysConfig.Proxy = exists.Proxy;
                }
            }
            SysConfig.Id = Guid.NewGuid().ToString().Replace("-", "");
        }

        private void LoadHistory()
        {
            using (var db = new LiteDatabase(@".\store.db"))
            {
                var chats = db.GetCollection<ChatDo>("chat");
                IEnumerable<ChatDo> chatAll = chats.FindAll();
                ChatList = new ObservableCollection<ChatViewModel>(
                        chatAll.Select(p =>
                        {
                            ChatViewModel chatViewModel = new ChatViewModel { Id = p.Id, Name = p.name, Time = p.time };
                            var allMessages = p.messages.Select(p =>
                                   new ChatMessageViewModel
                                   {
                                       Id = p.id,
                                       IsUser = p.isUser,
                                       Message = p.message,
                                       Time = p.time,
                                       ParentId = p.parentId
                                   }
                               );
                            foreach (var message in allMessages)
                            {
                                if (message.IsUser)
                                {
                                    message.Result = allMessages.FirstOrDefault(p => p.ParentId == message.Id);
                                }
                            }
                            chatViewModel.ChatMessageList = allMessages.ToList();
                            return chatViewModel;
                        }
                    ).OrderByDescending(p => p.Time).ToList());
                SelectedChat = ChatList?.FirstOrDefault();
            }
        }

        [RelayCommand]
        public void Setting()
        {
            var apiKeyBak = SysConfig.ApiKey;
            var proxyBak = SysConfig.Proxy;
            var rootDialog = _dialogService.GetDialogControl();
            rootDialog.Title = "Setting";
            rootDialog.ButtonLeftName = "OK";
            rootDialog.ButtonRightName = "Cancel";
            rootDialog.ButtonLeftClick += (_, _) => {
                SaveConfig();
                rootDialog.Hide();
            };
            rootDialog.ButtonRightClick += (_, _) => {
                SysConfig.ApiKey = apiKeyBak;
                SysConfig.Proxy = proxyBak;
                rootDialog.Hide();
            };
            rootDialog.Show();
        }

        public void DeleteChat(ChatViewModel chatViewModel)
        {
            if (chatViewModel.Name == "New Chat")
            {
                System.Windows.MessageBox.Show("Can not delete empty chat, you need chat something before delete!", "Delete Chat", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SelectedChat == ChatList[0])
            {
                if (chatViewModel.ChatMessageList != null && chatViewModel.ChatMessageList.Any())
                {
                    ChatList[0].ChatMessageList.Clear();
                }
                ChatList[0].Name = "New Chat";
                ChatMessageList = new ObservableCollection<ChatMessageViewModel>();
                SaveChatDeletion(ChatList[0]);
                return;
            }
            if (chatViewModel != null)
            {
                // Remove the chat from ChatList
                ChatList.Remove(chatViewModel);

                // Remove the associated messages from ChatMessageList
                foreach (var messageViewModel in chatViewModel.ChatMessageList)
                {
                    ChatMessageList.Remove(messageViewModel);
                }
                // Optionally, save your changes to persistent storage if needed
                SaveChatDeletion(chatViewModel);

            }

            // Select another chat (e.g., the first one) if there are remaining chats
            if (ChatList.Count > 0)
            {
                SelectedChat = ChatList[0];
            }
            else
            {
                // If there are no more chats, you may want to create a new chat
                NewChat();
            }
        }

        // Add a method to save chat deletion to persistent storage
        private void SaveChatDeletion(ChatViewModel chatViewModel)
        {
            using (var db = new LiteDatabase(@".\store.db"))
            {
                var chats = db.GetCollection<ChatDo>("chat");
                chats.Delete(chatViewModel.Id); // Use the chat's ID as the query
            }
        }


        public void Dispose()
        {
            this.PropertyChanged -= MainViewModel_PropertyChanged;
        }

    }
}
