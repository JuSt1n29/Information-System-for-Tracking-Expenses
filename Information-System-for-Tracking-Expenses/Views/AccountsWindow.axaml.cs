using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class AccountsWindow : Window
    {
        private readonly AccountsRep _repo = new();
        private AccountsTable? _selectedItem;

        public AccountsWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (string.Equals(e.PropertyName, "id", StringComparison.OrdinalIgnoreCase))
            {
                e.Cancel = true;
            }
        }

        private void LoadData()
        {
            AccountsGrid.ItemsSource = null;
            AccountsGrid.ItemsSource = _repo.GetAll();
            ResetForm();
        }

        private void ResetForm()
        {
            _selectedItem = null;
            AccountsGrid.SelectedItem = null;

            EditNameBox.Text = "";
            EditTypeBox.Text = "bank";
            EditCurrencyBox.Text = "UAH";
            EditBalanceBox.Text = "0";

            PanelTitleText.Text = "Add New Account:";
            AddButton.IsVisible = true;
            UpdateButton.IsVisible = false;
            DeleteButton.IsVisible = false;
            StatusText.IsVisible = false;
        }

        private int GetNextId()
        {
            int maxId = 0;
            foreach (var item in _repo.GetAll())
            {
                if (item.id > maxId) maxId = item.id;
            }
            return maxId + 1;
        }

        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (AccountsGrid.SelectedItem is AccountsTable item)
            {
                _selectedItem = item;
                EditNameBox.Text = item.name;
                EditTypeBox.Text = item.type;
                EditCurrencyBox.Text = item.currency;
                EditBalanceBox.Text = item.balance.ToString(CultureInfo.InvariantCulture);

                PanelTitleText.Text = $"Selected Account ({item.name}):";
                AddButton.IsVisible = false;
                UpdateButton.IsVisible = true;
                DeleteButton.IsVisible = true;
                StatusText.IsVisible = false;
            }
            else if (AccountsGrid.SelectedItem == null && _selectedItem != null)
            {
                ResetForm();
            }
        }

        private void OnAddClick(object? sender, RoutedEventArgs e)
        {
            string name = EditNameBox.Text?.Trim() ?? "";
            string type = string.IsNullOrWhiteSpace(EditTypeBox.Text) ? "bank" : EditTypeBox.Text.Trim();
            string currency = string.IsNullOrWhiteSpace(EditCurrencyBox.Text) ? "UAH" : EditCurrencyBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowStatus("Name cannot be empty.");
                return;
            }

            if (!TryParseDecimal(EditBalanceBox.Text, out decimal balance))
            {
                ShowStatus("Invalid balance value.");
                return;
            }

            var created = new AccountsTable(GetNextId(), name, type, currency, balance);
            if (_repo.Add(created))
            {
                LoadData();
            }
            else
            {
                ShowStatus("Failed to add account.");
            }
        }

        private void OnUpdateClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedItem == null) return;

            string name = EditNameBox.Text?.Trim() ?? "";
            string type = string.IsNullOrWhiteSpace(EditTypeBox.Text) ? "bank" : EditTypeBox.Text.Trim();
            string currency = string.IsNullOrWhiteSpace(EditCurrencyBox.Text) ? "UAH" : EditCurrencyBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowStatus("Name cannot be empty.");
                return;
            }

            if (!TryParseDecimal(EditBalanceBox.Text, out decimal balance))
            {
                ShowStatus("Invalid balance value.");
                return;
            }

            var updated = new AccountsTable(_selectedItem.id, name, type, currency, balance);
            if (_repo.Update(updated))
            {
                LoadData();
            }
            else
            {
                ShowStatus("Failed to update account.");
            }
        }

        private void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedItem == null) return;

            if (_repo.Delete(_selectedItem))
            {
                LoadData();
            }
            else
            {
                ShowStatus("Failed to delete account.");
            }
        }

        private void OnClearClick(object? sender, RoutedEventArgs e) => ResetForm();

        private void OnRefreshClick(object? sender, RoutedEventArgs e) => LoadData();

        private void ShowStatus(string message)
        {
            StatusText.Text = message;
            StatusText.IsVisible = true;
        }

        private static bool TryParseDecimal(string? text, out decimal value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;
            return decimal.TryParse(text.Trim().Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }
    }
}