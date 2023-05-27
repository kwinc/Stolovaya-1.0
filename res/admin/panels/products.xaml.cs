using Stolovaya_1._0.res.libs;
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
    /// Логика взаимодействия для products.xaml
    /// </summary>
    public partial class products : Page
    {
        List<product_storage> storage;
        DataTable curUser;
        public products(DataTable u)
        {
            InitializeComponent();
            curUser = u;
            update_btn_Click(null, null); //говнокодер

        }
        private async void export_btn_Click(object sender, RoutedEventArgs e)
        {
            loading_anim.Visibility = Visibility.Visible;
            await asyncs.ExportToWord(asyncs.DataGridtoDataTable(mainDG));
            loading_anim.Visibility = Visibility.Hidden;
        }
        async Task<List<product_storage>> getProducts()
        {
            List<product_storage> tmp = new List<product_storage>();
            DataTable db = libs.dbc.Select($"SELECT * FROM dbo.product_storage");

            await Task.Run(() =>
            {
                for (int i = 0; i < db.Rows.Count; i++)
                {
                    tmp.Add(new product_storage()
                    {
                        id_product_storage = db.Rows[i].Field<int>("id_product_storage"),
                        id_type_product = new type_product()
                        {
                            id_type_product = db.Rows[i].Field<int>("id_type_product"),
                            name =  libs.dbc.Select($"SELECT * FROM dbo.type_products WHERE id_type_product = {db.Rows[i].Field<int>("id_type_product")}").Rows[0].Field<string>("name"),
                            price = libs.dbc.Select($"SELECT * FROM dbo.type_products WHERE id_type_product = {db.Rows[i].Field<int>("id_type_product")}").Rows[0].Field<double>("price")
                        },
                        date_start =    db.Rows[i].Field<DateTime>("date_start"),
                        date_end =      db.Rows[i].Field<DateTime>("date_end"),
                        count = db.Rows[i].Field<double>("count"),
                        id_unit = new idParam()
                        {
                            id = db.Rows[i].Field<int>("id_unit"),
                            name = libs.dbc.Select($"SELECT * FROM dbo.unit_types WHERE id_unit = {db.Rows[i].Field<int>("id_unit")}").Rows[0].Field<string>("name_unit")
                        }
                    });
                }
            });
            return tmp;
        }
        private async void update_btn_Click(object sender, RoutedEventArgs e)
        {
            loading_anim.Visibility = Visibility.Visible;
            storage = await getProducts();
            mainDG.ItemsSource = storage;
            loading_anim.Visibility = Visibility.Hidden;
        }
        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            //this.NavigationService.Navigate(new personalManipulating(false));
            productsManipulating win = new productsManipulating(false);
            win.ShowDialog();
            update_btn_Click(null, null);
        }
        private void edit_btn_Click(object sender, RoutedEventArgs e)
        {
            if (mainDG.SelectedIndex != -1)
            {
                productsManipulating win = new productsManipulating(true, (product_storage)mainDG.SelectedItem);//(product_storage)mainDG.SelectedItem);
                win.ShowDialog();
                update_btn_Click(null, null);
            }
            else
            {
                MessageBox.Show("Не выбран продукт", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBoxResult.Yes == MessageBox.Show($"Действительно удалить {((product_storage)mainDG.SelectedItem).id_product_storage}|{((product_storage)mainDG.SelectedItem).id_type_product.name}", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    dbc.Select($"DELETE FROM dbo.product_storage WHERE id_product_storage = {((product_storage)mainDG.SelectedItem).id_product_storage}");
                    update_btn_Click(null, null); //говнокодер2
                }
            }
            catch
            {
                //MessageBox.Show("Не выбран пользователь", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                case 0: mainDG.ItemsSource = storage.Where(a => a.id_product_storage.ToString().Contains(search_tb.Text)); break;
                case 1: mainDG.ItemsSource = storage.Where(a => a.id_type_product.name.Contains(search_tb.Text)); break;
                case 2: mainDG.ItemsSource = storage.Where(a => a.date_start.ToString().Contains(search_tb.Text)); break;
                case 3: mainDG.ItemsSource = storage.Where(a => a.date_end.ToString().Contains(search_tb.Text)); break;
                case 4: mainDG.ItemsSource = storage.Where(a => a.count.ToString().Contains(search_tb.Text)); break;
                case 5: mainDG.ItemsSource = storage.Where(a => a.id_unit.name.ToString().Contains(search_tb.Text)); break;
                default: MessageBox.Show("Что-то пошло не туда и не так", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); break;
            }
        }
        #endregion

        private void man_type_btn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new typeProducts(curUser));
        }

        private void man_dec_btn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new decommissionedProducts());
        }
    }
}
