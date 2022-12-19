using CoD_BSP_Editor.Data;
using CoD_BSP_Editor.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

using Brush = CoD_BSP_Editor.Data.Brush;

namespace CoD_BSP_Editor
{
    /// <summary>
    /// Logika interakcji dla klasy BrushEditor.xaml
    /// </summary>
    public partial class BrushEditor : Window
    {
        private readonly SolidColorBrush SectionBackground = (SolidColorBrush)new BrushConverter().ConvertFrom("#F0F0F0");
        private readonly float Font = 20;
        private readonly Thickness SectionMargin = new(5);
        private readonly Thickness BrushSidesMargin = new(25, 5, 0, 5);
        private readonly Thickness TextInputsMargin = new(25, 0, 10, 0);

        private Dictionary<int, int> BrushModelIndexes = new(); // [BrushIndex] = ModelIndex;
        private List<BrushInfo> BrushData = new();

        private string LastFindBrushOrigin = "0 0 0";
        private string LastFindBrushShader = "";

        public BrushEditor()
        {
            InitializeComponent();

            this.CreateBrushModelMap();
            this.PrepareBrushData();
            this.InitializeBrushesEditView();
        }

        private void CreateBrushModelMap()
        {
            int modelIndex = 0;
            foreach (Model model in MainWindow.bsp.Models)
            {
                int firstBrushIndex = (int)model.BrushesOffset;

                for (int i = 0; i < model.BrushesSize; i++)
                {
                    BrushModelIndexes[firstBrushIndex + i] = modelIndex;
                }

                modelIndex++;
            }
        }

        private void PrepareBrushData()
        {
            for (int index = 0; index < MainWindow.bsp.Brushes.Count; index++)
            {
                int bsOffset = MainWindow.bsp.FindBrushSidesOffset(index);
                int bsCount = MainWindow.bsp.Brushes[index].Sides;

                BrushInfo brushInfo = new BrushInfo(index, bsOffset, bsCount);
                BrushData.Add(brushInfo);
            }
        }

        private void InitializeBrushesEditView()
        {
            foreach (BrushInfo brushInfo in this.BrushData)
            {
                int modelIndex = this.BrushModelIndexes[brushInfo.Index];
                this.CreateBrushEditField(brushInfo.Index, modelIndex);
            }
        }

        private void CreateBrushEditField(int index, int modelIndex)
        {
            Expander brushSectionExpander = new Expander()
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = SectionMargin, Padding = SectionMargin,
                FontSize = Font, Background = SectionBackground,

                Header = $"Brush *{modelIndex} | [{index}] - ( {BrushData[index].Center.String(", ")} )",
                Tag = index // Brush Index
            };
            brushSectionExpander.Expanded += OnFirstExpand;

            BrushFields.Children.Add(brushSectionExpander);
        }

