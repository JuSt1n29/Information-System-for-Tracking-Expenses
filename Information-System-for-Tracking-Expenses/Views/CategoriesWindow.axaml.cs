using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class CategoriesWindow : Window
    {
        private CategoriesRep _repo = new CategoriesRep();
        private int _selectedId = -1;

        public CategoriesWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            CategoriesGrid.ItemsSource = null;
            CategoriesGrid.ItemsSource = _repo.GetAll();
            ActionPanel.IsVisible = false;
            CategoriesGrid.SelectedItem = null;
        }

        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (CategoriesGrid.SelectedItem is CategoriesTable item)
            {
                _selectedId = item.id;
                EditNameBox.Text = item.name;
                EditTypeBox.Text = item.type;
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

            var updated = new CategoriesTable(
                _selectedId,
                EditNameBox.Text?.Trim() ?? "",
                EditTypeBox.Text?.Trim() ?? ""
            );

            if (_repo.Update(updated))
            {
                LoadData();
            }
        }

        private void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedId <= 0) return;

            var empty = new CategoriesTable(_selectedId, "", "");
            if (_repo.Delete(empty))
            {
                LoadData();
            }
        }

        private void OnRefreshClick(object? sender, RoutedEventArgs e) => LoadData();
    }
}