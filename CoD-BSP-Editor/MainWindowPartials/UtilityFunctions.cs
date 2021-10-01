using CoD_BSP_Editor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
