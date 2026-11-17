using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class CounterpartiesWindow : Window
    {
        private CounterpartiesRep _repo = new CounterpartiesRep();
        private int _selectedId = -1;

        public CounterpartiesWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            CounterpartiesGrid.ItemsSource = null;
            CounterpartiesGrid.ItemsSource = _repo.GetAll();
            ActionPanel.IsVisible = false;
            CounterpartiesGrid.SelectedItem = null;
        }

        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (CounterpartiesGrid.SelectedItem is CounterpartiesTable item)
            {
                _selectedId = item.id;
                EditNameBox.Text = item.name;
                EditTypeBox.Text = item.type;
                EditTaxBox.Text = item.tax_number;
                EditPhoneBox.Text = item.phone;
                EditEmailBox.Text = item.email;
                UpdateIdBox.Text = item.id.ToString();

                ActionPanel.IsVisible = true;
            }
            else
            {
                ActionPanel.IsVisible = false;
            }
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

        private void OnAddClick(object? sender, RoutedEventArgs e)
        {
            int newId = GetNextId();
            var newCounterparty = new CounterpartiesTable(newId, "Новий контрагент", "client", null, null, null);
            if (_repo.Add(newCounterparty))
            {
                LoadData();
            }
        }

        private void OnUpdateClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedId <= 0) return;

            var updated = new CounterpartiesTable(
                _selectedId,
                EditNameBox.Text?.Trim() ?? "",
                EditTypeBox.Text?.Trim() ?? "",
                string.IsNullOrWhiteSpace(EditTaxBox.Text) ? null : EditTaxBox.Text.Trim(),
                string.IsNullOrWhiteSpace(EditPhoneBox.Text) ? null : EditPhoneBox.Text.Trim(),
                string.IsNullOrWhiteSpace(EditEmailBox.Text) ? null : EditEmailBox.Text.Trim()
            );

            if (_repo.Update(updated))
            {
                LoadData();
            }
        }

        private void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedId <= 0) return;

            var empty = new CounterpartiesTable(_selectedId, "", "", null, null, null);
            if (_repo.Delete(empty))
            {
                LoadData();
            }
        }

        private void OnRefreshClick(object? sender, RoutedEventArgs e) => LoadData();
    }
}