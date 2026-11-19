using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class MainWindow : Window
    {
        private readonly TransactionsRep _transactionsRep = new();
        private UsersTable? _currentUser;
        private TransactionsTable? _selectedTransaction;

        private static readonly HashSet<string> HiddenColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            "id", "account_id", "counterparty_id", "employee_id", "category_id", "user_id"
        };

        public MainWindow() : this(null)
        {
        }

        public MainWindow(UsersTable? currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            ApplyRolePermissions();
            RefreshHeaderUserDisplay();
            LoadData();
        }

        private bool IsAdmin => string.Equals(_currentUser?.role, "admin", StringComparison.OrdinalIgnoreCase);

        private void ApplyRolePermissions()
        {
            EmployeesButton.IsVisible = IsAdmin;
            UsersButton.IsVisible = IsAdmin;
        }

        private void RefreshHeaderUserDisplay()
        {
            if (_currentUser != null)
            {
                CurrentUserText.Text = $"{_currentUser.name} ({_currentUser.role})";
                LoadBitmapToImage(HeaderAvatarImage, _currentUser.avatar_path);
            }
            else
            {
                CurrentUserText.Text = "Guest";
                HeaderAvatarImage.Source = null;
            }
        }

        private static void LoadBitmapToImage(Image target, string? path)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                {
                    using var stream = File.OpenRead(path);
                    target.Source = new Bitmap(stream);
                    return;
                }
            }
            catch
            {
                // Очищаем при ошибке чтения файла
            }
            target.Source = null;
        }

        private async void OnOpenProfileClick(object? sender, RoutedEventArgs e)
        {
            if (_currentUser == null) return;

            var profileWindow = new ProfileWindow(_currentUser);
            await profileWindow.ShowDialog(this);

            if (profileWindow.UpdatedUser != null)
            {
                _currentUser = profileWindow.UpdatedUser;
                RefreshHeaderUserDisplay();
            }
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
            TransactionsGrid.ItemsSource = null;
            TransactionsGrid.ItemsSource = _transactionsRep.GetAll();

            _selectedTransaction = null;
            ActionPanel.IsVisible = false;
            StatusText.IsVisible = false;
            TransactionsGrid.SelectedItem = null;
        }

        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (TransactionsGrid.SelectedItem is TransactionsTable item)
            {
                _selectedTransaction = item;
                DocumentBox.Text = item.document_number;
                EditAmountBox.Text = item.amount.ToString(CultureInfo.InvariantCulture);
                EditDescriptionBox.Text = item.description;
                StatusText.IsVisible = false;
                ActionPanel.IsVisible = true;
            }
            else
            {
                _selectedTransaction = null;
                ActionPanel.IsVisible = false;
            }
        }

        private async void OnAddClick(object? sender, RoutedEventArgs e)
        {
            int nextId = _transactionsRep.GetNextId();
            int userId = _currentUser?.id > 0 ? _currentUser.id : 1;
            var addWindow = new AddTransactionWindow(nextId, userId);

            await addWindow.ShowDialog(this);

            if (addWindow.CreatedTransaction != null)
            {
                if (_transactionsRep.Add(addWindow.CreatedTransaction))
                {
                    LoadData();
                }
            }
        }

        private void OnUpdateClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedTransaction == null) return;

            if (!TryParsePositiveDecimal(EditAmountBox.Text, out decimal amount))
            {
                ShowStatus("Введите корректную сумму больше 0.");
                return;
            }

            string description = EditDescriptionBox.Text?.Trim() ?? "";

            var updated = new TransactionsTable(
                _selectedTransaction.id,
                _selectedTransaction.transaction_date,
                _selectedTransaction.type,
                amount,
                _selectedTransaction.currency,
                _selectedTransaction.document_number,
                description,
                _selectedTransaction.account_name,
                _selectedTransaction.account_type,
                _selectedTransaction.category_name,
                _selectedTransaction.category_type,
                _selectedTransaction.counterparty_name,
                _selectedTransaction.counterparty_type,
                _selectedTransaction.counterparty_tax_number,
                _selectedTransaction.employee_name,
                _selectedTransaction.employee_position,
                _selectedTransaction.user_name,
                _selectedTransaction.user_email
            );

            if (_transactionsRep.Update(updated))
            {
                LoadData();
            }
            else
            {
                ShowStatus("Не удалось обновить транзакцию в базе данных.");
            }
        }

        private void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedTransaction == null) return;

            if (_transactionsRep.Delete(_selectedTransaction))
            {
                LoadData();
            }
            else
            {
                ShowStatus("Не удалось удалить транзакцию из базы данных.");
            }
        }

        private void OnRefreshClick(object? sender, RoutedEventArgs e) => LoadData();

        private void ShowStatus(string message)
        {
            StatusText.Text = message;
            StatusText.IsVisible = true;
        }

        public static bool TryParsePositiveDecimal(string? input, out decimal result)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(input)) return false;
            string normalized = input.Trim().Replace(',', '.');
            return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out result) && result > 0;
        }

        private void OnOpenAccountsClick(object? sender, RoutedEventArgs e) => new AccountsWindow().ShowDialog(this);
        private void OnOpenCounterpartiesClick(object? sender, RoutedEventArgs e) => new CounterpartiesWindow().ShowDialog(this);
        private void OnOpenCategoriesClick(object? sender, RoutedEventArgs e) => new CategoriesWindow().ShowDialog(this);

        private void OnOpenEmployeesClick(object? sender, RoutedEventArgs e)
        {
            if (!IsAdmin) return;
            new EmployeesWindow().ShowDialog(this);
        }

        private void OnOpenUsersClick(object? sender, RoutedEventArgs e)
        {
            if (!IsAdmin) return;
            new UsersWindow().ShowDialog(this);
        }
    }
}