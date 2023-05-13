using KindergardenFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindergardenFood.Classes
{
    public class Reports
    {
        public class SumReport
        {
            public Food Food { get; set; }
            public Units Unit { get; set; }
            public Cooked Cooked { get; set; }
            public double Quantity { get; set; }
            public double Price { get; set; }
            public double PerPerson_Quantity { get; set; }
            public double PerPerson_Price { get; set; }
            public double Norm { get; set; }
            public double Difference { get; set; }
        }
        public class NoGroupSumReport
        {
            public DateTime? Date { get; set; }
            public string Food { get; set; }
            public int Portions { get; set; }
            public string Unit { get; set; }
            public double Price { get; set; }
            public double Quantity { get; set; }
            public double Sum { get; set; }
        }
    }
}
