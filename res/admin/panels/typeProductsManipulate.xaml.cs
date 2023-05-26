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
using System.Windows.Xps.Serialization;

namespace Stolovaya_1._0.res.admin.panels
{
    /// <summary>
    /// Логика взаимодействия для typeProductsManipulate.xaml
    /// </summary>
    public partial class typeProductsManipulate : Window
    {
        bool isEdit;
        type_product t;
        public typeProductsManipulate(bool isEdit, type_product t = new type_product())
        {
            InitializeComponent();

            this.isEdit = isEdit;
            this.t = t;

            #region Загрузка комбобокса
            List<idParam> types = new List<idParam>();
            DataTable db = libs.dbc.Select("SELECT * FROM dbo.unit_types");
            for (int i = 0; i < db.Rows.Count; i++)
            {
                types.Add(new idParam()
                {
                    id   =  db.Rows[i].Field<int>("id_unit"),
                    name =  db.Rows[i].Field<string>("name_unit"),
                    desc =  db.Rows[i].Field<string>("desc_unit")
                });
            }
            search_type_cb.ItemsSource = types;
            #endregion

            if (isEdit)
            {
                this.Title = "Редактирование типа";
                name_tb.Text = t.name;
                price_tb.Text = t.price.ToString();
                search_type_cb.SelectedValue = t.id_unit.id;
            }
            this.Title = "Добавление типа";
        }
        private void name_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    e.Handled = true;
                    break;
                }
            }
        }
        private void price_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
                    libs.dbc.Select($"UPDATE dbo.type_products SET name = '{name_tb.Text}', price = '{price_tb.Text.Replace(',', '.')}', id_unit = {search_type_cb.SelectedValue} WHERE id_type_product = {t.id_type_product}");
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
                    libs.dbc.Select($"INSERT INTO dbo.type_products (name,  price, id_unit) VALUES ('{name_tb.Text}', '{price_tb.Text.Replace(',', '.')}', {search_type_cb.SelectedValue})");
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
