using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockView.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string baseURI = "https://financialmodelingprep.com/api/v3/";
        private readonly string apiKeyStr;
        private ObservableCollection<Stock> _stocks;
        private ObservableCollection<StockDetail> _stockDetails;

        public MainWindow()
        {
            apiKeyStr = ConfigurationManager.AppSettings["apiKey"].ToString();
            _stocks = new ObservableCollection<Stock>();
            _stockDetails = new ObservableCollection<StockDetail>();

            InitializeComponent();

            btnAddStockToList.IsEnabled = false;
            
            LoadData();

            cmbStocks.ItemsSource = _stocks;
            stocksWatchListDataGrid.ItemsSource = _stockDetails;
        }
        private void LoadData()
        {
            try
            {
                _stocks = new ObservableCollection<Stock>(LoadStockList());
                Task.Run(() => PullLatestStockDetailsPeriodically());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Please close and open again.\nException occured : {ex.Message}");
            }
        }

        private void stocksWatchListDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private async void btnAddStockToList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = (Stock)cmbStocks.SelectedItem;
                if (selectedItem == null)
                {
                    lblErrorMsg.Content = "No Stock is selected. Please Select any stock.";
                    lblErrorMsg.Visibility = Visibility.Visible;
                    return;
                }
                if (selectedItem.Tracking)
                {
                    lblErrorMsg.Content = $"'{selectedItem.Name}' Stock is already being tracked. Please Select any other stock.";
                    lblErrorMsg.Visibility = Visibility.Visible;
                    return;
                }

                StockDetail newStockToWatch = await FetchStockDetail(selectedItem);
                if (newStockToWatch != null)
                {
                    _stockDetails.Add(newStockToWatch);
                    selectedItem.Tracking = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task<StockDetail> FetchStockDetail(Stock stock)
        {

            StockDetail stockDetail = null;
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{baseURI}profile/{stock.Symbol}{apiKeyStr}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonString = response.Content.ReadAsStringAsync().Result;
                var companies = JsonConvert.DeserializeObject<List<Company>>(jsonString);
                var company = companies.First();
                stockDetail = new StockDetail(
                    stock.Id.ToString(),
                    stock.Name,
                    stock.Symbol,
                    float.Parse(company.price),
                    float.Parse(company.changes),
                    float.Parse(company.range.Split("-")[1]),
                    float.Parse(company.range.Split("-")[0]));                            
            }
            return stockDetail;
        }

        private async void btnPullLatestData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PullLatestStockDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void PullLatestStockDetailsPeriodically()
        {
            while (true)
            {
                await PullLatestStockDetails();
                await Task.Delay(60000);
            }
        }

        private async Task PullLatestStockDetails()
        {
            foreach (var stockDetail in _stockDetails)
            {
                var latestStockDetail = await FetchStockDetail(new Stock(stockDetail.Name, stockDetail.Symbol));
                stockDetail.CurrentPrice = latestStockDetail.CurrentPrice;
                stockDetail.PercentageChangeFromLastDay = latestStockDetail.PercentageChangeFromLastDay;
                stockDetail.High52Week = latestStockDetail.High52Week;
                stockDetail.Low52Week = latestStockDetail.Low52Week;
            }
        }

        private List<Stock> LoadStockList()
        {
            List<Stock> stocks = new List<Stock>();
            HttpClient httpClient = new HttpClient();
            var response = httpClient.GetAsync($"{baseURI}nasdaq_constituent{apiKeyStr}").GetAwaiter().GetResult();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonString = response.Content.ReadAsStringAsync().Result;
                var companies = JsonConvert.DeserializeObject<List<Company>>(jsonString);
                stocks.AddRange(companies.Select(company => new Stock(company.name, company.symbol)).ToList());
            }
            return stocks;
        }

        private void cmbStocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblErrorMsg.Visibility = Visibility.Hidden;
            var selectedItem = (Stock)cmbStocks.SelectedItem;
            if (selectedItem != null)
            {
                btnAddStockToList.IsEnabled = true;
            }
            
        }
    }    
}
