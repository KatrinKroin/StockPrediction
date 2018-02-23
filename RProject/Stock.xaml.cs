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
    /// Interaction logic for Stock.xaml
    /// </summary>
    public partial class Stock : Window
    {
        List<Companies> c_list = new List<Companies>();
        REngine engine;
        public Stock()
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

            

            lvUsers.ItemsSource = c_list;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvUsers.ItemsSource);
            view.Filter = UserFilter;


        }

        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(txtFilter.Text))
                return true;
            else
                return ((item as Companies).Symbol.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvUsers.ItemsSource).Refresh();
        }



        

        private void listView_Click(object sender, SelectionChangedEventArgs e)
        {

             // Companies id = (Companies)e.AddedItems[0];
           // string name = id.Symbol;
            if (lvUsers.SelectedIndex != -1)
            {
                string date;
                string fileName = @"myplot2.png";

                Companies c = (Companies)lvUsers.SelectedItem;

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("http://ichart.finance.yahoo.com/table.csv?s={0}&ignore=.csv", c.Symbol));
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
               // engine = REngine.GetInstance();
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

                DataFrame obj = engine.Evaluate(temp).AsDataFrame();


                //engine.Evaluate("plot(data$Close,type=\"l\",col=\"red\")");

                if (File.Exists(fileName))
                {
                    this.Graph.Source = null;
                    File.Delete(fileName);
                }

                //CharacterVector uVector = engine.CreateCharacterVector(u_date_list_h);
                engine.SetSymbol("date", obj["Date"]);

                //CharacterVector pVector = engine.CreateCharacterVector(help);
                engine.SetSymbol("value", obj["Close"]);

                //engine.SetSymbol("obj", obj);



                CharacterVector fileNameVector = engine.CreateCharacterVector(new[] { fileName });
                engine.SetSymbol("fileName", fileNameVector);


                engine.Evaluate("png(filename=fileName, width=6, height=6, units='in', res=100)");
                engine.Evaluate("date <- as.Date(date)");
                engine.Evaluate("family <- as.factor(data[,4])");
                engine.Evaluate("par(bg = \"transparent\")");
                engine.Evaluate("plot(value~date,pch=16, col=family, ann=FALSE)");
                obj.Close();
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

        private void date_Click(object sender, RoutedEventArgs e)
        {
            //<float> p_list_h = new List<float>();
            //List<string> u_date_list_h = new List<string>();
            List<string> help = new List<string>();
            if (lvUsers.SelectedIndex != -1)
            {
                Companies temp = (Companies)lvUsers.SelectedItem;

                Date window = new Date(temp.Symbol,0);
                window.Owner = Window.GetWindow(this);
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                //window.Owner.Hide();
                if (window.ShowDialog() != true)
                {
                    return;
                }
                //window.Owner.ShowDialog();


                //p_list_h = window.p_list;
                //u_date_list_h = window.u_date_list;
                DataFrame obj =window.obj;
                string fileName = @"myplot2.png";
                //foreach (float v in window.p_list) help.Add(v.ToString());
                //foreach (string v in window.u_date_list) u_date_list_h.Add(v.ToString().Substring(0,2));
                //REngine engine;
                REngine.SetEnvironmentVariables();
                // engine = REngine.GetInstance();
                engine.Initialize();

                //engine.Evaluate("plot(data$Close,type=\"l\",col=\"red\")");

                if (File.Exists(fileName))
                {
                    this.Graph.Source = null;
                    File.Delete(fileName);
                }


                //CharacterVector uVector = engine.CreateCharacterVector(u_date_list_h);
                engine.SetSymbol("date", obj["Date"]);

                //CharacterVector pVector = engine.CreateCharacterVector(help);
                engine.SetSymbol("value", obj["Close"]);

                //engine.SetSymbol("obj", obj);

                CharacterVector fileNameVector = engine.CreateCharacterVector(new[] { fileName });
                engine.SetSymbol("fileName", fileNameVector);


                engine.Evaluate("png(filename=fileName, width=6, height=6, units='in', res=100)");


                engine.Evaluate("date <- as.Date(date)");

                engine.Evaluate("value <- signif(value, digits = 6)");
                engine.Evaluate("family <- as.factor(data[,4])");
                engine.Evaluate("par(bg = \"transparent\")");
                try
                {
                    engine.Evaluate("plot(value~date,pch=16, col=family,xaxt='n', yaxt='n', ann=FALSE)");
                    engine.Evaluate("axis(1, at=date,labels=date, las=2)");
                    engine.Evaluate("axis(2, at=value,labels=value, las=2)");

                }

                catch //aa
                {
                    MessageBox.Show("שגיאה: אין נתונים בימים שנבחרו");
                    engine.Evaluate("dev.off()");
                    return;
                }



                //engine.Evaluate("barplot(value,names.arg=date,type='o',space=1,col=blues9[5])");
                //engine.Evaluate("plot(DF)");
                /*
                 * lm1 <-lm(y ~ log(x)+x, data=d)
lm2 <-lm(y ~ I(x^2)+x, data=d)
xvec <- seq(0,12,length=101)
plot(d)
lines(xvec,predict(lm1,data.frame(x=xvec)))
lines(xvec,predict(lm2,data.frame(x=xvec)))
                 * 
                 */

                /*
                engine.Evaluate("axis(1, at = NULL, labels = F)");

                engine.Evaluate("axis.Date(side = 1,date,format = \"%Y/%m/%d\"");
                engine.Evaluate("plot(date,value,xaxt=\"n\")");
                engine.Evaluate("text(x = dm$Date, par(\"usr\")[3] * .97, labels = paste(dm$Date, \' \'), srt = 45, pos = 1, xpd = TRUE, cex = .7)");
                */

                engine.Evaluate("dev.off()");


                /*
                  
                 dm <- read.table(textConnection(Lines), header = TRUE)
                 dm$Date <- as.Date(dm$Date, "%m/%d/%Y")
                 plot(Visits ~ Date, dm, xaxt = "n", type = "l")
                 axis(1, at = NULL, labels = F)
                 text(x = dm$Date, par("usr")[3] * .97, labels = paste(dm$Date, ' '), srt = 45, pos = 1, xpd = TRUE, cex = .7) 
                 
                 */

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

            }
        }

        private void reg_Click(object sender, RoutedEventArgs e)
        {
            List<string> help = new List<string>();
            if (lvUsers.SelectedIndex != -1)
            {
                Companies temp = (Companies)lvUsers.SelectedItem;

                Date window = new Date(temp.Symbol, 1);
                window.Owner = Window.GetWindow(this);
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                if(window.ShowDialog() != true)
                {
                    return;
                }
                //window.Owner.ShowDialog();

                DataFrame obj = window.obj;
                string fileName = @"myplot2.png";
                REngine.SetEnvironmentVariables();
                engine.Initialize();

                if (File.Exists(fileName))
                {
                    this.Graph.Source = null;
                    File.Delete(fileName);
                }

                engine.SetSymbol("date", obj["Date"]);
                engine.SetSymbol("value", obj["Close"]);


                CharacterVector fileNameVector = engine.CreateCharacterVector(new[] { fileName });
                engine.SetSymbol("fileName", fileNameVector);


                engine.Evaluate("png(filename=fileName, width=6, height=6, units='in', res=100)");

                try
                {
                    engine.Evaluate("library(lubridate)");
                }

                catch
                {
                    engine.Evaluate(@"install.packages(""lubridate"")");
                    engine.Evaluate("library(lubridate)");
                }
                
                engine.Evaluate("dates <- as.numeric(ymd(date))"); //2007-03-1
                engine.Evaluate("try <- data.frame(x = dates, y = value)");
                engine.Evaluate("stackloss.lm <- lm(formula = y ~ x, data = try)");
                engine.Evaluate("z <- as.numeric(dmy(user))");
                engine.Evaluate("newdata <- data.frame(x = z)");
                engine.Evaluate("result<-predict(stackloss.lm, newdata)");
                engine.Evaluate("user2 <- as.Date(user2)");
                engine.Evaluate(@"result");
                engine.Evaluate(@"user");
                engine.Evaluate(@"user2");
                engine.Evaluate("result <- signif(result, digits = 6)");
                engine.Evaluate(@"result");
                engine.Evaluate("family <- as.factor(data[,4])");
                engine.Evaluate("par(bg = \"transparent\")");
                try
                {
                    engine.Evaluate("plot(result~user2,pch=16, col=family,xaxt='n', yaxt='n', ann=FALSE)");
                    engine.Evaluate("axis(1, at=user2,labels=user2, las=2)");
                    engine.Evaluate("axis(2, at=result,labels=result, las=2)");

                }

                catch //aa
                {
                    MessageBox.Show("שגיאה: אין נתונים בימים שנבחרו");
                    engine.Evaluate("dev.off()");
                    return;
                }
                //engine.Evaluate("barplot(value,names.arg=date,type='o',space=1,col=blues9[5])");
                //engine.Evaluate("plot(DF)");


                /*
                 * 
                 library(lubridate)
                  date <- c("11/1/2010", " 11/2/2010", "11/3/2010", "11/4/2010", "11/5/2010")
                  size <- c(1, 2, 3, 4, 5)
                  dates <- as.numeric(dmy(date))



                  try <- data.frame(x = dates, y = size)
                  stackloss.lm <- lm(formula = y ~ poly(x, 3), data = try)
                  z <- as.numeric(dmy(c("11/7/2010", "11/6/2010"," 11/8/2010")))
                  newdata <- data.frame(x = z)
                  predict(stackloss.lm, newdata)
                 * 
                 * 
                 * 
                 * 
                 * 
                 * 
                 * 
                engine.Evaluate("axis(1, at = NULL, labels = F)");

                engine.Evaluate("axis.Date(side = 1,date,format = \"%Y/%m/%d\"");
                engine.Evaluate("plot(date,value,xaxt=\"n\")");
                engine.Evaluate("text(x = dm$Date, par(\"usr\")[3] * .97, labels = paste(dm$Date, \' \'), srt = 45, pos = 1, xpd = TRUE, cex = .7)");
                */
                engine.Evaluate("dev.off()");




                /*
                  
                 dm <- read.table(textConnection(Lines), header = TRUE)
                 dm$Date <- as.Date(dm$Date, "%m/%d/%Y")
                 plot(Visits ~ Date, dm, xaxt = "n", type = "l")
                 axis(1, at = NULL, labels = F)
                 text(x = dm$Date, par("usr")[3] * .97, labels = paste(dm$Date, ' '), srt = 45, pos = 1, xpd = TRUE, cex = .7) 
                 
                 */
                var bitmap = new BitmapImage();
                var stream = File.OpenRead(fileName);
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                stream.Close();
                stream.Dispose();
                this.Graph.Source = bitmap;
            }
        }
    }
}
