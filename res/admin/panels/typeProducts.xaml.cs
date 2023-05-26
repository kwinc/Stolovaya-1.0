using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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
    /// Логика взаимодействия для typeProductsManipulate.xaml
    /// </summary>
    public partial class typeProducts : Page
    {
        List<type_product> typesInClass;
        DataTable curUser;
        public typeProducts(DataTable u)
        {
            InitializeComponent();
            curUser = u;
            update_btn_Click(null, null);
        }

        async Task<List<type_product>> getTypes()
        {
            List<type_product> tmp = new List<type_product>();
            DataTable db = libs.dbc.Select($"SELECT * FROM dbo.type_products");

            await Task.Run(() =>
            {
                for (int i = 0; i < db.Rows.Count; i++)
                {
                    tmp.Add(new type_product()
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
            });

            return tmp;
        }

        private async void update_btn_Click(object sender, RoutedEventArgs e)
        {
            loading_anim.Visibility = Visibility.Visible;
            typesInClass = await getTypes();
            mainDG.ItemsSource = typesInClass;
            loading_anim.Visibility = Visibility.Hidden;
        }
        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            typeProductsManipulate win = new typeProductsManipulate(false);
            win.ShowDialog();
            update_btn_Click(null, null);
        }
        private void edit_btn_Click(object sender, RoutedEventArgs e)
        {
            if (mainDG.SelectedIndex != -1)
            {
                typeProductsManipulate win = new typeProductsManipulate(true, typesInClass[mainDG.SelectedIndex]);
                win.ShowDialog();
                update_btn_Click(null, null);
            }
            else
            {
                MessageBox.Show("Не выбрана запись для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void back_btn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
