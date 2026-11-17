using Avalonia.Controls;
using Avalonia.Interactivity;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void OnLoginClick(object? sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text?.Trim() ?? "";
            string password = PasswordBox.Text ?? "";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowError("Please enter both email and password.");
                return;
            }

            // Здесь можно подключить проверку через UsersRep (поиск пользователя в БД)
            // Если всё верно — открываем MainWindow и закрываем окно логина:
            var mainWindow = new MainWindow();
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