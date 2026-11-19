using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class UsersWindow : Window
    {
        private readonly UsersRep _repo = new();
        private UsersTable? _selectedUser;

        private static readonly HashSet<string> HiddenColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            "id", "password_hash"
        };

        public UsersWindow()
        {
            InitializeComponent();
            RoleComboBox.SelectedIndex = 0;
            LoadData();
        }

        private void OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName != null && HiddenColumns.Contains(e.PropertyName))
            {
                e.Cancel = true;
            }
        }

        private void LoadData()
        {
            UsersGrid.ItemsSource = null;
            UsersGrid.ItemsSource = _repo.GetAll();
            ResetForm();
        }

        private void ResetForm()
        {
            _selectedUser = null;
            UsersGrid.SelectedItem = null;

            NameBox.Text = "";
            EmailBox.Text = "";
            PasswordBox.Text = "";
            RoleComboBox.SelectedIndex = 0;

            PanelTitleText.Text = "Add New User:";
            PasswordLabel.Text = "Password *";
            AddButton.IsVisible = true;
            UpdateButton.IsVisible = false;
            DeleteButton.IsVisible = false;
            StatusText.IsVisible = false;
        }

        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (UsersGrid.SelectedItem is UsersTable user)
            {
                _selectedUser = user;
                NameBox.Text = user.name;
                EmailBox.Text = user.email;
                PasswordBox.Text = "";

                bool isAdmin = string.Equals(user.role, "admin", StringComparison.OrdinalIgnoreCase);
                RoleComboBox.SelectedIndex = isAdmin ? 1 : 0;

                PanelTitleText.Text = $"Selected User ({user.name}):";
                PasswordLabel.Text = "New Password (optional)";
                AddButton.IsVisible = false;
                UpdateButton.IsVisible = true;
                DeleteButton.IsVisible = true;
                StatusText.IsVisible = false;
            }
            else if (UsersGrid.SelectedItem == null && _selectedUser != null)
            {
                ResetForm();
            }
        }

        private string GetSelectedRole()
        {
            return (RoleComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString()?.ToLowerInvariant() ?? "user";
        }

        private void OnAddClick(object? sender, RoutedEventArgs e)
        {
            string name = NameBox.Text?.Trim() ?? "";
            string email = EmailBox.Text?.Trim() ?? "";
            string password = PasswordBox.Text ?? "";
            string role = GetSelectedRole();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
            {
                ShowStatus("Fill in Name, Email and Password.");
                return;
            }

            if (_repo.GetByEmail(email) != null)
            {
                ShowStatus("A user with this email already exists.");
                return;
            }

            var newUser = new UsersTable(0, name, email, password, role, DateTime.Now);
            if (_repo.Add(newUser, isAlreadyHashed: false))
            {
                LoadData();
            }
            else
            {
                ShowStatus("Failed to add user.");
            }
        }

        private void OnUpdateClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedUser == null) return;

            string name = NameBox.Text?.Trim() ?? "";
            string email = EmailBox.Text?.Trim() ?? "";
            string newPassword = PasswordBox.Text ?? "";
            string role = GetSelectedRole();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            {
                ShowStatus("Name and Email cannot be empty.");
                return;
            }

            bool changePassword = !string.IsNullOrEmpty(newPassword);
            var updatedUser = new UsersTable(
                _selectedUser.id,
                name,
                email,
                changePassword ? newPassword : _selectedUser.password_hash,
                role,
                _selectedUser.created_at
            );

            if (_repo.Update(updatedUser, hashNewPassword: changePassword))
            {
                LoadData();
            }
            else
            {
                ShowStatus("Failed to update user.");
            }
        }

        private void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedUser == null) return;

            if (_repo.Delete(_selectedUser))
            {
                LoadData();
            }
            else
            {
                ShowStatus("Failed to delete user.");
            }
        }

        private void OnClearClick(object? sender, RoutedEventArgs e) => ResetForm();

        private void OnRefreshClick(object? sender, RoutedEventArgs e) => LoadData();

        private void ShowStatus(string message)
        {
            StatusText.Text = message;
            StatusText.IsVisible = true;
        }
    }
}