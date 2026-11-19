using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class CounterpartiesWindow : Window
    {
        private readonly CounterpartiesRep _repo = new();
        private CounterpartiesTable? _selectedItem;

        public CounterpartiesWindow()
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
            CounterpartiesGrid.ItemsSource = null;
            CounterpartiesGrid.ItemsSource = _repo.GetAll();
            ResetForm();
        }

        private void ResetForm()
        {
            _selectedItem = null;
            CounterpartiesGrid.SelectedItem = null;

            EditNameBox.Text = "";
            EditTypeBox.Text = "client";
            EditTaxBox.Text = "";
            EditPhoneBox.Text = "";
            EditEmailBox.Text = "";

            PanelTitleText.Text = "Add New Counterparty:";
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
            if (CounterpartiesGrid.SelectedItem is CounterpartiesTable item)
            {
                _selectedItem = item;
                EditNameBox.Text = item.name;
                EditTypeBox.Text = item.type;
                EditTaxBox.Text = item.tax_number ?? "";
                EditPhoneBox.Text = item.phone ?? "";
                EditEmailBox.Text = item.email ?? "";

                PanelTitleText.Text = $"Selected Counterparty ({item.name}):";
                AddButton.IsVisible = false;
                UpdateButton.IsVisible = true;
                DeleteButton.IsVisible = true;
                StatusText.IsVisible = false;
            }
            else if (CounterpartiesGrid.SelectedItem == null && _selectedItem != null)
            {
                ResetForm();
            }
        }

        private void OnAddClick(object? sender, RoutedEventArgs e)
        {
            string name = EditNameBox.Text?.Trim() ?? "";
            string type = string.IsNullOrWhiteSpace(EditTypeBox.Text) ? "client" : EditTypeBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowStatus("Name cannot be empty.");
                return;
            }

            var created = new CounterpartiesTable(
                GetNextId(),
                name,
                type,
                string.IsNullOrWhiteSpace(EditTaxBox.Text) ? null : EditTaxBox.Text.Trim(),
                string.IsNullOrWhiteSpace(EditPhoneBox.Text) ? null : EditPhoneBox.Text.Trim(),
                string.IsNullOrWhiteSpace(EditEmailBox.Text) ? null : EditEmailBox.Text.Trim()
            );

            if (_repo.Add(created))
            {
                LoadData();
            }
            else
            {
                ShowStatus("Failed to add counterparty.");
            }
        }

        private void OnUpdateClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedItem == null) return;

            string name = EditNameBox.Text?.Trim() ?? "";
            string type = string.IsNullOrWhiteSpace(EditTypeBox.Text) ? "client" : EditTypeBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowStatus("Name cannot be empty.");
                return;
            }

            var updated = new CounterpartiesTable(
                _selectedItem.id,
                name,
                type,
                string.IsNullOrWhiteSpace(EditTaxBox.Text) ? null : EditTaxBox.Text.Trim(),
                string.IsNullOrWhiteSpace(EditPhoneBox.Text) ? null : EditPhoneBox.Text.Trim(),
                string.IsNullOrWhiteSpace(EditEmailBox.Text) ? null : EditEmailBox.Text.Trim()
            );

            if (_repo.Update(updated))
            {
                LoadData();
            }
            else
            {
                ShowStatus("Failed to update counterparty.");
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
                ShowStatus("Failed to delete counterparty.");
            }
        }

        private void OnClearClick(object? sender, RoutedEventArgs e) => ResetForm();

        private void OnRefreshClick(object? sender, RoutedEventArgs e) => LoadData();

        private void ShowStatus(string message)
        {
            StatusText.Text = message;
            StatusText.IsVisible = true;
        }
    }
}