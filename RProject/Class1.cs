using System;
using RDotNet;
using System.Collections.Generic;

namespace RProject
{
    public class aaa
    {
        public static void Main1()
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

            //output
            Console.WriteLine("");
            Console.WriteLine("Press any key to exit");
        }
    }
}