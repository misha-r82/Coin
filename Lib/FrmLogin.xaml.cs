using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lib
{
    /// <summary>
    /// Interaction logic for FrmLogin.xaml
    /// </summary>
    public partial class FrmLogin : Window
    {
        public FrmLogin()
        {
            InitializeComponent();
            okPressed = false;
        }
        public bool okPressed;
        public string getlogin()
        {
            return txtLogin.Text;
        }
        public string getPassword()
        {
            return txtPassword.Password;
        }

        public void setUserText(string userTxt)
        {
            lblInfo.Text = userTxt;
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            okPressed = true;
        }
    }
}
