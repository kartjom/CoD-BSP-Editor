using CoD_BSP_Editor.BSP;
using CoD_BSP_Editor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CoD_BSP_Editor
{
    public partial class MainWindow : Window
    {
        private void AddEntity(Entity item)
        {
            EntityBoxList.Items.Add(item);
            EntityBoxList.Items.Refresh();
        }

        private void AddEntityList(List<Entity> list)
        {
            foreach (Entity ent in list)
            {
                EntityBoxList.Items.Add(ent);
            }

            EntityBoxList.Items.Refresh();
        }

        public Entity FindEntityByKeyValue(string key, string value)
        {
            foreach (Entity ent in EntityBoxList.Items)
            {
                if (ent.HasKey(key) && ent.GetValue(key) == value)
                {
                    return ent;
                }
            }

            return null;
        }

        public void AddModel(ModelData modelData)
        {
            MainWindow.bsp.AddShaders(modelData.Shaders.ToArray());
            modelData.CorrectShaderIndexes();

            Brush[] brushes = modelData.GetBrushes();
            BrushSides[] brushSides = modelData.GetBrushSides(MainWindow.bsp.Planes.Count);
            Plane[] planes = modelData.GetPlanes();
            Model model = modelData.GetModel();

            MainWindow.bsp.Brushes.AddRange(brushes);
            MainWindow.bsp.BrushSides.AddRange(brushSides);
            MainWindow.bsp.Planes.AddRange(planes);
            MainWindow.bsp.Models.Add(model);

            this.AddEntityList(modelData.Entities);
        }

        private int RemoveByClassname(string classname)
        {
            int removed = 0;
            for (int i = 0; i < EntityBoxList.Items.Count; i++)
            {
                Entity ent = EntityBoxList.Items[i] as Entity;
                if (ent.Classname == classname)
                {
                    EntityBoxList.Items.RemoveAt(i--);
                    removed++;
                }
            }

            return removed;
        }
    
        public void OpenFile(string filePath)
        {
            this.Activate();

            if (bsp != null)
            {
                MessageBoxResult result = MessageBox.Show("Any unsaved progress will be lost. Proceed?", "Open file", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No) return;
            }

            bsp = new d3dbsp(filePath);

            string fileName = System.IO.Path.GetFileName(filePath);
            this.Title = this.GetTitle() + $" ({fileName})";

            this.CloseAllWindows();
            this.InitializeWorkingEnvironment(bsp.Entities);
        }
    }
}
