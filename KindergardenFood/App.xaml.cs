using KindergardenFood.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace KindergardenFood
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static KindergardenFoodDataBaseEntities _context = new KindergardenFoodDataBaseEntities();
        public static DateTime? StartPeriod { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1);
        public static DateTime? EndPeriod { get; set; } = StartPeriod.Value.AddMonths(1).AddDays(-1);
    }
}