        private void OnFirstExpand(object sender, RoutedEventArgs e)
        {
            Expander brushSectionExpander = (Expander)sender;
            brushSectionExpander.Expanded -= OnFirstExpand;

            int index = (int)brushSectionExpander.Tag;

            Brush brush = BrushData[index].GetBrush();
            List<BrushSides> brushSides = BrushData[index].GetSides();

            /* It will hold the brush data */
            StackPanel brushDataContainer = new StackPanel()
            { Orientation = Orientation.Vertical };

            brushSectionExpander.Content = brushDataContainer;

            /* Brush Shader Info */
            StackPanel brushShaderContainer = new StackPanel()
            { Orientation = Orientation.Horizontal, Margin = BrushSidesMargin };

            TextBlock brushShaderTextBlock = new TextBlock()
            { Text = "Brush Shader: ", FontSize = Font, Margin = new Thickness(0, 0, 10, 0) };

            TextBox brushShaderInput = new TextBox()
            { Text = brush.MaterialID.ToString(), Tag = index, Width = 65, FontSize = Font, TextAlignment = TextAlignment.Center };
            brushShaderInput.TextChanged += BrushShaderChange_Handler;

            brushDataContainer.Children.Add(brushShaderContainer);
            brushShaderContainer.Children.Add(brushShaderTextBlock);
            brushShaderContainer.Children.Add(brushShaderInput);

            for (int i = 0; i < brushSides.Count; i++)
            {
                StackPanel bsInfoContainer = new StackPanel()
                { Orientation = Orientation.Horizontal, Margin = BrushSidesMargin };

                /* Brush Side Index */
                TextBlock bsIndexText = new TextBlock()
                { Width = 40, Text = $"[{i}]", FontSize = Font };

                /* Shader Index */
                TextBlock bsShaderIndexText = new TextBlock()
                { Text = "Shader Index: ", Margin = TextInputsMargin };

                TextBox bsShaderInput = new TextBox()
                { Tag = i, Width = 65, FontSize = Font, TextAlignment = TextAlignment.Center, Text = brushSides[i].MaterialID.ToString() };
                bsShaderInput.TextChanged += BrushSideShaderChange_Handler;

                /* Offset */
                TextBlock bsOffsetText = new TextBlock()
                { Text = "Offset: ", Margin = TextInputsMargin };

                TextBox bsOffsetInput = new TextBox()
                { Tag = i, Width = 140, FontSize = Font, TextAlignment = TextAlignment.Center };

                if (i >= 6)
                {
                    int planeIndex = (int)brushSides[i].GetPlaneIndex();
                    Plane plane = MainWindow.bsp.Planes[planeIndex];

                    string PlaneOffsetString = $"{plane.D}";
                    bsOffsetInput.Text = PlaneOffsetString;
                }
                else
                {
                    string BrushSideOffsetString = $"{brushSides[i].GetDistance()}";
                    bsOffsetInput.Text = BrushSideOffsetString;
                }

                bsOffsetInput.TextChanged += BrushSideOffsetChange_Handler;

                brushDataContainer.Children.Add(bsInfoContainer);

                bsInfoContainer.Children.Add(bsIndexText);

                bsInfoContainer.Children.Add(bsShaderIndexText);
                bsInfoContainer.Children.Add(bsShaderInput);

                bsInfoContainer.Children.Add(bsOffsetText);
                bsInfoContainer.Children.Add(bsOffsetInput);
            }
        }

        private void BrushShaderChange_Handler(object sender, TextChangedEventArgs e)
        {
            if (MainWindow.bsp == null) return;

            TextBox BrushShaderInput = sender as TextBox;
            int BrushIndex = (int)BrushShaderInput.Tag;

            string String_ShaderIndex = BrushShaderInput.Text;
            String_ShaderIndex = new string(String_ShaderIndex.Where(char.IsDigit).ToArray());

            if (String_ShaderIndex.Length > 1)
            {
                String_ShaderIndex = String_ShaderIndex.TrimStart('0');
            }

            if (string.IsNullOrEmpty(String_ShaderIndex))
            {
                String_ShaderIndex = "0";
            }

            try
            {
                ushort ShaderIndex = ushort.Parse(String_ShaderIndex);

                Brush brush = MainWindow.bsp.Brushes[BrushIndex];
                brush.MaterialID = ShaderIndex;
                MainWindow.bsp.Brushes[BrushIndex] = brush;

                BrushShaderInput.Text = String_ShaderIndex;
                BrushShaderInput.CaretIndex = String_ShaderIndex.Length;
            }
            catch
            {
                MessageBox.Show($"Couldn't parse value '{String_ShaderIndex}'");
                BrushShaderInput.Text = "0";
            }
        }

        private void BrushSideShaderChange_Handler(object sender, TextChangedEventArgs e)
        {
            if (MainWindow.bsp == null) return;

            TextBox BrushSideShaderInput = sender as TextBox;
            Expander BrushInfoContainer = ((BrushSideShaderInput.Parent as StackPanel).Parent as StackPanel).Parent as Expander;

            int BrushIndex = (int)BrushInfoContainer.Tag;
            int BrushSideIndex = (int)BrushSideShaderInput.Tag;

            string String_ShaderIndex = BrushSideShaderInput.Text;
            String_ShaderIndex = new string(String_ShaderIndex.Where(char.IsDigit).ToArray());

            if (String_ShaderIndex.Length > 1)
            {
                String_ShaderIndex = String_ShaderIndex.TrimStart('0');
            }

            if (string.IsNullOrEmpty(String_ShaderIndex))
            {
                String_ShaderIndex = "0";
            }

            try
            {
                ushort ShaderIndex = ushort.Parse(String_ShaderIndex);

                int SideOffset = BrushData[BrushIndex].SidesOffset + BrushSideIndex;

                BrushSides brushSide = MainWindow.bsp.BrushSides[SideOffset];
                brushSide.MaterialID = ShaderIndex;
                MainWindow.bsp.BrushSides[SideOffset] = brushSide;

                BrushSideShaderInput.Text = String_ShaderIndex;
                BrushSideShaderInput.CaretIndex = String_ShaderIndex.Length;
            }
            catch
            {
                MessageBox.Show($"Couldn't parse value '{String_ShaderIndex}'");
                BrushSideShaderInput.Text = "0";
            }
        }

