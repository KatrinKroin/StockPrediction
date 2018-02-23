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

namespace RProject
{
    /// <summary>
    /// Interaction logic for Date.xaml
    /// </summary>
    public partial class Date : Window
    {
        string c_name;
        public List<float> p_list = new List<float>();
        public List<string> u_date_list = new List<string>();
        public DataFrame obj;
        int flag;

        public Date(string c_name,int f)
        {
            this.c_name = c_name;
            InitializeComponent();
            flag = f;

            if (flag == 0)
            {
                c1.DisplayDateStart = DateTime.MinValue;
                c1.DisplayDateEnd = DateTime.Today;
            }
            else
            {
                DateTime temp = new DateTime(2007,1,1);
                c1.DisplayDateStart = temp;
            }
            
        }
        /*
        private void B3_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void B4_Click(object sender, RoutedEventArgs e)
        {
            string start = null, end = null;
            int counter = 0;
            if (c1.SelectedDate.HasValue)
            {
                REngine engine;
                REngine.SetEnvironmentVariables();
                engine = REngine.GetInstance();
                engine.Initialize();

                foreach (DateTime d in c1.SelectedDates)
                {
                    if (counter == 0)
                    {
                        start = "" + d.Year.ToString() + "-" + d.Month.ToString() + "-" + d.Day.ToString();
                        u_date_list.Add(start);
                    }
                    else
                    {
                        end = "" + d.Year.ToString() + "-" + d.Month.ToString() + "-" + d.Day.ToString();
                        u_date_list.Add(end);
                    }

                    counter++;
                }


                //string symbol = "" + "library(TTR)\n library(quantmod)\n all <- getSymbols( \"" + c_name + "\", src = \"yahoo\", from = \"" + start + "\", to = \"" + end + "\")";

                string temp = "getYahooStockUrl <- function(symbol, start, end, type = \"d\") {\n start <- as.Date(start)\n end <- as.Date(end)\n url <-\"http://ichart.finance.yahoo.com/table.csv?s=%s&a=%d&b=%s&c=%s&d=%d&e=%s&f=%s&g=%s&ignore=.csv\"\n sprintf(url,\ntoupper(symbol),\nas.integer(format(start, \"%m\")) - 1,\nformat(start, \"%d\"),\nformat(start, \"%Y\"),\nas.integer(format(end, \"%m\")) - 1, \nformat(end, \"%d\"),\nformat(end, \"%Y\"),\ntype)} \ndata<- read.csv(getYahooStockUrl(\"" + c_name + "\", \"1000-1-1\", \"" + start + "\"),\nstringsAsFactors = FALSE)";

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
            }

            this.Close();
        }
        */

