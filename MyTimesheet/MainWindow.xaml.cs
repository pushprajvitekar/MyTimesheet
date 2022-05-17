using System.IO;
using System.Linq;
using System.Windows;
using System;
using System.Globalization;
using Microsoft.Win32;

namespace MyTimesheet
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            comboBoxMonth.ItemsSource = CultureInfo.InvariantCulture.DateTimeFormat
                                                     .MonthNames.Take(12).ToList();
            comboBoxMonth.SelectedItem = CultureInfo.InvariantCulture.DateTimeFormat
                                                    .MonthNames[DateTime.Now.AddMonths(-1).Month];

            comboBoxYear.ItemsSource = Enumerable.Range(2022, DateTime.Now.Year - 2022 + 2).ToList();
            comboBoxYear.SelectedItem = DateTime.Now.Year;
            var ctx = BsonStorage.FetchFromStore(DateTime.Now.Year, DateTime.Now.ToString("MMMM"));
            RefreshDataContext(ctx);
        }
        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "xls files (*.xls)|*.xls|html files (*.html)|*.html|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                SetDataContext(openFileDialog.FileName);
            }
        }
        private void BtnFetch_Click(object sender, RoutedEventArgs e)
        {
            var month = comboBoxMonth.SelectedValue.ToString();
            var year = Convert.ToInt32(comboBoxYear.SelectedValue);
            var ctx = BsonStorage.FetchFromStore(year, month);
            RefreshDataContext(ctx);
        }

        private void RefreshDataContext(TimeSheetMonth ctx)
        {
            var tsm = new TimeSheetMonth(ctx.DailyTimeSheets);
            DataContext = tsm;
        }

        private void SetDataContext(string filePath)
        {
            var fileText = File.ReadAllText(filePath);
            var ctx = GetTimeSheet(fileText);
            BsonStorage.AddToStore(ctx);
            RefreshDataContext(ctx);
        }

        private TimeSheetMonth GetTimeSheet(string fileText)
        {
            var rows = HtmlDocumentParser.ParseHtmlFile(fileText);
            var lst = TimeSheetDay.GetTimeSheetRows(rows);
            return new TimeSheetMonth(lst);
        }

        private void BtnRecalculate_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TimeSheetMonth ctx)
            {
                RefreshDataContext(ctx);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TimeSheetMonth ctx)
            {
                BsonStorage.AddToStore(ctx);
            }
        }
    }
}
