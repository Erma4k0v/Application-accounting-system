using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SqlClient;

namespace RepairRequests
{
    public partial class autorization : Window
    {
        private const string connectionString = @"Server=DESKTOP-1DTLLG9\SQLEXPRESS; Database=RepairRequestsDB; Integrated Security=True; TrustServerCertificate=True";

        public autorization()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Loaded += AuthorizationWindow_Loaded;
        }

        private void AuthorizationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoginTextBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            PasswordBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            RegistrationLoginTextBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            RegistrationPasswordBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            ConfirmPasswordBox.PreviewKeyDown += TextBox_PreviewKeyDown;

            LoginButton.Click += LoginButton_Click;
            RegistrationButton.Click += RegistrationButton_Click;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                (sender as FrameworkElement)?.MoveFocus(request);
            }
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            string login = RegistrationLoginTextBox.Text;
            string password = RegistrationPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string role = ((ComboBoxItem)RoleComboBox.SelectedItem)?.Content?.ToString();

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword) || string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Please fill in all fields and select a role.");
            }
            else if (password != confirmPassword)
            {
                MessageBox.Show("Password and confirm password do not match.");
            }
            else
            {
                RegisterUser(login, password, role);
            }
        }

        private (bool, string, string, int) CheckUserAuthentication(string login, string password)
        {
            bool isAuthenticated = false;
            string role = string.Empty;
            string userLogin = string.Empty;
            int userId = 0; // Используем UserID вместо ClientID

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT UserID, UserLogin, Role FROM Users WHERE UserLogin = @Login AND UserPassword = @Password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userId = reader.GetInt32(0); // Получаем UserID из базы данных
                            userLogin = reader.GetString(1);
                            role = reader.GetString(2);
                            isAuthenticated = true;
                        }
                    }
                }
            }
            return (isAuthenticated, role, userLogin, userId);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in all fields.");
            }
            else
            {
                var (isAuthenticated, role, userLogin, userId) = CheckUserAuthentication(login, password);
                if (isAuthenticated)
                {
                    MessageBox.Show($"Authorization successful. Welcome, {login}!");
                    Window nextWindow;
                    switch (role.ToLower())
                    {
                        case "admin":
                            nextWindow = new AdminWindow();
                            break;
                        case "master":
                            nextWindow = new MasterWindow(userLogin); // Передаем логин мастера
                            break;
                        case "client":
                            nextWindow = new ClientWindow(userId); // Передаем userId для ClientWindow
                            break;
                        default:
                            throw new Exception("Unknown role");
                    }
                    nextWindow.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Invalid login or password.");
                }
            }
        }

        private void RegisterUser(string login, string password, string role)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Users (UserLogin, UserPassword, UserName, Role) VALUES (@Login, @Password, @Login, @Role)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Role", role);
                    command.ExecuteNonQuery();
                }
            }
            MessageBox.Show("Registration successful.");
        }
    }
}
