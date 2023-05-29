using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KindergardenFood.Models;
using KindergardenFood.UserControls;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System.Windows.Ink;
using System.IO;
using KindergardenFood.Classes;

namespace KindergardenFood
{
    /// <summary>
    /// Логика взаимодействия для Account.xaml
    /// </summary>
    public partial class Account : Window
    {
        public Account()
        {
            InitializeComponent();
        }
        private void HeaderButtonsClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in HeaderButtons.Children)
            {
                if (item is ToggleButton)
                {
                    var local = item as ToggleButton;
                    if (local != sender as ToggleButton)
                    {
                        local.IsChecked = false;
                    }
                }
            }
            WelcomeTB.Visibility = Visibility.Collapsed;
        }
        private void HeaderButtonsUnchecked(object sender, RoutedEventArgs e)
        {
            //ContentController.Content = new FrameworkElement();
        }
        public List<Reports.NoGroupSumReport> noGroup { get; set; }
        public List<Reports.SumReport> sumReports { get; set; }

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            App.StartPeriod = StartDatePicker.SelectedDate;
        }

        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            App.EndPeriod = EndDatePicker.SelectedDate;
        }
        private void SumReport_NoGroupingButton_Click(object sender, RoutedEventArgs e)
        {
            noGroup = new List<Reports.NoGroupSumReport> { };
            //Запрос
            var query = from cooked in App._context.Cooked
                        where cooked.Record_date >= App.StartPeriod
                        && cooked.Record_date <= App.EndPeriod
                        join food in App._context.Food on cooked.Food_ID equals food.Id
                        join units in App._context.Units on cooked.Unit_ID equals units.Id
                        join eating in App._context.Kids_eating on cooked.Category_ID equals eating.Category
                        where
                        (eating.Id == (from kids in App._context.Kids_eating
                                                where kids.Record_date <= cooked.Record_date
                                                && eating.Category == cooked.Category_ID
                                                orderby kids.Record_date descending
                                                select kids.Id).FirstOrDefault())
                        select new Reports.NoGroupSumReport()
                        {
                            Date = cooked.Record_date,
                            Food = food.Title,
                            Portions = eating.Quantity,
                            Unit = units.Title,
                            Price = cooked.Price,
                            Quantity = cooked.Quantity,
                            Sum = cooked.Price * cooked.Quantity
                        };
            noGroup = query.ToList();
            int totallines = noGroup.Count;
            //Подсчёт суммы
            double sum = noGroup.Sum(x => x.Sum);
            Debug.WriteLine(sum);
            Debug.WriteLine(totallines);


            //Работа с Excel
            SaveFileDialog Report = new SaveFileDialog()
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm",
                RestoreDirectory = false
            };
            if (Report.ShowDialog() == true)
            {
                Excel.Application app = new Excel.Application
                {
                    DisplayAlerts = false,
                    SheetsInNewWorkbook = 1
                };
                Excel.Workbook wb = app.Workbooks.Add();
                Excel.Worksheet sheet = (Excel.Worksheet)app.Worksheets.get_Item(1);
                Debug.WriteLine($"Накопительная за {DateTime.Now: dd.MM.yyyy}");
                sheet.Name = $"Накопительная за {DateTime.Now: dd.MM.yyyy}";
                //Шапка таблицы
                sheet.Cells[1, 1] = $"Накопительная ведомость по продуктам питания";
                sheet.Range[sheet.Cells[1, 1], sheet.Cells[1, 7]].Merge();
                sheet.Cells[2, 1] = $"период с {App.StartPeriod: dd.MM.yyyy} по {App.EndPeriod: dd.MM.yyyy}";
                sheet.Range[sheet.Cells[2, 1], sheet.Cells[2, 7]].Merge();
                sheet.Cells[4, 1] = "Учреждение: МБДОУ №2 г. Азова";
                sheet.Range[sheet.Cells[4, 1], sheet.Cells[4, 3]].Merge();
                sheet.Cells[5, 1] = "Структруное подразделение МБДОУ №2 г.Азова";
                sheet.Range[sheet.Cells[5, 1], sheet.Cells[5, 3]].Merge();
                sheet.Cells[6, 1] = "МОЛ Прохорова Л.В.";
                sheet.Range[sheet.Cells[6, 1], sheet.Cells[6, 3]].Merge();
                sheet.Cells[7, 1] = "Единица измерения: руб.";
                sheet.Range[sheet.Cells[7, 1], sheet.Cells[7, 3]].Merge();
                sheet.Cells[9, 1] = "Дата";
                sheet.Cells[9, 2] = "Продукт";
                sheet.Cells[9, 3] = "Кол-во порций";
                sheet.Cells[9, 4] = "Ед. изм.";
                sheet.Cells[9, 5] = "Цена";
                sheet.Cells[9, 6] = "Кол-во";
                sheet.Cells[9, 7] = "Сумма";
                Excel.Range header = sheet.Range[sheet.Cells[1, 1], sheet.Cells[1, 7]];
                header.Font.Bold = true;
                Excel.Range header2 = sheet.Range[sheet.Cells[1, 1], sheet.Cells[2, 7]];
                header2.Font.Size = 16;
                header2.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                header2.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                var arr = query.ToArray();
                for (int i = 0; i < arr.Count(); i++)
                {
                    sheet.Cells[10 + i, 1] = arr[i].Date;
                    sheet.Cells[10 + i, 2] = arr[i].Food.ToUpper();
                    sheet.Cells[10 + i, 3] = arr[i].Portions;
                    sheet.Cells[10 + i, 4] = arr[i].Unit.ToUpper();
                    sheet.Cells[10 + i, 5] = arr[i].Price;
                    sheet.Cells[10 + i, 6] = arr[i].Quantity;
                    sheet.Cells[10 + i, 7] = arr[i].Sum;
                }
                sheet.Cells[arr.Count() + 10, 1] = "Итого";
                sheet.Range[sheet.Cells[arr.Count() + 10, 1], sheet.Cells[arr.Count() + 10, 6]].Merge();
                sheet.Cells[arr.Count() + 10, 7] = sum;
                Excel.Range values = sheet.Range[sheet.Cells[9, 1], sheet.Cells[arr.Count() + 10, 7]];
                values.EntireColumn.AutoFit();
                values.Font.Size = 14;
                values.Borders.Weight = 2;
                wb.SaveAs(Report.FileName);
                wb.Close();
            }
        }
        private void SumReportButton_Click(object sender, RoutedEventArgs e)
        {
            sumReports = new List<Reports.SumReport> { };
            string Cat = ((ComboBoxItem)SelectedCat.SelectedItem).Content.ToString();
            int kids_days = 0;
            int Days = (from cooked in App._context.Cooked
                        join cats in App._context.Categories on cooked.Category_ID equals cats.Id
                        where cooked.Record_date >= App.StartPeriod
                        && cooked.Record_date <= App.EndPeriod
                        && cats.Category_Name == Cat
                        group cooked by cooked.Record_date).Count();
            var CookedQuery = from cooked in App._context.Cooked
                        where cooked.Record_date >= App.StartPeriod
                        && cooked.Record_date <= App.EndPeriod
                        join cats in App._context.Categories on cooked.Category_ID equals cats.Id
                        where cats.Category_Name == Cat
                        join food in App._context.Food on cooked.Food_ID equals food.Id
                        join units in App._context.Units on cooked.Unit_ID equals units.Id
                        select new Reports.SumReport()
                        {
                            Food = food,
                            Unit = units,
                            Cooked = cooked,
                        };
            foreach (var line in CookedQuery)
            {
                var Kids = (from kids in App._context.Kids_eating
                           where kids.Category == line.Cooked.Category_ID
                           && kids.Record_date <= line.Cooked.Record_date
                           select kids).FirstOrDefault();
                kids_days += Kids.Quantity;
                var Norms = (from norms in App._context.Food_Norm
                             where norms.Category == line.Cooked.Category_ID
                             && norms.Food_ID == line.Cooked.Food_ID
                             && norms.Norm_date <= line.Cooked.Record_date
                             select norms).FirstOrDefault();
                if(Norms != null)
                {
                    line.Norm = Norms.Norm_value;
                    line.Quantity = line.Cooked.Quantity;
                    line.Price = line.Cooked.Price * line.Quantity;
                    line.PerPerson_Quantity = line.Cooked.Quantity / (Kids.Quantity * Days);
                    line.PerPerson_Price = line.Cooked.Price * line.PerPerson_Quantity;
                    line.Difference = line.PerPerson_Quantity / line.Norm * 100;
                    sumReports.Add(line);
                }
            }
            var list = sumReports.GroupBy(x => x.Food.Title).ToList();
            try
            {
                kids_days = (int)Math.Ceiling((decimal)(kids_days / sumReports.Count)) * Days;
                //Работа с Excel
                SaveFileDialog Report = new SaveFileDialog()
                {
                    Filter = "Excel Files|*.xls;*.xlsx;*.xlsm",
                    RestoreDirectory = false
                };
                if (Report.ShowDialog() == true)
                {
                    Excel.Application app = new Excel.Application
                    {
                        DisplayAlerts = false,
                        SheetsInNewWorkbook = 1
                    };
                    Excel.Workbook wb = app.Workbooks.Add();
                    Excel.Worksheet sheet = (Excel.Worksheet)app.Worksheets.get_Item(1);
                    sheet.Name = $"Расход продуктов за {DateTime.Now: dd.MM.yyyy}";
                    sheet.Cells[1, 1] = $"МБДОУ № 2 г.Азова подразделение МБДОУ №2 г.Азова";
                    sheet.Range[sheet.Cells[1, 1], sheet.Cells[1, 6]].Merge();
                    sheet.Cells[2, 1] = $"Отчёт по расходу продуктов питания";
                    sheet.Range[sheet.Cells[2, 1], sheet.Cells[2, 6]].Merge();
                    sheet.Cells[3, 1] = $"за период с {App.StartPeriod: dd.MM.yyyy} по {App.EndPeriod: dd.MM.yyyy}";
                    sheet.Range[sheet.Cells[3, 1], sheet.Cells[3, 6]].Merge();
                    sheet.Cells[4, 1] = $"Категоря: {Cat}";
                    sheet.Range[sheet.Cells[4, 1], sheet.Cells[4, 2]].Merge();
                    sheet.Cells[4, 3] = $"Количество детодней: {kids_days}";
                    sheet.Range[sheet.Cells[4, 3], sheet.Cells[4, 6]].Merge();
                    //Шапка таблицы
                    sheet.Cells[6, 1] = "Продукт";
                    sheet.Range[sheet.Cells[6, 1], sheet.Cells[7, 1]].Merge();
                    sheet.Cells[6, 2] = "Ед. изм.";
                    sheet.Range[sheet.Cells[6, 2], sheet.Cells[7, 2]].Merge();
                    sheet.Cells[6, 3] = "Израсходовано";
                    sheet.Range[sheet.Cells[6, 3], sheet.Cells[7, 4]].Merge();
                    sheet.Cells[7, 3] = "кол-во";
                    sheet.Cells[7, 4] = "Сумма";
                    sheet.Cells[6, 5] = "На 1 ребенка";
                    sheet.Range[sheet.Cells[6, 5], sheet.Cells[7, 6]].Merge();
                    sheet.Cells[7, 5] = "кол-во";
                    sheet.Cells[7, 6] = "Сумма";
                    sheet.Cells[6, 7] = "норма на 1реб";
                    sheet.Range[sheet.Cells[6, 7], sheet.Cells[7, 7]].Merge();
                    sheet.Cells[6, 8] = "% выполнено";
                    sheet.Range[sheet.Cells[6, 8], sheet.Cells[7, 8]].Merge();
                    //форматирование
                    Excel.Range header = sheet.Range[sheet.Cells[6, 1], sheet.Cells[7, 8]];
                    header.Font.Size = 14;
                    header.Font.Bold = true;
                    header.Borders.Weight = 2;
                    header.WrapText = true;
                    header.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    header.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                    //заполнить таблицу данными
                    var arr = list.ToArray();
                    for (int i = 0; i < arr.Count(); i++)
                    {
                        sheet.Cells[5 + i, 1] = arr[i].First().Food.Title.ToUpper();
                        sheet.Cells[5 + i, 2] = arr[i].First().Unit.Title.ToUpper();
                        sheet.Cells[5 + i, 3] = arr[i].Sum(x => x.Quantity);
                        sheet.Cells[5 + i, 4] = arr[i].Sum(x => x.Price);
                        sheet.Cells[5 + i, 5] = arr[i].Sum(x => x.PerPerson_Quantity);
                        sheet.Cells[5 + i, 6] = arr[i].Sum(x => x.PerPerson_Price);
                        sheet.Cells[5 + i, 7] = arr[i].Average(x => x.Norm);
                        sheet.Cells[5 + i, 8] = arr[i].Sum(x => x.Difference);
                    }
                    //Форматирование значений
                    Excel.Range values = sheet.Range[sheet.Cells[8, 1], sheet.Cells[arr.Count() + 7, 8]];
                    values.EntireColumn.AutoFit();
                    values.Font.Size = 12;
                    values.Borders.Weight = 1;

                    //Ряд "Итого"
                    sheet.Cells[arr.Count() + 8, 1] = "Всего";
                    sheet.Cells[arr.Count() + 8, 4] = arr.Sum(x => x.Sum(y => y.Price));
                    sheet.Cells[arr.Count() + 8, 6] = arr.Sum(x => x.Sum(y => y.PerPerson_Price));
                    sheet.Cells[arr.Count() + 8, 8] = arr.Average(x => x.Sum(y => y.Difference));

                    //Форматирование "Итого"
                    Excel.Range total = sheet.Range[sheet.Cells[arr.Count() + 8, 1], sheet.Cells[arr.Count() + 8, 8]];
                    total.Font.Size = 12;
                    total.Font.Italic = true;


                    wb.SaveAs(Report.FileName);
                    wb.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Похоже, за выбранный период нет записей!");
            }
            
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var window = new MainWindow();
            window.Show();
        }

        private void ProductsButton_Checked(object sender, RoutedEventArgs e)
        {
            ContentController.Content = new Products();
        }

        private void EatingButton_Checked(object sender, RoutedEventArgs e)
        {
            ContentController.Content = new EatingControl();
        }

        private void CookedButton_Checked(object sender, RoutedEventArgs e)
        {
            ContentController.Content = new UserControls.Cooked();
        }

        private void NormsButton_Checked(object sender, RoutedEventArgs e)
        {
            ContentController.Content = new Norms();
        }
    }
}
