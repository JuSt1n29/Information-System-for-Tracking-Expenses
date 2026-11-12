using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class AccountsWindow : Window
    {
        private AccountsRep _repo = new AccountsRep();
        private int _selectedId = -1;

        public AccountsWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            AccountsGrid.ItemsSource = null;
            AccountsGrid.ItemsSource = _repo.GetAll();
            ActionPanel.IsVisible = false;
            AccountsGrid.SelectedItem = null;
        }

        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (AccountsGrid.SelectedItem is AccountsTable item)
            {
                _selectedId = item.id;
                EditNameBox.Text = item.name;
                EditTypeBox.Text = item.type;
                EditCurrencyBox.Text = item.currency;
                EditBalanceBox.Text = item.balance.ToString();
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
            if (!decimal.TryParse(EditBalanceBox.Text, out decimal balance)) return;

            var updated = new AccountsTable(
                _selectedId,
                EditNameBox.Text?.Trim() ?? "",
                EditTypeBox.Text?.Trim() ?? "",
                EditCurrencyBox.Text?.Trim() ?? "UAH",
                balance
            );

            if (_repo.Update(updated))
            {
                LoadData();
            }
        }

        private void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedId <= 0) return;

            var empty = new AccountsTable(_selectedId, "", "", "", 0);
            if (_repo.Delete(empty))
            {
                LoadData();
            }
        }

        private void OnRefreshClick(object? sender, RoutedEventArgs e) => LoadData();
    }
}
