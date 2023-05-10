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

namespace TravelAgent.CustomComponent
{
    /// <summary>
    /// Interaction logic for BindablePasswordBox.xaml
    /// </summary>
    public partial class BindablePasswordBox : UserControl
    {
        // Password box style property
        //----------------------------------------------
        public static readonly DependencyProperty PasswordBoxStyleProperty =
        DependencyProperty.Register("PasswordBoxStyle", typeof(Style), typeof(BindablePasswordBox),
            new PropertyMetadata(null, PasswordBoxStylePropertyChanged));

        public Style PasswordBoxStyle
        {
            get { return (Style)GetValue(PasswordBoxStyleProperty); }
            set { SetValue(PasswordBoxStyleProperty, value); }
        }

        private static void PasswordBoxStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BindablePasswordBox bindablePasswordBox)
            {
                bindablePasswordBox.ApplyPasswordBoxStyle();
            }
        }

        private void ApplyPasswordBoxStyle()
        {
            passwordBox.Style = PasswordBoxStyle;
        }
        //----------------------------------------------

        // Password value property 
        //----------------------------------------------
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(BindablePasswordBox), 
                new PropertyMetadata(string.Empty, PasswordPropertyChanged));

        private static void PasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BindablePasswordBox bindablePasswordBox)
            {
                bindablePasswordBox.UpdatePassword();
            } 
        }

        private bool _isPasswordChanging = false;

        private void UpdatePassword()
        {
            if (!_isPasswordChanging)
            {
                passwordBox.Password = Password;
            }
        }
        //----------------------------------------------

        public BindablePasswordBox()
        {
            InitializeComponent();
            ApplyPasswordBoxStyle();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _isPasswordChanging = true;
            Password = passwordBox.Password;
            _isPasswordChanging = false;
        }


    }
}
