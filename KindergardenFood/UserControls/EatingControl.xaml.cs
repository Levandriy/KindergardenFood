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
using static System.Net.Mime.MediaTypeNames;

namespace KindergardenFood.UserControls
{
    /// <summary>
    /// Логика взаимодействия для EatingControl.xaml
    /// </summary>
    public partial class EatingControl : UserControl
    {
        public EatingControl()
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
                        if (line[0].Length > 0)
                        {
                            Kids_eating eating = new Kids_eating()
                            {
                                Record_date = DateTime.Parse(line[0]),
                                Quantity = int.Parse(line[1]),
                                Category = int.Parse(line[2])
                            };
                            App._context.Kids_eating.AddOrUpdate(eating);
                        }
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
            var query = from eating in App._context.Kids_eating
                       join cats in App._context.Categories on eating.Category equals cats.Id
                       where cats.Category_Name.Contains(text)
                       && eating.Record_date == (DateFilter.SelectedDate ?? eating.Record_date)
                        select new
                        {
                            eating.Id,
                            Category = cats.Category_Name,
                            eating.Quantity,
                            eating.Record_date
                        };
            MainView.ItemsSource = query.ToList();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Load(SearchTextBox.Text);
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
            SearchTextBox.Text = string.Empty;
            DateFilter.Text = string.Empty;
            Load(string.Empty);
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            var page = new AddOrUpdate_Eating(0);
            page.Show();
        }
        public int selectedid = 0;
        private void Edit_button_Click(object sender, RoutedEventArgs e)
        {
            var page = new AddOrUpdate_Eating(selectedid);
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
            Kids_eating eating = App._context.Kids_eating.Find(selectedid);
            App._context.Kids_eating.Remove(eating);
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
