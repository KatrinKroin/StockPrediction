using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RProject
{
    class Companies
    {
        private string name;
        private string symbol;
        private string industry;

        public string Name { set { name = value; } get { return name; } }
        public string Symbol { set { symbol = value; } get { return symbol; } }
        public string Industry { set { industry = value; } get { return industry; } }



        public Companies(string name, string symbol, string industry)
        {
            this.name = name;
            this.symbol = symbol;
            this.industry = industry;
        }

    }
}
