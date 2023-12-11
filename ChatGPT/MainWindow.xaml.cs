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
using SharpHook;
using SharpHook.Native;
using ChatWpfUI.View;

namespace ChatAI
{
    public class ShareData
    {
        // các biến được sử dụng chung bởi các views
        public static string transText = string.Empty;
        public static string langSecond = "vi";
    }
}

namespace ChatAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IWindow
    {
        public MainViewModel ViewModel { get; set; }

        // Khởi tạo các biến hook và giả lập sự kiện
        TaskPoolGlobalHook hook = new TaskPoolGlobalHook();
        EventSimulator simulator = new EventSimulator();
        // currentText dùng để tránh popup window mở sai thời điểm
        string currentText = string.Empty;

        public MainWindow(MainViewModel viewModel, INavigationService navigationService,
            IServiceProvider serviceProvider, ISnackbarService snackbarService,
            IDialogService dialogService)
        {
            //Wpf.Ui.Appearance.Watcher.Watch(this);
            System.Windows.Clipboard.Clear();

            ViewModel = viewModel;
            DataContext = this;
            LoginWindow login = new LoginWindow();
            login.ShowDialog();
            if (login.IsLogin == false)
            {
                Environment.Exit(0);
            }

            InitializeComponent();
            hook.MouseReleased += OnMouseRelease;
            hook.RunAsync();
            RootDialog.Content = RootDialog.ContentTemplate.LoadContent();
            dialogService.SetDialogControl(RootDialog);
        }
        public void OnMouseRelease(object sender, MouseHookEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                // lấy giá trị hiện tại của Clipboard để khi xử lý xong sẽ trả lại
                // string temp = await Clipboard.GetTextAsync();
                // await Clipboard.ClearAsync();

                Task.Delay(75);
                // Press Ctrl+C
                simulator.SimulateKeyPress(KeyCode.VcLeftControl);
                simulator.SimulateKeyPress(KeyCode.VcC);
                // Release Ctrl+C
                simulator.SimulateKeyRelease(KeyCode.VcC);
                simulator.SimulateKeyRelease(KeyCode.VcLeftControl);
                Task.Delay(75);

                // Lấy giá trị text đang có trong Clipboard
                string text = System.Windows.Clipboard.GetText();

                // Nếu text ko có giá trị hoặc trùng giá trị trước đó thì ko làm gì
                if (text is null || text == " " || text == currentText)
                {
                    //await Clipboard.SetTextAsync(temp);
                    return;
                }
                currentText = text;
                // gán text cho transText để đưa đi dịch
                ShareData.transText = text;
                // Mở popup window tại vị trí con chuột đang đứng
                TestTrans.Text = text;

                DesignButtonTrans designButtonTrans = new DesignButtonTrans();
                designButtonTrans.Left = e.Data.X;
                designButtonTrans.Top = e.Data.Y;
                designButtonTrans.Topmost = true;
                designButtonTrans.WindowStartupLocation = WindowStartupLocation.Manual;
                designButtonTrans.Show();
                //DesignButtonTrans.Position = 
            });
        }
        private void Window_Closed(object? sender, System.EventArgs e)
        {
            hook.Dispose();
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
