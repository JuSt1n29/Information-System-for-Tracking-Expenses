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
        private CounterpartiesRep _counterpartiesRep = new CounterpartiesRep();
        private EmployeesRep _employeesRep = new EmployeesRep();

        private List<AccountsTable> _accountsList = new List<AccountsTable>();
        private List<CategoriesTable> _categoriesList = new List<CategoriesTable>();
        private List<CounterpartiesTable> _counterpartiesList = new List<CounterpartiesTable>();
        private List<EmployeesTable> _employeesList = new List<EmployeesTable>();

        public AddTransactionWindow(int nextId)
        {
            InitializeComponent();
            _nextId = nextId;

            LoadDataToComboBoxes();
        }

        private void LoadDataToComboBoxes()
        {
            // Счета
            _accountsList = new List<AccountsTable>(_accountsRep.GetAll());
            foreach (var acc in _accountsList) AccountComboBox.Items.Add(acc.name);
            if (AccountComboBox.Items.Count > 0) AccountComboBox.SelectedIndex = 0;

            // Контрагенты
            _counterpartiesList = new List<CounterpartiesTable>(_counterpartiesRep.GetAll());
            foreach (var cp in _counterpartiesList) CounterpartyComboBox.Items.Add(cp.name);
            if (CounterpartyComboBox.Items.Count > 0) CounterpartyComboBox.SelectedIndex = 0;

            // Сотрудники
            _employeesList = new List<EmployeesTable>(_employeesRep.GetAll());
            foreach (var emp in _employeesList) EmployeeComboBox.Items.Add(emp.full_name);
            if (EmployeeComboBox.Items.Count > 0) EmployeeComboBox.SelectedIndex = 0;

            // Категории
            _categoriesList = new List<CategoriesTable>(_categoriesRep.GetAll());
            foreach (var cat in _categoriesList) CategoryComboBox.Items.Add(cat.name);
            if (CategoryComboBox.Items.Count > 0) CategoryComboBox.SelectedIndex = 0;
        }

        private void OnSaveClick(object? sender, RoutedEventArgs e)
        {
            int accountIndex = AccountComboBox.SelectedIndex;
            int counterpartyIndex = CounterpartyComboBox.SelectedIndex;
            int employeeIndex = EmployeeComboBox.SelectedIndex;
            int categoryIndex = CategoryComboBox.SelectedIndex;

            if (accountIndex < 0 || categoryIndex < 0 || counterpartyIndex < 0 || employeeIndex < 0 ||
                !decimal.TryParse(AmountBox.Text, out decimal amount))
            {
                return; 
            }

            var selectedAccount = _accountsList[accountIndex];
            var selectedCounterparty = _counterpartiesList[counterpartyIndex];
            var selectedEmployee = _employeesList[employeeIndex];
            var selectedCategory = _categoriesList[categoryIndex];

            string type = TypeBox.Text?.Trim() ?? "expense";
            string currency = CurrencyBox.Text?.Trim() ?? "UAH";
            string description = DescriptionBox.Text?.Trim() ?? "";

            // Передаем реальные выбранные ID вместо null
            CreatedTransaction = new TransactionsTable(
                _nextId, 
                selectedAccount.id, 
                selectedCounterparty.id, 
                selectedEmployee.id, 
                selectedCategory.id, 
                1, // user_id
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