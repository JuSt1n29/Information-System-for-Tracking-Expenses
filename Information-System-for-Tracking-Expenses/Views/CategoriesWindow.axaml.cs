using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class CategoriesWindow : Window
    {
        private CategoriesRep repo = new CategoriesRep();

        public CategoriesWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            CategoriesGrid.ItemsSource = null;
            CategoriesGrid.ItemsSource = repo.GetAll();
        }

        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (CategoriesGrid.SelectedItem is CategoriesTable item)
            {
                NameBox.Text = item.name;
                TypeBox.Text = item.type;
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
            var newItem = new CategoriesTable(nextId, name, TypeBox.Text?.Trim() ?? "expense");
            if (repo.Add(newItem)) { LoadData(); ClearInputs(); }
        }

        private void OnUpdateClick(object? sender, RoutedEventArgs e)
        {
            if (!int.TryParse(UpdateIdBox.Text, out int id) || id <= 0) return;
            string name = NameBox.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(name)) return;

            var updatedItem = new CategoriesTable(id, name, TypeBox.Text?.Trim() ?? "expense");
            if (repo.Update(updatedItem)) LoadData();
        }

        private void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            if (!int.TryParse(UpdateIdBox.Text, out int id) || id <= 0) return;
            var emptyItem = new CategoriesTable(id, "", "");
            if (repo.Delete(emptyItem)) { LoadData(); ClearInputs(); }
        }

        private void OnCloseClick(object? sender, RoutedEventArgs e) => Close();

        private void ClearInputs()
        {
            NameBox.Text = ""; TypeBox.Text = ""; UpdateIdBox.Text = "";
        }
    }
}