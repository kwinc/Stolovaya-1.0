using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
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
using System.Windows.Shapes;

namespace Stolovaya_1._0.res.admin.panels
{
    /// <summary>
    /// Логика взаимодействия для dailyMenuAdd.xaml
    /// </summary>
    public partial class dailyMenuAdd : Window
    {
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
                case 0: mainDG.ItemsSource = dishesInClass.Where(a => a.id_dish.ToString().Contains(search_tb.Text)); break;
                case 1: mainDG.ItemsSource = dishesInClass.Where(a => a.name_dish.Contains(search_tb.Text)); break;
                case 2: mainDG.ItemsSource = dishesInClass.Where(a => a.price.ToString().Contains(search_tb.Text)); break;
                default: MessageBox.Show("Что-то пошло не туда и не так", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); break;
            }
        }
        #endregion
        List<dish> dishesInClass = new List<dish>();
        bool isSelected = false;
        List<dailyMenuS> sT;
        public dailyMenuAdd(List<dailyMenuS> s)
        {
            InitializeComponent();
            update_btn_Click(null, null);
            sT = s;
        }
        async Task<List<dish>> getDishes()
        {
            List<dish> tmp = new List<dish>();
            DataTable db = libs.dbc.Select($"SELECT * FROM dbo.dishes");
            await Task.Run(() =>
            {
                for (int i = 0; i < db.Rows.Count; i++)
                {
                    tmp.Add(new dish()
                    {
                        id_dish = db.Rows[i].Field<int>("id_dish"),
                        name_dish = db.Rows[i].Field<string>("name_dish"),
                        price = db.Rows[i].Field<double>("price"),
                        products = db.Rows[i].Field<string>("products"),
                        weight = db.Rows[i].Field<double>("weight"),
                        picture = (byte[])db.Rows[i].Field<byte[]>("picture")
                    });
                }
            });
            return tmp;
        }
        private async void update_btn_Click(object sender, RoutedEventArgs e)
        {
            loading_anim.Visibility = Visibility.Visible;
            dishesInClass = await getDishes();
            mainDG.ItemsSource = dishesInClass;
            loading_anim.Visibility = Visibility.Hidden;
        }
        List<dish> selectedDishes = new List<dish>();
        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            if (mainDG.SelectedIndex != -1)
            {
                if (selectedDishes.Exists(p => p.id_dish == ((dish)mainDG.SelectedItem).id_dish))
                {
                    selectedDishes.Remove((dish)mainDG.SelectedItem);
                    add_btn.Content = "Добавить";
                }
                else
                {
                    selectedDishes.Add((dish)mainDG.SelectedItem);
                    add_btn.Content = "Убрать";
                }
            }
            else
            {
                MessageBox.Show("Не выбрано блюдо", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            List<string> tmp = new List<string>();
            foreach (dish v in selectedDishes)
            {
                tmp.Add(v.id_dish.ToString());
            }

            DataTable db = libs.dbc.Select($"SELECT * FROM dbo.dailyMenu WHERE date = '{DateTime.Now.ToString("yyyy-MM-dd")}'");
            if (db.Rows.Count == 0)
            {
                string dishes = JsonSerializer.Serialize(tmp);
                libs.dbc.Select($"INSERT INTO dbo.dailyMenu (dishes, date) VALUES ('{dishes}', '{DateTime.Now.ToString("yyyy-MM-dd")}')");
            }
            else
            {
                //
                List<string> su = new List<string>();
                su = JsonSerializer.Deserialize<List<string>>(db.Rows[0].Field<string>("dishes"));

                List<string> neww = tmp;

                List<string> goToDB = su.Union(neww).ToList();
                string dishes = JsonSerializer.Serialize(goToDB);
                libs.dbc.Select($"UPDATE dbo.dailyMenu SET dishes = '{dishes}' WHERE date = '{DateTime.Now.ToString("yyyy-MM-dd")}'");
            }
        }

        private void mainDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedDishes.Exists(p => p.id_dish == ((dish)mainDG.SelectedItem).id_dish))
            {
                add_btn.Content = "Убрать";
            }
            else
            {
                add_btn.Content = "Добавить";
            }
        }
    }
}
