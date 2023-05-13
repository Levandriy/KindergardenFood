using KindergardenFood.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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

namespace KindergardenFood.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddOrUpdate_Food.xaml
    /// </summary>
    public partial class AddOrUpdate_Food : Window
    {
        public AddOrUpdate_Food(int id)
        {
            InitializeComponent();
            var check = from products in App._context.Food
                        where products.Id == id
                        select products;
            if (check.FirstOrDefault() != null )
            {
                Food = check.FirstOrDefault();
            }
            else
            {
                var query = from products in App._context.Food
                            orderby products.Id descending
                            select products.Id;
                Food = new Food();
                Food.Id = query.First() + 1;
            }
        }

        public Food Food { get; set; }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            App._context.Food.AddOrUpdate(Food);
            App._context.SaveChanges();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainView.DataContext = Food;
        }
    }
}
