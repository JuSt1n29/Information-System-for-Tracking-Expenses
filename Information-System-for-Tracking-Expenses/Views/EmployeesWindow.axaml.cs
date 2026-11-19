using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class EmployeesWindow : Window
    {
        private readonly EmployeesRep _repo = new();
        private EmployeesTable? _selectedItem;

        public EmployeesWindow()
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
            EmployeesGrid.ItemsSource = null;
            EmployeesGrid.ItemsSource = _repo.GetAll();
            ResetForm();
        }

        private void ResetForm()
        {
            _selectedItem = null;
            EmployeesGrid.SelectedItem = null;

            EditNameBox.Text = "";
            EditPositionBox.Text = "Manager";
            EditPhoneBox.Text = "";
            EditEmailBox.Text = "";
            EditSalaryBox.Text = "0";
            EditStatusBox.Text = "active";

            PanelTitleText.Text = "Add New Employee:";
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
            if (EmployeesGrid.SelectedItem is EmployeesTable item)
            {
                _selectedItem = item;
                EditNameBox.Text = item.full_name;
                EditPositionBox.Text = item.position ?? "";
                EditPhoneBox.Text = item.phone ?? "";
                EditEmailBox.Text = item.email ?? "";
                EditSalaryBox.Text = item.salary.ToString(CultureInfo.InvariantCulture);
                EditStatusBox.Text = item.status;

                PanelTitleText.Text = $"Selected Employee ({item.full_name}):";
                AddButton.IsVisible = false;
                UpdateButton.IsVisible = true;
                DeleteButton.IsVisible = true;
                StatusText.IsVisible = false;
            }
            else if (EmployeesGrid.SelectedItem == null && _selectedItem != null)
            {
                ResetForm();
            }
        }

        private void OnAddClick(object? sender, RoutedEventArgs e)
        {
            string fullName = EditNameBox.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(fullName))
            {
                ShowStatus("Full Name cannot be empty.");
                return;
            }

            if (!TryParseDecimal(EditSalaryBox.Text, out decimal salary))
            {
                ShowStatus("Invalid salary value.");
                return;
            }

            var created = new EmployeesTable(
                GetNextId(),
                fullName,
                string.IsNullOrWhiteSpace(EditPositionBox.Text) ? null : EditPositionBox.Text.Trim(),
                string.IsNullOrWhiteSpace(EditPhoneBox.Text) ? null : EditPhoneBox.Text.Trim(),
                string.IsNullOrWhiteSpace(EditEmailBox.Text) ? null : EditEmailBox.Text.Trim(),
                salary,
                DateTime.Now,
                string.IsNullOrWhiteSpace(EditStatusBox.Text) ? "active" : EditStatusBox.Text.Trim()
            );

            if (_repo.Add(created))
            {
                LoadData();
            }
            else
            {
                ShowStatus("Failed to add employee.");
            }
        }

        private void OnUpdateClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedItem == null) return;

            string fullName = EditNameBox.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(fullName))
            {
                ShowStatus("Full Name cannot be empty.");
                return;
            }

            if (!TryParseDecimal(EditSalaryBox.Text, out decimal salary))
            {
                ShowStatus("Invalid salary value.");
                return;
            }

            var updated = new EmployeesTable(
                _selectedItem.id,
                fullName,
                string.IsNullOrWhiteSpace(EditPositionBox.Text) ? null : EditPositionBox.Text.Trim(),
                string.IsNullOrWhiteSpace(EditPhoneBox.Text) ? null : EditPhoneBox.Text.Trim(),
                string.IsNullOrWhiteSpace(EditEmailBox.Text) ? null : EditEmailBox.Text.Trim(),
                salary,
                _selectedItem.hire_date,
                string.IsNullOrWhiteSpace(EditStatusBox.Text) ? "active" : EditStatusBox.Text.Trim()
            );

            if (_repo.Update(updated))
            {
                LoadData();
            }
            else
            {
                ShowStatus("Failed to update employee.");
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
                ShowStatus("Failed to delete employee.");
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