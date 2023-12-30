using ChatAI.ViewModel;
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
    /// Interaction logic for WordWindow.xaml
    /// </summary>
    public partial class WordWindow : Window
    {
        public WordWindowViewModel WordWindowViewModel { get; set; }
        public WordWindow()
        {
            InitializeComponent();
            this.DataContext = new WordWindowViewModel();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
