using CoD_BSP_Editor.BSP;
using CoD_BSP_Editor.Data;
using Microsoft.Win32;
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

namespace CoD_BSP_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static d3dbsp bsp = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateCurrentEntityText()
        {
            if (EntityBoxList.SelectedIndex == -1)
            {
                CurrentEntityText.Text = "Select entity to edit Key Value pairs";
            }
            else
            {
                Entity entity = (Entity)EntityBoxList.Items[EntityBoxList.SelectedIndex];

                if (entity != null)
                {
                    CurrentEntityText.Text = $"Selected Entity: '{entity.Classname}'";
                }
                else
                {
                    CurrentEntityText.Text = "Could not read entity";
                }
            }
        }

        private void CloseAllWindows()
        {
            foreach (Window win in App.Current.Windows)
            {
                if (win != this) win.Close();
            }
        }
        
        private void InitializeWorkingEnvironment(List<Entity> EntityList)
        {
            KeyValueFields.Children.Clear();
            EntityBoxList.Items.Clear();

            foreach (Entity entity in EntityList)
            {
                EntityBoxList.Items.Add(entity);
            }

            EntityBoxList.SelectedIndex = 0;
        }

        private void CreateKeyValueField(string key, string value, int index)
        {
            Thickness Margin = new Thickness(10, 5, 10, 5);
            Thickness Padding = new Thickness(10, 5, 10, 5);

            StackPanel InputContainer = new StackPanel();
            InputContainer.Orientation = Orientation.Horizontal;
            InputContainer.Tag = index;

            Button RemoveButton = new Button()
            { Content = "-", FontSize = 18, Margin = Margin, Padding = new Thickness(10), Background = Brushes.Transparent, };
            RemoveButton.Click += RemoveField;

            TextBox KeyInput = new TextBox()
            { Text = key, FontSize = 20, Width = 300, Margin = Margin, Padding = Padding };
            KeyInput.TextChanged += OnInputChange;

            TextBox ValueInput = new TextBox()
            { Text = value, FontSize = 20, Width = 1000, Margin = Margin, Padding = Padding };
            ValueInput.TextChanged += OnInputChange;

            KeyValueFields.Children.Add(InputContainer);

            InputContainer.Children.Add(RemoveButton);
            InputContainer.Children.Add(KeyInput);
            InputContainer.Children.Add(ValueInput);
        }

        private void CreateEditView()
        {
            KeyValueFields.Children.Clear();

            Entity SelectedEntity = (Entity)EntityBoxList.SelectedItem;

            if (SelectedEntity == null)
            {
                UpdateCurrentEntityText();
                return;
            }

            UpdateCurrentEntityText();

            Thickness Margin = new Thickness(10, 5, 10, 5);
            Thickness Padding = new Thickness(10, 5, 10, 5);

            StackPanel AddNewFieldContainer = new StackPanel();
            AddNewFieldContainer.Orientation = Orientation.Horizontal;

            Button AddNewFieldButton = new Button()
            { Content = "+ Add new field", FontSize = 18, Margin = Margin, Padding = Padding };
            AddNewFieldButton.Click += AddNewField;

            KeyValueFields.Children.Add(AddNewFieldContainer);
            AddNewFieldContainer.Children.Add(AddNewFieldButton);

            int index = 0;
            foreach (var (Key, Value) in SelectedEntity.KeyValues)
            {
                CreateKeyValueField(Key, Value, index++);
            }
        }
    }
}
