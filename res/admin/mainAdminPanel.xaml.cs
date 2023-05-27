using Stolovaya_1._0.res.admin.panels;
using System;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Stolovaya_1._0.res.admin
{
    /// <summary>
    /// Логика взаимодействия для mainAdminPanel.xaml
    /// </summary>
    public partial class mainAdminPanel : Window
    {
        DataTable curUser;
        public mainAdminPanel(DataTable u, bool isWorker = false)
        {
            InitializeComponent();
            curUser = u;
            fio_txt.Text = curUser.Rows[0].Field<string>("fio");
            controlPanel.RenderTransform = new TranslateTransform();
            var translateY = new DoubleAnimation
            {
                From = 0,
                To = -80,
                Duration = TimeSpan.FromSeconds(0.15),
            };
            ((TranslateTransform)controlPanel.RenderTransform).BeginAnimation(TranslateTransform.YProperty, translateY);
            IsToggle = false;

            if (isWorker )
            {
                //deauth_btn.Visibility = Visibility.Visible;
                meals_btn.Visibility = Visibility.Hidden;
                menu_btn.Visibility = Visibility.Hidden;
                orders_btn.Visibility = Visibility.Hidden;
                personal_btn.Visibility = Visibility.Hidden;
                reports_btn.Visibility = Visibility.Hidden;
                foodstuff_btn.Visibility = Visibility.Hidden;
                mainSign.Text = "Управление продуктами";
                main_content.Navigate(new panels.products(curUser));
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //brd.Height = 0;
        }

        private bool IsToggle = true;

        private void navigateMenuOpenClose_Click(object sender, RoutedEventArgs e)
        {
            controlPanel.RenderTransform = new TranslateTransform();
            if (!IsToggle)
            {
                var translateY = new DoubleAnimation
                {
                    From = -80,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.15),
                };
                ((TranslateTransform)controlPanel.RenderTransform).BeginAnimation(TranslateTransform.YProperty, translateY);
                IsToggle = true;
            }
            else
            {
                var translateY = new DoubleAnimation
                {
                    From = 0,
                    To = -80,
                    Duration = TimeSpan.FromSeconds(0.15),
                };
                ((TranslateTransform)controlPanel.RenderTransform).BeginAnimation(TranslateTransform.YProperty, translateY);
                IsToggle = false;
            }
        }
        private void deauth_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult questionExit = MessageBox.Show("Действительно выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MessageBoxResult.Yes == questionExit)
            {
                MainWindow win = new MainWindow();
                win.Show();
                this.Close();
            }
        }
        private void orders_btn_Click(object sender, RoutedEventArgs e)
        {

        }
        private void personal_btn_Click(object sender, RoutedEventArgs e)
        {            
            mainSign.Text = "Управление персоналом";
            main_content.Navigate(new panels.personal(curUser));

            controlPanel.RenderTransform = new TranslateTransform();
            var translateY = new DoubleAnimation
            {
                From = 0,
                To = -80,
                Duration = TimeSpan.FromSeconds(0.15),
            };
            ((TranslateTransform)controlPanel.RenderTransform).BeginAnimation(TranslateTransform.YProperty, translateY);
            IsToggle = false;
        }
        private void foodstuff_btn_Click(object sender, RoutedEventArgs e)
        {
            mainSign.Text = "Управление продуктами";
            main_content.Navigate(new panels.products(curUser));

            controlPanel.RenderTransform = new TranslateTransform();
            var translateY = new DoubleAnimation
            {
                From = 0,
                To = -80,
                Duration = TimeSpan.FromSeconds(0.15),
            };
            ((TranslateTransform)controlPanel.RenderTransform).BeginAnimation(TranslateTransform.YProperty, translateY);
            IsToggle = false;
        }

        private void meals_btn_Click(object sender, RoutedEventArgs e)
        {
            mainSign.Text = "Управление блюдами";
            main_content.Navigate(new dishes(curUser));

            controlPanel.RenderTransform = new TranslateTransform();
            var translateY = new DoubleAnimation
            {
                From = 0,
                To = -80,
                Duration = TimeSpan.FromSeconds(0.15),
            };
            ((TranslateTransform)controlPanel.RenderTransform).BeginAnimation(TranslateTransform.YProperty, translateY);
            IsToggle = false;
        }

        private void menu_btn_Click(object sender, RoutedEventArgs e)
        {
            mainSign.Text = $"Меню на {DateTime.Now.ToString("dd.MM.yyyy")}";
            main_content.Navigate(new dailyMenu());

            controlPanel.RenderTransform = new TranslateTransform();
            var translateY = new DoubleAnimation
            {
                From = 0,
                To = -80,
                Duration = TimeSpan.FromSeconds(0.15),
            };
            ((TranslateTransform)controlPanel.RenderTransform).BeginAnimation(TranslateTransform.YProperty, translateY);
            IsToggle = false;
        }

        private void dbControl_btn_Click(object sender, RoutedEventArgs e)
        {
            mainSign.Text = "Управление базой данных";
            main_content.Navigate(new panels.dbControlPanel(curUser));

            controlPanel.RenderTransform = new TranslateTransform();
            var translateY = new DoubleAnimation
            {
                From = 0,
                To = -80,
                Duration = TimeSpan.FromSeconds(0.15),
            };
            ((TranslateTransform)controlPanel.RenderTransform).BeginAnimation(TranslateTransform.YProperty, translateY);
            IsToggle = false;
        }

        private void reports_btn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
