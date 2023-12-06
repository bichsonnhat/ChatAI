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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Collections.Specialized;
using static System.Net.Mime.MediaTypeNames;
using OpenAI;
using Wpf.Ui.Contracts;
using System.Windows.Forms;
using ChatAI.View;

namespace ChatAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IWindow
    {
        public MainViewModel ViewModel { get; set; }
        public MainWindow(MainViewModel viewModel, INavigationService navigationService,
            IServiceProvider serviceProvider, ISnackbarService snackbarService,
            IDialogService dialogService)
        {
            //Wpf.Ui.Appearance.Watcher.Watch(this);

            ViewModel = viewModel;
            DataContext = this;
            LoginWindow login = new LoginWindow();
            login.ShowDialog();
            if (login.IsLogin == false)
            {
                Environment.Exit(0);
            }

            InitializeComponent();
            RootDialog.Content = RootDialog.ContentTemplate.LoadContent();
            dialogService.SetDialogControl(RootDialog);
        }


        private void DeleteChat(object sender, RoutedEventArgs e)
        {
            // Access the selected chat
            var selectedChat = ViewModel.SelectedChat;
            // Delete the selected chat
            ViewModel.DeleteChat(selectedChat);
            //ViewModel.NewChat();
        }

        private void AddChat(object sender, RoutedEventArgs e)
        {
            ViewModel.NewChat();
        }
    }
}
