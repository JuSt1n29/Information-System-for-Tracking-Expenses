using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class MainWindow : Window
    {
        private TransactionsRep _transactionsRep = new TransactionsRep();
        private TransactionsTable? _selectedTransaction;

        private static readonly HashSet<string> HiddenColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            "id", "account_id", "counterparty_id", "employee_id", "category_id", "user_id"
        };

        public MainWindow()
        {
            InitializeComponent();
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
            var addWindow = new AddTransactionWindow(nextId);

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
        private void OnOpenEmployeesClick(object? sender, RoutedEventArgs e) => new EmployeesWindow().ShowDialog(this);
        private void OnOpenCategoriesClick(object? sender, RoutedEventArgs e) => new CategoriesWindow().ShowDialog(this);
    }
}