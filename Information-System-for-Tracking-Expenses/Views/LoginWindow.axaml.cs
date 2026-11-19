using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class LoginWindow : Window
    {
        private readonly UsersRep _usersRep = new();

        public LoginWindow()
        {
            InitializeComponent();
            _usersRep.EnsureDefaultAdminExists();
        }

        private void OnLoginClick(object? sender, RoutedEventArgs e)
        {
            string login = EmailBox.Text?.Trim() ?? "";
            string password = PasswordBox.Text ?? "";

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ShowError("Please enter both email/username and password.");
                return;
            }

            var user = _usersRep.Authenticate(login, password);
            if (user == null)
            {
                ShowError("Invalid credentials.");
                return;
            }

            var mainWindow = new MainWindow(user);
            mainWindow.Show();
            Close();
        }

        private void OnOpenRegisterClick(object? sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            Close();
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.IsVisible = true;
        }
    }
}