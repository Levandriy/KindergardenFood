using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для FoodCook.xaml
    /// </summary>
    public partial class FoodCook : Window
    {
        public FoodCook(Views.FoodView view)
        {
            InitializeComponent();
            View = view;
        }
        public Views.FoodView View { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Element.DataContext = View;
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
