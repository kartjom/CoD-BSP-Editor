using CoD_BSP_Editor.BSP;
using CoD_BSP_Editor.Data;
using CoD_BSP_Editor.GametypeTools;
using CoD_BSP_Editor.Libs;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
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
        public string LastFindClosestString { get; set; }

        public string LastCreateBrushShaderString { get; set; }
        public bool LastCreateBrushIsStatic { get; set; }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "BSP files (*.bsp,*.d3dbsp)|*.bsp;*.d3dbsp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                this.OpenFile(openFileDialog.FileName);
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
                    try
                    {
                        // File may be in use, app will crash and all progress will be lost
                        File.Delete(pk3FilePath);
                    }
                    catch (Exception exception)
                    {
                        // Remove temporary remains
                        Directory.Delete(Pk3FolderDirectory, true);
                        
                        MessageBox.Show(exception.Message);
                        return;
                    }
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
                "{\n\"classname\" \"mp_deathmatch_intermission\"\n\"origin\" \"0 0 0\"\n}",
                "{\n\"classname\" \"mp_uo_spawn_allies\"\n\"origin\" \"0 0 0\"\n}",
                "{\n\"classname\" \"mp_uo_spawn_axis\"\n\"origin\" \"0 0 0\"\n}",
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

            classname = classname.ToLower().Trim();
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
                    string entClassname = ent.Classname.ToLower();

                    bool IsWanted = false;

                    if (exactSearch && entClassname == classname)
                    {
                        IsWanted = true;
                    }
                    else if (startsWithSearch && entClassname.StartsWith(classname))
                    {
                        IsWanted = true;
                    }
                    else if (endsWithSearch && entClassname.EndsWith(classname))
                    {
                        IsWanted = true;
                    }
                    else if (containsSearch && entClassname.Contains(classname))
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

            keyValueString = keyValueString.ToLower().Trim();
            LastFindByKeyValueString = keyValueString;

            string Key, Value;

            try
            {
                int spaceIndex = keyValueString.IndexOf(' ');

                if (spaceIndex == -1)
                {
                    throw new Exception();
                }

                Key = keyValueString.Substring(0, spaceIndex).Trim();
                Value = keyValueString.Substring(spaceIndex).Trim();
            }
            catch
            {
                MessageBox.Show("Invalid format for key value pair");
                return;
            }

            for (int i = EntityBoxList.SelectedIndex + 1; i < EntityBoxList.Items.Count; i++)
            {
                if (EntityBoxList.Items[i] is Entity)
                {
                    Entity ent = EntityBoxList.Items[i] as Entity;

                    if (ent.HasKey(Key) && ent.GetValue(Key).ToLower() == Value)
                    {
                        EntityBoxList.SelectedIndex = EntityBoxList.Items.IndexOf(ent);
                        EntityBoxList.ScrollIntoView(ent);

                        return;
                    }
                }
            }

            MessageBox.Show("Could not find entity of given key value pair");
        }

        private void FindClosest(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            InputDialogWindow wndDialog = new InputDialogWindow("Find closest entity");
            wndDialog.FirstLabel.Text = "Enter origin:";
            wndDialog.FirstInput.Text = this.LastFindClosestString;

            wndDialog.ShowDialog();

            if (wndDialog.IsConfirmed == false)
            {
                return;
            }

            string SeekOriginString = wndDialog.GetValue();

            if (string.IsNullOrEmpty(SeekOriginString))
            {
                MessageBox.Show("Enter valid origin");
                return;
            }

            Vector3 SeekOrigin;
            try
            {
                SeekOrigin = Vec3.FromString(SeekOriginString);
            }
            catch
            {
                MessageBox.Show("Could not parse origin data");
                return;
            }

            float closestDistance = int.MaxValue;
            int closestDistanceIndex = -1;

            for (int i = 0; i < EntityBoxList.Items.Count; i++)
            {
                Entity ent = EntityBoxList.Items[i] as Entity;

                if (ent.HasKey("origin") == false)
                {
                    continue;
                }

                string originValue = ent.GetValue("origin");
                Vector3 EntityOrigin = Vec3.FromString(originValue);

                float newDistance = Vector3.Distance(SeekOrigin, EntityOrigin);
                if (newDistance < closestDistance)
                {
                    closestDistance = newDistance;
                    closestDistanceIndex = i;
                }
            }

            this.LastFindClosestString = SeekOriginString;

            if (closestDistanceIndex != -1)
            {
                Entity closestEntity = EntityBoxList.Items[closestDistanceIndex] as Entity;
                EntityBoxList.SelectedItem = closestEntity;
                EntityBoxList.ScrollIntoView(closestEntity);

                MessageBox.Show($"Closest Entity is at index {closestDistanceIndex}. Distance is '{closestDistance:0.00}' units.");
            }
            else
            {
                MessageBox.Show("Could not match any elements");
            }
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

        private void ImportModel(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "model file (*.model)|*.model|All files (*.*)|*.*";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    string modelJson = File.ReadAllText(fileName);
                    ModelData modelData = ModelData.Deserialize(modelJson);
                    modelData.CorrectModelIndexInEntities();

                    this.AddModel(modelData);
                }

                MessageBox.Show($"Finished importing {openFileDialog.FileNames.Length} model" + (openFileDialog.FileNames.Length > 1 ? "s" : ""));
            }
        }

        private void ExportModel(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            InputDialogWindow input = new("Export model", "Model index to export:", "");
            input.ShowDialog();

            if (input.IsConfirmed == false) return;

            string modelIndexStr = input.GetValue();

            if (string.IsNullOrEmpty(modelIndexStr))
            {
                MessageBox.Show("Fill model index field before submiting");
                return;
            }

            int modelIndex;
            if (int.TryParse(modelIndexStr, out modelIndex) == false)
            {
                MessageBox.Show("Could not parse index data");
                return;
            }

            if (modelIndex < 0 || modelIndex >= bsp.Models.Count)
            {
                MessageBox.Show("Model index out of bounds");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "model file (*.model)|*.model|All files (*.*)|*.*";
            saveFileDialog.InitialDirectory = bsp.FileDirectory;
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(bsp.FileName) + ".model";
            
            if (saveFileDialog.ShowDialog() == false) return;

            ModelData modelData = new ModelData();
            
            Model model = bsp.Models[modelIndex];
            HashSet<int> uniqueShaders = new();

            int brushStart = (int)model.BrushesOffset;
            int brushEnd = brushStart + (int)model.BrushesSize;

            int planeCount = 0;
            for (int i = brushStart; i < brushEnd; i++)
            {
                BrushSides[] brushSides = BrushSides.GetBrushSides(i);
                int shaderIndex = (ushort)bsp.Brushes[i].MaterialID;

                Plane[] planes = PlaneExt.GetBrushPlanes(i);

                for (int j = 0; j < planes.Length; j++)
                {
                    brushSides[6 + j].SetPlaneIndex((uint)planeCount++);
                }

                BrushVolume brushVolume = new BrushVolume(brushSides) { ShaderIndex = shaderIndex };
                brushVolume.Planes.AddRange(planes);

                uniqueShaders.Add(shaderIndex);
                modelData.Brushes.Add(brushVolume);
            }

            foreach (int shaderIndex in uniqueShaders)
            {
                modelData.Shaders.Add(bsp.Shaders[shaderIndex]);

                for (int i = 0; i < modelData.Brushes.Count; i++)
                {
                    if (modelData.Brushes[i].ShaderIndex == shaderIndex)
                    {
                        modelData.Brushes[i].ShaderIndex = modelData.Shaders.Count - 1;
                    }
                }
            }

            Entity modelEntity = this.FindEntityByKeyValue("model", $"*{modelIndex}");
            if (modelEntity is not null)
            {
                modelData.Entities.Add(modelEntity);

                if (modelEntity.HasKey("origin") && modelEntity.GetValue("origin") != "0 0 0")
                {
                    Vector3 originVec = Vec3.FromString(modelEntity.GetValue("origin"));

                    modelData.CenterBeforeRotation = originVec;
                    modelData.RotationEnabled = true;
                }
            }

            string json = modelData.Serialize();
            File.WriteAllText(saveFileDialog.FileName, json);

            MessageBox.Show("Finished exporting model");
        }

        private void CreateBrush(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            CreateBrushWindow input = new CreateBrushWindow();
            input.ShaderName.Text = LastCreateBrushShaderString ?? "textures/common/trigger";
            input.IsStatic.IsChecked = LastCreateBrushIsStatic;
            input.ShowDialog();

            if (input.IsConfirmed == false)
            {
                return;
            }

            var (BBoxMinStr, BBoxMaxStr, ShaderName, IsStatic) = input.GetValues();

            if (string.IsNullOrEmpty(BBoxMinStr) || string.IsNullOrEmpty(BBoxMaxStr) || string.IsNullOrEmpty(ShaderName))
            {
                MessageBox.Show("Fill all fields before submiting");
                return;
            }

            Vector3 BBoxMin, BBoxMax, BBoxCenter;
            try
            {
                LastCreateBrushShaderString = ShaderName;
                LastCreateBrushIsStatic = IsStatic;

                BBoxMin = Vec3.FromString(BBoxMinStr);
                BBoxMax = Vec3.FromString(BBoxMaxStr);
                BBoxCenter = (BBoxMin + BBoxMax) / 2;
            }
            catch
            {
                MessageBox.Show("Could not parse bounding box");
                return;
            }

            ModelData modelData = new ModelData();

            Shader brushShader = new Shader(ShaderName, 128, 671088641);
            BrushVolume brushVolume = new BrushVolume(BBoxMin, BBoxMax);

            modelData.Shaders.Add(brushShader);
            modelData.Brushes.Add(brushVolume);
            
            if (IsStatic == false)
            {
                modelData.EnableRotation();
            }

            modelData.Entities.Add(modelData.GetBrushEntity());

            this.AddModel(modelData);

            MessageBox.Show("Brush created successfully");
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

            if (string.IsNullOrEmpty(ModelIndexStr))
            {
                ModelData modelData = CtfTools.CreateModel();

                int modelIndex = bsp.Models.Count;

                Entity editor_info = new Entity("editor_ctf_info");
                editor_info.AddKeyValue("info_model", $"*{modelIndex}");

                modelData.Entities.Add(editor_info);

                // Import flag model (collisions)
                this.AddModel(modelData);

                ModelIndexStr = $"*{modelIndex}";
            }
            else
            {
                ModelIndexStr = '*' + ModelIndexStr.Trim('*');
            }

            string alliedEntities = CtfTools.GetAlliesFlagEntities(ModelIndexStr, AlliesFlagPos);
            string axisEntities = CtfTools.GetAxisFlagEntities(ModelIndexStr, AxisFlagPos);
            string allEntities = alliedEntities + '\n' + axisEntities;

            List<Entity> ParsedEntities = Entity.ParseEntitiesData(allEntities);

            this.AddEntityList(ParsedEntities);

            MessageBox.Show("Gametype 'Capture The Flag' successfully added");
        }

        private void AddSD(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            MessageBoxResult result = MessageBox.Show("'Search & Destroy' gametype will be imported. Proceed?", "Add Search & Destroy", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) return;

            string bombsiteEntitiesString = SdTools.GetBombsiteEntities();
            List<Entity> bombsiteEntities = Entity.ParseEntitiesData(bombsiteEntitiesString);

            foreach (Entity entity in bombsiteEntities)
            {
                EntityBoxList.Items.Add(entity);
            }
            EntityBoxList.Items.Refresh();

            MessageBox.Show("Gametype 'Search & Destroy' successfully added");
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
                string entClassname = ent.Classname.ToLower();

                if (entClassname == Classname || Classname == "*")
                {
                    if (ent.HasKey(WantedKeyValue.Key) == false) continue;

                    int index = 0;
                    foreach (var KeyValue in ent.KeyValues)
                    {
                        if (KeyValue.Key.ToLower() == WantedKeyValue.Key && KeyValue.Value.ToLower() == WantedKeyValue.Value)
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
        
        private void RemoveWithinBounds(object sender, RoutedEventArgs e)
        {
            if (bsp == null || bsp.Lumps == null) return;

            DoubleInputDialogWindow wndDialog = new DoubleInputDialogWindow("Remove within bounds");
            wndDialog.FirstLabel.Text = "Bounding Box Start:";
            wndDialog.FirstInput.Text = "0 0 0";
            wndDialog.SecondLabel.Text = "Bounding Box End:";
            wndDialog.SecondInput.Text = "1 1 1";
            wndDialog.ShowDialog();

            if (wndDialog.IsConfirmed == false) return;

            var (BBoxStart, BBoxEnd) = wndDialog.GetValue();

            if (string.IsNullOrEmpty(BBoxStart) || string.IsNullOrEmpty(BBoxEnd))
            {
                MessageBox.Show("Fill all fields before submiting"); return;
            }

            BrushVolume BoundingBox;
            try
            {
                BoundingBox = new BrushVolume(BBoxStart, BBoxEnd);
            }
            catch
            {
                MessageBox.Show("Could not parse data");
                return;
            }

            int removed = 0;
            for (int i = 0; i < EntityBoxList.Items.Count; i++)
            {
                Entity ent = EntityBoxList.Items[i] as Entity;

                if (ent.HasKey("origin") == false) continue;

                Vector3 entOrigin;
                try
                {
                    string originString = ent.GetValue("origin");
                    entOrigin = Vec3.FromString(originString);
                }
                catch
                {
                    continue;
                }

                if (BoundingBox.ContainsVector(entOrigin))
                {
                    EntityBoxList.Items.RemoveAt(i--);
                    removed++;
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

        private void OpenBrushEditor(object sender, RoutedEventArgs e)
        {
            if (bsp == null) return;

            foreach (Window win in App.Current.Windows)
            {
                if (win is BrushEditor) return;
            }

            BrushEditor editor = new BrushEditor();
            editor.Owner = this;
            editor.Title = $"Brush editor ({bsp.FileName})";
            editor.Show();
        }
        
        private void MoveVertex(object sender, RoutedEventArgs e)
        {
            if (bsp == null || bsp.Lumps == null) return;

            DoubleInputDialogWindow wndDialog = new DoubleInputDialogWindow("Move vertex");
            wndDialog.FirstLabel.Text = "Vertex origin:";
            wndDialog.FirstInput.Text = "0 0 0";
            wndDialog.SecondLabel.Text = "Move to:";
            wndDialog.SecondInput.Text = "1 1 1";
            wndDialog.ShowDialog();

            if (wndDialog.IsConfirmed == false) return;

            var (origin_s, newOrigin_s) = wndDialog.GetValue();

            if (string.IsNullOrEmpty(origin_s) || string.IsNullOrEmpty(newOrigin_s))
            {
                MessageBox.Show("Fill all fields before submiting"); return;
            }

            Vector3 origin, newOrigin;
            try
            {
                origin = Vec3.FromString(origin_s);
                newOrigin = Vec3.FromString(newOrigin_s);

                Vertex vertex = new Vertex(origin);

                if (vertex.IsValid() == false)
                {
                    MessageBox.Show($"Could not find vertex near {origin}");
                    return;
                }
                vertex.MoveTo(newOrigin);

                MessageBox.Show($"Moved {vertex.VisualVertices.Count} visual and {vertex.CollisionVertices.Count} collision verts");
            }
            catch
            {
                MessageBox.Show("Could not parse data");
                return;
            }
        }

        private void MoveVerticesWithinBounds(object sender, RoutedEventArgs e)
        {
            if (bsp == null || bsp.Lumps == null) return;

            TripleInputDialogWindow wndDialog = new TripleInputDialogWindow("Move vertices within bounds");
            wndDialog.FirstLabel.Text = "Bounds min:";
            wndDialog.FirstInput.Text = "0 0 0";
            wndDialog.SecondLabel.Text = "Bounds max:";
            wndDialog.SecondInput.Text = "1 1 1";
            wndDialog.ThirdLabel.Text = "Offset:";
            wndDialog.ThirdInput.Text = "2 2 2";
            wndDialog.ShowDialog();

            if (wndDialog.IsConfirmed == false) return;

            var (min_s, max_s, offset_s) = wndDialog.GetValue();

            if (string.IsNullOrEmpty(min_s) || string.IsNullOrEmpty(max_s) || string.IsNullOrEmpty(offset_s))
            {
                MessageBox.Show("Fill all fields before submiting"); return;
            }

            Vector3 min, max, offset;
            try
            {
                min = Vec3.FromString(min_s);
                max = Vec3.FromString(max_s);
                offset = Vec3.FromString(offset_s);

                List<Vertex> vertices = Vertex.FindWithinBounds(min, max);

                int visualVertices = 0;
                int collisionVertices = 0;
                foreach (Vertex vertex in vertices)
                {
                    vertex.MoveByOffset(offset);

                    visualVertices += vertex.VisualVertices.Count;
                    collisionVertices += vertex.CollisionVertices.Count;
                }

                MessageBox.Show($"Moved {visualVertices} visual and {collisionVertices} collision verts");
            }
            catch
            {
                MessageBox.Show("Could not parse data");
                return;
            }
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
        
        private void AboutInfo(object sender, RoutedEventArgs e)
        {
            foreach (Window win in App.Current.Windows)
            {
                if (win is AboutInfo) return;
            }

            AboutInfo aboutInfoWindow = new AboutInfo(this.GetTitle(), this.GetAuthor(), this.GetGithub());
            aboutInfoWindow.Owner = this;
            aboutInfoWindow.Show();
        }
    }
}
