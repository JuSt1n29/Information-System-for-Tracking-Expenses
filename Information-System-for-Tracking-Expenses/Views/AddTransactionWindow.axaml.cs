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
        private readonly int _nextId;
        private readonly int _userId;

        private readonly AccountsRep _accountsRep = new();
        private readonly CategoriesRep _categoriesRep = new();
        private readonly CounterpartiesRep _counterpartiesRep = new();
        private readonly EmployeesRep _employeesRep = new();

        private List<AccountsTable> _accountsList = new();
        private List<CategoriesTable> _categoriesList = new();
        private List<CounterpartiesTable?> _counterpartiesList = new();
        private List<EmployeesTable?> _employeesList = new();

        public AddTransactionWindow(int nextId, int userId = 1)
        {
            InitializeComponent();
            _nextId = nextId;
            _userId = userId > 0 ? userId : 1;

            TypeComboBox.SelectedIndex = 0;
            CurrencyComboBox.SelectedIndex = 0;

            LoadDataToComboBoxes();
        }

        private void LoadDataToComboBoxes()
        {
            _accountsList = new List<AccountsTable>(_accountsRep.GetAll());
            foreach (var acc in _accountsList)
                AccountComboBox.Items.Add(acc.name);
            if (AccountComboBox.Items.Count > 0) AccountComboBox.SelectedIndex = 0;

            _categoriesList = new List<CategoriesTable>(_categoriesRep.GetAll());
            foreach (var cat in _categoriesList)
                CategoryComboBox.Items.Add(cat.name);
            if (CategoryComboBox.Items.Count > 0) CategoryComboBox.SelectedIndex = 0;

            _counterpartiesList.Add(null);
            CounterpartyComboBox.Items.Add("— Not specified —");
            foreach (var cp in _counterpartiesRep.GetAll())
            {
                _counterpartiesList.Add(cp);
                CounterpartyComboBox.Items.Add(cp.name);
            }
            CounterpartyComboBox.SelectedIndex = 0;

            _employeesList.Add(null);
            EmployeeComboBox.Items.Add("— Not specified —");
            foreach (var emp in _employeesRep.GetAll())
            {
                _employeesList.Add(emp);
                EmployeeComboBox.Items.Add(emp.full_name);
            }
            EmployeeComboBox.SelectedIndex = 0;
        }

        private void OnSaveClick(object? sender, RoutedEventArgs e)
        {
            int accountIndex = AccountComboBox.SelectedIndex;
            int categoryIndex = CategoryComboBox.SelectedIndex;
            int counterpartyIndex = CounterpartyComboBox.SelectedIndex;
            int employeeIndex = EmployeeComboBox.SelectedIndex;

            if (accountIndex < 0 || accountIndex >= _accountsList.Count)
            {
                ShowError("Выберите счёт.");
                return;
            }

            if (categoryIndex < 0 || categoryIndex >= _categoriesList.Count)
            {
                ShowError("Выберите категорию.");
                return;
            }

            if (!MainWindow.TryParsePositiveDecimal(AmountBox.Text, out decimal amount))
            {
                ShowError("Введите корректную сумму больше 0.");
                return;
            }

            var selectedAccount = _accountsList[accountIndex];
            var selectedCategory = _categoriesList[categoryIndex];
            int? counterpartyId = (counterpartyIndex > 0 && counterpartyIndex < _counterpartiesList.Count)
                ? _counterpartiesList[counterpartyIndex]?.id
                : null;
            int? employeeId = (employeeIndex > 0 && employeeIndex < _employeesList.Count)
                ? _employeesList[employeeIndex]?.id
                : null;

            string type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "expense";
            string currency = (CurrencyComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "UAH";
            string description = DescriptionBox.Text?.Trim() ?? "";

            CreatedTransaction = new TransactionsTable(
                _nextId,
                selectedAccount.id,
                counterpartyId,
                employeeId,
                selectedCategory.id,
                _userId,
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

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.IsVisible = true;
        }
    }
}