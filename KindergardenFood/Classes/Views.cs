using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindergardenFood.Classes
{
    public class Views
    {
        public class Cooked_View
        {
            public int Id { get; set; }
            public DateTime? Date { get; set; }
            public string Food { get; set; }
            public string Category { get; set; }
            public string Unit { get; set; }
            public double Quantity { get; set; }
            public double Price { get; set; }
            public List<string> UnitList { get; set; }
            public List<string> FoodList { get; set; }
            public List<string> CategoryList { get; set; }
        }
        public class Eating_View
        {
            public int Id { get; set; }
            public DateTime? Date { get; set; }
            public string Category { get; set; }
            public int Quantity { get; set; }
            public List<string> CategoryList { get; set; }
        }
        public class Norm_View
        {
            public int Id { get; set; }
            public DateTime? Date { get; set; }
            public string Food { get; set; }
            public string Category { get; set; }
            public double Norm { get; set; }
            public List<string> FoodList { get; set; }
            public List<string> CategoryList { get; set; }
        }
        public class FoodView
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public double FirstNorm { get; set; }
            public double SecondNorm { get; set; }
            public int PortionsFirst { get; set; }
            public int PortionsSecond { get; set; }
        }
    }
}
