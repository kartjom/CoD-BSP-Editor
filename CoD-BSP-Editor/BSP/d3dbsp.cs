using CoD_BSP_Editor.Data;
using CoD_BSP_Editor.Libs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CoD_BSP_Editor.BSP
{
    public class d3dbsp
    {
        public Lump[] Lumps = new Lump[33];
        public List<byte[]> BinaryLumps = new List<byte[]>();

        public List<Shader> Shaders { get; set; }
        public List<Plane> Planes { get; set; }
        public List<BrushSides> BrushSides { get; set; }
        public List<Brush> Brushes { get; set; }
        public List<DrawVert> DrawVerts { get; set; }
        public List<Vector3> CollisionVerts { get; set; }
        public List<Model> Models { get; set; }
        public List<Entity> Entities { get; set; }

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileDirectory { get; set; }

        public d3dbsp(string filePath)
        {
            this.FilePath = filePath;
            this.FileName = Path.GetFileName(filePath);
            this.FileDirectory = Path.GetDirectoryName(filePath);

            this.LoadLumpsInfo();
            this.CorrectLumpsLength();
            this.LoadLumpsData();

            this.ParseLumpsDataToLists();
        }

        private void LoadLumpsInfo()
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(this.FilePath)))
            {
                reader.BaseStream.Position = 8;
                for (int i = 0; i < 33; i++)
                {
                    int length = reader.ReadInt32();
                    int offset = reader.ReadInt32();

                    Lumps[i] = new Lump(length, offset);
                }
            }
        }

        private void CorrectLumpsLength()
        {
            for (int i = 0; i < Lumps.Length - 1; i++)
            {
                int lumpLength = Lumps[i].Offset + Lumps[i].Length;
                int offsetDiff = Lumps[i + 1].Offset - lumpLength;
                if (offsetDiff > 0)
                {
                    Lumps[i].Length += offsetDiff;
                }
            }
        }

        private void LoadLumpsData()
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(this.FilePath)))
            {
                for (int i = 0; i < 33; i++)
                {
                    reader.BaseStream.Position = Lumps[i].Offset;
                    byte[] lumpData = reader.ReadBytes(Lumps[i].Length);

                    BinaryLumps.Add(lumpData);
                }
            }
        }

        private void ParseLumpsDataToLists()
        {
            this.Shaders = BinLib.ReadListFromByteArray<Shader>(BinaryLumps[0]);
            this.Planes = BinLib.ReadListFromByteArray<Plane>(BinaryLumps[2]);
            this.BrushSides = BinLib.ReadListFromByteArray<BrushSides>(BinaryLumps[3]);
            this.Brushes = BinLib.ReadListFromByteArray<Brush>(BinaryLumps[4]);
            this.DrawVerts = BinLib.ReadListFromByteArray<DrawVert>(BinaryLumps[7]);
            this.CollisionVerts = BinLib.ReadListFromByteArray<Vector3>(BinaryLumps[25]);
            this.Models = BinLib.ReadListFromByteArray<Model>(BinaryLumps[27]);

            string entData = this.EntityLumpToString();
            this.Entities = Entity.ParseEntitiesData(entData);
        }

        public byte[] ExtractCollmapData()
        {
            List<byte> collmapContent = new();
            byte[] shaderContent = BinLib.ListToByteArray<Shader>(this.Shaders);
            byte[] brushSidesContent = BinLib.ListToByteArray<BrushSides>(this.BrushSides);
            byte[] brushContent = BinLib.ListToByteArray<Brush>(this.Brushes);
            byte[] modelContent = BinLib.ToByteArray<Model>(this.Models[0]);

            byte[] shaderNum = BinLib.ToByteArray<int>(this.Shaders.Count);
            byte[] brushSidesNum = BinLib.ToByteArray<int>(this.BrushSides.Count);
            byte[] brushNum = BinLib.ToByteArray<int>(this.Brushes.Count);

            collmapContent.AddRange(shaderNum);
            collmapContent.AddRange(brushSidesNum);
            collmapContent.AddRange(brushNum);

            collmapContent.AddRange(shaderContent);
            collmapContent.AddRange(brushSidesContent);
            collmapContent.AddRange(brushContent);
            collmapContent.AddRange(modelContent);

            return collmapContent.ToArray();
        }

        public string EntityLumpToString()
        {
            string stringFromBytes = BinLib.BytesToString(this.BinaryLumps[29]);
            return stringFromBytes;
        }

        public Lump[] RecreateLumpOffsets(List<byte[]> LumpsData)
        {
            Lump[] newLumpsData = new Lump[33];
            int offset = Lumps[0].Offset;

            int index = 0;
            foreach (byte[] data in LumpsData)
            {
                int lumpOffset = offset;
                int lumpLength = data.Length;

                offset += data.Length;

                newLumpsData[index++] = new Lump(lumpLength, lumpOffset);
            }

            return newLumpsData;
        }

        public void UpdateLumps()
        {
            // Shaders
            byte[] newShaders = BinLib.ListToByteArray<Shader>(this.Shaders);
            this.BinaryLumps[0] = newShaders;

            // Planes
            byte[] newPlanes = BinLib.ListToByteArray<Plane>(this.Planes);
            this.BinaryLumps[2] = newPlanes;

            // BrushSides
            byte[] newBrushSides = BinLib.ListToByteArray<BrushSides>(this.BrushSides);
            this.BinaryLumps[3] = newBrushSides;

            // Brushes
            byte[] newBrushes = BinLib.ListToByteArray<Brush>(this.Brushes);
            this.BinaryLumps[4] = newBrushes;

            // DrawVerts
            byte[] newDrawVerts = BinLib.ListToByteArray<DrawVert>(this.DrawVerts);
            this.BinaryLumps[7] = newDrawVerts;

            // CollisionVerts
            byte[] newCollisionVerts = BinLib.ListToByteArray<Vector3>(this.CollisionVerts);
            this.BinaryLumps[25] = newCollisionVerts;

            // Models
            byte[] newModels = BinLib.ListToByteArray<Model>(this.Models);
            this.BinaryLumps[27] = newModels;
        }

        public void UpdateEntities(List<Entity> entities)
        {
            this.Entities = entities;
            
            string entityLumpAsString = Entity.WriteStringEntityLump(this.Entities);
            this.BinaryLumps[29] = BinLib.StringToBytes(entityLumpAsString);
        }

        public byte[] CreateBSP()
        {
            List<byte> bspFile = new List<byte>();

            bspFile.AddRange(BinLib.StringToBytes("IBSP"));
            bspFile.AddRange(BinLib.ToByteArray<int>(59));

            Lump[] newLumps = this.RecreateLumpOffsets(BinaryLumps);
            bspFile.AddRange(Lump.ListToByteArray(newLumps));

            foreach (byte[] data in BinaryLumps)
            {
                bspFile.AddRange(data);
            }

            return bspFile.ToArray();
        }

        public static string CreateArenaFile(string fileName)
        {
            List<string> arenaContentList = new List<string>()
            {
                "{",
                $"\tmap\t\t\t\"{fileName}\"",
                $"\tlongname\t\"{fileName}\"",
                "\tgametype\t\"dm tdm ctf sd\"",
                "}"
            };

            string arenaContent = string.Join('\n', arenaContentList);
            return arenaContent;
        }

        public static string CreateGscFile(string fileName)
        {
            List<string> gscContentList = new List<string>()
            {
                "main()",
                "{",
                "\tmaps\\mp\\_load::main();",
                "\tmaps\\mp\\_flak_gmi::main();",
                "\n\tsetcvar(\"sv_night\", \"0\");\n",
                "\tlevel thread maps\\mp\\_tankdrive_gmi::main();",
                "\tlevel thread maps\\mp\\_jeepdrive_gmi::main();",
                "}"
            };

            string gscContent = string.Join('\n', gscContentList);
            return gscContent;
        }

        public void AddShaders(Shader[] shaders)
        {
            foreach (Shader shader in shaders)
            {
                int MaterialID = this.FindMaterialIndex(shader.ToString());

                if (MaterialID == -1)
                {
                    this.Shaders.Add(shader);
                }
            }
        }

        public int FindMaterialIndex(string material)
        {
            material = material.ToLower();

            for (int i = 0; i < this.Shaders.Count; i++)
            {
                string shader = this.Shaders[i].ToString().ToLower();
                if (shader == material)
                {
                    return i;
                }
            }

            return -1;
        }

        public int FindBrushSidesOffset(int brushIndex)
        {
            if (brushIndex > Brushes.Count)
            {
                return -1;
            }

            int brushSidesOffset = 0;
            for (int i = 0; i < brushIndex; i++)
            {
                brushSidesOffset += Brushes[i].Sides;
            }

            return brushSidesOffset;
        }
    }
}
