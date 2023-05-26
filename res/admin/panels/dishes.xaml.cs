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
    /// Логика взаимодействия для dishes.xaml
    /// </summary>
    public partial class dishes : Page
    {
        DataTable curUser;
        List<dish> dishesInClass = new List<dish>();

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
        public dishes(DataTable u)
        {
            InitializeComponent();
            update_btn_Click(null, null);
            curUser = u;
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
                        products = db.Rows[i].Field<string>("products")//JsonSerializer.Deserialize<int[]>(db.Rows[i].Field<double>("products"))
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

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            dishManipulate win = new dishManipulate(false);
            win.ShowDialog();
            update_btn_Click(null, null);

        }

        private void edit_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dishManipulate win = new dishManipulate(true, (dish)mainDG.SelectedItem);
                win.ShowDialog();
                update_btn_Click(null, null);
            }
            catch
            {
                MessageBox.Show("Не выбран элемент", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            dish tmp = (dish)mainDG.SelectedItem;
            if (MessageBoxResult.Yes == MessageBox.Show($"Действительно удалить блюдо '{tmp.name_dish}'", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                libs.dbc.Select($"DELETE FROM dbo.dishes WHERE id_dish = {tmp.id_dish}");
                update_btn_Click(null, null);
            }
        }
    }
}
