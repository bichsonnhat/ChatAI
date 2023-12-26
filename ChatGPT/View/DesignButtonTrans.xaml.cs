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
            TranslateWindow translateWindow = new TranslateWindow();
            translateWindow.Show();
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
    }
}
