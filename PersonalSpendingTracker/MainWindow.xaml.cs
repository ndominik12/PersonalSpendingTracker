using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PersonalSpendingTracker
{
    public partial class MainWindow : Window
    {
        ObservableCollection<Subscription> monthly = new ObservableCollection<Subscription>();
        ObservableCollection<Subscription> yearly = new ObservableCollection<Subscription>();

        public MainWindow()
        {
            InitializeComponent();

            Database.Init();
            foreach (Subscription s in Database.Load(false))
            {
                monthly.Add(s);
            }
            foreach (Subscription s in Database.Load(true))
            {
                yearly.Add(s);
            }

            dgMonthly.ItemsSource = monthly;
            dgYearly.ItemsSource = yearly;
            UpdateSummary();
        }

        private void AddMonthly_Click(object sender, RoutedEventArgs e)
        {
            Add(monthly, tbMonthlyName, tbMonthlyPrice, false);
        }

        private void AddYearly_Click(object sender, RoutedEventArgs e)
        {
            Add(yearly, tbYearlyName, tbYearlyPrice, true);
        }

        private void Add(ObservableCollection<Subscription> list, TextBox name, TextBox price, bool isYearly)
        {
            decimal p;
            if (!decimal.TryParse(price.Text, out p))
            {
                MessageBox.Show("Az ár nem szám.");
                return;
            }

            Subscription sub = new Subscription { Name = name.Text, Price = p };
            sub.Id = Database.Add(sub, isYearly);
            list.Add(sub);

            name.Clear();
            price.Clear();
            UpdateSummary();
        }

        private void DeleteMonthly_Click(object sender, RoutedEventArgs e)
        {
            Delete(monthly, dgMonthly);
        }

        private void DeleteYearly_Click(object sender, RoutedEventArgs e)
        {
            Delete(yearly, dgYearly);
        }

        private void Delete(ObservableCollection<Subscription> list, DataGrid grid)
        {
            Subscription selected = grid.SelectedItem as Subscription;
            if (selected == null)
            {
                MessageBox.Show("Válassz ki egy sort a törléshez.");
                return;
            }

            Database.Delete(selected.Id);
            list.Remove(selected);
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            decimal m = monthly.Sum(s => s.Price);
            decimal y = yearly.Sum(s => s.Price);

            tbMonthlyTotal.Text = $"Havi előfizetések: {m:N0} Ft / hó";
            tbYearlyTotal.Text = $"Éves előfizetések: {y:N0} Ft / év";
            tbPerYear.Text = $"Összesen egy évben: {m * 12 + y:N0} Ft";
            tbPerMonth.Text = $"Havi átlagban: {m + y / 12:N0} Ft";
        }
    }
}