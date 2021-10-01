using CoD_BSP_Editor.Data;
using CoD_BSP_Editor.Libs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoD_BSP_Editor.BSP
{
    public class d3dbsp
    {
        public Lump[] Lumps = new Lump[33];
        public List<byte[]> BinaryLumps = new List<byte[]>();

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

            this.ParseStringEntityLumpToList();
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

        private void ParseStringEntityLumpToList()
        {
            string entData = this.EntityLumpToString();
            this.Entities = Entity.ParseEntitiesData(entData);
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
    }
}
