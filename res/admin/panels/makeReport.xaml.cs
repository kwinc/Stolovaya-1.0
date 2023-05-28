using Stolovaya_1._0.res.libs;
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

namespace Stolovaya_1._0.res.admin.panels
{
    /// <summary>
    /// Логика взаимодействия для makeReport.xaml
    /// </summary>
    public partial class makeReport : Window
    {
        DataGrid dg = new DataGrid();
        public makeReport(DataGrid d)
        {
            InitializeComponent();
            dg = d;
            content_wp.Children.Add(d);
        }

        private async void export_word_btn_Click(object sender, RoutedEventArgs e)
        {
            loading_anim.Visibility = Visibility.Visible;
            await asyncs.ExportToWord(asyncs.DataGridtoDataTable(dg));
            loading_anim.Visibility = Visibility.Hidden;
        }
    }
}
