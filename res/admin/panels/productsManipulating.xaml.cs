using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для productsManipulating.xaml
    /// </summary>
    public partial class productsManipulating : Window
    {
        product_storage p;
        bool isEdit;
        public productsManipulating(bool isEdit, product_storage p = new product_storage())
        {
            InitializeComponent();
            this.isEdit = isEdit;
            this.p = p;

            #region Загрузка комбобокса с типами
            List<type_product> types = new List<type_product>();
            DataTable db = libs.dbc.Select("SELECT * FROM dbo.type_products");
            for (int i = 0; i < db.Rows.Count; i++)
            {
                types.Add(new type_product()
                {
                    id_type_product = db.Rows[i].Field<int>("id_type_product"),
                    name = db.Rows[i].Field<string>("name"),
                    price = db.Rows[i].Field<double>("price"),
                    id_unit = new idParam()
                    {
                        id = db.Rows[i].Field<int>("id_unit"),
                        name = libs.dbc.Select($"SELECT * FROM dbo.unit_types WHERE id_unit = {db.Rows[i].Field<int>("id_unit")}").Rows[0].Field<string>("name_unit"),
                        desc = libs.dbc.Select($"SELECT * FROM dbo.unit_types WHERE id_unit = {db.Rows[i].Field<int>("id_unit")}").Rows[0].Field<string>("desc_unit")
                    }
                });
            }
            product_type_cb.ItemsSource = types;
            #endregion

            #region Загрузка комбобокса с ед. изм.
            List<idParam> unit_types = new List<idParam>();
            DataTable dbS = libs.dbc.Select("SELECT * FROM dbo.unit_types");
            for (int i = 0; i < dbS.Rows.Count; i++)
            {
                unit_types.Add(new idParam()
                {
                    id = dbS.Rows[i].Field<int>("id_unit"),
                    name = dbS.Rows[i].Field<string>("name_unit"),
                    desc = dbS.Rows[i].Field<string>("desc_unit")
                });
            }
            unit_type_cb.ItemsSource = unit_types;
            #endregion

            if (isEdit)
            {
                this.Title = "Редактирование продукта";
                product_type_cb.SelectedValue = p.id_type_product.id_type_product;
                date_start_dp.SelectedDate = p.date_start;
                date_end_dp.SelectedDate = p.date_end;
                count_tb.Text = p.count.ToString();
                unit_type_cb.SelectedValue = p.id_unit.id;
            }
            this.Title = "Добавление продукта";
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
        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            if (isEdit)
            {
                try
                {
                    libs.dbc.Select($"UPDATE dbo.product_storage SET id_type_product = {product_type_cb.SelectedValue}, date_start = '{date_start_dp.SelectedDate}', date_end = '{date_end_dp.SelectedDate}', count = '{count_tb.Text.Replace(',', '.')}', id_unit = {unit_type_cb.SelectedValue} WHERE id_product_storage = {p.id_product_storage}");
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
                    libs.dbc.Select($"INSERT INTO dbo.product_storage (id_type_product,  date_start, date_end, count, id_unit) VALUES ({product_type_cb.SelectedValue}, '{date_start_dp.SelectedDate}', '{date_end_dp.SelectedDate}', '{count_tb.Text.Replace(',', '.')}', {unit_type_cb.SelectedValue})");
                    this.Close();
                }
                catch
                {
                    MessageBox.Show("Произошла ошибка при внесении изменений в базу данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }
        #region Приколы с ДатаПикером
        DateTime math_year(DatePicker d, bool isPlus)
        {
            if (d.SelectedDate.ToString() == "")
            {
                if (isPlus)
                    return DateTime.Now.AddYears(1);
                else
                    return DateTime.Now.AddYears(-1); //гений, да
            }
            else
            {
                if (isPlus)
                    return d.SelectedDate.Value.AddYears(1);
                else
                    return d.SelectedDate.Value.AddYears(-1); //гений, да x2
            }
        }
        DateTime math_month(DatePicker d, bool isPlus)
        {
            if (d.SelectedDate.ToString() == "")
            {
                if (isPlus)
                    return DateTime.Now.AddMonths(1);
                else
                    return DateTime.Now.AddMonths(-1); //гений, да x3
            }
            else
            {
                if (isPlus)
                    return d.SelectedDate.Value.AddMonths(1);
                else
                    return d.SelectedDate.Value.AddMonths(-1); //гений, да x4 /* шутка смешнее, если ее повторить несколько раз ☝️☝️☝️ */
            }
        }
        private void min_year_btn_Click(object sender, RoutedEventArgs e)
        {
            date_start_dp.SelectedDate = math_year(date_start_dp, false);
        }

        private void min_mont_btn_Click(object sender, RoutedEventArgs e)
        {
            date_start_dp.SelectedDate = math_month(date_start_dp, false);
        }

        private void plu_mont_btn_Click(object sender, RoutedEventArgs e)
        {
            date_start_dp.SelectedDate = math_month(date_start_dp, true);
        }

        private void plu_year_btn_Click(object sender, RoutedEventArgs e)
        {
            date_start_dp.SelectedDate = math_year(date_start_dp, true);
        }

        private void min_year_end_btn_Click(object sender, RoutedEventArgs e)
        {
            date_end_dp.SelectedDate = math_year(date_end_dp, false);
        }

        private void min_mont_end_btn_Click(object sender, RoutedEventArgs e)
        {
            date_end_dp.SelectedDate = math_month(date_end_dp, false);
        }

        private void plu_mont_end_btn_Click(object sender, RoutedEventArgs e)
        {
            date_end_dp.SelectedDate = math_month(date_end_dp, true);
        }

        private void plu_year_end_btn_Click(object sender, RoutedEventArgs e)
        {
            date_end_dp.SelectedDate = math_year(date_end_dp, true);
        }
        #endregion        
    }
}
