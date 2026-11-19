using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly UsersRep _usersRep = new();

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

            if (_usersRep.GetByEmail(email) != null)
            {
                ShowError("User with this email already exists.");
                return;
            }

            var newUser = new UsersTable(0, name, email, pass, "user", DateTime.Now);
            if (!_usersRep.Add(newUser, isAlreadyHashed: false))
            {
                ShowError("Failed to register user.");
                return;
            }

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