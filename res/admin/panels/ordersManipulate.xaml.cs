using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
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
using static System.Net.Mime.MediaTypeNames;

namespace Stolovaya_1._0.res.admin.panels
{
    /// <summary>
    /// Логика взаимодействия для ordersManipulate.xaml
    /// </summary>
    public partial class ordersManipulate : Window
    {
        List<dish> dishess = new List<dish>();
        int countPanels = 0;
        public ordersManipulate(bool isEdit, ordersS a = new ordersS())
        {
            InitializeComponent();

            #region Загрузка комбобокса с БЛЮДАМИ
            DataTable db = libs.dbc.Select("SELECT * FROM dbo.dishes");

            for (int i = 0; i < db.Rows.Count; i++)
            {
                dishess.Add(new dish()
                {
                    id_dish = db.Rows[i].Field<int>("id_dish"),
                    name_dish = db.Rows[i].Field<string>("name_dish"),
                    price = db.Rows[i].Field<double>("price"),
                    products = db.Rows[i].Field<string>("products"),
                    weight = db.Rows[i].Field<double>("weight"),
                    picture = (byte[])db.Rows[i].Field<byte[]>("picture")
                });
            }
            #endregion

            if (isEdit)
            {
                this.Title = "Редактирование заказа";
                //countPanels = 
            }
        }
        Dictionary<int, WrapPanel> dishesInListBox = new Dictionary<int, WrapPanel>();
        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            var p = new WrapPanel() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Width = 300 };
            p.Children.Add(new ComboBox()
            { 
                HorizontalAlignment = HorizontalAlignment.Stretch, 
                VerticalAlignment = VerticalAlignment.Stretch, 
                Width = 310 / 3 * 2, 
                ItemsSource = dishess, 
                DisplayMemberPath = "name_dish", 
                SelectedValuePath = "id_dish"
            });
            TextBox tmppp = new TextBox()
            {
                MaxLength = 7,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 310 / 3 * 0.5,
            };
            tmppp.PreviewKeyUp += new KeyEventHandler(tb_PreviewTextInput);
            p.Children.Add(tmppp);
            dishesInListBox.Add(countPanels, p);
            listbox.Items.Add(dishesInListBox[countPanels]);
            countPanels++;
        }
        private void tb_PreviewTextInput(object sender, KeyEventArgs e)
        {
            List<string> dishType = new List<string>();
            List<string> dishCoun = new List<string>();
            foreach (var a in dishesInListBox.Values)
            {
                foreach (var b in a.Children)
                {
                    if (b is ComboBox)
                    {
                        if (((ComboBox)b).SelectedValue != null)
                            dishType.Add(((ComboBox)b).SelectedValue.ToString());
                        
                    }
                    else if (b is TextBox)
                    {
                        //((TextBox)b).Text = "1";
                        try
                        {
                            dishCoun.Add(((TextBox)b).Text);
                        }
                        catch { }
                    }
                }
            }

            double sum = 0;
            for (int i = 0;  i < dishType.Count; i++)
            {
                double pricee = dishess.Find(p => p.id_dish == Convert.ToInt32(dishType[i])).price;
                int count = 0;
                try
                {
                    count = Convert.ToInt32(dishCoun[i]);
                }
                catch
                {
                    count = 1;
                }
                
                sum += pricee * count;
            }
            price_tb.Text = sum.ToString();
        }
        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            if (listbox.SelectedIndex != -1)
            {
                dishesInListBox.Remove(listbox.SelectedIndex);
                listbox.Items.RemoveAt(listbox.SelectedIndex);
                listbox.SelectedIndex = -1;
                tb_PreviewTextInput(null, null);
                countPanels--;
            }
        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            List<string> dishes = new List<string>();
            foreach (var a in dishesInListBox.Values)
            {
                foreach (var b in a.Children)
                {
                    if (b is ComboBox)
                    {
                        if (((ComboBox)b).SelectedValue != null)
                        {
                            dishes.Add(((ComboBox)b).SelectedValue.ToString());
                        }
                    }
                }
            }
            string dishesGoToDB = JsonSerializer.Serialize(dishes);
            libs.dbc.Select($"INSERT INTO dbo.orders (dishes, price, date) VALUES ('{dishesGoToDB}', '{price_tb.Text}', '{DateTime.Now}')");
        }
    }
}
