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
    /// Логика взаимодействия для AddOrUpdate_Eating.xaml
    /// </summary>
    public partial class AddOrUpdate_Eating : Window
    {
        public AddOrUpdate_Eating(int id)
        {
            InitializeComponent();
            view = new Views.Eating_View();
            var check = from eating in App._context.Kids_eating
                        where eating.Id == id
                        select eating;
            if (check.FirstOrDefault() != null)
            {
                var query = from eating in App._context.Kids_eating
                            where eating.Id == id
                            join cats in App._context.Categories on eating.Category equals cats.Id
                            select new Views.Eating_View()
                            {
                                Id = eating.Id,
                                Date = eating.Record_date,
                                Category = cats.Category_Name,
                                Quantity = eating.Quantity
                            };
                view = query.FirstOrDefault();
            }
            else
            {
                view.Id = 0;
                view.Date = DateTime.Now;
            }
            view.CategoryList = App._context.Categories.Select(x => x.Category_Name).ToList();
        }
        public Views.Eating_View view { get; set; }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Kids_eating eating = new Kids_eating()
            {
                Id = view.Id,
                Category = App._context.Categories.Where(x => x.Category_Name == view.Category).Select(x => x.Id).FirstOrDefault(),
                Record_date = view.Date,
                Quantity = view.Quantity,
            };
            Debug.WriteLine(eating.Id);
            Debug.WriteLine(eating.Category);
            Debug.WriteLine(eating.Record_date);
            Debug.WriteLine(eating.Quantity);
            App._context.Kids_eating.AddOrUpdate(eating);
            App._context.SaveChanges();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainView.DataContext = view;
        }
    }
}
