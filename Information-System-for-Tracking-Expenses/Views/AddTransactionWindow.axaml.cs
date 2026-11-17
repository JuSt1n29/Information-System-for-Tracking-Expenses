using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class AddTransactionWindow : Window
    {
        public TransactionsTable? CreatedTransaction { get; private set; }
        private int _nextId;

        private AccountsRep _accountsRep = new AccountsRep();
        private CategoriesRep _categoriesRep = new CategoriesRep();

        // Списки для хранения реальных объектов, чтобы связывать их по индексу с ComboBox
        private List<AccountsTable> _accountsList = new List<AccountsTable>();
        private List<CategoriesTable> _categoriesList = new List<CategoriesTable>();

        public AddTransactionWindow(int nextId)
        {
            InitializeComponent();
            _nextId = nextId;

            LoadDataToComboBoxes();
        }

        private void LoadDataToComboBoxes()
        {
            // Загружаем счета
            _accountsList = new List<AccountsTable>(_accountsRep.GetAll());
            foreach (var acc in _accountsList)
            {
                AccountComboBox.Items.Add(acc.name);
            }
            if (AccountComboBox.Items.Count > 0) AccountComboBox.SelectedIndex = 0;

            // Загружаем категории
            _categoriesList = new List<CategoriesTable>(_categoriesRep.GetAll());
            foreach (var cat in _categoriesList)
            {
                CategoryComboBox.Items.Add(cat.name);
            }
            if (CategoryComboBox.Items.Count > 0) CategoryComboBox.SelectedIndex = 0;
        }

        private void OnSaveClick(object? sender, RoutedEventArgs e)
        {
            int accountIndex = AccountComboBox.SelectedIndex;
            int categoryIndex = CategoryComboBox.SelectedIndex;

            if (accountIndex < 0 || categoryIndex < 0 ||
                !decimal.TryParse(AmountBox.Text, out decimal amount))
            {
                return; 
            }

            // Получаем выбранные объекты по индексу из наших списков
            var selectedAccount = _accountsList[accountIndex];
            var selectedCategory = _categoriesList[categoryIndex];

            string type = TypeBox.Text?.Trim() ?? "expense";
            string currency = CurrencyBox.Text?.Trim() ?? "UAH";
            string description = DescriptionBox.Text?.Trim() ?? "";

            CreatedTransaction = new TransactionsTable(
                _nextId, 
                selectedAccount.id, 
                null, 
                null, 
                selectedCategory.id, 
                1,
                type, 
                amount, 
                currency, 
                DateTime.Now, 
                description, 
                "DOC-" + _nextId
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