        private void User_Click(object sender, SelectionChangedEventArgs e)
        {
            string start = null, end = null;
            //int counter = 0;
            if (c1.SelectedDate.HasValue)
            {
                REngine engine;
                REngine.SetEnvironmentVariables();
                engine = REngine.GetInstance();
                engine.Initialize();

                
                SelectedDatesCollection dates = c1.SelectedDates;
                if(flag == 0) {
                    start = dates.First().ToString("yyyy-MM-dd");
                    end = dates.Last().ToString("yyyy-MM-dd");
                }

                else
                {
                    if(c1.SelectedDates.First() > DateTime.Now)
                    {
                        DateTime strdate = DateTime.Today.AddDays(-14);
                        //DateTime strdate = new DateTime(1900, 01, 01);
                        start = strdate.ToString("yyyy-MM-dd");
                        end = dates.First().ToString("yyyy-MM-dd");
                    }

                    else
                    {
                        DateTime strdate = c1.SelectedDates.First().AddDays(-14);
                        start = strdate.ToString("yyyy-MM-dd");
                        end = dates.First().ToString("yyyy-MM-dd");
                    }
                }


                //foreach (DateTime d in c1.SelectedDates)
                //{
                //    if (counter == 0)
                //    {
                //        start = "" + d.Year.ToString() + "-" + d.Month.ToString() + "-" + d.Day.ToString();
                //        if (d.DayOfWeek.ToString().Equals("Sunday") || d.DayOfWeek.ToString().Equals("Saturday")) ;
                //        else
                //            u_date_list.Add(start);
                //    }
                //    else
                //    {
                //        end = "" + d.Year.ToString() + "-" + d.Month.ToString() + "-" + d.Day.ToString();
                //        if (d.DayOfWeek.ToString().Equals("Sunday") || d.DayOfWeek.ToString().Equals("Saturday")) ;
                //        else
                //            u_date_list.Add(end);
                //    }

                //    counter++;
                //}


                //string symbol = "" + "library(TTR)\n library(quantmod)\n all <- getSymbols( \"" + c_name + "\", src = \"yahoo\", from = \"" + start + "\", to = \"" + end + "\")";

                string temp = "getYahooStockUrl <- function(symbol, start, end, type = \"d\") {\n start <- as.Date(start)\n end <- as.Date(end)\n url <-\"http://ichart.finance.yahoo.com/table.csv?s=%s&ignore=.csv\"\n sprintf(url,\ntoupper(symbol),\nas.integer(format(start, \"%m\")) - 1,\nformat(start, \"%d\"),\nformat(start, \"%Y\"),\nas.integer(format(end, \"%m\")) - 1, \nformat(end, \"%d\"),\nformat(end, \"%Y\"),\ntype)} \ndata<- read.csv(getYahooStockUrl(\"" + c_name + "\", \"" + start + "\", \"" + end + "\"),\nstringsAsFactors = FALSE)";
                //    string temp = "getYahooStockUrl <- function(symbol, start, end, type = \"d\") {\n start <- as.Date(start)\n end <- as.Date(end)\n url <-\"http://ichart.finance.yahoo.com/table.csv?s=%s&a=%d&b=%s&c=%s&d=%d&e=%s&f=%s&g=%s&ignore=.csv\"\n sprintf(url,\ntoupper(symbol),\nas.integer(format(start, \"%m\")) - 1,\nformat(start, \"%d\"),\nformat(start, \"%Y\"),\nas.integer(format(end, \"%m\")) - 1, \nformat(end, \"%d\"),\nformat(end, \"%Y\"),\ntype)} \ndata<- read.csv(getYahooStockUrl(\"" + c_name + "\", \"1000-1-1\", \"" + start + "\"),\nstringsAsFactors = FALSE)";
                //string point = null;
                string tmp = "";
                foreach (DateTime d in c1.SelectedDates)
                    tmp = tmp + '"' + d.ToString("dd/MM/yyyy") + '"' + " , ";
                tmp = tmp.Substring(0, tmp.Length - 2);

                obj = engine.Evaluate(temp).AsDataFrame();
                engine.Evaluate("user <- c(" + tmp + ")");
                tmp = "";
                foreach (DateTime d in c1.SelectedDates)
                    tmp = tmp + '"' + d.ToString("yyyy-MM-dd") + '"' + " , ";
                tmp = tmp.Substring(0, tmp.Length - 2);
                engine.Evaluate("user2 <- (c(" + tmp + "))");




                obj = engine.Evaluate(temp).AsDataFrame();
                engine.SetSymbol("data", obj);
                temp = "data[data$Date >= \"" + start + "\"& data$Date <= \"" + end + "\",]";
                obj = engine.Evaluate(temp).AsDataFrame();
                //DataFrame obj = engine.Evaluate(temp).AsDataFrame();

                //for (int i = 0; i < obj.RowCount; ++i)
                //{
                //    for (int k = 0; k < obj.ColumnCount; ++k)
                //    {
                //        if (obj.ColumnNames[k].Equals("Close"))
                //        {
                //            point = obj[i, k].ToString();
                //        }
                //    }
                //    p_list.Add(float.Parse(point, System.Globalization.CultureInfo.InvariantCulture));
                //}
            }



            this.DialogResult = true;
            //p_list.Reverse();
            this.Close();
        }
    }
}
