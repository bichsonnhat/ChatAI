using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Speech.Synthesis;

namespace ChatAI.View
{
    /// <summary>
    /// Interaction logic for TextToSpeech.xaml
    /// </summary>
    public partial class TextToSpeech : Window
    {
        SpeechSynthesizer sapi = new SpeechSynthesizer();
        bool isStop = false;
        public TextToSpeech()
        {
            InitializeComponent();
            string input = ConvertString(ShareData.transText);
            engBox.Text = input;
            Closing += TextToSpeech_Closing;
        }

        private void TextToSpeech_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            sapi.Dispose();
        }

        string ConvertString(string input)
        {
            string convertInput = input.Replace(System.Environment.NewLine, "");
            return convertInput;
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(engBox.Text))
            {
                sapi.Dispose();
                sapi = new SpeechSynthesizer();
                sapi.SpeakAsync(engBox.Text);
                isStop = false;
            } else
            {
                MessageBox.Show("Text is empty!");
            }
        }

        private void Pause(object sender, RoutedEventArgs e)
        {
            if (isStop == true)
            {
                return;
            }
            if (sapi.State == SynthesizerState.Speaking)
            {
                sapi.Pause();
            }
        }

        private void Resume(object sender, RoutedEventArgs e)
        {
            if (isStop == true)
            {
                return;
            }
            if (sapi != null)
            {
                if (sapi.State == SynthesizerState.Paused)
                {
                    sapi.Resume();
                }
            }
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            if (sapi != null)
            {
                sapi.Dispose();
                isStop = true;
            }
        }

    }
}
