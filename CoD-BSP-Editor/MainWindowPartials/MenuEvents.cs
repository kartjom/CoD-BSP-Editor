﻿using CoD_BSP_Editor.BSP;
using CoD_BSP_Editor.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CoD_BSP_Editor
{
    public partial class MainWindow : Window
    {
        public string LastFindByClassnameString { get; set; }
        public string LastFindByKeyValueString { get; set; }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "BSP files (*.bsp,*.d3dbsp)|*.bsp;*.d3dbsp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                if (bsp != null)
                {
                    MessageBoxResult result = MessageBox.Show("Any unsaved progress will be lost. Proceed?", "Open file", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.No) return;
                }

                bsp = new d3dbsp(openFileDialog.FileName);

                string fileName = Path.GetFileName(openFileDialog.FileName);
                this.Title = $"Call of Duty - BSP Editor ({fileName})";

                InitializeWorkingEnvironment(bsp.Entities);
            }
        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            string SavePath = "";

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "BSP files (*.bsp)|*.bsp";
            saveFileDialog.InitialDirectory = bsp.FileDirectory;
            saveFileDialog.FileName = bsp.FileName;

            if (saveFileDialog.ShowDialog() == true)
            {
                SavePath = saveFileDialog.FileName;
            }
            else
            {
                return;
            }

            List<Entity> entityList = new List<Entity>();
            foreach (Entity item in EntityBoxList.Items)
            {
                entityList.Add(item);
            }

            bsp.UpdateEntities(entityList);
            byte[] bspContent = bsp.CreateBSP();

            File.WriteAllBytes(SavePath, bspContent);

            MessageBox.Show("Finished exporting");
        }

        private void AddByString(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            InputDialogWindow input = new InputDialogWindow("Add by string", "Enter entities string", LastFindByClassnameString, false);
            input.Height = 600;
            input.Width = 600;
            input.Container.Width = 500;
            input.ValueTextBox.Height = 400;
            input.ValueTextBox.Width = 500;
            input.ValueTextBox.AcceptsReturn = true;
            input.ShowDialog();

            string entitiesString = input.GetValue();
            if (entitiesString == null) return;

            try
            {
                List<Entity> ParsedEntities = Entity.ParseEntitiesData(entitiesString);

                foreach (Entity newEntity in ParsedEntities)
                {
                    EntityBoxList.Items.Add(newEntity);
                }

                MessageBox.Show($"Added {ParsedEntities.Count} new entities");
            }
            catch
            {
                MessageBox.Show("Could not parse string");
            }
        }

        private void AddBasicSpawns(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            List<string> defaultEntsList = new()
            {
                "{\n\"classname\" \"mp_deathmatch_spawn\"\n\"origin\" \"0 0 0\"\n}",
                "{\n\"classname\" \"mp_uo_spawn_allies\"\n\"model\" \"xmodel/airborne\"\n\"origin\" \"0 0 0\"\n}",
                "{\n\"classname\" \"mp_uo_spawn_axis\"\n\"model\" \"xmodel/wehrmacht_soldier\"\n\"origin\" \"0 0 0\"\n}",
                "{\n\"classname\" \"mp_teamdeathmatch_spawn\"\n\"origin\" \"0 0 0\"\n}",
                "{\n\"classname\" \"mp_teamdeathmatch_intermission\"\n\"origin\" \"0 0 0\"\n}",
                "{\n\"classname\" \"mp_ctf_intermission\"\n\"origin\" \"0 0 0\"\n}",
            };

            InputDialogWindow input = new InputDialogWindow("Add by string", "Enter entities string", LastFindByClassnameString, false);
            input.Height = 600;
            input.Width = 600;
            input.Container.Width = 500;
            input.ValueTextBox.Height = 400;
            input.ValueTextBox.Width = 500;
            input.ValueTextBox.AcceptsReturn = true;
            input.ValueTextBox.Text = string.Join('\n', defaultEntsList);
            input.ShowDialog();

            string entitiesString = input.GetValue();
            if (entitiesString == null) return;

            try
            {
                List<Entity> ParsedEntities = Entity.ParseEntitiesData(entitiesString);

                foreach (Entity newEntity in ParsedEntities)
                {
                    EntityBoxList.Items.Add(newEntity);
                }

                MessageBox.Show($"Added {ParsedEntities.Count} new entities");
            }
            catch
            {
                MessageBox.Show("Could not parse string");
            }
        }

        private void FindByClassname(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            InputDialogWindow input = new InputDialogWindow("Find by classname", "Enter classname", LastFindByClassnameString);
            input.ShowDialog();

            string classname = input.GetValue();
            if (classname == null) return;

            LastFindByClassnameString = classname;

            bool containsSearch = classname.EndsWith("*");
            classname = classname.TrimEnd('*');

            for (int i = EntityBoxList.SelectedIndex + 1; i < EntityBoxList.Items.Count; i++)
            {
                if (EntityBoxList.Items[i] is Entity)
                {
                    Entity ent = EntityBoxList.Items[i] as Entity;

                    if (containsSearch)
                    {
                        if (ent.Classname.StartsWith(classname))
                        {
                            EntityBoxList.SelectedIndex = EntityBoxList.Items.IndexOf(ent);
                            EntityBoxList.ScrollIntoView(ent);

                            return;
                        }
                    }
                    else
                    {
                        if (ent.Classname == classname)
                        {
                            EntityBoxList.SelectedIndex = EntityBoxList.Items.IndexOf(ent);
                            EntityBoxList.ScrollIntoView(ent);

                            return;
                        }
                    }
                }
            }

            MessageBox.Show($"Could not find entity of type '{classname}'");
        }

        private void FindByKeyValue(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            InputDialogWindow input = new InputDialogWindow("Find by key value pair", "Enter key value separated by space", LastFindByKeyValueString);
            input.ShowDialog();

            string keyValueString = input.GetValue();
            if (keyValueString == null) return;

            string[] KeyValue = keyValueString.Split(" ");
            if (KeyValue.Length != 2)
            {
                MessageBox.Show("Invalid format for key value pair");
                return;
            }

            LastFindByKeyValueString = keyValueString;

            string Key = KeyValue[0];
            string Value = KeyValue[1];

            for (int i = EntityBoxList.SelectedIndex + 1; i < EntityBoxList.Items.Count; i++)
            {
                if (EntityBoxList.Items[i] is Entity)
                {
                    Entity ent = EntityBoxList.Items[i] as Entity;

                    if (ent.HasKey(Key) && ent.GetValue(Key) == Value)
                    {
                        EntityBoxList.SelectedIndex = EntityBoxList.Items.IndexOf(ent);
                        EntityBoxList.ScrollIntoView(ent);

                        return;
                    }
                }
            }

            MessageBox.Show("Could not find entity of given key value pair");
        }

        private void CreateNewEntity(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            InputDialogWindow input = new InputDialogWindow("Create new entity", "Enter classname");
            input.ShowDialog();

            string classname = input.GetValue();
            if (classname == null) return;

            Entity newEntity = new Entity(classname);
            AddEntity(newEntity);
        }

        private void RemoveSingleplayerEntities(object sender, RoutedEventArgs e)
        {
            if (bsp == null || bsp.Lumps == null) return;

            int removed = 0;
            for (int i = 0; i < EntityBoxList.Items.Count; i++)
            {
                if (EntityBoxList.Items[i] is Entity)
                {
                    Entity ent = EntityBoxList.Items[i] as Entity;

                    bool isActor = ent.Classname.StartsWith("actor_");
                    bool isNode = ent.Classname.StartsWith("node_");

                    if (isActor || isNode)
                    {
                        EntityBoxList.Items.RemoveAt(i--);
                        removed++;
                    }
                }
            }

            MessageBox.Show($"Removed {removed} entities");
        }
        
        private void ShowLumpInfo(object sender, RoutedEventArgs e)
        {
            if (bsp == null || bsp.Lumps == null) return;

            string mapName = Path.GetFileName(bsp.FilePath);

            LumpInfo lumpInfoWindow = new LumpInfo(mapName, bsp.Lumps);
            lumpInfoWindow.Owner = this;
            lumpInfoWindow.Show();
        }
    }
}
