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
    /// Logika interakcji dla klasy InputDialogWindow.xaml
    /// </summary>
    public partial class InputDialogWindow : Window
    {
        public bool IsConfirmed = false;
        public bool ConfirmWithEnter = true;

        public InputDialogWindow(string title, string labelValue = "", string inputValue = "")
        {
            InitializeComponent();

            this.Title = title;
            this.KeyDown += ConfirmWithEnterEvent;

            this.FirstLabel.Text = labelValue;
            this.FirstInput.Text = inputValue;

            this.FirstInput.Focus();
            this.FirstInput.SelectionStart = int.MaxValue;
        }

        public string GetValue()
        {
            return FirstInput.Text;
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

        private void ConfirmWithEnterEvent(object sender, KeyEventArgs e)
        {
            if (this.ConfirmWithEnter == false) return;

            if (e.Key == Key.Return)
            {
                this.Confirm();
            }
        }
    }
}
