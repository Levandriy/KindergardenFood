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
    /// Логика взаимодействия для Cooked.xaml
    /// </summary>
    public partial class Cooked : UserControl
    {
        public Cooked()
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
                        Models.Cooked cooked = new Models.Cooked()
                        {
                            Record_date = Convert.ToDateTime(line[0]),
                            Food_ID = Convert.ToInt32(line[1]),
                            Unit_ID = Convert.ToInt32(line[2]),
                            Quantity = Convert.ToDouble(line[3]),
                            Price = Convert.ToDouble(line[4]),
                            Category_ID = Convert.ToInt32(line[5]),
                        };
                        App._context.Cooked.AddOrUpdate(cooked);
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
            var query = from cooked in App._context.Cooked
                        join food in App._context.Food on cooked.Food_ID equals food.Id
                        join cats in App._context.Categories on cooked.Category_ID equals cats.Id
                        join units in App._context.Units on cooked.Unit_ID equals units.Id
                        where (food.Title.Contains(text)
                        || cats.Category_Name.Contains(text)
                        || units.Title.Contains(text))
                        && cooked.Record_date == (DateFilter.SelectedDate ?? cooked.Record_date)
                        select new
                        {
                            cooked.Id,
                            cooked.Record_date,
                            Product = food.Title.ToUpper(),
                            cooked.Quantity,
                            cooked.Price,
                            Unit = units.Title,
                            Category = cats.Category_Name,
                            Sum = cooked.Quantity * cooked.Price,
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

        private void DateFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Load(SearchTextBox.Text);
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SearchTextBox.Text = string.Empty;
                DateFilter.Text = string.Empty;
                Load(string.Empty);
            }
            catch
            {

            }
        }
        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            var page = new AddOrUpdate_Cooked(0);
            page.Show();
        }
        public int selectedid = 0;
        private void Edit_button_Click(object sender, RoutedEventArgs e)
        {
            var page = new AddOrUpdate_Cooked(selectedid);
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
            Models.Cooked cooked = App._context.Cooked.Find(selectedid);
            App._context.Cooked.Remove(cooked);
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
