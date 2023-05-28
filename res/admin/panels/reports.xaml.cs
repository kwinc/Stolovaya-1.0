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
    /// Логика взаимодействия для reports.xaml
    /// </summary>
    public partial class reports : Page
    {
        public reports()
        {
            InitializeComponent();
        }
        async Task<List<product_storage>> getProducts(string req)
        {
            List<product_storage> tmp = new List<product_storage>();
            DataTable db = libs.dbc.Select(req);

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
                        }
                    });
                }
            });
            return tmp;
        }
        async Task<List<ordersS>> getOrders(string req)
        {
            List<ordersS> tmp = new List<ordersS>();
            DataTable db = libs.dbc.Select(req);

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

        List<product_storage> storage;
        List<ordersS> orders;
        private async void go_btn_Click(object sender, RoutedEventArgs e)
        {
            if (report_type_cb.SelectedIndex != -1)
            {
                if (report_type_cb.SelectedIndex == 0)
                {
                    loading_anim.Visibility = Visibility.Visible;
                    storage = await getProducts($"SELECT * FROM dbo.product_storage WHERE date_end < '{DateTime.Now.ToString("dd-MM-yyyy")}'");
                    
                    DataGrid ans = new DataGrid()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Margin = new Thickness(5, 5, 5, 5),
                        AutoGenerateColumns = false,
                        CanUserResizeRows = false,
                    };

                    List<string[]> strings = new List<string[]>();
                    strings.Add(new string[2]{ "id_product_storage", "ID"});
                    strings.Add(new string[2]{ "id_type_product.name", "Наименование" });
                    strings.Add(new string[2]{ "date_start", "Начало хранения" });
                    strings.Add(new string[2]{ "date_end", "Окончание хранения" });
                    strings.Add(new string[2]{ "count", "Количество" });
                    strings.Add(new string[2]{ "id_unit.name", "Ед. изм." });

                    foreach (string[] s in strings)
                    {
                        ans.Columns.Add(new DataGridTextColumn()
                        {
                            Header = s[1],
                            Binding = new Binding(s[0]),
                            //Width = Double.NaN,
                        });
                    }
                    ans.ItemsSource = storage;
                    makeReport win = new makeReport(ans);
                    win.ShowDialog();
                    loading_anim.Visibility = Visibility.Hidden;
                }
                else if (report_type_cb.SelectedIndex == 1)
                {
                    loading_anim.Visibility = Visibility.Visible;
                    orders = await getOrders($"SELECT * FROM dbo.orders WHERE date > '{start_dp.SelectedDate}' AND date < '{stop_dp.SelectedDate}'");

                    DataGrid ans = new DataGrid()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Margin = new Thickness(5, 5, 5, 5),
                        AutoGenerateColumns = false,
                        CanUserResizeRows = false,
                    };

                    List<string[]> strings = new List<string[]>();
                    strings.Add(new string[2] { "id_order", "ID" });
                    strings.Add(new string[2] { "namesDishes", "Блюдо" });
                    strings.Add(new string[2] { "price", "Цена" });
                    strings.Add(new string[2] { "date", "Дата" });

                    foreach (string[] s in strings)
                    {
                        ans.Columns.Add(new DataGridTextColumn()
                        {
                            Header = s[1],
                            Binding = new Binding(s[0]),
                            //Width = Double.NaN,
                        });
                    }
                    ans.ItemsSource = orders;
                    makeReport win = new makeReport(ans);
                    win.ShowDialog();
                    loading_anim.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                MessageBox.Show("Ошибка. Возможные причины:\n1) Не указан тип отчёта\n2)Не указан период (указан некорректно)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
