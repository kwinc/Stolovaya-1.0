using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    /// Логика взаимодействия для dbControlPanel.xaml
    /// </summary>
    public partial class dbControlPanel : Page
    {
        DataTable curUser;
        List<idParam> postsInClass;
        List<idParam> decomInClass;
        List<idParam> unitTInClass;

        bool costil = false;
        public dbControlPanel(DataTable u)
        {
            InitializeComponent();
            curUser = u;
            costil = true;
            edit_type_cb.SelectedIndex = 0;
            update_btn_Click(null, null);
        }

        private void edit_type_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (costil)
                update_btn_Click(null, null);
        }

        async Task<List<idParam>> get(string s)
        {
            List<idParam> tmp = new List<idParam>();
            DataTable db = libs.dbc.Select($"SELECT * FROM dbo.{s}");

            await Task.Run(() =>
            {
                for (int i = 0; i < db.Rows.Count; i++)
                {
                    if (s == "posts")
                    {
                        tmp.Add(new idParam()
                        {
                            id = db.Rows[i].Field<int>("id_post"),
                            name = db.Rows[i].Field<string>("name_post"),
                            desc = db.Rows[i].Field<string>("desc_post")
                        });
                    }
                    else if (s == "decommissioned_reasons")
                    {
                        tmp.Add(new idParam()
                        {
                            id = db.Rows[i].Field<int>("id_reason"),
                            name = db.Rows[i].Field<string>("name_reason"),
                            desc = db.Rows[i].Field<string>("desc_reason")
                        });
                    }
                    else if (s == "unit_types")
                    {
                        tmp.Add(new idParam()
                        {
                            id = db.Rows[i].Field<int>("id_unit"),
                            name = db.Rows[i].Field<string>("name_unit"),
                            desc = db.Rows[i].Field<string>("desc_unit")
                        });
                    }
                    else
                    {
                        MessageBox.Show("Произошла внутренняя ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        this.NavigationService.GoBack();
                    }
                }
            });
            return tmp;
        }
        private async void update_btn_Click(object sender, RoutedEventArgs e)
        {
            loading_anim.Visibility = Visibility.Visible;

            switch (edit_type_cb.SelectedIndex)
            {
                case 0:
                    postsInClass = await get("posts");
                    mainDG.ItemsSource = postsInClass; break;
                case 1:
                    decomInClass = await get("decommissioned_reasons");
                    mainDG.ItemsSource = decomInClass; break;
                case 2:
                    unitTInClass = await get("unit_types");
                    mainDG.ItemsSource = unitTInClass; break;
            }

            loading_anim.Visibility = Visibility.Hidden;
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            string tmp  = "";
            string tmpS = "";
            string tmpT = "";            
            switch (edit_type_cb.SelectedIndex)
            {
                case 0: 
                    tmp = "должности";          
                    tmpS = "posts";                  
                    tmpT = "_post";
                    break;
                case 1: 
                    tmp = "причины списания";   
                    tmpS = "decommissioned_reasons"; 
                    tmpT = "_reason";  
                    break;
                case 2: 
                    tmp = "единицы измерения";  
                    tmpS = "unit_types";             
                    tmpT = "_unit";    
                    break;
            }
            dbManipulating win = new dbManipulating(new forDBmanip() { isEdit = false, title = $"Добавление {tmp}", fast_db = tmpS, fast_db_row = tmpT });
            win.ShowDialog();
            update_btn_Click(null, null);
        }

        private void edit_btn_Click(object sender, RoutedEventArgs e)
        {
            if (mainDG.SelectedIndex != -1)
            {
                string tmp = "";
                string tmpS = "";
                string tmpT = "";
                idParam tmpF = new idParam();
                switch (edit_type_cb.SelectedIndex)
                {
                    case 0:
                        tmp = "должности";
                        tmpS = "posts";
                        tmpT = "_post";
                        tmpF = postsInClass[mainDG.SelectedIndex];
                        break;
                    case 1:
                        tmp = "причины списания";
                        tmpS = "decommissioned_reasons";
                        tmpT = "_reason";
                        tmpF = decomInClass[mainDG.SelectedIndex];
                        break;
                    case 2:
                        tmp = "единицы измерения";
                        tmpS = "unit_types";
                        tmpT = "_unit";
                        tmpF = unitTInClass[mainDG.SelectedIndex];
                        break;
                }
                dbManipulating win = new dbManipulating(new forDBmanip() { isEdit = true, title = $"Редактирование {tmp}", fast_db = tmpS, fast_db_row = tmpT, element = tmpF });
                win.ShowDialog();
                update_btn_Click(null, null);
            }
            else
            {
                MessageBox.Show("Не выбрана запись для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            switch (edit_type_cb.SelectedIndex)
            {
                case 0:
                    if (MessageBoxResult.Yes == MessageBox.Show($"Действительно удалить '{postsInClass[mainDG.SelectedIndex].name}'?\nПредупреждение: Удаление привидет к потере всех сотрудников этой должности!", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question))
                    {
                        try
                        {
                            libs.dbc.Select($"DELETE FROM dbo.posts WHERE id_post = {postsInClass[mainDG.SelectedIndex].id}");
                        }
                        catch
                        {
                            MessageBox.Show("Произошла ошибка при внесении изменений в базу данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }                    
                    break;
                case 1:
                    if (MessageBoxResult.Yes == MessageBox.Show($"Действительно удалить '{decomInClass[mainDG.SelectedIndex].name}'?\nПредупреждение: Удаление приведет к потере всех списанных продуктов по этой причине!", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question))
                    {
                        try
                        {
                            libs.dbc.Select($"DELETE FROM dbo.decommissioned_reasons WHERE id_reason = {decomInClass[mainDG.SelectedIndex].id}");
                        }
                        catch
                        {
                            MessageBox.Show("Произошла ошибка при внесении изменений в базу данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    break;
                case 2:
                    if (MessageBoxResult.Yes == MessageBox.Show($"Действительно удалить '{unitTInClass[mainDG.SelectedIndex].name}'?\nПредупреждение: Удаление привидет к потере всех продуктов с этой единицей измерения!", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question))
                    {
                        try
                        {
                            libs.dbc.Select($"DELETE FROM dbo.unit_types WHERE id_unit = {unitTInClass[mainDG.SelectedIndex].id}");
                        }
                        catch
                        {
                            MessageBox.Show("Произошла ошибка при внесении изменений в базу данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    break;
            }
            update_btn_Click(null, null);
        }
    }
}
