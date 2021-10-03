using CoD_BSP_Editor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CoD_BSP_Editor
{
    public partial class MainWindow : Window
    {
        private void OnEntitySelect(object sender, SelectionChangedEventArgs e)
        {
            if (bsp == null) return;

            CreateEditView();
        }

        private void DeleteSelectedEntity(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;
            if (EntityBoxList.SelectedIndex == -1) return;

            MessageBoxResult result = MessageBox.Show("Confirm deleting selected entity", "Remove entity", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) return;

            EntityBoxList.Items.RemoveAt(EntityBoxList.SelectedIndex);
        }

        private void DeleteEntitiesByClassname(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            Entity SelectedEntity = EntityBoxList.SelectedItem as Entity;
            string Classname = SelectedEntity.Classname;

            int removedCount = 0;
            if (SelectedEntity != null)
            {
                MessageBoxResult result = MessageBox.Show("Confirm deleting selected entities", "Remove entity", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No) return;

                removedCount = RemoveByClassname(Classname);
            }

            MessageBox.Show($"Removed {removedCount} entities");
        }

        private void RenameSelectedEntity(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;
            if (EntityBoxList.SelectedIndex == -1) return;

            Entity ent = EntityBoxList.Items[EntityBoxList.SelectedIndex] as Entity;

            InputDialogWindow input = new("Rename entity", "Rename entity:", ent.Classname);
            input.ShowDialog();

            if (input.IsConfirmed == false) return;

            string newClassname = input.GetValue();
            if (newClassname == ent.Classname) return;

            if (string.IsNullOrEmpty(newClassname))
            {
                MessageBox.Show("Classname cannot be empty"); return;
            }

            ent.Classname = newClassname;
            EntityBoxList.Items.Refresh();

            UpdateCurrentEntityText();
        }

        private void RenameEntitiesByClassname(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;
            if (EntityBoxList.SelectedIndex == -1) return;

            Entity selected = EntityBoxList.Items[EntityBoxList.SelectedIndex] as Entity;
            string classname = selected.Classname;

            InputDialogWindow input = new("Rename entities", $"Rename all of type '{classname}':", classname);
            input.ShowDialog();

            if (input.IsConfirmed == false) return;

            string newClassname = input.GetValue();
            if (newClassname == classname) return;

            if (string.IsNullOrEmpty(newClassname))
            {
                MessageBox.Show("Classname cannot be empty"); return;
            }

            int renamed = 0;
            foreach (object obj in EntityBoxList.Items)
            {
                if (obj is Entity)
                {
                    Entity ent = obj as Entity;
                    if (ent.Classname == classname)
                    {
                        ent.Classname = newClassname;
                        renamed++;
                    }
                }
            }

            EntityBoxList.Items.Refresh();
            UpdateCurrentEntityText();

            MessageBox.Show($"Renamed {renamed} entities");
        }

        private void ShowAsText(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;
            if (EntityBoxList.SelectedIndex == -1) return;

            Entity selected = EntityBoxList.Items[EntityBoxList.SelectedIndex] as Entity;

            EntityTextWindow wnd = new EntityTextWindow();
            wnd.EntityText.Text = selected.WriteEntity();
            wnd.Show();
        }

        private void DuplicateSelectedEntity(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;
            if (EntityBoxList.SelectedIndex == -1) return;

            Entity original = EntityBoxList.Items[EntityBoxList.SelectedIndex] as Entity;

            InputDialogWindow input = new("Duplicate entity", "Name duplicated entity:", original.Classname);
            input.ShowDialog();

            if (input.IsConfirmed == false) return;

            string newClassname = input.GetValue();
            if (string.IsNullOrEmpty(newClassname))
            {
                MessageBox.Show("Classname cannot be empty"); return;
            }

            Entity copy = original.DeepCopy();
            copy.Classname = newClassname;

            AddEntity(copy);
        }
    }
}
