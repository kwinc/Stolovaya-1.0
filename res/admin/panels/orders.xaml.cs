using Stolovaya_1._0.res.libs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
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
    /// Логика взаимодействия для orders.xaml
    /// </summary>
    public partial class orders : Page
    {
        List<ordersS> ordersInClass = new List<ordersS>();
        public orders()
        {
            InitializeComponent();
            update_btn_Click(null, null);
        }

        async Task<List<ordersS>> getProducts()
        {
            List<ordersS> tmp = new List<ordersS>();
            DataTable db = libs.dbc.Select($"SELECT * FROM dbo.orders");

            await Task.Run(() =>
            {
                for (int i = 0; i < db.Rows.Count; i++)
                {
                    //DataTable a = libs.dbc.Select($"SELECT FROM * dbo.dishes WHERE id_dish = {db.Rows[i].Field<int>("id_dish")}");
                    List<string> dishesInOrderFromDB = JsonSerializer.Deserialize<List<string>>(db.Rows[i].Field<string>("dishes"));
                    dish[] dishesInOrder = new dish[dishesInOrderFromDB.Count];
                    string nameD = "";
                    foreach (string s in dishesInOrderFromDB)
                    {
                        DataTable a = libs.dbc.Select($"SELECT * FROM dbo.dishes WHERE id_dish = {s}");
                        nameD += a.Rows[0].Field<string>("name_dish") + " ";
                        dishesInOrder[i] = new dish()
                        {
                            id_dish = a.Rows[0].Field<int>("id_dish"),
                            name_dish = a.Rows[0].Field<string>("name_dish"),
                            price = a.Rows[0].Field<double>("price"),
                            products = a.Rows[0].Field<string>("products"),
                            weight = a.Rows[0].Field<double>("weight"),
                            picture = a.Rows[0].Field<byte[]>("picture"),
                            readyPic = ByteImage.Convert(ByteImage.GetImageFromByteArray(a.Rows[0].Field<byte[]>("picture")))
                        };                        
                    }
                    tmp.Add(new ordersS()
                    {
                        id_order = db.Rows[i].Field<int>("id_order"),
                        dishes = dishesInOrder,
                        price = db.Rows[i].Field<double>("price"),
                        date = db.Rows[i].Field<DateTime>("date"),
                        namesDishes = nameD
                    });
                }
            });
            return tmp;
        }
        private async void update_btn_Click(object sender, RoutedEventArgs e)
        {
            loading_anim.Visibility = Visibility.Visible;
            ordersInClass = await getProducts();
            mainDG.ItemsSource = ordersInClass;
            loading_anim.Visibility = Visibility.Hidden;
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            ordersManipulate win = new ordersManipulate(false);
            win.ShowDialog();
            update_btn_Click(null, null);
        }

        private void edit_btn_Click(object sender, RoutedEventArgs e)
        {
            ordersManipulate win = new ordersManipulate(false, (ordersS)mainDG.SelectedItem);
            win.ShowDialog();
            update_btn_Click(null, null);
        }

        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Действительно удалить заказ под номером {((ordersS)mainDG.SelectedItem).id_order}", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                dbc.Select($"DELETE FROM dbo.orders WHERE id_order = {((ordersS)mainDG.SelectedItem).id_order}");
                update_btn_Click(null, null);
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
                case 0: mainDG.ItemsSource = ordersInClass.Where(a => a.id_order.ToString().Contains(search_tb.Text)); break;
                case 1: mainDG.ItemsSource = ordersInClass.Where(a => a.namesDishes.Contains(search_tb.Text)); break;
                case 2: mainDG.ItemsSource = ordersInClass.Where(a => a.price.ToString().Contains(search_tb.Text)); break;
                case 3: mainDG.ItemsSource = ordersInClass.Where(a => a.date.ToString().Contains(search_tb.Text)); break;
                default: MessageBox.Show("Что-то пошло не туда и не так", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); break;
            }
        }
        #endregion
    }
}
