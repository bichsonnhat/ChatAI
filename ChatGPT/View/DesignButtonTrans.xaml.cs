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

namespace ChatAI.View
{
    /// <summary>
    /// Interaction logic for DesignButtonTrans.xaml
    /// </summary>
    public partial class DesignButtonTrans : Window
    {
        public DesignButtonTrans()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string input = ConvertString(ShareData.transText);
            int wordCount = CountWord(input);
            if (wordCount == 1)
            {
                WordWindow wordWindow = new WordWindow();
                wordWindow.Show();
            }
            else
            {
                TranslateWindow translateWindow = new TranslateWindow();
                translateWindow.Show();
            }
            this.Close();
        }

        private void Speaking(object sender, RoutedEventArgs e)
        {
            TextToSpeech textToSpeech = new TextToSpeech();
            textToSpeech.Show();
            this.Close();
        }

        private void Window_Deactivated(object? sender, System.EventArgs e)
        {
            this.Close();
        }

        string ConvertString(string input)
        {
            string convertInput = input.Replace(System.Environment.NewLine, "");
            return convertInput;
        }

        private int CountWord(string inputString)
        {
            int wordCount = 0;
            bool isPreviousCharSpace = true;

            foreach (char character in inputString)
            {
                if (character == ' ')
                {
                    isPreviousCharSpace = true;
                }
                else if (isPreviousCharSpace)
                {
                    wordCount++;
                    isPreviousCharSpace = false;
                }
            }
            return wordCount;
        }
    }
}
