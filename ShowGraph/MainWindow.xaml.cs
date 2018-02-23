using RDotNet;
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

namespace ShowGraph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            REngine engine;
            string fileName;

            //init the R engine            
            REngine.SetEnvironmentVariables();
            engine = REngine.GetInstance();
            engine.Initialize();

            //prepare data
            List<int> size = new List<int>() { 29, 33, 51, 110, 357, 45, 338, 543, 132, 70, 103, 301, 146, 10, 56, 243, 238 };
            List<int> population = new List<int>() { 3162, 11142, 3834, 7305, 81890, 1339, 5414, 65697, 11280, 4589, 320, 60918, 480, 1806, 4267, 63228, 21327 };

            fileName = @"myplot.png";

            //calculate
            IntegerVector sizeVector = engine.CreateIntegerVector(size);
            engine.SetSymbol("size", sizeVector);

            IntegerVector populationVector = engine.CreateIntegerVector(population);
            engine.SetSymbol("population", populationVector);

            CharacterVector fileNameVector = engine.CreateCharacterVector(new[] { fileName });
            engine.SetSymbol("fileName", fileNameVector);

            engine.Evaluate("reg <- lm(population~size)");
            engine.Evaluate("png(filename=fileName, width=6, height=6, units='in', res=100)");
            engine.Evaluate("plot(population~size)");
            engine.Evaluate("abline(reg)");
            engine.Evaluate("dev.off()");

            //clean up
            engine.Dispose();
            var uri = new Uri(string.Format("pack://siteoforigin:,,,/{0}", fileName));
            var ImageSource = new BitmapImage(uri); // set the ImageSource property

            this.Graph.Source = ImageSource;
        }
    }
}
