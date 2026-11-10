using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class LoginWindow : Window
    {
        private readonly UsersRep _usersRep = new UsersRep();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            string email = EmailInput.Text ?? "";
            string password = PasswordInput.Text ?? "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ErrorText.Text = "Заповніть всі поля!";
                ErrorText.IsVisible = true;
                return;
            }

            try
            {
                // Получаем пользователя по email из базы данных
                var user = _usersRep.GetByEmail(email);

                // Проверяем существование пользователя и валидность хэша пароля через BCrypt
                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.password_hash))
                {
                    // Успешный вход — открываем главное окно
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    ErrorText.Text = "Невірний email або пароль!";
                    ErrorText.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                ErrorText.Text = "Помилка підключення: " + ex.Message;
                ErrorText.IsVisible = true;
            }
        }

        private void OnOpenRegisterClick(object sender, RoutedEventArgs e)
        {
            // Переход на окно регистрации
            RegisterWindow regWindow = new RegisterWindow();
            regWindow.Show();
            this.Close();
        }
    }
}