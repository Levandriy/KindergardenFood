using KindergardenFood.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
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
using KindergardenFood.Classes;

namespace KindergardenFood.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddOrUpdate_Cooked.xaml
    /// </summary>
    public partial class AddOrUpdate_Cooked : Window
    {
        public AddOrUpdate_Cooked(int id)
        {
            InitializeComponent();
            view = new Views.Cooked_View();
            var check = from cooked in App._context.Cooked
                        where cooked.Id == id
                        select cooked;
            if (check.FirstOrDefault() != null)
            {
                var query = from cooked in App._context.Cooked
                            where cooked.Id == id
                            join food in App._context.Food on cooked.Food_ID equals food.Id
                            join cats in App._context.Categories on cooked.Category_ID equals cats.Id
                            join units in App._context.Units on cooked.Unit_ID equals units.Id
                            select new Views.Cooked_View()
                            {
                                Id = cooked.Id,
                                Date = cooked.Record_date,
                                Food = food.Title,
                                Unit = units.Title,
                                Category = cats.Category_Name,
                                Quantity = cooked.Quantity,
                                Price = cooked.Price,
                            };
                view = query.FirstOrDefault();
            }
            else
            {
                view.Id = 0;
                view.Date = DateTime.Now;
            }
            view.FoodList = App._context.Food.Select(x => x.Title).ToList();
            view.CategoryList = App._context.Categories.Select(x => x.Category_Name).ToList();
            view.UnitList = App._context.Units.Select(x => x.Title).ToList();
        }
        public Views.Cooked_View view { get; set; }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Cooked cooked = new Cooked()
            {
                Id = view.Id,
                Category_ID = App._context.Categories.Where(x => x.Category_Name == view.Category).Select(x => x.Id).FirstOrDefault(),
                Food_ID = App._context.Food.Where(x => x.Title == view.Food).Select(x => x.Id).FirstOrDefault(),
                Unit_ID = App._context.Units.Where(x => x.Title == view.Unit).Select(x => x.Id).FirstOrDefault(),
                Record_date = view.Date,
                Quantity = view.Quantity,
                Price = view.Price,
            };
            Debug.WriteLine(cooked.Id);
            Debug.WriteLine(cooked.Category_ID);
            Debug.WriteLine(cooked.Food_ID);
            Debug.WriteLine(cooked.Unit_ID);
            Debug.WriteLine(cooked.Record_date);
            Debug.WriteLine(cooked.Quantity);
            Debug.WriteLine(cooked.Price);
            App._context.Cooked.AddOrUpdate(cooked);
            App._context.SaveChanges();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainView.DataContext = view;
        }
    }
}
