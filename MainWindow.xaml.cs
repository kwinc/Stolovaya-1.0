using System.Windows;
using System.Data;

namespace Stolovaya_1._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //pack://application:,,,
        DataTable users;
        public MainWindow()
        {
            InitializeComponent();           
        }

        private void auth_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                users = res.libs.dbc.Select($"SELECT * FROM dbo.personal WHERE login='{login_auth.Text}' AND password='{password_auth.Password}'");
            }
            catch
            {
                MessageBox.Show("Не удалось подключится к базе данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (users.Rows.Count != 0)
            {
                if (users.Rows[0].Field<bool>("isAdmin"))
                {
                    //запуск окна одмена
                    //MessageBox.Show($"{users.Rows[0].Field<string>("fio")}, Заведующий столовой");
                    res.admin.mainAdminPanel win = new res.admin.mainAdminPanel(users);
                    win.Show();
                    this.Close();
                }
                else
                {
                    //запуск окна работника
                    //MessageBox.Show($"{users.Rows[0].Field<string>("fio")}, Сотрудник");
                    res.admin.mainAdminPanel win = new res.admin.mainAdminPanel(users, true);
                    win.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Не удалось найти пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void admin_btn_Click(object sender, RoutedEventArgs e)
        {
            users = res.libs.dbc.Select($"SELECT * FROM dbo.personal WHERE login='a' AND password='a'");
            res.admin.mainAdminPanel win = new res.admin.mainAdminPanel(users);
            win.Show();
            this.Close();
        }

        private void server_tb_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            res.libs.dbc.connectionString = $"server={server_tb.Text};Trusted_Connection=Yes;DataBase=stolovka;";
        }
    }
}
