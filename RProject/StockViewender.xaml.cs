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
using System.Windows.Shapes;
using RDotNet;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;

namespace RProject
{
    /// <summary>
    /// Interaction logic for StockViewender.xaml
    /// </summary>
    public partial class StockViewender : Window
    {

        List<Companies> c_list = new List<Companies>();
        private ObservableCollection<Companies> allCompanies = new ObservableCollection<Companies>();
        REngine engine;
        public StockViewender()
        {
            InitializeComponent();


            string n = null, s = null, ind = null;
            string input;

            
            REngine.SetEnvironmentVariables();
            engine = REngine.GetInstance();
            engine.Initialize();
            input = "library(TTR)\n library(quantmod)\n all <- stockSymbols()";
            DataFrame obj;
            try
            {
                obj = engine.Evaluate(input).AsDataFrame();
            }
           
            catch
            {
                engine.Evaluate(@"install.packages(""TTR"")");

                engine.Evaluate(@"install.packages(""quantmod"")");

                obj = engine.Evaluate(input).AsDataFrame();
            }
           

            for (int i = 0; i < obj.RowCount; ++i)
            {
                for (int k = 0; k < obj.ColumnCount; ++k)
                {
                    if (obj.ColumnNames[k].Equals("Name"))
                    {
                        n = obj[i, k].ToString();
                    }
                    else if (obj.ColumnNames[k].Equals("Industry"))
                    {
                        ind = obj[i, k].ToString();
                    }
                    else if (obj.ColumnNames[k].Equals("Symbol"))
                    {
                        s = obj[i, k].ToString();
                    }
                                    
                }

                c_list.Add(new Companies(n, s, ind));

                
                
                
            }
        }


        private void Companies_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Companies c in c_list)
            {
                allCompanies.Add(c);
            }
            listView.ItemsSource = allCompanies;
            listView.Items.Refresh();
            //listView.SelectedIndex=0;
        }

        private void listView_Click(object sender, SelectionChangedEventArgs e)
        {
            
            Companies id = (Companies)e.AddedItems[0];
            string name = id.Symbol;
            if (listView.SelectedIndex != -1)
            {
                string date;
                string fileName = @"myplot2.png";

                Companies c = (Companies)listView.SelectedItem;

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("http://chart.finance.yahoo.com/table.csv?s={0}&ignore=.csv", c.Symbol));
                    request.Method = WebRequestMethods.Http.Head;
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    response.Close();
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        MessageBox.Show("שגיאה: מידע פננסי עבור החברה שנבחרה לא קיים.");
                        this.Graph.Source = null;
                        return;
                    }

                }

                catch
                {

                    MessageBox.Show("שגיאה: מידע פננסי עבור החברה שנבחרה לא קיים.");
                    this.Graph.Source = null;
                    return;
                }

                //REngine engine;
                REngine.SetEnvironmentVariables();
                //engine = REngine.GetInstance();
                engine.Initialize();

                List<string> dates = new List<string>();
                List<float> p_list = new List<float>();

                for (int i = 14; i > 0; i--)
                {
                    if (DateTime.Now.Subtract(new TimeSpan(i, 0, 0, 0)).DayOfWeek.ToString() != "Sunday" && DateTime.Now.Subtract(new TimeSpan(i, 0, 0, 0)).DayOfWeek.ToString() != "Saturday")
                    {
                        date = "" + DateTime.Now.Subtract(new TimeSpan(i, 0, 0, 0)).Year.ToString() + "-" + DateTime.Now.Subtract(new TimeSpan(i, 0, 0, 0)).Month.ToString() + "-" + DateTime.Now.Subtract(new TimeSpan(i, 0, 0, 0)).Day.ToString();
                        dates.Add(date);
                    }
                }
                

                string temp = "getYahooStockUrl <- function(symbol, start, end, type = \"d\") {\n start <- as.Date(start)\n end <- as.Date(end)\n url <-\"http://ichart.finance.yahoo.com/table.csv?s=%s&a=%d&b=%s&c=%s&d=%d&e=%s&f=%s&g=%s&ignore=.csv\"\n sprintf(url,\ntoupper(symbol),\nas.integer(format(start, \"%m\")) - 1,\nformat(start, \"%d\"),\nformat(start, \"%Y\"),\nas.integer(format(end, \"%m\")) - 1, \nformat(end, \"%d\"),\nformat(end, \"%Y\"),\ntype)} \ndata<- read.csv(getYahooStockUrl(\"" + c.Symbol + "\", \"" + dates.First().ToString() + "\", \"" + dates.Last().ToString() + "\"),\nstringsAsFactors = FALSE)";

                string point = null;
                DataFrame obj = engine.Evaluate(temp).AsDataFrame();

                for (int i = 0; i < obj.RowCount; ++i)
                {
                    for (int k = 0; k < obj.ColumnCount; ++k)
                    {
                        if (obj.ColumnNames[k].Equals("Close"))
                        {
                            point = obj[i, k].ToString();
                        }
                    }
                    p_list.Add(float.Parse(point, System.Globalization.CultureInfo.InvariantCulture));
                }
                p_list.Reverse();

                //engine.Evaluate("plot(data$Close,type=\"l\",col=\"red\")");
                /*
                                var filePath = Server.MapPath("~/Images/" + fileName);
                                if (File.Exists(filePath))
                                {
                                    File.Delete(filePath);
                                }
                */
                if (File.Exists(fileName))
                {
                    this.Graph.Source = null;
                    File.Delete(fileName);
                }

                CharacterVector fileNameVector = engine.CreateCharacterVector(new[] { fileName });
                engine.SetSymbol("fileName", fileNameVector);


                engine.Evaluate("png(filename=fileName, width=6, height=6, units='in', res=100)");
                engine.Evaluate("plot(data$Close)");
                engine.Evaluate("dev.off()");


                //engine.Dispose();
                var bitmap = new BitmapImage();
                var stream = File.OpenRead(fileName);
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                stream.Close();
                stream.Dispose();
                this.Graph.Source = bitmap;
                

                //MessageBox.Show(Convert.ToString(listView.SelectedIndices[0]));
            }
        }
    }
}
