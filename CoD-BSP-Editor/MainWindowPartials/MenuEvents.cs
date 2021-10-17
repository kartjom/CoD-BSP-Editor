using CoD_BSP_Editor.BSP;
using CoD_BSP_Editor.Data;
using CoD_BSP_Editor.GametypeTools;
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

                this.CloseAllWindows();
                this.InitializeWorkingEnvironment(bsp.Entities);
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

            /* Update lumps */
            bsp.UpdateLumps();
            bsp.UpdateEntities(entityList);

            /* Create new BSP */
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

            InputDialogWindow input = new("Add by string", "Enter entities string");
            input.Height = 600;
            input.Width = 600;
            input.Container.Width = 500;
            input.FirstInput.Height = 400;
            input.FirstInput.Width = 500;
            input.ConfirmWithEnter = false;
            input.FirstInput.AcceptsReturn = true;
            input.ShowDialog();

            if (input.IsConfirmed == false) return;
            string entitiesString = input.GetValue();

            if (string.IsNullOrEmpty(entitiesString))
            {
                MessageBox.Show("Value cannot be empty"); return;
            }

            try
            {
                List<Entity> ParsedEntities = Entity.ParseEntitiesData(entitiesString);

                foreach (Entity newEntity in ParsedEntities)
                {
                    EntityBoxList.Items.Add(newEntity);
                }
                EntityBoxList.Items.Refresh();

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

            InputDialogWindow input = new("Add by string", "Enter entities string");
            input.Height = 600;
            input.Width = 600;
            input.Container.Width = 500;
            input.FirstInput.Height = 400;
            input.FirstInput.Width = 500;
            input.ConfirmWithEnter = false;
            input.FirstInput.AcceptsReturn = true;
            input.FirstInput.Text = string.Join('\n', defaultEntsList);
            input.ShowDialog();

            if (input.IsConfirmed == false) return;
            string entitiesString = input.GetValue();

            if (string.IsNullOrEmpty(entitiesString))
            {
                MessageBox.Show("Value cannot be empty"); return;
            }

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

            if (input.IsConfirmed == false) return;

            string classname = input.GetValue();
            if (string.IsNullOrEmpty(classname))
            {
                MessageBox.Show("Classname cannot be empty"); return;
            }

            LastFindByClassnameString = classname;

            /* Deciding which method to use */
            bool exactSearch = classname.StartsWith('*') == false && classname.EndsWith('*') == false;
            bool startsWithSearch = classname.StartsWith('*') == false && classname.EndsWith('*') == true;
            bool endsWithSearch = classname.StartsWith('*') == true && classname.EndsWith('*') == false;
            bool containsSearch = classname.StartsWith('*') == true && classname.EndsWith('*') == true;

            classname = classname.Trim('*');

            for (int i = EntityBoxList.SelectedIndex + 1; i < EntityBoxList.Items.Count; i++)
            {
                if (EntityBoxList.Items[i] is Entity)
                {
                    Entity ent = EntityBoxList.Items[i] as Entity;
                    bool IsWanted = false;

                    if (exactSearch && ent.Classname == classname)
                    {
                        IsWanted = true;
                    }
                    else if (startsWithSearch && ent.Classname.StartsWith(classname))
                    {
                        IsWanted = true;
                    }
                    else if (endsWithSearch && ent.Classname.EndsWith(classname))
                    {
                        IsWanted = true;
                    }
                    else if (containsSearch && ent.Classname.Contains(classname))
                    {
                        IsWanted = true;
                    }

                    if (IsWanted)
                    {
                        EntityBoxList.SelectedIndex = EntityBoxList.Items.IndexOf(ent);
                        EntityBoxList.ScrollIntoView(ent);

                        return;
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

            if (input.IsConfirmed == false) return;

            string keyValueString = input.GetValue();
            if (keyValueString == null)
            {
                MessageBox.Show("Value cannot be empty"); return;
            }

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

            if (input.IsConfirmed == false) return;

            string classname = input.GetValue();
            if (string.IsNullOrEmpty(classname))
            {
                MessageBox.Show("Classname cannot be empty"); return;
            }

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

        private void ImportCollmap(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "collmap file (*.collmap)|*.collmap|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);

                byte[] collmapData = File.ReadAllBytes(openFileDialog.FileName);
                CollmapData collmap = CollmapData.ReadFromByteArray(collmapData);

                /* Editing data to match the BSP's content */
                int MaterialID = bsp.Shaders.Count;

                collmap.Model.BrushesOffset = (uint)bsp.Brushes.Count;
                collmap.Brush.MaterialID = (ushort)MaterialID;

                for (int i = 0; i < 6; i++)
                {
                    collmap.BrushSides[i].MaterialID = (uint)MaterialID;
                }

                Entity collmapEntity = new Entity("script_vehicle_collmap")
                {
                    KeyValues = new()
                    {
                        new("model", $"*{bsp.Models.Count}"),
                        new("targetname", $"xmodel/{fileName}"),
                    }
                };

                bsp.Shaders.Add(collmap.Shader);
                bsp.BrushSides.AddRange(collmap.BrushSides);
                bsp.Brushes.Add(collmap.Brush);
                bsp.Models.Add(collmap.Model);

                // Add new entity to the list
                EntityBoxList.Items.Add(collmapEntity);
                EntityBoxList.Items.Refresh();

                MessageBox.Show("Finished importing collmap");
            }
        }

        private void ExportCollmap(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "collmap file (*.collmap)|*.collmap|All files (*.*)|*.*";
            saveFileDialog.InitialDirectory = bsp.FileDirectory;
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(bsp.FileName) + ".collmap";

            if (saveFileDialog.ShowDialog() == true)
            {
                byte[] collmapData = bsp.ExtractCollmapData();
                File.WriteAllBytes(saveFileDialog.FileName, collmapData);

                MessageBox.Show("Finished exporting collmap");
            }
        }

        private void AddCTF(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            CtfInputWindow wnd = new CtfInputWindow();
            wnd.ShowDialog();

            if (wnd.IsConfirmed == false) return;

            var (ModelIndexStr, AlliesFlagPos, AxisFlagPos) = wnd.GetValues();

            if (string.IsNullOrEmpty(AlliesFlagPos) || string.IsNullOrEmpty(AxisFlagPos))
            {
                MessageBox.Show("Fill all required fields before submiting");
                return;
            }

            ModelIndexStr = '*' + ModelIndexStr.Trim('*');

            string alliedEntities = CtfTools.GetAlliesFlagEntities(ModelIndexStr, AlliesFlagPos);
            string axisEntities = CtfTools.GetAxisFlagEntities(ModelIndexStr, AxisFlagPos);

            string allEntities = alliedEntities + '\n' + axisEntities;

            List<Entity> ParsedEntities = Entity.ParseEntitiesData(allEntities);

            foreach (Entity newEntity in ParsedEntities)
            {
                EntityBoxList.Items.Add(newEntity);
            }
            EntityBoxList.Items.Refresh();

            MessageBox.Show("Gametype 'Capture The Flag' successfully added");
        }

        private void ReplaceKeyValues(object sender, RoutedEventArgs e)
        {
            if (bsp == null || bsp.Lumps == null) return;

            ReplaceKeyValuePairsWindow wnd = new ReplaceKeyValuePairsWindow();
            wnd.ShowDialog();

            if (wnd.IsConfirmed == false) return;

            var (Classname, WantedStr, NewClassname, ReplaceStr) = wnd.GetValues();

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
                            if (string.IsNullOrEmpty(NewClassname) == false)
                            {
                                ent.Classname = NewClassname;
                            }

                            ent.KeyValues[index] = ReplaceKeyValue;
                            replaced++;
                            break;
                        }

                        index++;
                    }
                }
            }

            EntityBoxList.Items.Refresh();
            CreateEditView();
            MessageBox.Show($"Replaced {replaced} occurences");
        }
        
        private void RemoveByOrigin(object sender, RoutedEventArgs e)
        {
            if (bsp == null || bsp.Lumps == null) return;

            InputDialogWindow input = new InputDialogWindow("Remove by origin", "Syntax: operator x y z");
            input.ShowDialog();

            if (input.IsConfirmed == false) return;

            string inputString = input.GetValue();
            if (string.IsNullOrEmpty(inputString))
            {
                MessageBox.Show("Value cannot be null"); return;
            }

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

        private void OpenMaterialsEditor(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            foreach (Window win in App.Current.Windows)
            {
                if (win is MaterialsEditor) return;
            }

            MaterialsEditor editor = new MaterialsEditor();
            editor.Owner = this;
            editor.Title = $"Materials editor ({bsp.FileName})";
            editor.Show();
        }

        private void ShowLumpInfo(object sender, RoutedEventArgs e)
        {
            if (bsp == null || bsp.Lumps == null) return;

            foreach (Window win in App.Current.Windows)
            {
                if (win is LumpInfo) return;
            }

            string mapName = Path.GetFileName(bsp.FilePath);

            LumpInfo lumpInfoWindow = new LumpInfo(mapName, bsp.Lumps);
            lumpInfoWindow.Owner = this;
            lumpInfoWindow.Show();
        }
    }
}
