using CoD_BSP_Editor.Libs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CoD_BSP_Editor.Data
{
    public class ModelData
    {
        public List<Shader> Shaders = new();
        public List<BrushVolume> Brushes = new();
        public List<Entity> Entities = new();

        public bool RotationEnabled = false;
        public Vector3 CenterBeforeRotation;

        public Brush[] GetBrushes()
        {
            Brush[] newBrushes = new Brush[Brushes.Count];

            for (int i = 0; i < Brushes.Count; i++)
            {
                int sidesCount = 6 + Brushes[i].Planes.Count;
                Brush brushStruct = new Brush
                {
                    Sides = (ushort)sidesCount,
                    MaterialID = (ushort)Brushes[i].ShaderIndex
                };

                newBrushes[i] = brushStruct;
            }

            return newBrushes;
        }

        public BrushSides[] GetBrushSides(int planeIndexOffset = 0)
        {
            List<BrushSides> brushSides = new();

            for (int i = 0; i < Brushes.Count; i++)
            {
                BrushSides[] sides = Brushes[i].GetBrushSides(planeIndexOffset);
                brushSides.AddRange(sides);

                planeIndexOffset += Brushes[i].Planes.Count;
            }

            return brushSides.ToArray();
        }

        public Plane[] GetPlanes()
        {
            List<Plane> planes = new();

            for (int i = 0; i < Brushes.Count; i++)
            {
                planes.AddRange(Brushes[i].Planes);
            }

            return planes.ToArray();
        }

        public Vector3 GetBBoxMin()
        {
            if (Brushes.Count == 0) return new Vector3();
            if (Brushes.Count == 1) return Brushes[0].BBoxMin;

            Vector3 min = Brushes[0].BBoxMin;
            for (int i = 1; i < Brushes.Count; i++)
            {
                Vector3 current = Brushes[i].BBoxMin;
                min = Vector3.Min(min, current);
            }

            return min;
        }
        
        public Vector3 GetBBoxMax()
        {
            if (Brushes.Count == 0) return new Vector3();
            if (Brushes.Count == 1) return Brushes[0].BBoxMax;

            Vector3 max = Brushes[0].BBoxMax;
            for (int i = 1; i < Brushes.Count; i++)
            {
                Vector3 current = Brushes[i].BBoxMax;
                max = Vector3.Max(max, current);
            }

            return max;
        }

        public Vector3 GetCenter()
        {
            Vector3 min = this.GetBBoxMin();
            Vector3 max = this.GetBBoxMax();

            Vector3 center = (min + max) / 2;
            return center;
        }

        public Vector3 EnableRotation()
        {
            if (RotationEnabled) return this.CenterBeforeRotation;

            Vector3 centerBeforeRotation = this.GetCenter();
            for (int i = 0; i < Brushes.Count; i++)
            {
                Brushes[i].MoveByOffset(-centerBeforeRotation);
            }

            this.RotationEnabled = true;
            this.CenterBeforeRotation = centerBeforeRotation;

            return centerBeforeRotation;
        }

        public void CorrectShaderIndexes()
        {
            for (int i = 0; i < Brushes.Count; i++)
            {
                string shaderName = Shaders[Brushes[i].ShaderIndex].ToString();
                int shaderIndex = MainWindow.bsp.FindMaterialIndex(shaderName);

                Brushes[i].ShaderIndex = shaderIndex;
            }
        }

        public void CorrectModelIndexInEntities()
        {
            for (int i = 0; i < this.Entities.Count; i++)
            {
                for (int j = 0; j < this.Entities[i].KeyValues.Count; j++)
                {
                    if (this.Entities[i].KeyValues[j].Key == "model" && this.Entities[i].KeyValues[j].Value.StartsWith('*'))
                    {
                        this.Entities[i].KeyValues[j] = new("model", $"*{MainWindow.bsp.Models.Count}");
                    }
                }
            }
        }

        public Entity GetBrushEntity()
        {
            string classname = RotationEnabled ? "script_brushmodel" : "func_static";
            int modelIndex = MainWindow.bsp.Models.Count;

            Entity modelEntity = new Entity(classname);
            modelEntity.AddKeyValue("model", $"*{modelIndex}");

            if (RotationEnabled)
            {
                modelEntity.AddKeyValue("origin", this.CenterBeforeRotation.String());
                modelEntity.AddKeyValue("angles", "0 0 0");
            }

            return modelEntity;
        }

        public Model GetModel()
        {
            Model modelData = new Model
            {
                BBoxMin = this.GetBBoxMin().ToArray(),
                BBoxMax = this.GetBBoxMax().ToArray(),
                BrushesOffset = (uint)MainWindow.bsp.Brushes.Count,
                BrushesSize = (uint)Brushes.Count,
            };

            return modelData;
        }

        public string Serialize()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            return json;
        }

        public static ModelData Deserialize(string json)
        {
            ModelData modelData = JsonConvert.DeserializeObject<ModelData>(json);
            return modelData;
        }
    }
}