        private void BrushSideOffsetChange_Handler(object sender, TextChangedEventArgs e)
        {
            if (MainWindow.bsp == null) return;

            TextBox BrushSideOffsetInput = sender as TextBox;
            Expander BrushInfoContainer = ((BrushSideOffsetInput.Parent as StackPanel).Parent as StackPanel).Parent as Expander;

            int BrushIndex = (int)BrushInfoContainer.Tag;
            int ModelIndex = this.BrushModelIndexes[BrushIndex];
            int BrushSideIndex = (int)BrushSideOffsetInput.Tag;

            string String_Offset = BrushSideOffsetInput.Text;
            String_Offset = new string(String_Offset.Where(x => char.IsDigit(x) || x == '.' || x == '-' || x == 'E').ToArray());

            if (string.IsNullOrEmpty(String_Offset))
            {
                String_Offset = "0";
            }

            try
            {
                string String_OffsetToParse = String_Offset;
                float Offset;

                if (String_Offset.EndsWith('.'))
                {
                    Offset = float.Parse(String_OffsetToParse + '0');
                }
                else if (String_OffsetToParse.Length == 1 && String_OffsetToParse.StartsWith('-'))
                {
                    Offset = float.Parse(String_OffsetToParse + '0');
                }
                else
                {
                    Offset = float.Parse(String_OffsetToParse);
                }

                int SideOffset = BrushData[BrushIndex].SidesOffset + BrushSideIndex;

                if (BrushSideIndex > 5)
                {
                    // Plane
                    int planeIndex = (int)MainWindow.bsp.BrushSides[SideOffset].GetPlaneIndex();

                    Plane plane = MainWindow.bsp.Planes[planeIndex];
                    plane.D = Offset;
                    MainWindow.bsp.Planes[planeIndex] = plane;
                }
                else
                {
                    // BrushSide
                    BrushSides brushSide = MainWindow.bsp.BrushSides[SideOffset];
                    brushSide.PlaneDistanceUnion = BinLib.ToByteArray<float>(Offset);
                    MainWindow.bsp.BrushSides[SideOffset] = brushSide;
                }

                BrushSideOffsetInput.Text = String_Offset;
                BrushSideOffsetInput.CaretIndex = String_Offset.Length;

                // Recalculate center
                BrushData[BrushIndex].CalculateCenterFromSides();

                string Center = BrushData[BrushIndex].Center.String(", ");
                BrushInfoContainer.Header = $"Brush *{ModelIndex} | [{BrushIndex}] - ( {Center} )";
            }
            catch
            {
                MessageBox.Show($"Couldn't parse value '{String_Offset}'");
                BrushSideOffsetInput.Text = "0";

                // Recalculate center
                BrushData[BrushIndex].CalculateCenterFromSides();

                string Center = BrushData[BrushIndex].Center.String(", ");
                BrushInfoContainer.Header = $"Brush *{ModelIndex} | [{BrushIndex}] - ( {Center} )";
            }
        }

