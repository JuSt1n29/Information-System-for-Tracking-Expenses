using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class AddTransactionWindow : Window
    {
        public TransactionsTable? CreatedTransaction { get; private set; }
        private int _nextId;

        public AddTransactionWindow(int nextId)
        {
            InitializeComponent();
            _nextId = nextId;
        }

        private void OnSaveClick(object? sender, RoutedEventArgs e)
        {
            if (!int.TryParse(AccountIdBox.Text, out int accountId) ||
                !int.TryParse(CategoryIdBox.Text, out int categoryId) ||
                !decimal.TryParse(AmountBox.Text, out decimal amount))
            {
                return; 
            }

            string type = TypeBox.Text?.Trim() ?? "expense";
            string currency = CurrencyBox.Text?.Trim() ?? "UAH";
            string description = DescriptionBox.Text?.Trim() ?? "";

            CreatedTransaction = new TransactionsTable(
                _nextId, accountId, null, null, categoryId, 1,
                type, amount, currency, DateTime.Now, description, "DOC-" + _nextId
            );

            Close();
        }

        private void OnCancelClick(object? sender, RoutedEventArgs e)
        {
            CreatedTransaction = null;
            Close();
        }
    }
}