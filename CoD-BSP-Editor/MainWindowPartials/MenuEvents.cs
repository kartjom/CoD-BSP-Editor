using CoD_BSP_Editor.BSP;
using CoD_BSP_Editor.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "BSP files (*.bsp,*.pk3)|*.bsp;*.pk3";
            saveFileDialog.InitialDirectory = bsp.FileDirectory;
            saveFileDialog.FileName = bsp.FileName;

            string SaveDirectory, FileName, FileExtension;
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                FileName = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                FileExtension = Path.GetExtension(saveFileDialog.FileName);
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

            if (FileExtension == ".pk3")
            {
                string Pk3FolderDirectory = Path.Combine(SaveDirectory, FileName + "_temp");

                Directory.CreateDirectory(Pk3FolderDirectory);
                Directory.CreateDirectory(Pk3FolderDirectory + "/mp");
                Directory.CreateDirectory(Pk3FolderDirectory + "/maps/mp");

                string arenaContent = d3dbsp.CreateArenaFile(FileName);
                string gscContent = d3dbsp.CreateGscFile(FileName);

                File.WriteAllBytes(Pk3FolderDirectory + $"/maps/mp/{FileName}.bsp", bspContent);
                File.WriteAllText(Pk3FolderDirectory + $"/mp/{FileName}.arena", arenaContent);
                File.WriteAllText(Pk3FolderDirectory + $"/maps/mp/{FileName}.gsc", gscContent);

                string pk3FilePath = Path.Combine(SaveDirectory, FileName + ".pk3");
                if (File.Exists(pk3FilePath))
                {
                    File.Delete(pk3FilePath);
                }

                ZipFile.CreateFromDirectory(Pk3FolderDirectory, pk3FilePath);

                Directory.Delete(Pk3FolderDirectory, true);
            }
            else
            {
                string FullFilePath = Path.Combine(SaveDirectory, FileName + FileExtension);
                File.WriteAllBytes(FullFilePath, bspContent);
            }

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

        private void ReplaceKeyValues(object sender, RoutedEventArgs e)
        {
            if (bsp == null || bsp.Lumps == null) return;

            ReplaceKeyValuePairsWindow wnd = new ReplaceKeyValuePairsWindow();
            wnd.ShowDialog();

            if (wnd.IsConfirmed == false) return;

            var (Classname, WantedStr, ReplaceStr) = wnd.GetValues();

            if (string.IsNullOrEmpty(Classname) || string.IsNullOrEmpty(WantedStr) || string.IsNullOrEmpty(ReplaceStr))
            {
                MessageBox.Show("Fill all fields before submiting");
                return;
            }

            KeyValuePair<string, string> WantedKeyValue;
            KeyValuePair<string, string> ReplaceKeyValue;

            try
            {
                int spaceIndex = WantedStr.IndexOf(' ');
                string wantedKey = WantedStr.Substring(0, spaceIndex).Trim();
                string wantedValue = WantedStr.Substring(spaceIndex).Trim();
                WantedKeyValue = new(wantedKey, wantedValue);

                spaceIndex = ReplaceStr.IndexOf(' ');
                string replaceKey = ReplaceStr.Substring(0, spaceIndex).Trim();
                string replaceValue = ReplaceStr.Substring(spaceIndex).Trim();
                ReplaceKeyValue = new(replaceKey, replaceValue);
            }
            catch
            {
                MessageBox.Show("Couldn't parse given data");
                return;
            }

            int replaced = 0;
            for (int i = 0; i < EntityBoxList.Items.Count; i++)
            {
                Entity ent = EntityBoxList.Items[i] as Entity;

                if (ent.Classname == Classname || Classname == "*")
                {
                    if (ent.HasKey(WantedKeyValue.Key) == false) continue;

                    int index = 0;
                    foreach (var KeyValue in ent.KeyValues)
                    {
                        if (KeyValue.Key == WantedKeyValue.Key && KeyValue.Value == WantedKeyValue.Value)
                        {
                            ent.KeyValues[index] = ReplaceKeyValue;
                            replaced++;
                            break;
                        }

                        index++;
                    }
                }
            }

            CreateEditView();
            MessageBox.Show($"Replaced {replaced} occurences");
        }
        
        private void RemoveByOrigin(object sender, RoutedEventArgs e)
        {
            if (bsp == null || bsp.Lumps == null) return;

            InputDialogWindow input = new InputDialogWindow("Remove by origin", "Syntax: operator x y z");
            input.ShowDialog();

            string inputString = input.GetValue();
            if (inputString == null) return;

            char charOperator;
            int x, y, z;

            try
            {
                string[] tokens = inputString.Split(' ');

                x = tokens[1] != "*" ? int.Parse(tokens[1]) : int.MaxValue;
                y = tokens[2] != "*" ? int.Parse(tokens[2]) : int.MaxValue;
                z = tokens[3] != "*" ? int.Parse(tokens[3]) : int.MaxValue;

                charOperator = tokens[0][0];

                if (charOperator != '<' && charOperator != '>') throw new Exception();
                if (x == int.MaxValue && y == int.MaxValue && z == int.MaxValue) throw new Exception();
            }
            catch
            {
                MessageBox.Show("Invalid syntax");
                return;
            }

            int removed = 0;
            for (int i = 0; i < EntityBoxList.Items.Count; i++)
            {
                Entity ent = EntityBoxList.Items[i] as Entity;

                if (ent.HasKey("origin") == false) continue;

                string[] tokens = ent.GetValue("origin").Split(' ');
                int _x = int.Parse(tokens[0]), _y = int.Parse(tokens[1]), _z = int.Parse(tokens[2]);
               
                if (charOperator == '>')
                {
                    bool xMarked = (x == int.MaxValue || _x > x);
                    bool yMarked = (y == int.MaxValue || _y > y);
                    bool zMarked = (z == int.MaxValue || _z > z);

                    if (xMarked && yMarked && zMarked)
                    {
                        EntityBoxList.Items.RemoveAt(i--);
                        removed++;

                        continue;
                    }
                }
                else if (charOperator == '<')
                {
                    bool xMarked = (x == int.MaxValue || _x < x);
                    bool yMarked = (y == int.MaxValue || _y < y);
                    bool zMarked = (z == int.MaxValue || _z < z);

                    if (xMarked && yMarked && zMarked)
                    {
                        EntityBoxList.Items.RemoveAt(i--);
                        removed++;

                        continue;
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
