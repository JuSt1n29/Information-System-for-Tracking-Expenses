using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly UsersRep _usersRep = new UsersRep();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void OnRegisterClick(object sender, RoutedEventArgs e)
        {
            string name = NameInput.Text ?? "";
            string email = EmailInput.Text ?? "";
            string password = PasswordInput.Text ?? "";

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ErrorText.Text = "Заповніть всі поля!";
                ErrorText.IsVisible = true;
                return;
            }

            try
            {
                // Хешируем пароль перед сохранением в базу данных
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                var newUser = new UsersTable(
                    id: 0, 
                    name: name, 
                    email: email, 
                    password_hash: hashedPassword, // Сохраняем хэш вместо чистого текста
                    role: "user", 
                    created_at: DateTime.Now
                );

                _usersRep.Add(newUser);

                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                ErrorText.Text = "Помилка: " + ex.Message;
                ErrorText.IsVisible = true;
            }
        }

        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}