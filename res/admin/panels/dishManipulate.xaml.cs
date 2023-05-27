using Microsoft.Win32;
using Stolovaya_1._0.res.libs;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System;

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
        bool imgFlag = false;
        byte[] currentPic;
        BitmapImage logo = new BitmapImage();
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

            logo = new BitmapImage(new Uri("pack://application:,,,/res/pics/image-error.png"));

            if (isEdit)
            {
                this.Title = "Редактирование блюда";
                name_tb.Text = c.name_dish;
                price_tb.Text = c.price.ToString();
                weight_tb.Text = c.weight.ToString();
                if (c.picture != null)
                {
                    imgFlag = true;
                    imp_pick_btn.Content = "Удалить фотографию";
                    currentPic = c.picture;
                    mainPic.Source = ByteImage.Convert(ByteImage.GetImageFromByteArray(currentPic));
                }
                else
                {                    
                    mainPic.Source = logo;
                }
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
                if (currentPic != null)
                {
                    string connectionString = dbc.connectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString)) // создаем подключение
                    {
                        connection.Open(); // откроем подключение
                        SqlCommand command = new SqlCommand(); // создадим запрос
                        command.Connection = connection; // дадим запросу подключение
                        command.CommandText = @$"INSERT INTO dbo.dishes (name_dish, price, products, weight, picture) VALUES ('{name_tb.Text}', '{price_tb.Text.Replace(',', '.')}', '{json}', '{weight_tb.Text.Replace(',', '.')}', @ImageData)"; // пропишем запрос
                        command.Parameters.Add("@ImageData", SqlDbType.Image, 1000000);
                        command.Parameters["@ImageData"].Value = currentPic;// скалярной переменной ImageData присвоем массив байтов
                        command.ExecuteNonQuery(); // запустим
                    }
                }
                else
                {
                    dbc.Select($"INSERT INTO dbo.dishes (name_dish, price, products, weight, picture) VALUES ('{name_tb.Text}', '{price_tb.Text.Replace(',', '.')}', '{json}', '{weight_tb.Text.Replace(',', '.')}', NULL)");
                }
                MessageBox.Show("Сохранено");
                this.Close();
            }
            else
            {
                if (currentPic != null)
                {
                    string connectionString = dbc.connectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString)) // создаем подключение
                    {
                        connection.Open(); // откроем подключение
                        SqlCommand command = new SqlCommand(); // создадим запрос
                        command.Connection = connection; // дадим запросу подключение
                        command.CommandText = @$"UPDATE dbo.dishes SET name_dish = '{name_tb.Text}', price = '{price_tb.Text.Replace(',', '.')}', products = '{json}', weight = '{weight_tb.Text.Replace(',', '.')}', picture = @ImageData WHERE id_dish = {currC.id_dish}"; // пропишем запрос
                        command.Parameters.Add("@ImageData", SqlDbType.Image, 1000000);
                        command.Parameters["@ImageData"].Value = currentPic;// скалярной переменной ImageData присвоем массив байтов
                        command.ExecuteNonQuery(); // запустим
                    }
                }
                else
                {
                    dbc.Select($"UPDATE dbo.dishes SET name_dish = '{name_tb.Text}', price = '{price_tb.Text.Replace(',', '.')}', products = '{json}', weight = '{weight_tb.Text.Replace(',', '.')}', picture = NULL WHERE id_dish = {currC.id_dish}");
                }

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
        private void imp_pick_btn_Click(object sender, RoutedEventArgs e)
        {
            if (imgFlag)
            {
                currentPic = null;
                imgFlag = false;
                imp_pick_btn.Content = "Прикрепить фотографию";
                mainPic.Source = logo;
            }
            else
            {
                OpenFileDialog d = new OpenFileDialog();
                d.Filter = "PNG (*.png)|*.png|Все файлы (*.*)|*.*";
                if (d.ShowDialog() == false)
                    return;
                currentPic = File.ReadAllBytes(d.FileName);
                imgFlag = true;
                imp_pick_btn.Content = "Удалить фотографию";
                mainPic.Source = ByteImage.Convert(ByteImage.GetImageFromByteArray(currentPic));
            }
        }
    }
}