        private void FindBrush(object sender, RoutedEventArgs e)
        {
            if (MainWindow.bsp == null) return;

            DoubleInputDialogWindow wndDialog = new DoubleInputDialogWindow("Find brush");
            wndDialog.FirstLabel.Text = "Enter brush origin:";
            wndDialog.FirstInput.Text = this.LastFindBrushOrigin;
            wndDialog.SecondLabel.Text = "Search by shader index (empty for all):";
            wndDialog.SecondInput.Text = this.LastFindBrushShader;

            wndDialog.ShowDialog();

            if (wndDialog.IsConfirmed == false)
            {
                return;
            }

            var (BrushOrigin, BrushShaderIndex) = wndDialog.GetValue();

            if (string.IsNullOrEmpty(BrushOrigin))
            {
                MessageBox.Show("Enter valid origin");
                return;
            }
            
            if (string.IsNullOrEmpty(BrushShaderIndex))
            {
                BrushShaderIndex = "-1";
            }

            Vector3 SeekOrigin;
            int Shader;
            try
            {
                SeekOrigin = Vec3.FromString(BrushOrigin);

                Shader = int.Parse(BrushShaderIndex);
            }
            catch
            {
                MessageBox.Show("Could not parse data");
                return;
            }

            float closestDistance = int.MaxValue;
            int closestDistanceIndex = -1;

            for (int i = 0; i < BrushData.Count; i++)
            {
                if (Shader != -1)
                {
                    Brush brush = BrushData[i].GetBrush();

                    if (Shader != brush.MaterialID)
                    {
                        continue;
                    }
                }

                float newDistance = Vector3.Distance(SeekOrigin, BrushData[i].Center);
                if (newDistance < closestDistance)
                {
                    closestDistance = newDistance;
                    closestDistanceIndex = i;
                }
            }

            this.LastFindBrushOrigin = BrushOrigin;
            this.LastFindBrushShader = Shader == -1 ? "" : BrushShaderIndex;

            if (closestDistanceIndex != -1)
            {
                Expander foundElement = BrushFields.Children[closestDistanceIndex] as Expander;
                foundElement.IsExpanded = true;
                foundElement.BringIntoView();

                MessageBox.Show($"Closest Brush is at index {closestDistanceIndex}. Distance is '{closestDistance.ToString("0.00")}' units.");
            }
            else
            {
                MessageBox.Show("Could not match any elements");
            }
        }

        private void RemoveByArea(object sender, RoutedEventArgs e)
        {
            if (MainWindow.bsp == null) return;

            TripleInputDialogWindow wndDialog = new TripleInputDialogWindow("Remove brushes by area");
            wndDialog.FirstLabel.Text = "Bounding Box Start:";
            wndDialog.FirstInput.Text = "0 0 0";
            wndDialog.SecondLabel.Text = "Bounding Box End:";
            wndDialog.SecondInput.Text = "1 1 1";
            wndDialog.ThirdLabel.Text = "Brush Shader (leave empty for all):";
            wndDialog.ThirdInput.Text = "";

            wndDialog.ShowDialog();

            if (wndDialog.IsConfirmed == false)
            {
                return;
            }

            var (BBoxStart, BBoxEnd, Shader_String) = wndDialog.GetValue();

            if (string.IsNullOrEmpty(BBoxStart) || string.IsNullOrEmpty(BBoxEnd))
            {
                MessageBox.Show("Fill all fields before submiting");
                return;
            }

            if (string.IsNullOrEmpty(Shader_String))
            {
                Shader_String = "-1";
            }

            BrushVolume BoundingBox;
            float Shader;
            try
            {
                BoundingBox = new BrushVolume(BBoxStart, BBoxEnd);
                Shader = int.Parse(Shader_String);
            }
            catch
            {
                MessageBox.Show("Could not parse data");
                return;
            }

            int removedBrushesCount = 0;
            for (int i = 0; i < BrushData.Count; i++)
            {
                Vector3 brushCenter = BrushData[i].Center;

                if (BoundingBox.ContainsVector(brushCenter) == false)
                {
                    continue;
                }

                Brush brush = BrushData[i].GetBrush();

                if (Shader == brush.MaterialID || Shader == -1)
                {
                    int brushSidesOffset = BrushData[i].SidesOffset;

                    for (int sideIndex = 0; sideIndex < 6; sideIndex++)
                    {
                        BrushSides side = MainWindow.bsp.BrushSides[brushSidesOffset + sideIndex];
                        side.PlaneDistanceUnion = new byte[4] { 0, 0, 0, 0 };
                        MainWindow.bsp.BrushSides[brushSidesOffset + sideIndex] = side;
                    }

                    removedBrushesCount++;
                }
            }

            MessageBox.Show($"Removed {removedBrushesCount} brushes");
        }
    }
}
