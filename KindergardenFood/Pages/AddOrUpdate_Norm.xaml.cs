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
    /// Логика взаимодействия для AddOrUpdate_Norm.xaml
    /// </summary>
    public partial class AddOrUpdate_Norm : Window
    {
        public AddOrUpdate_Norm(int id)
        {
            InitializeComponent();
            view = new Views.Norm_View();
            var check = from norms in App._context.Food_Norm
                        where norms.Id == id
                        select norms;
            if (check.FirstOrDefault() != null)
            {
                var query = from norms in App._context.Food_Norm
                            where norms.Id == id
                            join food in App._context.Food on norms.Food_ID equals food.Id
                            join cats in App._context.Categories on norms.Category equals cats.Id
                            select new Views.Norm_View()
                            {
                                Id = norms.Id,
                                Food = food.Title,
                                Category = cats.Category_Name,
                                Norm = norms.Norm_value,
                                Date = norms.Norm_date
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
        }
        public Views.Norm_View view { get; set; }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Food_Norm norm = new Food_Norm()
            {
                Id = view.Id,
                Category = App._context.Categories.Where(x => x.Category_Name == view.Category).Select(x => x.Id).FirstOrDefault(),
                Food_ID = App._context.Food.Where(x => x.Title == view.Food).Select(x => x.Id).FirstOrDefault(),
                Norm_value = view.Norm,
                Norm_date = view.Date
            };
            Debug.WriteLine(norm.Id);
            Debug.WriteLine(norm.Category);
            Debug.WriteLine(norm.Food_ID);
            Debug.WriteLine(norm.Norm_value);
            Debug.WriteLine(norm.Norm_date);
            App._context.Food_Norm.AddOrUpdate(norm);
            App._context.SaveChanges();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainView.DataContext = view;
        }
    }
}
