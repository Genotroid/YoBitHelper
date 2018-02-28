using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

using System.Security.Cryptography;

using System.IO;
using System.IO.Ports;
using System.Net;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Threading;

namespace YoBit
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            int num = 0;
            TimerCallback tm = new TimerCallback(Refresh);
            Timer timer = new Timer(tm, num, 0, 2000);
        }

        public void GetInfo()
        {

            string parameters = $"method=getInfo&nonce=" + (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            string address = "https://yobit.io/tapi/";

            var secret = Encoding.UTF8.GetBytes(Properties.Settings.Default.secret);

            string sign1 = string.Empty;
            byte[] inputBytes = Encoding.UTF8.GetBytes(parameters);
            using (var hmac = new HMACSHA512(secret))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);

                StringBuilder hex1 = new StringBuilder(hashValue.Length * 2);
                foreach (byte b in hashValue)
                {
                    hex1.AppendFormat("{0:x2}", b);
                }
                sign1 = hex1.ToString();
            }

            WebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(address);
            if (webRequest != null)
            {
                webRequest.Method = "POST";
                webRequest.Timeout = 20000;
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Headers.Add("Key", Properties.Settings.Default.key);
                webRequest.Headers.Add("Sign", sign1);

                webRequest.ContentLength = parameters.Length;
                using (var dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(inputBytes, 0, parameters.Length);
                }

                using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                    {
                        string jsonResponse = sr.ReadToEnd();
                        JObject responce = JObject.Parse(jsonResponse);
                        Console.WriteLine(jsonResponse);
                        if (responce["success"].ToString() == "1")
                        {
                            FundsLV.Items.Clear();
                            IList<JToken> funds = responce["return"]["funds_incl_orders"].Children().ToList();
                            foreach (JToken fund in funds)
                            {
                                string fundName = fund.ToString().Substring(0, fund.ToString().IndexOf(':'));
                                double fundCount = 0.0;
                                try
                                {
                                    fundCount = Double.Parse(fund.ToString().Substring(fund.ToString().IndexOf(':') + 1), CultureInfo.InvariantCulture);
                                }
                                catch (FormatException exc)
                                {
                                    MessageBox.Show($"Не удалось привести к формату {fundName}. " + exc.Message);
                                }
                                catch (OverflowException)
                                {
                                    MessageBox.Show($"Выход за пределы. {fundName}");
                                }
                                FundsLV.Items.Add(fundName + ": " + fundCount.ToString("F8"));
                            }
                        }
                        else MessageBox.Show("Неудачная попытка, проверьте соединение, или данные, " +
                        "скорее всего косяк за Вами, потому что прога работает на ура, даже доебаться не до чего, " +
                        "а вот Вы скорее всего косяк, проверяйте.");


                    }
                }
            }
        }

        public bool Trade(string pair, string type, double rate, double amount)
        {
            string _rate = rate.ToString().Replace(',','.');
            string _amount = amount.ToString().Replace(',', '.');

            string parameters = $"pair={pair}&type={type}&rate={_rate}&amount={_amount}&"+
                "method=Trade&nonce=" + (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            string address = "https://yobit.io/tapi/";

            var secret = Encoding.UTF8.GetBytes(Properties.Settings.Default.secret);

            string sign1 = string.Empty;
            byte[] inputBytes = Encoding.UTF8.GetBytes(parameters);
            using (var hmac = new HMACSHA512(secret))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);

                StringBuilder hex1 = new StringBuilder(hashValue.Length * 2);
                foreach (byte b in hashValue)
                {
                    hex1.AppendFormat("{0:x2}", b);
                }
                sign1 = hex1.ToString();
            }

            WebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(address);
            if (webRequest != null)
            {
                webRequest.Method = "POST";
                webRequest.Timeout = 20000;
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Headers.Add("Key", Properties.Settings.Default.key);
                webRequest.Headers.Add("Sign", sign1);

                webRequest.ContentLength = parameters.Length;
                using (var dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(inputBytes, 0, parameters.Length);
                }

                using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                    {
                        string jsonResponse = sr.ReadToEnd();
                        JObject responce = JObject.Parse(jsonResponse);
                        if (responce["success"].ToString() == "1") return true;                        
                        else 
                        {
                            MessageBox.Show("Неудачная попытка, проверьте соединение, или данные, " +
                            "скорее всего косяк за Вами, потому что прога работает на ура, даже доебаться не до чего, " +
                            "а вот Вы скорее всего косяк, проверяйте.");
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        public bool ActiveOrders(string pair)
        {
            string parameters = $"pair={pair}&method=ActiveOrders&nonce=" + (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            string address = "https://yobit.io/tapi/";

            var secret = Encoding.UTF8.GetBytes(Properties.Settings.Default.secret);

            string sign1 = string.Empty;
            byte[] inputBytes = Encoding.UTF8.GetBytes(parameters);
            using (var hmac = new HMACSHA512(secret))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);

                StringBuilder hex1 = new StringBuilder(hashValue.Length * 2);
                foreach (byte b in hashValue)
                {
                    hex1.AppendFormat("{0:x2}", b);
                }
                sign1 = hex1.ToString();
            }

            WebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(address);
            if (webRequest != null)
            {
                webRequest.Method = "POST";
                webRequest.Timeout = 20000;
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Headers.Add("Key", Properties.Settings.Default.key);
                webRequest.Headers.Add("Sign", sign1);

                webRequest.ContentLength = parameters.Length;
                using (var dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(inputBytes, 0, parameters.Length);
                }

                using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                    {
                        string jsonResponse = sr.ReadToEnd();
                        Console.WriteLine(jsonResponse); 
                    }
                }
            }
            return false;
        }

        public void Depth(string fundName)
        {
            orderBuy.IsEnabled = true;
            orderBuy.ItemsSource = "";
            //orderBuy.Items.Clear();

            orderSell.IsEnabled = true;
            orderSell.ItemsSource = "";

            string address = "https://yobit.io/api/3/depth/" + fundName;
            WebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(address);
            if (webRequest != null)
            {
                webRequest.Method = "POST";
                webRequest.Timeout = 20000;
                webRequest.ContentType = "application/x-www-form-urlencoded";

                using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                    {
                        string jsonResponse = sr.ReadToEnd();
                        if (jsonResponse.IndexOf("success") < 0)
                        {
                            JObject responce = JObject.Parse(jsonResponse);

                            IList<JToken> asks = responce[fundName]["asks"].Children().ToList();
                            IList<JToken> bids = responce[fundName]["bids"].Children().ToList();

                            List<OrderTable> buyTable = new List<OrderTable>(3);
                            List<OrderTable> sellTable = new List<OrderTable>(3);
                            foreach (JToken ask in asks)
                            {

                                double coin1, coin2, price = 0.0;
                                price = Convert.ToDouble(ask[0].ToString());
                                coin1 = Convert.ToDouble(ask[1].ToString());
                                coin2 = Math.Round(coin1 * price,8);

                                buyTable.Add(new OrderTable(price, coin1, coin2));


                            }
                            
                            foreach (JToken bid in bids)
                            {
                                double coin1, coin2, price = 0.0;
                                price = Convert.ToDouble(bid[0].ToString());
                                coin1 = Convert.ToDouble(bid[1].ToString());
                                coin2 = Math.Round(coin1 * price, 8);

                                sellTable.Add(new OrderTable(price, coin1, coin2));
                            }
                            
                            orderBuy.ItemsSource = buyTable;
                            orderBuy.Columns[0].Header = "Цена";
                            orderBuy.Columns[1].Header = fundName.Substring(0, fundName.IndexOf('_')).ToUpper();
                            orderBuy.Columns[2].Header = fundName.Substring(fundName.IndexOf('_') + 1).ToUpper();

                            orderSell.ItemsSource = sellTable;
                            orderSell.Columns[0].Header = "Цена";
                            orderSell.Columns[1].Header = orderBuy.Columns[1].Header;
                            orderSell.Columns[2].Header = orderBuy.Columns[2].Header;
                        }
                        else MessageBox.Show("Неудачная попытка, проверьте соединение, или данные, " +
                        "скорее всего косяк за Вами, потому что прога работает на ура, даже доебаться не до чего, " +
                        "а вот Вы скорее всего косяк, проверяйте.");
                    }
                }
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.key == string.Empty || Properties.Settings.Default.secret == string.Empty)
            {
                settingsTab.Focus();
                mainTab.IsEnabled = false;
            }
        }

        private void SaveSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (keyBox.Text != string.Empty || secretBox.Text != string.Empty)
            {
                mainTab.IsEnabled = true;
                Properties.Settings.Default.key = keyBox.Text;
                Properties.Settings.Default.secret = secretBox.Text;
                Properties.Settings.Default.pair = pairBox.Text;
                Properties.Settings.Default.Save();
                mainTab.Focus();
            }
            else MessageBox.Show("Вы забыли заполнить одно из полей.");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetInfo();
        }

        private void settingsTab_GotFocus(object sender, RoutedEventArgs e)
        {
            //тут полная жопа, надо что-то делать, чтобы можно было менять значения в настройках
            keyBox.Text = Properties.Settings.Default.key;
            secretBox.Text = Properties.Settings.Default.secret;
            //pairBox.Text = Properties.Settings.Default.pair;
        }

        private void mainTab_GotFocus(object sender, RoutedEventArgs e)
        {
            //GetInfo();
        }

        private void btnDepth_Click(object sender, RoutedEventArgs e)
        {
            Depth(Properties.Settings.Default.pair);
        }

        private void orderBuy_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OrderTable path = orderBuy.SelectedItem as OrderTable;

            tbCountBuy.Text = path.coin1;
            tbCostBuy.Text = path.price;
            tbTotalBuy.Text = path.coin2;
            lblCommBuy.Content = CommRound(Convert.ToDouble(path.coin2) * 0.002).ToString("F8");
            lblTotalBuy.Content = CommRound(Convert.ToDouble(path.coin2) + Convert.ToDouble(lblCommBuy.Content)).ToString("F8");

            tbCountSell.Text = path.coin1;
            tbCostSell.Text = path.price;
            tbTotalSell.Text = path.coin2;
            lblCommSell.Content = CommRound(Convert.ToDouble(path.coin2) * 0.002).ToString("F8");
            lblTotalSell.Content = CommRound(Convert.ToDouble(path.coin2) - Convert.ToDouble(lblCommSell.Content)).ToString("F8");
        }

        private void orderSell_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OrderTable path = orderSell.SelectedItem as OrderTable;

            tbCountSell.Text = path.coin1;
            tbCostSell.Text = path.price;
            tbTotalSell.Text = path.coin2;
            lblCommSell.Content = CommRound(Convert.ToDouble(path.coin2) * 0.002).ToString("F8");
            lblTotalSell.Content = CommRound(Convert.ToDouble(path.coin2) - Convert.ToDouble(lblCommSell.Content)).ToString("F8");

            tbCountBuy.Text = path.coin1;
            tbCostBuy.Text = path.price;
            tbTotalBuy.Text = path.coin2;
            lblCommBuy.Content = CommRound(Convert.ToDouble(path.coin2) * 0.002).ToString("F8");
            lblTotalBuy.Content = CommRound(Convert.ToDouble(path.coin2) + Convert.ToDouble(lblCommBuy.Content)).ToString("F8");
        }

        static double CommRound(double x, int precision = 8)
        {
            return ((long)(x * Math.Pow(10.0, precision)) / Math.Pow(10.0, precision));
        }


        private void orderSell_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbTotalSell.Text != string.Empty && tbCostSell.Text != string.Empty && tbCountSell.Text != string.Empty)
            {
                tbTotalSell.Text = CommRound(Convert.ToDouble(tbCountSell.Text) * Convert.ToDouble(tbCostSell.Text)).ToString();
                lblCommSell.Content = CommRound(Convert.ToDouble(tbTotalSell.Text) * 0.002).ToString("F8");
                lblTotalSell.Content = CommRound(Convert.ToDouble(tbTotalSell.Text) + Convert.ToDouble(lblCommSell.Content)).ToString("F8");
            }
        }

        private void orderBuy_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbTotalBuy.Text != string.Empty && tbCostBuy.Text != string.Empty && tbCountBuy.Text != string.Empty)
            {
                tbTotalBuy.Text = CommRound(Convert.ToDouble(tbCountBuy.Text) * Convert.ToDouble(tbCostBuy.Text)).ToString();
                lblCommBuy.Content = CommRound(Convert.ToDouble(tbTotalBuy.Text) * 0.002).ToString("F8");
                lblTotalBuy.Content = CommRound(Convert.ToDouble(tbTotalBuy.Text) + Convert.ToDouble(lblCommBuy.Content)).ToString("F8");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (tbTotalBuy.Text != string.Empty && tbCostBuy.Text != string.Empty && tbCountBuy.Text != string.Empty)
                Trade(Properties.Settings.Default.pair, "buy", Convert.ToDouble(tbCostBuy.Text), Convert.ToDouble(tbCountBuy.Text));
            else MessageBox.Show("Заполните все необходимые поля.");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (tbTotalSell.Text != string.Empty && tbCostSell.Text != string.Empty && tbCountSell.Text != string.Empty)
                Trade(Properties.Settings.Default.pair, "sell", Convert.ToDouble(tbCostSell.Text), Convert.ToDouble(tbCountSell.Text));
            else MessageBox.Show("Заполните все необходимые поля.");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            string pair = Properties.Settings.Default.pair;
            double rate = Convert.ToDouble(tbCostBuy.Text);
            double amount = Convert.ToDouble(tbCountBuy.Text);
            double percent = rate * Convert.ToDouble(percentBox.Text)/100;
            if (Trade(pair, "buy", rate, amount))
             if (!Trade(pair, "sell", rate + percent, amount)) MessageBox.Show("Че то пошло не так, Саня, не продаются монеты, Саня, это жопа.");
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ActiveOrders(Properties.Settings.Default.pair);
        }

        async Task Refresh(object obj)
        {
            GetInfo();
            Depth(Properties.Settings.Default.pair);
        }
    }

    class OrderTable
    {
        public OrderTable(double price, double coin1, double coin2)
        {
            this.price = price.ToString("F8");
            this.coin1 = coin1.ToString("F8");
            this.coin2 = coin2.ToString("F8");
        }
        public string price { get; set; }
        public string coin1 { get; set; }
        public string coin2 { get; set; }
    }
}
