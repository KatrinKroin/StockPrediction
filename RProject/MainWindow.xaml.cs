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
using RDotNet;
using RDotNet.NativeLibrary;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace RProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Companies> c_list = new List<Companies>();
        private ObservableCollection<Companies> allCompanies = new ObservableCollection<Companies>();

        public MainWindow()
        {
            InitializeComponent();

            REngine engine;

            REngine.SetEnvironmentVariables();
            engine = REngine.GetInstance();
            engine.Initialize();

            //input = "library(TTR)\n library(quantmod)\n all <- stockSymbols()";
            //DataFrame obj = engine.Evaluate(input).AsDataFrame();

            //for (int i = 0; i < obj.RowCount; ++i)
            //{
            //    for (int k = 0; k < obj.ColumnCount; ++k)
            //    {
            //        if (obj.ColumnNames[k].Equals("Name"))
            //        {
            //            n = obj[i, k].ToString();
            //        }
            //        else if (obj.ColumnNames[k].Equals("Industry"))
            //        {
            //            ind = obj[i, k].ToString();
            //        }
            //        else if (obj.ColumnNames[k].Equals("Symbol"))
            //        {
            //            s = obj[i, k].ToString();
            //        }
            //    }
            //    c_list.Add(new Companies(n, s, ind));
            //}

            List<int> size = new List<int>() { 29, 33, 51, 110, 357, 45, 338, 543, 132, 70, 103, 301, 146, 10, 56, 243, 238 };
            List<int> population = new List<int>() { 3162, 11142, 3834, 7305, 81890, 1339, 5414, 65697, 11280, 4589, 320, 60918, 480, 1806, 4267, 63228, 21327 };



            //c_list.Sort();



            /*

            string result;
            string input;
            REngine engine;

            //init the R engine            
            REngine.SetEnvironmentVariables();
            engine = REngine.GetInstance();
            engine.Initialize();

            //input
            Console.WriteLine("Please enter the calculation");
            input = "2+3";

            //calculate
            CharacterVector vector = engine.Evaluate(input).AsCharacter();

            //shell(R CMD BATCH myRprogram.R)
            //Process.Start("script.R");

            //engine.Evaluate("foo()");
            string temp = "getYahooStockUrl <- function(symbol, start, end, type = \"d\") {\n start <- as.Date(start)\n end <- as.Date(end)\n url <-\"http://ichart.finance.yahoo.com/table.csv?s=%s&a=%d&b=%s&c=%s&d=%d&e=%s&f=%s&g=%s&ignore=.csv\"\n sprintf(url,\ntoupper(symbol),\nas.integer(format(start, \"%m\")) - 1,\nformat(start, \"%d\"),\nformat(start, \"%Y\"),\nas.integer(format(end, \"%m\")) - 1, \nformat(end, \"%d\"),\nformat(end, \"%Y\"),\ntype)} \ndata<- read.csv(getYahooStockUrl(\"sbux\", \"2008-1-2\", \"2008-12-31\"),\nstringsAsFactors = FALSE) \n data$High";

            //string temp = "dat<-as.Date(\"2008-12-31\")\n"+"class(dat)\n plot(dat)";
            //string temp = "dat<- 8+5 \n"+"dat";
            //string temp="";
            Console.WriteLine("!!    " + engine.Evaluate(temp).AsCharacter()[0] + "    !!" + engine.Evaluate(temp).AsCharacter()[1]);
        //engine.Evaluate("plot(data$High,type=\"l\",col=\"red\")");
        //gr.Navigate(engine.Evaluate("plot(data$High,type=\"l\",col=\"red\")"));
       // result = vector[0];
        //clean up
        engine.Dispose();
        //output
        Console.WriteLine("");
        Console.WriteLine("Result: '{0}'", result);
        Console.WriteLine("Press any key to exit");
        //Console.ReadKey();
        */
        }

        private void Companies_Loaded(object sender,RoutedEventArgs e)
        {
            foreach(Companies c in c_list)
            {
                allCompanies.Add(c);
            }
            listView.ItemsSource = allCompanies;
            listView.Items.Refresh();
        }
        private void butt_Click(object sender, RoutedEventArgs e)
        {
            //engine.Evaluate(&quot; plot(rnorm(100)) & quot;);
        }

        private void B1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void B2_Click(object sender, RoutedEventArgs e)
        {
            if(listView.SelectedIndex != -1)
            {

                Companies temp = (Companies)listView.SelectedItem;

                Date window = new Date(temp.Symbol,0);
                window.Owner = Window.GetWindow(this);
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.Owner.Hide();
                window.ShowDialog();
                window.Owner.ShowDialog();



                //CharacterVector vector = engine.Evaluate(symbol).AsCharacter();


                /*
                 input = "library(TTR)\n library(quantmod)\n all <- stockSymbols()";
                DataFrame obj = engine.Evaluate(input).AsDataFrame();

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

                 */
            }
        }
    }
}
