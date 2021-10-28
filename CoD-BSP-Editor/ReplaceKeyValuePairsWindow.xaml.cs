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
using System.Windows.Shapes;

namespace CoD_BSP_Editor
{
    /// <summary>
    /// Logika interakcji dla klasy ReplaceKeyValuePairsWindow.xaml
    /// </summary>
    public partial class ReplaceKeyValuePairsWindow : Window
    {
        public bool IsConfirmed = false;

        public ReplaceKeyValuePairsWindow()
        {
            InitializeComponent();

            this.KeyDown += ConfirmWithEnter;
        }

        public (string, string, string, string) GetValues()
        {
            return (
                ClassnameInput.Text?.ToLower().Trim(),
                KeyValueInput.Text?.ToLower().Trim(),
                NewClassnameInput.Text?.ToLower().Trim(),
                ReplaceInput.Text?.ToLower().Trim()
            );
        }

        private void Confirm()
        {
            this.IsConfirmed = true;
            this.Close();
        }

        private void OnConfirm(object sender, RoutedEventArgs e)
        {
            Confirm();
        }

        private void ConfirmWithEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.Confirm();
            }
        }
    }
}
