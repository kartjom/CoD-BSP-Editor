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
        private bool IsConfirmed = false;

        public InputDialogWindow(string title, string promptText, string inputDefaultValue = "", bool confirmWithEnter = true)
        {
            InitializeComponent();

            this.Title = title;
            this.PromptText.Text = promptText;
            this.ValueTextBox.Text = inputDefaultValue;

            this.ValueTextBox.Focus();
            this.ValueTextBox.SelectionStart = this.ValueTextBox.Text.Length;

            if (confirmWithEnter)
            {
                this.KeyDown += ConfirmWithEnter;
            }
        }

        public string GetValue()
        {
            return ValueTextBox != null && IsConfirmed ? ValueTextBox.Text : null;
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
