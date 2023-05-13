using KindergardenFood.Models;
using KindergardenFood.Pages;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KindergardenFood.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Norms.xaml
    /// </summary>
    public partial class Norms : UserControl
    {
        public Norms()
        {
            InitializeComponent();
        }
        private void Import_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                FileInfo file = new FileInfo(ofd.FileName);
                var lines = File.ReadAllLines(file.FullName).Select(x => x.Split(';')).ToArray();
                foreach (var line in lines)
                {
                    try
                    {
                        Food_Norm norm = new Food_Norm()
                        {
                            Category = Convert.ToInt32(line[0]),
                            Food_ID = Convert.ToInt32(line[1]),
                            Norm_value = Convert.ToDouble(line[2]),
                            Norm_date = Convert.ToDateTime(line[3])
                        };
                        App._context.Food_Norm.AddOrUpdate(norm);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    
                }
                try
                {
                    App._context.SaveChanges();

                }
                catch (DbEntityValidationException ex)
                {
                    foreach (DbEntityValidationResult validationError in ex.EntityValidationErrors)
                    {
                        Debug.WriteLine("Object: " + validationError.Entry.Entity.ToString());
                        foreach (DbValidationError err in validationError.ValidationErrors)
                        {
                            Debug.WriteLine(err.ErrorMessage);
                        }
                    }
                }

            }
        }
        private void Load(string text)
        {
            var query = from norms in App._context.Food_Norm
                        join food in App._context.Food on norms.Food_ID equals food.Id
                        join cats in App._context.Categories on norms.Category equals cats.Id
                        where (food.Title.Contains(text)
                        || cats.Category_Name.Contains(text))
                        && norms.Norm_date == (DateFilter.SelectedDate ?? norms.Norm_date)
                        select new
                        {
                            norms.Id,
                            norms.Norm_date,
                            Product = food.Title.ToUpper(),
                            Category = cats.Category_Name,
                            norms.Norm_value
                        };
            MainView.ItemsSource = query.ToList();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Load(string.Empty);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                MainView.UnselectAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            Load(SearchTextBox.Text);
        }

        private void DateFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                MainView.UnselectAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            Load(SearchTextBox.Text);
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            DateFilter.Text = string.Empty;
            Load(string.Empty);
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            var page = new AddOrUpdate_Norm(0);
            page.Show();
        }
        public int selectedid = 0;
        private void Edit_button_Click(object sender, RoutedEventArgs e)
        {
            var page = new AddOrUpdate_Norm(selectedid);
            page.Show();
            try
            {
                MainView.UnselectAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void MainView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cell = MainView.SelectedCells[0];
            if (cell != null)
            {
                var cell_content = cell.Column.GetCellContent(cell.Item);
                selectedid = int.Parse(((TextBlock)cell_content).Text);
            }
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            Food_Norm norm = App._context.Food_Norm.Find(selectedid);
            App._context.Food_Norm.Remove(norm);
            App._context.SaveChanges();
            try
            {
                MainView.UnselectAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            ResetFilters_Click(sender, e);
        }
    }
}
