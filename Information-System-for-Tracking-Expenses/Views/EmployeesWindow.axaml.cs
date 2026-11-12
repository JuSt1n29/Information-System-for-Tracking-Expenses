using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class EmployeesWindow : Window
    {
        private EmployeesRep repo = new EmployeesRep();

        public EmployeesWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            EmployeesGrid.ItemsSource = null;
            EmployeesGrid.ItemsSource = repo.GetAll();
        }

        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (EmployeesGrid.SelectedItem is EmployeesTable item)
            {
                NameBox.Text = item.full_name;
                PositionBox.Text = item.position;
                PhoneBox.Text = item.phone;
                EmailBox.Text = item.email;
                SalaryBox.Text = item.salary.ToString();
                StatusBox.Text = item.status;
                UpdateIdBox.Text = item.id.ToString();
            }
        }

        private int GetNextId()
        {
            int maxId = 0;
            foreach (var item in repo.GetAll()) if (item.id > maxId) maxId = item.id;
            return maxId + 1;
        }

        private void OnAddClick(object? sender, RoutedEventArgs e)
        {
            string name = NameBox.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(name) || !decimal.TryParse(SalaryBox.Text, out decimal salary)) return;

            int nextId = GetNextId();
            var newItem = new EmployeesTable(nextId, name, PositionBox.Text?.Trim() ?? "", PhoneBox.Text?.Trim() ?? "", EmailBox.Text?.Trim() ?? "", salary, DateTime.Now, StatusBox.Text?.Trim() ?? "active");
            if (repo.Add(newItem)) { LoadData(); ClearInputs(); }
        }

        private void OnUpdateClick(object? sender, RoutedEventArgs e)
        {
            if (!int.TryParse(UpdateIdBox.Text, out int id) || id <= 0) return;
            string name = NameBox.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(name) || !decimal.TryParse(SalaryBox.Text, out decimal salary)) return;

            var updatedItem = new EmployeesTable(id, name, PositionBox.Text?.Trim() ?? "", PhoneBox.Text?.Trim() ?? "", EmailBox.Text?.Trim() ?? "", salary, DateTime.Now, StatusBox.Text?.Trim() ?? "active");
            if (repo.Update(updatedItem)) LoadData();
        }

        private void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            if (!int.TryParse(UpdateIdBox.Text, out int id) || id <= 0) return;
            var emptyItem = new EmployeesTable(id, "", "", "", "", 0, DateTime.Now, "");
            if (repo.Delete(emptyItem)) { LoadData(); ClearInputs(); }
        }

        private void OnCloseClick(object? sender, RoutedEventArgs e) => Close();

        private void ClearInputs()
        {
            NameBox.Text = ""; PositionBox.Text = ""; PhoneBox.Text = ""; EmailBox.Text = ""; SalaryBox.Text = ""; StatusBox.Text = ""; UpdateIdBox.Text = "";
        }
    }
}