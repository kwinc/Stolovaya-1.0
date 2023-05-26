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
using System.Windows.Shapes;

namespace Stolovaya_1._0.res.worker
{
    /// <summary>
    /// Логика взаимодействия для mainWorkerPage.xaml
    /// </summary>
    public partial class mainWorkerPage : Window
    {
        DataTable curUser;
        public mainWorkerPage(DataTable u)
        {
            InitializeComponent();
            curUser = u;
        }
    }
}
