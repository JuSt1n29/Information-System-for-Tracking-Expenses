using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class CategoriesWindow : Window
    {
        private readonly CategoriesRep _repo = new();
        private CategoriesTable? _selectedItem;

        public CategoriesWindow()
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
            CategoriesGrid.ItemsSource = null;
            CategoriesGrid.ItemsSource = _repo.GetAll();
            ResetForm();
        }

        private void ResetForm()
        {
            _selectedItem = null;
            CategoriesGrid.SelectedItem = null;

            EditNameBox.Text = "";
            EditTypeBox.SelectedIndex = 0;

            PanelTitleText.Text = "Add New Category:";
            AddButton.IsVisible = true;
            UpdateButton.IsVisible = false;
            DeleteButton.IsVisible = false;
            StatusText.IsVisible = false;
        }

        private string GetSelectedType()
        {
            return (EditTypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "expense";
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
            if (CategoriesGrid.SelectedItem is CategoriesTable item)
            {
                _selectedItem = item;
                EditNameBox.Text = item.name;
                EditTypeBox.SelectedIndex = string.Equals(item.type, "income", StringComparison.OrdinalIgnoreCase) ? 1 : 0;

                PanelTitleText.Text = $"Selected Category ({item.name}):";
                AddButton.IsVisible = false;
                UpdateButton.IsVisible = true;
                DeleteButton.IsVisible = true;
                StatusText.IsVisible = false;
            }
            else if (CategoriesGrid.SelectedItem == null && _selectedItem != null)
            {
                ResetForm();
            }
        }

        private void OnAddClick(object? sender, RoutedEventArgs e)
        {
            string name = EditNameBox.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(name))
            {
                ShowStatus("Name cannot be empty.");
                return;
            }

            var created = new CategoriesTable(GetNextId(), name, GetSelectedType());
            if (_repo.Add(created))
            {
                LoadData();
            }
            else
            {
                ShowStatus("Failed to add category.");
            }
        }

        private void OnUpdateClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedItem == null) return;

            string name = EditNameBox.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(name))
            {
                ShowStatus("Name cannot be empty.");
                return;
            }

            var updated = new CategoriesTable(_selectedItem.id, name, GetSelectedType());
            if (_repo.Update(updated))
            {
                LoadData();
            }
            else
            {
                ShowStatus("Failed to update category.");
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
                ShowStatus("Failed to delete category.");
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