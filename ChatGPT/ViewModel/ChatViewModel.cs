using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace ChatAI
{
    public class ChatViewModel : ObservableObject
    {
        public string Id { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public DateTime Time { get; set; }

        public string Avatar { get; set; }

        public List<ChatMessageViewModel> ChatMessageList { get; set; } 

    }
}
