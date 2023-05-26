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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Stolovaya_1._0.res.admin.panels
{
    /// <summary>
    /// Логика взаимодействия для decommissionedProducts.xaml
    /// </summary>
    public partial class decommissionedProducts : Page
    {
        List<decommissioned_product> products;
        public decommissionedProducts()
        {
            InitializeComponent();
            update_btn_Click(null, null);
        }

        private void back_btn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
        async Task<List<decommissioned_product>> getProducts()
        {
            List<decommissioned_product> tmp = new List<decommissioned_product>();
            DataTable db = libs.dbc.Select($"SELECT * FROM dbo.decommissioned_products");

            await Task.Run(() =>
            {
                for (int i = 0; i < db.Rows.Count; i++)
                {
                    tmp.Add(new decommissioned_product()
                    {
                        id_decommission = db.Rows[i].Field<int>("id_decommission"),
                        id_type_product = new type_product()
                        {
                            id_type_product = db.Rows[i].Field<int>("id_type_product"),
                            name = libs.dbc.Select($"SELECT * FROM dbo.type_products WHERE id_type_product = {db.Rows[i].Field<int>("id_type_product")}").Rows[0].Field<string>("name"),
                            price = libs.dbc.Select($"SELECT * FROM dbo.type_products WHERE id_type_product = {db.Rows[i].Field<int>("id_type_product")}").Rows[0].Field<double>("price")
                        },
                        date_start = db.Rows[i].Field<DateTime>("date_start"),
                        date_end = db.Rows[i].Field<DateTime>("date_end"),
                        count = db.Rows[i].Field<double>("count"),
                        id_unit = new idParam()
                        {
                            id = db.Rows[i].Field<int>("id_unit"),
                            name = libs.dbc.Select($"SELECT * FROM dbo.unit_types WHERE id_unit = {db.Rows[i].Field<int>("id_unit")}").Rows[0].Field<string>("name_unit")
                        },
                        id_reason = new idParam()
                        {
                            id = db.Rows[i].Field<int>("id_reason"),
                            name = libs.dbc.Select($"SELECT * FROM dbo.decommissioned_reasons WHERE id_reason = {db.Rows[i].Field<int>("id_reason")}").Rows[0].Field<string>("name_reason")
                        },
                        description = db.Rows[i].Field<string>("description"),
                        whoDecommissioned = new person()
                        {
                            id_person = db.Rows[i].Field<int>("whoDecommissioned"),
                            fio = libs.dbc.Select($"SELECT * FROM dbo.personal WHERE id_person = {db.Rows[i].Field<int>("whoDecommissioned")}").Rows[0].Field<string>("fio")
                        },
                        whenDecommissioned = db.Rows[i].Field<DateTime>("whenDecommissioned")
                    });
                }
            });
            return tmp;
        }
        private async void update_btn_Click(object sender, RoutedEventArgs e)
        {
            loading_anim.Visibility = Visibility.Visible;
            products = await getProducts();
            mainDG.ItemsSource = products;
            loading_anim.Visibility = Visibility.Hidden;
        }
        #region Методы
        private void search_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    if (c == '_' || c == '-')
                    {
                        e.Handled = false;
                        break;
                    }
                    e.Handled = true;
                    break;
                }
            }
        }
        private void search_tb_KeyUp(object sender, KeyEventArgs e)
        {
            switch (search_type_cb.SelectedIndex)
            {
                case 0: mainDG.ItemsSource = products.Where(a => a.id_decommission.ToString().Contains(search_tb.Text)); break;
                case 1: mainDG.ItemsSource = products.Where(a => a.id_type_product.name.Contains(search_tb.Text)); break;
                case 2: mainDG.ItemsSource = products.Where(a => a.date_start.ToString().Contains(search_tb.Text)); break;
                case 3: mainDG.ItemsSource = products.Where(a => a.date_end.ToString().Contains(search_tb.Text)); break;
                case 4: mainDG.ItemsSource = products.Where(a => a.count.ToString().Contains(search_tb.Text)); break;
                case 5: mainDG.ItemsSource = products.Where(a => a.id_unit.name.ToString().Contains(search_tb.Text)); break;
                case 6: mainDG.ItemsSource = products.Where(a => a.id_reason.name.ToString().Contains(search_tb.Text)); break;
                case 7: mainDG.ItemsSource = products.Where(a => a.description.ToString().Contains(search_tb.Text)); break;
                case 8: mainDG.ItemsSource = products.Where(a => a.whoDecommissioned.fio.ToString().Contains(search_tb.Text)); break;
                case 9: mainDG.ItemsSource = products.Where(a => a.whenDecommissioned .ToString().Contains(search_tb.Text)); break;

                default: MessageBox.Show("Что-то пошло не туда и не так", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); break;
            }
        }
        #endregion

    }
}
