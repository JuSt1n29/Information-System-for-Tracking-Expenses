using Avalonia.Controls;
using Avalonia.Interactivity;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void OnRegisterClick(object? sender, RoutedEventArgs e)
        {
            string name = NameBox.Text?.Trim() ?? "";
            string email = EmailBox.Text?.Trim() ?? "";
            string pass = PasswordBox.Text ?? "";
            string confirm = ConfirmPasswordBox.Text ?? "";

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                ShowError("All fields are required.");
                return;
            }

            if (pass != confirm)
            {
                ShowError("Passwords do not match.");
                return;
            }

            // Здесь можно добавить сохранение нового пользователя в таблицу users через UsersRep
            // После успешной регистрации возвращаемся на окно логина:
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        private void OnBackToLoginClick(object? sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.IsVisible = true;
        }
    }
}