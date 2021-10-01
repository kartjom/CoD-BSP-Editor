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
        private void OnInputChange(object sender, TextChangedEventArgs e)
        {
            if (bsp == null) return;

            StackPanel parent = (StackPanel)(sender as TextBox).Parent;
            int entityIndex = EntityBoxList.SelectedIndex;
            int keyValueIndex = (int)parent.Tag;

            string keyInput = (parent.Children[1] as TextBox).Text;
            string valueInput = (parent.Children[2] as TextBox).Text;

            KeyValuePair<string, string> updatedKeyValues = new(keyInput, valueInput);

            Entity EntityToEdit = (Entity)EntityBoxList.Items[entityIndex];
            EntityToEdit.KeyValues[keyValueIndex] = updatedKeyValues;
        }

        private void AddNewField(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;
            if (EntityBoxList.SelectedIndex == -1) return;

            Entity selectedEntity = (Entity)EntityBoxList.SelectedItem;
            selectedEntity.KeyValues.Add(new KeyValuePair<string, string>());

            CreateKeyValueField("", "", selectedEntity.KeyValues.Count - 1);
        }

        private void RemoveField(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            if (EntityBoxList.SelectedIndex == -1) return;

            Entity entity = (Entity)EntityBoxList.SelectedItem;

            Button btn = (Button)sender;
            StackPanel btnContainer = (StackPanel)btn.Parent;
            int keyValueIndex = (int)btnContainer.Tag;

            entity.KeyValues.RemoveAt(keyValueIndex);
            KeyValueFields.Children.Remove(btnContainer);

            // Refresh edit view because keyvalue indexes change after removal
            CreateEditView();
        }
    }
}
