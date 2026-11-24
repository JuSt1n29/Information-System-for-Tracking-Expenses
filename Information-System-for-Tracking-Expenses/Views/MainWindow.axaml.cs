using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class MainWindow : Window
    {
        private readonly TransactionsRep _transactionsRep = new();
        private UsersTable? _currentUser;
        private TransactionsTable? _selectedTransaction;

        private List<TransactionsTable> _allTransactions = new();
        private string _selectedTypeFilter = "all";
        private bool _isInitializingFilters = false;

        private static readonly HashSet<string> HiddenColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            "id", "account_id", "counterparty_id", "employee_id", "category_id", "user_id"
        };

        private static readonly string[] ChartPalette =
        {
            "#10B981", "#3B82F6", "#F59E0B", "#EC4899", "#8B5CF6", "#06B6D4"
        };

        public MainWindow() : this(null)
        {
        }

        public MainWindow(UsersTable? currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            ApplyRolePermissions();
            RefreshHeaderUserDisplay();
            LoadData();
        }

        private bool IsAdmin => string.Equals(_currentUser?.role, "admin", StringComparison.OrdinalIgnoreCase);

        private void ApplyRolePermissions()
        {
            EmployeesButton.IsVisible = IsAdmin;
            UsersButton.IsVisible = IsAdmin;
        }

        private void RefreshHeaderUserDisplay()
        {
            if (_currentUser != null)
            {
                CurrentUserText.Text = $"{_currentUser.name} ({_currentUser.role})";
                LoadBitmapToImage(HeaderAvatarImage, _currentUser.avatar_path);
            }
            else
            {
                CurrentUserText.Text = "Guest";
                HeaderAvatarImage.Source = null;
            }
        }

        private static void LoadBitmapToImage(Image target, string? path)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                {
                    using var stream = File.OpenRead(path);
                    target.Source = new Bitmap(stream);
                    return;
                }
            }
            catch
            {
            }
            target.Source = null;
        }

        private async void OnOpenProfileClick(object? sender, RoutedEventArgs e)
        {
            if (_currentUser == null) return;

            var profileWindow = new ProfileWindow(_currentUser);
            await profileWindow.ShowDialog(this);

            if (profileWindow.UpdatedUser != null)
            {
                _currentUser = profileWindow.UpdatedUser;
                RefreshHeaderUserDisplay();
            }
        }

        private void OnLogoutClick(object? sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        private void OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName != null && HiddenColumns.Contains(e.PropertyName))
            {
                e.Cancel = true;
            }
        }

        private void LoadData()
        {
            _allTransactions = _transactionsRep.GetAll().ToList();
            PopulateFilterDropdowns();
            ApplyFiltersAndAnalytics();

            _selectedTransaction = null;
            ActionPanel.IsVisible = false;
            StatusText.IsVisible = false;
            TransactionsGrid.SelectedItem = null;
        }

        private void PopulateFilterDropdowns()
        {
            _isInitializingFilters = true;

            string? prevCat = CategoryFilterBox.SelectedItem?.ToString();
            string? prevAcc = AccountFilterBox.SelectedItem?.ToString();
            string? prevCur = CurrencyFilterBox.SelectedItem?.ToString();

            var categories = new List<string> { "Всі категорії" };
            categories.AddRange(_allTransactions
                .Select(t => t.category_name)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .OrderBy(s => s)!);
            CategoryFilterBox.ItemsSource = categories;
            CategoryFilterBox.SelectedItem = (prevCat != null && categories.Contains(prevCat)) ? prevCat : "Всі категорії";

            var accounts = new List<string> { "Всі рахунки" };
            accounts.AddRange(_allTransactions
                .Select(t => t.account_name)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .OrderBy(s => s)!);
            AccountFilterBox.ItemsSource = accounts;
            AccountFilterBox.SelectedItem = (prevAcc != null && accounts.Contains(prevAcc)) ? prevAcc : "Всі рахунки";

            var currencies = new List<string> { "Валюта" };
            currencies.AddRange(_allTransactions
                .Select(t => t.currency)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .OrderBy(s => s)!);
            CurrencyFilterBox.ItemsSource = currencies;
            CurrencyFilterBox.SelectedItem = (prevCur != null && currencies.Contains(prevCur)) ? prevCur : "Валюта";

            _isInitializingFilters = false;
        }

        private void OnTypeFilterClick(object? sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not string tag) return;

            _selectedTypeFilter = tag;

            FilterAllBtn.Classes.Set("filterPillActive", tag == "all");
            FilterAllBtn.Classes.Set("filterPill", tag != "all");

            FilterIncomeBtn.Classes.Set("filterPillActive", tag == "income");
            FilterIncomeBtn.Classes.Set("filterPill", tag != "income");

            FilterExpenseBtn.Classes.Set("filterPillActive", tag == "expense");
            FilterExpenseBtn.Classes.Set("filterPill", tag != "expense");

            ApplyFiltersAndAnalytics();
        }

        private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (_isInitializingFilters) return;
            ApplyFiltersAndAnalytics();
        }

        private void OnFilterSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_isInitializingFilters) return;
            ApplyFiltersAndAnalytics();
        }

        private void ApplyFiltersAndAnalytics()
        {
            string query = SearchBox?.Text?.Trim().ToLowerInvariant() ?? "";
            string selectedCat = CategoryFilterBox?.SelectedItem?.ToString() ?? "Всі категорії";
            string selectedAcc = AccountFilterBox?.SelectedItem?.ToString() ?? "Всі рахунки";
            string selectedCur = CurrencyFilterBox?.SelectedItem?.ToString() ?? "Валюта";

            var filtered = _allTransactions.Where(t =>
            {
                if (_selectedTypeFilter != "all" &&
                    !string.Equals(t.type, _selectedTypeFilter, StringComparison.OrdinalIgnoreCase))
                    return false;

                if (selectedCat != "Всі категорії" &&
                    !string.Equals(t.category_name, selectedCat, StringComparison.OrdinalIgnoreCase))
                    return false;

                if (selectedAcc != "Всі рахунки" &&
                    !string.Equals(t.account_name, selectedAcc, StringComparison.OrdinalIgnoreCase))
                    return false;

                if (selectedCur != "Валюта" &&
                    !string.Equals(t.currency, selectedCur, StringComparison.OrdinalIgnoreCase))
                    return false;

                if (!string.IsNullOrEmpty(query))
                {
                    bool match =
                        (t.description?.ToLowerInvariant().Contains(query) ?? false) ||
                        (t.document_number?.ToLowerInvariant().Contains(query) ?? false) ||
                        (t.account_name?.ToLowerInvariant().Contains(query) ?? false) ||
                        (t.category_name?.ToLowerInvariant().Contains(query) ?? false) ||
                        (t.counterparty_name?.ToLowerInvariant().Contains(query) ?? false) ||
                        (t.employee_name?.ToLowerInvariant().Contains(query) ?? false);
                    if (!match) return false;
                }

                return true;
            }).ToList();

            TransactionsGrid.ItemsSource = null;
            TransactionsGrid.ItemsSource = filtered;

            UpdateKpiAndChart(filtered, selectedCur == "Валюта" ? "UAH" : selectedCur);
        }

        private void UpdateKpiAndChart(List<TransactionsTable> items, string currencyLabel)
        {
            decimal income = items
                .Where(t => string.Equals(t.type, "income", StringComparison.OrdinalIgnoreCase))
                .Sum(t => t.amount);

            decimal expenses = items
                .Where(t => string.Equals(t.type, "expense", StringComparison.OrdinalIgnoreCase))
                .Sum(t => t.amount);

            decimal net = income - expenses;

            NetBalanceText.Text = $"{net:N2} {currencyLabel}";
            TotalIncomeText.Text = $"+{income:N2} {currencyLabel}";
            TotalExpenseText.Text = $"-{expenses:N2} {currencyLabel}";

            RenderExpenseDonutChart(items);
        }

        private void RenderExpenseDonutChart(List<TransactionsTable> items)
        {
            DonutChartCanvas.Children.Clear();
            ChartLegendPanel.Children.Clear();

            var expenseItems = items
                .Where(t => string.Equals(t.type, "expense", StringComparison.OrdinalIgnoreCase))
                .ToList();

            ChartCountText.Text = $"{expenseItems.Count} оп.";

            var grouped = expenseItems
                .GroupBy(t => string.IsNullOrWhiteSpace(t.category_name) ? "Інше" : t.category_name)
                .Select(g => new { Category = g.Key, Total = g.Sum(x => x.amount) })
                .Where(x => x.Total > 0)
                .OrderByDescending(x => x.Total)
                .Take(5)
                .ToList();

            if (grouped.Count == 0)
            {
                ChartLegendPanel.Children.Add(new TextBlock
                {
                    Text = "Немає витрат за обраними фільтрами",
                    FontSize = 11,
                    Foreground = Brush.Parse("#64748B")
                });
                return;
            }

            decimal totalExpenses = grouped.Sum(x => x.Total);
            double startAngle = -90.0;
            const double cx = 58.0;
            const double cy = 58.0;
            const double radius = 49.0;

            for (int i = 0; i < grouped.Count; i++)
            {
                var entry = grouped[i];
                double fraction = (double)(entry.Total / totalExpenses);
                double sweepAngle = fraction * 360.0;
                var colorBrush = Brush.Parse(ChartPalette[i % ChartPalette.Length]);

                if (sweepAngle >= 359.9)
                {
                    var ring = new Ellipse
                    {
                        Width = radius * 2,
                        Height = radius * 2,
                        Stroke = colorBrush,
                        StrokeThickness = 18
                    };
                    Canvas.SetLeft(ring, cx - radius);
                    Canvas.SetTop(ring, cy - radius);
                    DonutChartCanvas.Children.Add(ring);
                }
                else if (sweepAngle > 1.0)
                {
                    var arc = CreateArcPath(cx, cy, radius, startAngle, startAngle + sweepAngle, colorBrush, 18);
                    DonutChartCanvas.Children.Add(arc);
                }

                startAngle += sweepAngle;

                var row = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto, *, Auto") };
                var dot = new Border
                {
                    Width = 9,
                    Height = 9,
                    CornerRadius = new CornerRadius(4.5),
                    Background = colorBrush,
                    Margin = new Thickness(0, 0, 8, 0),
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                };
                var nameBlock = new TextBlock
                {
                    Text = entry.Category,
                    FontSize = 11,
                    Foreground = Brush.Parse("#E2E8F0"),
                    TextTrimming = TextTrimming.CharacterEllipsis
                };
                var pctBlock = new TextBlock
                {
                    Text = $"{fraction * 100:0}% ({entry.Total:N0})",
                    FontSize = 11,
                    FontWeight = FontWeight.SemiBold,
                    Foreground = Brush.Parse("#94A3B8"),
                    Margin = new Thickness(8, 0, 0, 0)
                };

                Grid.SetColumn(dot, 0);
                Grid.SetColumn(nameBlock, 1);
                Grid.SetColumn(pctBlock, 2);
                row.Children.Add(dot);
                row.Children.Add(nameBlock);
                row.Children.Add(pctBlock);

                ChartLegendPanel.Children.Add(row);
            }
        }

        private static Avalonia.Controls.Shapes.Path CreateArcPath(double cx, double cy, double r, double startDeg, double endDeg, IBrush stroke, double thickness)
        {
            double startRad = startDeg * Math.PI / 180.0;
            double endRad = endDeg * Math.PI / 180.0;

            var startPoint = new Point(cx + r * Math.Cos(startRad), cy + r * Math.Sin(startRad));
            var endPoint = new Point(cx + r * Math.Cos(endRad), cy + r * Math.Sin(endRad));
            bool isLargeArc = (endDeg - startDeg) > 180.0;

            var figure = new PathFigure
            {
                StartPoint = startPoint,
                IsClosed = false
            };
            figure.Segments?.Add(new ArcSegment
            {
                Point = endPoint,
                Size = new Size(r, r),
                IsLargeArc = isLargeArc,
                SweepDirection = SweepDirection.Clockwise
            });

            var geometry = new PathGeometry();
            geometry.Figures?.Add(figure);

            return new Avalonia.Controls.Shapes.Path
            {
                Data = geometry,
                Stroke = stroke,
                StrokeThickness = thickness
            };
        }

        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (TransactionsGrid.SelectedItem is TransactionsTable item)
            {
                _selectedTransaction = item;
                DocumentBox.Text = item.document_number;
                EditAmountBox.Text = item.amount.ToString(CultureInfo.InvariantCulture);
                EditDescriptionBox.Text = item.description;
                StatusText.IsVisible = false;
                ActionPanel.IsVisible = true;
            }
            else
            {
                _selectedTransaction = null;
                ActionPanel.IsVisible = false;
            }
        }

        private async void OnAddClick(object? sender, RoutedEventArgs e)
        {
            int nextId = _transactionsRep.GetNextId();
            int userId = _currentUser?.id > 0 ? _currentUser.id : 1;
            var addWindow = new AddTransactionWindow(nextId, userId);

            await addWindow.ShowDialog(this);

            if (addWindow.CreatedTransaction != null)
            {
                if (_transactionsRep.Add(addWindow.CreatedTransaction))
                {
                    LoadData();
                }
            }
        }

        private void OnUpdateClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedTransaction == null) return;

            if (!TryParsePositiveDecimal(EditAmountBox.Text, out decimal amount))
            {
                ShowStatus("Введіть коректну суму більше 0.");
                return;
            }

            string description = EditDescriptionBox.Text?.Trim() ?? "";

            var updated = new TransactionsTable(
                _selectedTransaction.id,
                _selectedTransaction.transaction_date,
                _selectedTransaction.type,
                amount,
                _selectedTransaction.currency,
                _selectedTransaction.document_number,
                description,
                _selectedTransaction.account_name,
                _selectedTransaction.account_type,
                _selectedTransaction.category_name,
                _selectedTransaction.category_type,
                _selectedTransaction.counterparty_name,
                _selectedTransaction.counterparty_type,
                _selectedTransaction.counterparty_tax_number,
                _selectedTransaction.employee_name,
                _selectedTransaction.employee_position,
                _selectedTransaction.user_name,
                _selectedTransaction.user_email
            );

            if (_transactionsRep.Update(updated))
            {
                LoadData();
            }
            else
            {
                ShowStatus("Не вдалося оновити транзакцію в базі даних.");
            }
        }

        private void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            if (_selectedTransaction == null) return;

            if (_transactionsRep.Delete(_selectedTransaction))
            {
                LoadData();
            }
            else
            {
                ShowStatus("Не вдалося видалити транзакцію з бази даних.");
            }
        }

        private void OnRefreshClick(object? sender, RoutedEventArgs e) => LoadData();

        private void ShowStatus(string message)
        {
            StatusText.Text = message;
            StatusText.IsVisible = true;
        }

        public static bool TryParsePositiveDecimal(string? input, out decimal result)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(input)) return false;
            string normalized = input.Trim().Replace(',', '.');
            return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out result) && result > 0;
        }

        private async void OnOpenAccountsClick(object? sender, RoutedEventArgs e)
        {
            await new AccountsWindow().ShowDialog(this);
            LoadData();
        }

        private async void OnOpenCounterpartiesClick(object? sender, RoutedEventArgs e)
        {
            await new CounterpartiesWindow().ShowDialog(this);
            LoadData();
        }

        private async void OnOpenCategoriesClick(object? sender, RoutedEventArgs e)
        {
            await new CategoriesWindow().ShowDialog(this);
            LoadData();
        }

        private async void OnOpenEmployeesClick(object? sender, RoutedEventArgs e)
        {
            if (!IsAdmin) return;
            await new EmployeesWindow().ShowDialog(this);
            LoadData();
        }

        private async void OnOpenUsersClick(object? sender, RoutedEventArgs e)
        {
            if (!IsAdmin) return;
            await new UsersWindow().ShowDialog(this);
        }
    }
}