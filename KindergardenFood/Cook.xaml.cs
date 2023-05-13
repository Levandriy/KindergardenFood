using KindergardenFood.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
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
using KindergardenFood.Pages;

namespace KindergardenFood
{
    /// <summary>
    /// Логика взаимодействия для Cook.xaml
    /// </summary>
    public partial class Cook : Window
    {
        public Cook()
        {
            InitializeComponent();
        }
        public List<Views.FoodView> View { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Load(string.Empty);
        }
        private void Load(string text)
        {
            View = new List<Views.FoodView>();
            var query = from food in App._context.Food
                        where food.Title.Contains(string.IsNullOrEmpty(text) ? food.Title : text)
                        select food;
            foreach (var food in query)
            {
                var fstnorm = from n in App._context.Food_Norm
                              where n.Food_ID == food.Id
                                && n.Category == 1
                              orderby n.Norm_date descending
                              select n.Norm_value;
                if (fstnorm.Any())
                {
                    var scndnorm = from n in App._context.Food_Norm
                                   where n.Food_ID == food.Id
                                       && n.Category == 2
                                   orderby n.Norm_date descending
                                   select n.Norm_value;
                    if (scndnorm.Any())
                    {
                        var pfirst = (from eating in App._context.Kids_eating
                                      where eating.Category == 1
                                      orderby eating.Record_date descending
                                      select eating.Quantity).FirstOrDefault();
                        var psecond = (from eating in App._context.Kids_eating
                                       where eating.Category == 2
                                       orderby eating.Record_date descending
                                       select eating.Quantity).FirstOrDefault();
                        Views.FoodView item = new Views.FoodView()
                        {
                            Id = food.Id,
                            Title = food.Title.ToUpper(),
                            FirstNorm = fstnorm.FirstOrDefault() * pfirst,
                            SecondNorm = scndnorm.FirstOrDefault() * psecond,
                            PortionsFirst = pfirst,
                            PortionsSecond = psecond
                        };
                        View.Add(item);
                    }
                }
            }
            DataPanel.ItemsSource = View;
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Load(SearchBox.Text);
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var obj = e.OriginalSource as FrameworkElement;
            if (obj != null)
            {
                if (obj.DataContext != null)
                {
                    var foodview = obj.DataContext as Views.FoodView;
                    var window = new FoodCook(foodview);
                    window.Show();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var window = new MainWindow();
            window.Show();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
