using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class AccountsWindow : Window
    {
        private AccountsRep repo = new AccountsRep();

        public AccountsWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            AccountsGrid.ItemsSource = null;
            AccountsGrid.ItemsSource = repo.GetAll();
        }

        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (AccountsGrid.SelectedItem is AccountsTable item)
            {
                NameBox.Text = item.name;
                TypeBox.Text = item.type;
                CurrencyBox.Text = item.currency;
                BalanceBox.Text = item.balance.ToString();
                UpdateIdBox.Text = item.id.ToString();
            }
        }

        private int GetNextId()
        {
            int maxId = 0;
            foreach (var item in repo.GetAll())
            {
                if (item.id > maxId) maxId = item.id;
            }
            return maxId + 1;
        }

        private void OnAddClick(object? sender, RoutedEventArgs e)
        {
            string name = NameBox.Text?.Trim() ?? "";
            string type = TypeBox.Text?.Trim() ?? "bank";
            string currency = CurrencyBox.Text?.Trim() ?? "UAH";
            
            if (string.IsNullOrEmpty(name) || !decimal.TryParse(BalanceBox.Text, out decimal balance)) return;

            int nextId = GetNextId();
            var newAccount = new AccountsTable(nextId, name, type, currency, balance);

            if (repo.Add(newAccount))
            {
                LoadData();
                ClearInputs();
            }
        }

        private void OnUpdateClick(object? sender, RoutedEventArgs e)
        {
            if (!int.TryParse(UpdateIdBox.Text, out int id) || id <= 0) return;
            string name = NameBox.Text?.Trim() ?? "";
            string type = TypeBox.Text?.Trim() ?? "bank";
            string currency = CurrencyBox.Text?.Trim() ?? "UAH";

            if (string.IsNullOrEmpty(name) || !decimal.TryParse(BalanceBox.Text, out decimal balance)) return;

            var updatedAccount = new AccountsTable(id, name, type, currency, balance);

            if (repo.Update(updatedAccount)) LoadData();
        }

        private void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            if (!int.TryParse(UpdateIdBox.Text, out int id) || id <= 0) return;

            var emptyAccount = new AccountsTable(id, "", "", "", 0);
            if (repo.Delete(emptyAccount))
            {
                LoadData();
                ClearInputs();
            }
        }

        private void OnCloseClick(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ClearInputs()
        {
            NameBox.Text = "";
            TypeBox.Text = "";
            CurrencyBox.Text = "";
            BalanceBox.Text = "";
            UpdateIdBox.Text = "";
        }
    }
}