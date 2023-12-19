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
using LiteDB;
using System.ComponentModel;

namespace ChatAI
{
    public class ShareData
    {
        // các biến được sử dụng chung bởi các views
        public static string transText = string.Empty;
        public static string langSecond = "vi";
    }
}

public class ComboBoxItemModel
{
    public ObjectId Id { get; set; }
    public string Content { get; set; }
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
            //LoginWindow login = new LoginWindow();
            //login.ShowDialog();
            //if (login.IsLogin == false)
            //{
            //    Environment.Exit(0);
            //}

            InitializeComponent();
            hook.MouseReleased += OnMouseRelease;
            hook.RunAsync();
            RootDialog.Content = RootDialog.ContentTemplate.LoadContent();
            dialogService.SetDialogControl(RootDialog);
            using (var db = new LiteDatabase(@".\store.db"))
            {
                List<ComboBoxItemModel> items;
                var comboBoxItems = db.GetCollection<ComboBoxItemModel>("ComboBoxItems");
                items = comboBoxItems.FindAll().ToList();
                foreach (var item in items)
                {
                    Topic.Items.Add(item.Content);
                }

            }
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
                if (String.IsNullOrWhiteSpace(text) || text == currentText)
                {
                    //await Clipboard.SetTextAsync(temp);
                    return;
                }
                currentText = text;
                // gán text cho transText để đưa đi dịch
                ShareData.transText = text;
                // Mở popup window tại vị trí con chuột đang đứng
                //TestTrans.Text = text;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String getTopic = AddTopic.Text;
            if (String.IsNullOrWhiteSpace(getTopic))
            {
                System.Windows.MessageBox.Show("Please fill topic box!", "Notification",  MessageBoxButton.OK, MessageBoxImage.Warning);
            } else
            {
                using (var db = new LiteDatabase(@".\store.db"))
                {
                    ComboBoxItem newItem = new ComboBoxItem();
                    newItem.Content = getTopic;
                    Topic.Items.Add(newItem);
                    var comboBoxItems = db.GetCollection<ComboBoxItemModel>("ComboBoxItems");
                    // Insert the new item into the LiteDB collection
                    var newItemModel = new ComboBoxItemModel
                    {
                        Content = getTopic
                    };
                    comboBoxItems.Insert(newItemModel);
                }
                System.Windows.MessageBox.Show("Add your topic successful", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.IsAdvance = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ViewModel.isAdvance = false;
        }

        private void SkillChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            if (comboBox.SelectedItem != null)
            {
                ViewModel.getSkill = comboBox.SelectedItem.ToString().Remove(0, 38);
            }
            else
            {
                ViewModel.getSkill = "";
            }
        }

        private void BandChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            if (comboBox.SelectedItem != null)
            {
                ViewModel.getBand = comboBox.SelectedItem.ToString().Remove(0, 38);
            }
            else
            {
                ViewModel.getBand = "";
            }
        }

        private void TopicChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            if (comboBox.SelectedItem != null && comboBox != null)
            {
                string selectedItemString = comboBox.SelectedItem.ToString();
                if (selectedItemString.StartsWith("System.Windows.Controls.ComboBoxItem:"))
                {
                    ViewModel.getTopic = selectedItemString.Remove(0, 38);
                } else
                {
                    ViewModel.getTopic = selectedItemString;
                }
            }
            else
            {
                ViewModel.getTopic = "";
            }
        }
    }
}
