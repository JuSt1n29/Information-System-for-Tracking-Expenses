using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class MainWindow : Window
    {
        private TransactionsRep _transactionsRep = new TransactionsRep();

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            TransactionsGrid.ItemsSource = null;
            TransactionsGrid.ItemsSource = _transactionsRep.GetAll();
        }

        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (TransactionsGrid.SelectedItem is TransactionsTable item)
            {
                AccountIdBox.Text = item.account_id.ToString();
                TypeBox.Text = item.type;
                AmountBox.Text = item.amount.ToString();
                CurrencyBox.Text = item.currency;
                CategoryIdBox.Text = item.category_id.ToString();
                DescriptionBox.Text = item.description;
                UpdateIdBox.Text = item.id.ToString();
            }
        }

        private int GetNextId()
        {
            int maxId = 0;
            foreach (var item in _transactionsRep.GetAll())
            {
                if (item.id > maxId) maxId = item.id;
            }
            return maxId + 1;
        }

        private void OnAddClick(object? sender, RoutedEventArgs e)
        {
            if (!int.TryParse(AccountIdBox.Text, out int accountId) ||
                !int.TryParse(CategoryIdBox.Text, out int categoryId) ||
                !decimal.TryParse(AmountBox.Text, out decimal amount))
            {
                return;
            }

            int nextId = GetNextId();
            string type = TypeBox.Text?.Trim() ?? "expense";
            string currency = CurrencyBox.Text?.Trim() ?? "UAH";
            string description = DescriptionBox.Text?.Trim() ?? "";

            var newTransaction = new TransactionsTable(
                nextId, accountId, null, null, categoryId, 1, 
                type, amount, currency, DateTime.Now, description, "DOC-" + nextId
            );

            if (_transactionsRep.Add(newTransaction))
            {
                LoadData();
                ClearInputs();
            }
        }

        private void OnUpdateClick(object? sender, RoutedEventArgs e)
        {
            if (!int.TryParse(UpdateIdBox.Text, out int id) || id <= 0) return;
            if (!int.TryParse(AccountIdBox.Text, out int accountId) ||
                !int.TryParse(CategoryIdBox.Text, out int categoryId) ||
                !decimal.TryParse(AmountBox.Text, out decimal amount))
            {
                return;
            }

            string type = TypeBox.Text?.Trim() ?? "expense";
            string currency = CurrencyBox.Text?.Trim() ?? "UAH";
            string description = DescriptionBox.Text?.Trim() ?? "";

            var updatedTransaction = new TransactionsTable(
                id, accountId, null, null, categoryId, 1, 
                type, amount, currency, DateTime.Now, description, "DOC-UPD"
            );

            if (_transactionsRep.Update(updatedTransaction))
            {
                LoadData();
            }
        }

        private void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            if (!int.TryParse(UpdateIdBox.Text, out int id) || id <= 0) return;

            var emptyTransaction = new TransactionsTable(
                id, 0, null, null, 0, 0, "", 0, "", DateTime.Now, "", ""
            );

            if (_transactionsRep.Delete(emptyTransaction))
            {
                LoadData();
                ClearInputs();
            }
        }

        private void OnRefreshClick(object? sender, RoutedEventArgs e)
        {
            LoadData();
        }

        // Методи відкриття інших вікон-довідників
        private void OnOpenAccountsClick(object? sender, RoutedEventArgs e)
        {
            new AccountsWindow().ShowDialog(this);
        }

        private void OnOpenCounterpartiesClick(object? sender, RoutedEventArgs e)
        {
            new CounterpartiesWindow().ShowDialog(this);
        }

        private void OnOpenEmployeesClick(object? sender, RoutedEventArgs e)
        {
            new EmployeesWindow().ShowDialog(this);
        }

        private void OnOpenCategoriesClick(object? sender, RoutedEventArgs e)
        {
            new CategoriesWindow().ShowDialog(this);
        }

        private void ClearInputs()
        {
            AccountIdBox.Text = "";
            TypeBox.Text = "";
            AmountBox.Text = "";
            CurrencyBox.Text = "";
            CategoryIdBox.Text = "";
            DescriptionBox.Text = "";
            UpdateIdBox.Text = "";
        }
    }
}