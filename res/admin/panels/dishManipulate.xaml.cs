using Stolovaya_1._0.res.libs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Xml;
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
    /// Логика взаимодействия для dishManipulate.xaml
    /// </summary>
    public partial class dishManipulate : Window
    {
        List<type_product> types = new List<type_product>();
        bool isE;
        dish currC;
        Dictionary<int, WrapPanel> panels = new Dictionary<int, WrapPanel>();
        int countPanels = 0;
        public dishManipulate(bool isEdit, dish c = new dish())
        {
            isE = isEdit;
            currC = c;
            #region Загрузка комбобокса с типами
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
            #endregion

            InitializeComponent();
            

            if (isEdit)
            {
                this.Title = "Редактирование блюда";
                name_tb.Text = c.name_dish;
                price_tb.Text = c.price.ToString();
                List<string> products = JsonSerializer.Deserialize<List<string>>(c.products);


                foreach (var param in products)
                {
                    var p = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Width = 200 };
                    p.Children.Add(new ComboBox() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Width = 210 / 3 * 2, ItemsSource = types, DisplayMemberPath = "name", SelectedValuePath = "id_type_product", SelectedValue = param.Split('|')[0] });
                    p.Children.Add(new TextBox() { MaxLength = 7, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Width = 210 / 3 * 0.5, Text = param.Split('|')[1] });
                    panels.Add(countPanels, p);
                    listbox.Items.Add(panels[countPanels]);
                    countPanels++;
                }
                
            }

        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            if (countPanels == 0) {
                MessageBox.Show("Отсутствуют продукты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            List<string> productsInThis = new List<string>();
            foreach (var k in panels.Values)
            {
                string tmpTB = "";
                string tmpCB = "";
                foreach (var v in k.Children)
                {
                    if (v is TextBox)
                    {
                        tmpTB = ((TextBox)v).Text;
                    }
                    else if (v is ComboBox)
                    {
                        tmpCB = ((ComboBox)v).SelectedValue.ToString(); 
                    }
                }
                productsInThis.Add(tmpCB + "|" + tmpTB);
            }
            string json = JsonSerializer.Serialize(productsInThis);
            //MessageBox.Show(json);
            if (!isE)
            {
                dbc.Select($"INSERT INTO dbo.dishes (name_dish, price, products) VALUES ('{name_tb.Text}', '{price_tb.Text.Replace(',', '.')}', '{json}')");
                MessageBox.Show("Сохранено");
                this.Close();
            }
            else
            {
                dbc.Select($"UPDATE dbo.dishes SET name_dish = '{name_tb.Text}', price = '{price_tb.Text.Replace(',', '.')}', products = '{json}' WHERE id_dish = {currC.id_dish}");
                MessageBox.Show("Сохранено");
                this.Close();
            }
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            var p = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Width = 200 };
            p.Children.Add(new ComboBox() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Width = 210 / 3 * 2, ItemsSource = types, DisplayMemberPath = "name", SelectedValuePath = "id_type_product"});
            p.Children.Add(new TextBox() { MaxLength = 7, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Width = 210 / 3 * 0.5});
            panels.Add(countPanels, p);
            listbox.Items.Add(panels[countPanels]);
            countPanels++;
        }

        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            if (listbox.SelectedIndex != -1)
            {
                panels.Remove(listbox.SelectedIndex);
                listbox.Items.RemoveAt(listbox.SelectedIndex);
                listbox.SelectedIndex = -1;
                countPanels--;
            }
        }
    }
}
