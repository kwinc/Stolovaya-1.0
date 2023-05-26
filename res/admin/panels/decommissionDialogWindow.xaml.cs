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
    /// Логика взаимодействия для decommissionDialogWindow.xaml
    /// </summary>
    public partial class decommissionDialogWindow : Window
    {
        product_storage p;
        public decommissionDialogWindow(product_storage p)
        {
            InitializeComponent();
            this.p = p;
            product_tb.Text = p.id_product_storage.ToString() + " " + p.id_type_product.name;
        }

        private void count_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    if (c == ',')
                    {
                        e.Handled = false;
                        break;
                    }
                    e.Handled = true;
                    break;
                }
            }
        }

        private void back_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                /*if (Convert.ToDouble(count_tb) > p.count)
                {
                    MessageBox.Show("Указанное количество превышает наличие на складе", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    double now_count = p.count - Convert.ToDouble(count_tb);
                }*/
            }
            catch
            {
                MessageBox.Show("Неизвестная ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //dbc.Select($"INSERT INTO dbo.decommissioned_products (id_type_product, date_start, date_end, count, id_unit, id_reason, description, whoDecommissioned, whenDecommissioned) VALUES ({})");
            //dbc.Select($"DELETE FROM dbo.product_storage WHERE id_product_storage = {storage[mainDG.SelectedIndex].id_product_storage}");
        }

        private void slValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double thisValue = slValue.Value;
            thisValue = Math.Round(thisValue, 2);
            slValue.Value = thisValue;
        }
    }
}
