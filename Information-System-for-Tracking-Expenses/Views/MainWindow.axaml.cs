using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class MainWindow : Window
    {
        private TransactionsRep _transactionsRep = new TransactionsRep();
        private int _selectedTransactionId = -1;

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            TransactionsGrid.ItemsSource = null;
            TransactionsGrid.ItemsSource = _transactionsRep.GetAll();
            
            // Скрываем панель редактирования при обновлении списка
            ActionPanel.IsVisible = false;
            TransactionsGrid.SelectedItem = null;
        }

        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (TransactionsGrid.SelectedItem is TransactionsTable item)
            {
                _selectedTransactionId = item.id;
                EditAmountBox.Text = item.amount.ToString();
                EditDescriptionBox.Text = item.description;
                UpdateIdBox.Text = item.id.ToString();

                // Показываем панель управления при выборе строки
                ActionPanel.IsVisible = true;
            }
            else
            {
                ActionPanel.IsVisible = false;
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

        // Открытие формы добавления в виде модального окна
        private async void OnAddClick(object? sender, RoutedEventArgs e)
        {
            int nextId = GetNextId();
            var addWindow = new AddTransactionWindow(nextId);
            
            await addWindow.ShowDialog(this);

            // Проверяем, была ли создана транзакция в окне ввода
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
            if (_selectedTransactionId <= 0) return;
            if (!decimal.TryParse(EditAmountBox.Text, out decimal amount)) return;

            string description = EditDescriptionBox.Text?.Trim() ?? "";

            var existingItems = _transactionsRep.GetAll();
            
            // Используем FirstOrDefault вместо Find
            TransactionsTable? current = existingItems.FirstOrDefault(x => x.id == _selectedTransactionId);

            if (current != null)
            {
                var updatedTransaction = new TransactionsTable(
                    current.id, current.account_id, current.counterparty_id, current.employee_id, 
                    current.category_id, current.user_id, current.type, amount, 
                    current.currency, current.transaction_date, description, current.document_number
                );

                if (_transactionsRep.Update(updatedTransaction))
                {
                    LoadData();
                }
            }
        }

        private void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedTransactionId <= 0) return;

            var emptyTransaction = new TransactionsTable(
                _selectedTransactionId, 0, null, null, 0, 0, "", 0, "", DateTime.Now, "", ""
            );

            if (_transactionsRep.Delete(emptyTransaction))
            {
                LoadData();
            }
        }

        private void OnRefreshClick(object? sender, RoutedEventArgs e)
        {
            LoadData();
        }

        // Открытие справочников
        private void OnOpenAccountsClick(object? sender, RoutedEventArgs e) => new AccountsWindow().ShowDialog(this);
        private void OnOpenCounterpartiesClick(object? sender, RoutedEventArgs e) => new CounterpartiesWindow().ShowDialog(this);
        private void OnOpenEmployeesClick(object? sender, RoutedEventArgs e) => new EmployeesWindow().ShowDialog(this);
        private void OnOpenCategoriesClick(object? sender, RoutedEventArgs e) => new CategoriesWindow().ShowDialog(this);
    }
}