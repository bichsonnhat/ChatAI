﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ChatAI.Model;
using Newtonsoft.Json.Linq;

namespace ChatAI.ViewModel
{
    public partial class WordWindowViewModel : ObservableObject
    {
        private HttpClient httpClient = new HttpClient();

        [ObservableProperty]
        public string buttonContent = "Add";

        [ObservableProperty]
        public string buttonColor = "Blue";

        [ObservableProperty]
        public bool isHasAudio = false;

        [ObservableProperty]
        public string word = "empty";

        [ObservableProperty]
        public string sound = "default";

        [ObservableProperty]
        public string phonetic = "muted";

        [ObservableProperty]
        public ObservableCollection<Definition> definitions = new ObservableCollection<Definition> { };

        //[ObservableProperty]
        //public Dictionary<string, ObservableCollection<Def2ex>> definitions = new Dictionary<string, ObservableCollection<Def2ex>>();

        public WordWindowViewModel()
        {
            string input = ConvertString(ShareData.transText);
            TranslateWord(input);
        }
        public void TranslateWord(string input)
        {
            // tạo link để gọi API
            string url = "https://api.dictionaryapi.dev/api/v2/entries/en/" + input;
            // gọi API và lấy kết quả trả về
            string result = httpClient.GetStringAsync(url).Result;
            JArray jsonArray = JArray.Parse(result);
            string rawResult = jsonArray.ToString();
            foreach (JToken entry in jsonArray)
            {
                // lay word
                Word = entry["word"].ToString();
                // lay cach doc
                if (entry["phonetic"] != null)
                {
                    Phonetic = entry["phonetic"].ToString();
                }
                // lay file audio
                JToken phonetics = entry["phonetics"];
                foreach (JToken phonetic in phonetics)
                {
                    if (phonetic["audio"].ToString() != "")
                    {
                        Sound = phonetic["audio"].ToString();
                        IsHasAudio = true;
                        break;
                    }
                }
                // lay phan dich nghia va dong nghia, trai nghia
                JToken meanings = entry["meanings"];
                foreach (JToken meaning in meanings)
                {
                    // mỗi item là 1 loại từ
                    Definition item = new Definition();

                    // lấy loại từ
                    item.PartOfSpeech = meaning["partOfSpeech"].ToString();

                    /*                    string partOfSpeech = meaning["partOfSpeech"].ToString();
                                        if (!Definitions.ContainsKey(partOfSpeech))
                                        {
                                            Definitions[partOfSpeech] = new ObservableCollection<Def2ex>();
                                        }*/

                    // lấy danh sách các định nghĩa và ví dụ
                    JToken definitions = meaning["definitions"];
                    foreach (JToken definition in definitions)
                    {
                        Def2ex def2ex = new Def2ex();
                        def2ex.Meaning = "Definition: " + definition["definition"].ToString();
                        if (definition["example"] != null)
                        {
                            def2ex.Example = "Example: " + definition["example"].ToString() + "\n";
                            def2ex.IsHasExample = true;
                        }
                        else
                        {
                            def2ex.Example = "Example: " + "nan";
                        }
                        item.Def2exs.Add(def2ex);
                        //Definitions[partOfSpeech].Add(def2ex);
                    }

                    // lấy từ đồng nghĩa và trái nghĩa
                    JToken synonyms = meaning["synonyms"];
                    foreach (JToken synonym in synonyms)
                    {
                        item.IsHasSynonym = true;
                        item.Synonyms.Add(synonym.ToString());
                    }
                    JToken antonyms = meaning["antonyms"];
                    foreach (JToken antonym in antonyms)
                    {
                        item.IsHasAntonym = true;
                        item.Antonyms.Add(antonym.ToString());
                    }

                    Definitions.Add(item);
                }
            }
        }

        [RelayCommand]
        public void AudioPlay()
        {
            
        }

        string ConvertString(string input)
        {
            string convertInput = input.Replace(System.Environment.NewLine, "");
            return convertInput;
        }
    }
}
