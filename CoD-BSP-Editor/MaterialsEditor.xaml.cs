using CoD_BSP_Editor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Logika interakcji dla klasy MaterialsEditor.xaml
    /// </summary>
    public partial class MaterialsEditor : Window
    {
        public MaterialsEditor()
        {
            InitializeComponent();

            this.InitializeMaterialsEditView();
        }

        private void InitializeMaterialsEditView()
        {
            int index = 0;
            foreach (Shader shader in MainWindow.bsp.Shaders)
            {
                this.CreateMaterialEditField(index, shader);
                index++;
            }
        }

        private void CreateMaterialEditField(int index, Shader shader)
        {
            Thickness Margin = new Thickness(10, 5, 10, 5);
            Thickness Padding = new Thickness(10, 5, 10, 5);

            StackPanel InputContainer = new StackPanel();
            InputContainer.Orientation = Orientation.Horizontal;
            InputContainer.Tag = index;

            TextBlock IndexText = new TextBlock()
            { Text = $"[{index}]", FontSize = 20, Width = 80, Margin = new Thickness(5), Padding = Padding };

            TextBox MaterialInput = new TextBox()
            { Text = ShaderUtils.GetMaterial(shader), FontSize = 20, Width = 800, Margin = Margin, Padding = Padding, MaxLength = 64};
            MaterialInput.TextChanged += OnInputChange;

            TextBox FlagInput = new TextBox()
            { Text = shader.Flags[0].ToString(), FontSize = 20, Width = 150, Margin = Margin, Padding = Padding };
            FlagInput.TextChanged += OnInputChange;
            FlagInput.PreviewTextInput += NumberValidationTextBox;
            
            TextBox ContentFlagInput = new TextBox()
            { Text = shader.Flags[1].ToString(), FontSize = 20, Width = 150, Margin = Margin, Padding = Padding };
            ContentFlagInput.TextChanged += OnInputChange;
            ContentFlagInput.PreviewTextInput += NumberValidationTextBox;

            ShaderFields.Children.Add(InputContainer);

            InputContainer.Children.Add(IndexText);
            InputContainer.Children.Add(MaterialInput);
            InputContainer.Children.Add(FlagInput);
            InputContainer.Children.Add(ContentFlagInput);
        }

        private void OnInputChange(object sender, TextChangedEventArgs e)
        {
            if (MainWindow.bsp == null) return;

            StackPanel parent = (StackPanel)(sender as TextBox).Parent;
            int shaderIndex = (int)parent.Tag;

            /* Data Extracting */
            TextBox MaterialInput = parent.Children[1] as TextBox;
            TextBox FlagInput = parent.Children[2] as TextBox;
            TextBox ContentFlagInput = parent.Children[3] as TextBox;

            string Material = MaterialInput.Text;
            string FlagStr = FlagInput.Text;
            string ContentFlagStr = ContentFlagInput.Text;

            /* Error Checks */
            bool canParseFlag = uint.TryParse(FlagStr, out _);
            bool canParseContentFlag = uint.TryParse(ContentFlagStr, out _);

            if (string.IsNullOrEmpty(Material))
            {
                Material = "default";
            }

            if (string.IsNullOrEmpty(FlagStr) || canParseFlag == false)
            {
                FlagStr = "0";
            }
            
            if (string.IsNullOrEmpty(ContentFlagStr) || canParseContentFlag == false)
            {
                ContentFlagStr = "0";
            }

            /* Parsing */
            uint Flag = uint.Parse(FlagStr);
            uint ContentFlag = uint.Parse(ContentFlagStr);

            /* Updating shaders list */
            Shader shader = ShaderUtils.Construct(Material, Flag, ContentFlag);
            MainWindow.bsp.Shaders[shaderIndex] = shader;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void FindMaterialIndex(object sender, RoutedEventArgs e)
        {
            if (MainWindow.bsp == null) return;

            DoubleInputDialogWindow wndDialog = new DoubleInputDialogWindow("Find material");
            wndDialog.FirstLabel.Text = "Enter material name";
            wndDialog.SecondLabel.Text = "Enter start index (leave empty for default 0):";

            wndDialog.ShowDialog();

            if (wndDialog.IsConfirmed == false)
            {
                return;
            }

            var (material, startIndexStr) = wndDialog.GetValue();

            if (string.IsNullOrEmpty(material))
            {
                MessageBox.Show("Enter valid material name");
                return;
            }

            material = material.ToLower().Trim();

            if (string.IsNullOrEmpty(startIndexStr))
            {
                startIndexStr = "0";
            }
            else
            {
                startIndexStr = startIndexStr.ToLower().Trim();
            }

            bool exactSearch = material.StartsWith('*') == false && material.EndsWith('*') == false;
            bool startsWithSearch = material.StartsWith('*') == false && material.EndsWith('*') == true;
            bool endsWithSearch = material.StartsWith('*') == true && material.EndsWith('*') == false;
            bool containsSearch = material.StartsWith('*') == true && material.EndsWith('*') == true;

            material = material.Trim('*');

            int startIndex = int.Parse(startIndexStr);
            for (int i = startIndex; i < MainWindow.bsp.Shaders.Count; i++)
            {
                string shaderName = ShaderUtils.GetMaterial(MainWindow.bsp.Shaders[i]).ToLower();
                int foundAt = -1;

                if (exactSearch && shaderName == material)
                {
                    foundAt = i;
                }
                else if (startsWithSearch && shaderName.StartsWith(material))
                {
                    foundAt = i;
                }
                else if (endsWithSearch && shaderName.EndsWith(material))
                {
                    foundAt = i;
                }
                else if (containsSearch && shaderName.Contains(material))
                {
                    foundAt = i;
                }

                if (foundAt != -1)
                {
                    MessageBox.Show($"Material found on index {foundAt}");
                    return;
                }
            }

            MessageBox.Show("No materials found");
        }
    }
}
