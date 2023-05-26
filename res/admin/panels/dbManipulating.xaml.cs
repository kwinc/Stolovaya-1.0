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
    /// Логика взаимодействия для dbManipulating.xaml
    /// </summary>
    public partial class dbManipulating : Window
    {
        forDBmanip a;
        public dbManipulating(forDBmanip param)
        {
            InitializeComponent();

            a.isEdit = param.isEdit;
            this.Title = param.title;
            a = param;
            
            if (param.isEdit)
            {
                name_tb.Text = param.element.name;
                desc_tb.Text = param.element.desc;
            }
        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            if (a.isEdit)
            {
                try
                {
                    libs.dbc.Select($"UPDATE dbo.{a.fast_db} SET name{a.fast_db_row} = '{name_tb.Text}', desc{a.fast_db_row} = '{desc_tb.Text}' WHERE id{a.fast_db_row} = {a.element.id}");
                    this.Close();
                }
                catch
                {
                    MessageBox.Show("Произошла ошибка при внесении изменений в базу данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                try
                {
                    libs.dbc.Select($"INSERT INTO dbo.{a.fast_db} (name{a.fast_db_row},  desc{a.fast_db_row}) VALUES ('{name_tb.Text}', '{desc_tb.Text}')");
                    this.Close();
                }
                catch
                {
                    MessageBox.Show("Произошла ошибка при внесении изменений в базу данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }
    }
}
