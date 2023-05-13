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
using System.Reflection;
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
using static System.Net.Mime.MediaTypeNames;

namespace KindergardenFood.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Products.xaml
    /// </summary>
    public partial class Products : UserControl
    {
        public Products()
        {
            InitializeComponent();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() == true)
            {
                FileInfo file = new FileInfo(ofd.FileName);
                var lines = File.ReadAllLines(file.FullName).Select(x => x.Split(';')).ToArray();
                foreach(var line in lines)
                {
                    Food food = new Food()
                    {
                        Id = Convert.ToInt32(line[0]),
                        Title = line[1],
                    };
                    App._context.Food.AddOrUpdate(food);
                }
                try
                {
                    App._context.SaveChanges();

                }
                catch(DbEntityValidationException ex)
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
            var query = from food in App._context.Food
                        where food.Title.Contains(text)
                        select new
                        {
                            food.Id,
                            Title = food.Title.ToUpper(),
                        };
            MainView.ItemsSource = query.ToList();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Load(string.Empty);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Load(SearchTextBox.Text);
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            Load(string.Empty);
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            var page = new AddOrUpdate_Food(0);
            page.Show();
        }
        public int selectedid = 0;
        private void Edit_button_Click(object sender, RoutedEventArgs e)
        {
            var page = new AddOrUpdate_Food(selectedid);
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
            try
            {
                if (cell != null)
                {
                    var cell_content = cell.Column.GetCellContent(cell.Item);
                    selectedid = int.Parse(((TextBlock)cell_content).Text);
                }
            }
            catch
            {
                try
                {
                    MainView.UnselectAll();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            Food food = App._context.Food.Find(selectedid);
            var norms = App._context.Food_Norm.Where(x => x.Food_ID == food.Id).ToList();
            App._context.Food_Norm.RemoveRange(norms);
            App._context.Food.Remove(food);
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
