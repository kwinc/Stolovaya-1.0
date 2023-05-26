using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Xceed.Wpf.Toolkit;

namespace Stolovaya_1._0.res.admin.panels
{
    /// <summary>
    /// Логика взаимодействия для personalManipulating.xaml
    /// </summary>
    public partial class personalManipulating : Page
    {
        bool isE;
        DataTable pfe;
        List<idParam> posts = new List<idParam>();
        public personalManipulating(bool isEdit, DataTable pFe = null)
        {
            InitializeComponent();
            isE = isEdit;
            pfe = pFe;
            #region Запроление комбобокса с постами
            DataTable tmp = libs.dbc.Select("SELECT * FROM dbo.posts");
            for (int i = 0; i < tmp.Rows.Count; i++)
            {
                posts.Add(new idParam()
                {
                    id = tmp.Rows[i].Field<int>("id_post"),
                    name = tmp.Rows[i].Field<string>("name_post"),
                    desc = tmp.Rows[i].Field<string>("desc_post"),
                });
            }
            post_add.ItemsSource = posts;
            #endregion

            if (isEdit)
            {
                fio_add.Text = pFe.Rows[0].Field<string>("fio");
                login_add.Text = pFe.Rows[0].Field<string>("login");
                password_add.Password = pFe.Rows[0].Field<string>("password");
                isAdmin_add.IsChecked = pFe.Rows[0].Field<bool>("isAdmin");
                string p = pFe.Rows[0].Field<string>("phone");
                phone_add.Value = $"{p[0]} ({p[1]}{p[2]}{p[3]}) {p[4]}{p[5]}{p[6]} {p[7]}{p[8]} - {p[9]}{p[10]}"; //тут без мата никак
                email_add.Text = pFe.Rows[0].Field<string>("email");
                birthday_add.SelectedDate = pFe.Rows[0].Field<DateTime>("birthday");
                post_add.SelectedValue = pFe.Rows[0].Field<int>("post");
            }
        }
        public bool IsValidEmailAddress(string s)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(s);
        }
        public bool isValidFIO(string s)
        {
            if (s.Split(' ').Length == 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void fio_add_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsLetter(c))
                {
                    e.Handled = true;
                    break;
                }
            }
        }
        private void login_add_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    e.Handled = true;
                    break;
                }
            }
        }
        private void email_add_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    if (c == '@' || c == '.' || c == '_' || c == '-')
                    {
                        e.Handled = false;
                        break;
                    }
                    e.Handled = true;
                    break;
                }

            }
        }
        private void phone_add_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
        private void goBack_btn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
        private bool isValidLogin(string s)
        {
            DataTable tmp = libs.dbc.Select($"SELECT * FROM dbo.personal WHERE login = '{s}'");
            int crit = isE ? 1 : 0; //я королева тернарных операторов после этого :0
            return tmp.Rows.Count <= crit;
        }
        private void action_btn_Click(object sender, RoutedEventArgs e)
        {
            string phone = phone_add.Text;
            var charsToRemove = new string[] { "(", ")", " ", "-" };
            foreach (var c in charsToRemove)
            {
                phone = phone.Replace(c, string.Empty);
            }
            bool isAdmin_prepare = (bool)isAdmin_add.IsChecked;
            string isAdmin = "";
            if (isAdmin_prepare)
            {
                isAdmin = "1";
            }
            else
            {
                isAdmin = "0";
            }

            if (fio_add.Text != "" && isValidFIO(fio_add.Text) &&
                login_add.Text != "" &&
                password_add.Password != "" &&
                phone != "" &&
                IsValidEmailAddress(email_add.Text) &&
                libs.dbc.isAgeAllowed(18, (DateTime)birthday_add.SelectedDate))
            {
                if (isValidLogin(login_add.Text))
                {
                    if (isE)
                    {
                        try
                        {
                            libs.dbc.Select($"UPDATE dbo.personal SET fio = '{fio_add.Text}', login = '{login_add.Text}', password = '{password_add.Password}', isAdmin = {isAdmin}, phone = '{phone}', email = '{email_add.Text}', birthday = '{birthday_add.SelectedDate}', post = {post_add.SelectedValue} WHERE id_person = {pfe.Rows[0].Field<int>("id_person")}");
                        }
                        catch
                        {
                            System.Windows.MessageBox.Show("Ошибка при внесении изменений в базу данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    else
                    {
                        try
                        {
                            libs.dbc.Select($"INSERT INTO dbo.personal (fio, login, password, isAdmin, phone, email, birthday, post) VALUES ('{fio_add.Text}', '{login_add.Text}', '{password_add.Password}', {isAdmin}, '{phone}', '{email_add.Text}', '{birthday_add.SelectedDate}', {post_add.SelectedValue})");
                        }
                        catch
                        {
                            System.Windows.MessageBox.Show("Ошибка при внесении изменений в базу данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Данный логин занят", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Неккоректность данных. Возможные причины:\n1. Не полное ФИО\n2.Что-то не введено\n3. Введен некорректный E-Mail адрес\n4. Указан несовершеннолетний возраст", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            this.NavigationService.GoBack();
        }   
    }
}
