﻿using System;
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
    /// Logika interakcji dla klasy CtfInputWindow.xaml
    /// </summary>
    public partial class CtfInputWindow : Window
    {
        public bool IsConfirmed = false;

        public CtfInputWindow()
        {
            InitializeComponent();

            this.KeyDown += ConfirmWithEnter;
        }

        public (string, string, string) GetValues()
        {
            return (ModelIndexInput.Text, AlliesFlagInput.Text, AxisFlagInput.Text);
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
