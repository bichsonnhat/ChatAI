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
using System.Diagnostics;
using System.Windows.Navigation;
using System.Data.SqlClient;
using System.Data;

namespace ChatAI.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public bool IsLogin { get; private set; } = false;
        public LoginWindow()
        {
            InitializeComponent();
        }
        SqlConnection conn = new SqlConnection(@"Data Source=NHATBS;Initial Catalog=LoginChat;Integrated Security=True");
        bool isLogin = false;
        private void Login(object sender, RoutedEventArgs e)
        {
           string enteredPassword = passwordBox.Password;
           if (string.IsNullOrEmpty(enteredPassword) ) {
                MessageBox.Show("Please enter your key", "Login Unsucessful", MessageBoxButton.OK, MessageBoxImage.Warning);
           } else
           {
                try
                {
                    string query = "SELECT * FROM Login WHERE [key] = @EnteredPassword";
                    SqlCommand com = new SqlCommand(query, conn);
                    com.Parameters.AddWithValue("@EnteredPassword", enteredPassword);

                    using (SqlDataAdapter sda = new SqlDataAdapter(com))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            IsLogin = true;
                            MessageBox.Show("Login successful", "ChatGPT-UIT", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close();
                            //Environment.Exit(0);
                        }
                        else
                        {
                            MessageBox.Show("Oops! You pressed the wrong key! Try again!", "Login Unsucessful", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Error", "Login Unsucessful", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
           }
        }

    }
}
