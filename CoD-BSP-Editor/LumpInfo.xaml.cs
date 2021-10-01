using CoD_BSP_Editor.BSP;
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
    /// Logika interakcji dla klasy LumpInfo.xaml
    /// </summary>
    public partial class LumpInfo : Window
    {
        string MapName;
        Lump[] LumpsData;

        string[] lumpNames = new string[]
        {
            "Shaders",
            "Lightmaps",
            "Planes",
            "Brush Sides",
            "Brushes",
            "unknown",
            "Triangle Soups",
            "Draw Verts",
            "Draw Indexes",
            "Cull Groups",
            "Cull Group Indexes",
            "Portal Verts",
            "Occluders",
            "Occluder Planes",
            "Occluder Edges",
            "Occluder Indexes",
            "AABB Trees",
            "Cells",
            "Portals",
            "Light Indexes",
            "Nodes",
            "Leafs",
            "Leaf Brushes",
            "Leaf Surfaces",
            "Patch Collision",
            "Collision Verts",
            "Collision Indexes",
            "Models",
            "Visibility",
            "Entities",
            "Lights",
            "unknown",
            "Fogs",
        };

        int[] lumpSize = new int[]
        {
            72,
            786432,
            16,
            8,
            4,
            -2,
            16,
            44,
            2,
            32,
            4,
            12,
            20,
            4,
            4,
            2,
            12,
            52,
            16,
            2,
            36,
            36,
            4,
            4,
            16,
            12,
            2,
            48,
            -2,
            -1,
            72,
            -2,
            -2,
        };

        public LumpInfo(string mapName, Lump[] data)
        {
            MapName = mapName;
            LumpsData = data;

            InitializeComponent();
            this.Title = $"{this.MapName} - Lump Viewer";

            CreateLumpsInfoView();
        }

        private void CreateLumpsInfoView()
        {
            int index = 0;
            foreach (Lump lump in LumpsData)
            {
                string entryCount = "";
                switch (lumpSize[index])
                {
                    case -1:
                        entryCount = "string"; break;
                    case -2:
                        entryCount = "unknown"; break;
                    default: entryCount = $"{lump.Length / lumpSize[index]}"; break;
                }

                StackPanel dataContainer = new StackPanel()
                { Orientation = Orientation.Horizontal, Margin = new Thickness(2) };
                dataContainer.MouseDown += ClickChangeBackground;

                LumpsPanel.Children.Add(dataContainer);

                TextBlock lumpID = new TextBlock()
                { Text = $"[{index}]", FontSize = 20, Width = 40 };
                lumpID.MouseDown += CopyLumpHeaderStart;

                TextBlock lumpName = new TextBlock()
                { Text = lumpNames[index], FontSize = 20, Width = 200 };
                lumpName.MouseDown += CopyText;

                TextBlock lumpOffset = new TextBlock()
                { Text = $"{lump.Offset}", FontSize = 20, Width = 150 };
                lumpOffset.MouseDown += CopyText;

                TextBlock lumpLength = new TextBlock()
                { Text = $"{lump.Length}", FontSize = 20, Width = 150 };
                lumpLength.MouseDown += CopyText;

                TextBlock lumpEnd = new TextBlock()
                { Text = $"{lump.Offset + lump.Length}", FontSize = 20, Width = 150 };
                lumpEnd.MouseDown += CopyText;

                TextBlock lumpEntries = new TextBlock()
                { Text = entryCount, FontSize = 20, Width = 100 };
                lumpEntries.MouseDown += CopyText;

                dataContainer.Children.Add(lumpID);
                dataContainer.Children.Add(lumpName);
                dataContainer.Children.Add(lumpOffset);
                dataContainer.Children.Add(lumpLength);
                dataContainer.Children.Add(lumpEnd);
                dataContainer.Children.Add(lumpEntries);

                index++;
            }
        }

        private void ClickChangeBackground(object sender, MouseButtonEventArgs e)
        {
            foreach (StackPanel item in LumpsPanel.Children)
            {
                item.Background = Brushes.White;
            }

            StackPanel panel = (StackPanel)sender;
            panel.Background = Brushes.LightGray;
        }

        private void CopyText(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            Clipboard.SetText(textBlock.Text);
        }
        
        private void CopyLumpHeaderStart(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            int lumpIndex = int.Parse(textBlock.Text.Trim('[',']'));

            Clipboard.SetText( (8 + (lumpIndex * 8)).ToString() );
        }
    }
}
