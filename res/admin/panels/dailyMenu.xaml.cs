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
    /// Логика взаимодействия для dailyMenu.xaml
    /// </summary>
    public partial class dailyMenu : Page
    {
        List<dailyMenuS> dailyMenus = new List<dailyMenuS>();
        public dailyMenu()
        {
            InitializeComponent();
            update_btn_Click(null, null);
        }
        List<dailyMenuS> getDailyMenu()
        {
            List<dailyMenuS> tmp = new List<dailyMenuS>();
            DataTable db = libs.dbc.Select($"SELECT * FROM dbo.dailyMenu WHERE date = '{DateTime.Now.ToString("yyyy-MM-dd")}'");

            for (int i = 0; i < db.Rows.Count; i++)
            {
                List<string> tmpS = JsonSerializer.Deserialize<List<string>>(db.Rows[i].Field<string>("dishes"));
                dish[] dishes = new dish[tmpS.Count];
                for (int j = 0; j < tmpS.Count; j++)
                {
                    DataTable d = libs.dbc.Select($"SELECT * FROM dbo.dishes WHERE id_dish = {tmpS[j]}");
                    dishes[j] = new dish()
                    {
                        id_dish = d.Rows[0].Field<int>("id_dish"),
                        name_dish = d.Rows[0].Field<string>("name_dish"),
                        price = d.Rows[0].Field<double>("price"),
                        products = d.Rows[0].Field<string>("products"),//JsonSerializer.Deserialize<int[]>(db.Rows[i].Field<double>("products"))
                        weight = d.Rows[0].Field<double>("weight"),
                        picture = (byte[])d.Rows[0].Field<byte[]>("picture"),
                    };
                    if (dishes[j].picture != null)
                    {
                        dishes[j].readyPic = ByteImage.Convert(ByteImage.GetImageFromByteArray(dishes[j].picture));
                    }
                    else
                    {
                        dishes[j].readyPic = new BitmapImage(new Uri("pack://application:,,,/res/pics/image-error.png"));
                    }
                }

                tmp.Add(new dailyMenuS()
                {
                    id_menu = db.Rows[i].Field<int>("id_menu"),
                    dishes = dishes,
                    date = db.Rows[i].Field<DateTime>("date")
                });
            }

            return tmp;
        }
        private void update_btn_Click(object sender, RoutedEventArgs e)
        {
            loading_anim.Visibility = Visibility.Visible;
            mainItems.Items.Clear();
            dailyMenus = getDailyMenu();

            foreach (var dailyMenu in dailyMenus)
            {
                foreach (var v in dailyMenu.dishes)
                {
                    mainItems.Items.Add(v);
                }
            }

            loading_anim.Visibility = Visibility.Hidden;
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            dailyMenuAdd win = new dailyMenuAdd(dailyMenus);
            win.ShowDialog();
            update_btn_Click(null, null);
        }

        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            mainItems.Items.Remove(mainItems.SelectedItem);
            dish[] currentDishesInList = mainItems.Items.Cast<dish>().ToArray();

            List<string> tmp = new List<string>();
            foreach (dish v in currentDishesInList)
            {
                tmp.Add(v.id_dish.ToString());
            }
            string dishes = JsonSerializer.Serialize(tmp);
            libs.dbc.Select($"UPDATE dbo.dailyMenu SET dishes = '{dishes}' WHERE date = '{DateTime.Now.ToString("yyyy-MM-dd")}'");

        }
    }
}
