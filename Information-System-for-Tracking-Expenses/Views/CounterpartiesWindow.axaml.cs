using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class CounterpartiesWindow : Window
    {
        private CounterpartiesRep repo = new CounterpartiesRep();

        public CounterpartiesWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            CounterpartiesGrid.ItemsSource = null;
            CounterpartiesGrid.ItemsSource = repo.GetAll();
        }

        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (CounterpartiesGrid.SelectedItem is CounterpartiesTable item)
            {
                NameBox.Text = item.name;
                TypeBox.Text = item.type;
                TaxBox.Text = item.tax_number;
                PhoneBox.Text = item.phone;
                EmailBox.Text = item.email;
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
            if (string.IsNullOrEmpty(name)) return;

            int nextId = GetNextId();
            var newItem = new CounterpartiesTable(nextId, name, TypeBox.Text?.Trim() ?? "", TaxBox.Text?.Trim() ?? "", PhoneBox.Text?.Trim() ?? "", EmailBox.Text?.Trim() ?? "");
            if (repo.Add(newItem)) { LoadData(); ClearInputs(); }
        }

        private void OnUpdateClick(object? sender, RoutedEventArgs e)
        {
            if (!int.TryParse(UpdateIdBox.Text, out int id) || id <= 0) return;
            string name = NameBox.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(name)) return;

            var updatedItem = new CounterpartiesTable(id, name, TypeBox.Text?.Trim() ?? "", TaxBox.Text?.Trim() ?? "", PhoneBox.Text?.Trim() ?? "", EmailBox.Text?.Trim() ?? "");
            if (repo.Update(updatedItem)) LoadData();
        }

        private void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            if (!int.TryParse(UpdateIdBox.Text, out int id) || id <= 0) return;
            var emptyItem = new CounterpartiesTable(id, "", "", "", "", "");
            if (repo.Delete(emptyItem)) { LoadData(); ClearInputs(); }
        }

        private void OnCloseClick(object? sender, RoutedEventArgs e) => Close();

        private void ClearInputs()
        {
            NameBox.Text = ""; TypeBox.Text = ""; TaxBox.Text = ""; PhoneBox.Text = ""; EmailBox.Text = ""; UpdateIdBox.Text = "";
        }
    }
}