using Noam.Data.ViewModels;
using System;
using System.Collections.Generic;
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

namespace Noam.NET
{
    /// <summary>
    /// Interaction logic for DisplayUserControl.xaml
    /// </summary>
    public partial class DisplayUserControl : UserControl
    {
        public DisplayUserControl()
        {
            InitializeComponent();

        }

        public UserViewModel SelectedUser
        {
            get { return (UserViewModel)GetValue(SelectedUserProperty); }
            set
            {
                if (GetValue(SelectedUserProperty) != value)
                {
                    SetValue(SelectedUserProperty, value);
                    this.DataContext = value;
                }
            }
        }

        public static DependencyProperty SelectedUserProperty = DependencyProperty.Register("SelectedUser", typeof(UserViewModel), typeof(DisplayUserControl));
    }
}
