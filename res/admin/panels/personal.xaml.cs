using Stolovaya_1._0.res.libs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Stolovaya_1._0.res.admin.panels
{
    /// <summary>
    /// Логика взаимодействия для personal.xaml
    /// </summary>
    public partial class personal : Page
    {
        DataTable curUser;
        List<person> persons = new List<person>();
        public personal(DataTable u)
        {
            InitializeComponent();
            curUser = u;
            update_btn_Click(null, null); //говнокодер
        }
        /* 
         * как бы не было хуёво завтра будет лучше
         * я знаю завта будет новый день
         * и даже если под землей
         * я все равно оставлю след
         * ведь нищета не повод не вставать с колен
         * 22:50 18.04.2023
         */
        async Task<List<person>> getPersons()
        {
            List<person> tmp = new List<person>();
            DataTable personalFromDB = libs.dbc.Select($"SELECT * FROM dbo.personal");

            await Task.Run(() =>
            {
                for (int i = 0; i < personalFromDB.Rows.Count; i++)
                {
                    tmp.Add(new person()
                    {
                        id_person = personalFromDB.Rows[i].Field<int>("id_person"),
                        fio = personalFromDB.Rows[i].Field<string>("fio"),
                        login = personalFromDB.Rows[i].Field<string>("login"),
                        password = personalFromDB.Rows[i].Field<string>("password"),
                        isAdmin = personalFromDB.Rows[i].Field<bool>("isAdmin"),
                        phone = personalFromDB.Rows[i].Field<string>("phone"),
                        email = personalFromDB.Rows[i].Field<string>("email"),
                        birthday = personalFromDB.Rows[i].Field<DateTime>("birthday"),
                        post = new idParam()
                        {
                            id = personalFromDB.Rows[i].Field<int>("post"),
                            name = libs.dbc.Select($"SELECT * FROM dbo.posts WHERE id_post = {personalFromDB.Rows[i].Field<int>("post")}").Rows[0].Field<string>("name_post"),
                            desc = libs.dbc.Select($"SELECT * FROM dbo.posts WHERE id_post = {personalFromDB.Rows[i].Field<int>("post")}").Rows[0].Field<string>("desc_post"),
                        }
                    });
                }
            });
            return tmp;
        }
        private void search_tb_KeyUp(object sender, KeyEventArgs e)
        {
            switch (search_type_cb.SelectedIndex)
            {
                case 0: mainDG.ItemsSource = persons.Where(animal => animal.id_person.ToString().Contains(search_tb.Text)); break;
                case 1: mainDG.ItemsSource = persons.Where(animal => animal.fio.Contains(search_tb.Text)); break;
                case 2: mainDG.ItemsSource = persons.Where(animal => animal.login.Contains(search_tb.Text)); break;
                case 3: mainDG.ItemsSource = persons.Where(animal => animal.phone.Contains(search_tb.Text)); break;
                case 4: mainDG.ItemsSource = persons.Where(animal => animal.email.Contains(search_tb.Text)); break;
                case 5: mainDG.ItemsSource = persons.Where(animal => animal.birthday.ToString().Contains(search_tb.Text)); break;
                case 6: mainDG.ItemsSource = persons.Where(animal => animal.post.name.Contains(search_tb.Text)); break;
                default: MessageBox.Show("Что-то пошло не туда и не так", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); break;
            }
        }

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
        //мне не нужна поддержка я и сам вывезу
        private async void update_btn_Click(object sender, RoutedEventArgs e)
        {
            loading_anim.Visibility = Visibility.Visible;
            persons = await getPersons();
            mainDG.ItemsSource = persons;
            loading_anim.Visibility = Visibility.Hidden;
        }
        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new personalManipulating(false));
        }
        private void edit_btn_Click(object sender, RoutedEventArgs e)
        {
            if (mainDG.SelectedIndex != -1)
            {
                try
                {
                    DataTable personForEdit = dbc.Select($"SELECT * FROM dbo.personal WHERE id_person = {((person)mainDG.SelectedItem).id_person}");
                    this.NavigationService.Navigate(new personalManipulating(true, personForEdit));
                }
                catch
                {
                    MessageBox.Show("Не выбран пользователь", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Не выбрана запись для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dbc.Select($"DELETE FROM dbo.personal WHERE id_person = {((person)mainDG.SelectedItem).id_person}");
                update_btn_Click(null, null); //говнокодер2
            }
            catch
            {
                MessageBox.Show("Не выбран пользователь", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }        
    }
}
