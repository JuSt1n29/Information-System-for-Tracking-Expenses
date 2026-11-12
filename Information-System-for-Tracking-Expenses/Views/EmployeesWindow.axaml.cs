using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class EmployeesWindow : Window
    {
        private EmployeesRep _repo = new EmployeesRep();
        private int _selectedId = -1;

        public EmployeesWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            EmployeesGrid.ItemsSource = null;
            EmployeesGrid.ItemsSource = _repo.GetAll();
            ActionPanel.IsVisible = false;
            EmployeesGrid.SelectedItem = null;
        }

        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (EmployeesGrid.SelectedItem is EmployeesTable item)
            {
                _selectedId = item.id;
                EditNameBox.Text = item.full_name;
                EditPositionBox.Text = item.position;
                EditPhoneBox.Text = item.phone;
                EditEmailBox.Text = item.email;
                EditSalaryBox.Text = item.salary.ToString();
                EditStatusBox.Text = item.status;
                UpdateIdBox.Text = item.id.ToString();

                ActionPanel.IsVisible = true;
            }
            else
            {
                ActionPanel.IsVisible = false;
            }
        }

        private void OnUpdateClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedId <= 0) return;
            if (!decimal.TryParse(EditSalaryBox.Text, out decimal salary)) return;

            var existing = _repo.GetAll();
            var current = existing.FirstOrDefault(x => x.id == _selectedId);

            if (current != null)
            {
                var updated = new EmployeesTable(
                    current.id,
                    EditNameBox.Text?.Trim() ?? "",
                    string.IsNullOrWhiteSpace(EditPositionBox.Text) ? null : EditPositionBox.Text.Trim(),
                    string.IsNullOrWhiteSpace(EditPhoneBox.Text) ? null : EditPhoneBox.Text.Trim(),
                    string.IsNullOrWhiteSpace(EditEmailBox.Text) ? null : EditEmailBox.Text.Trim(),
                    salary,
                    current.hire_date,
                    EditStatusBox.Text?.Trim() ?? "active"
                );

                if (_repo.Update(updated))
                {
                    LoadData();
                }
            }
        }

        private void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedId <= 0) return;

            var empty = new EmployeesTable(_selectedId, "", null, null, null, 0, DateTime.Now, "");
            if (_repo.Delete(empty))
            {
                LoadData();
            }
        }

        private void OnRefreshClick(object? sender, RoutedEventArgs e) => LoadData();
    }
}