using CoD_BSP_Editor.Libs;
using System.IO;

namespace CoD_BSP_Editor.Data
{
    public struct CollmapData
    {
        public Shader Shader;
        public BrushSides[] BrushSides;
        public Brush Brush;
        public Model Model;

        public static CollmapData ReadFromByteArray(byte[] collmapData)
        {
            CollmapData collmap = new CollmapData();

            using (BinaryReader reader = new BinaryReader(new MemoryStream(collmapData)))
            {
                byte[] shaderData = reader.ReadBytes(BinLib.SizeOf<Shader>());
                byte[] brushSidesData = reader.ReadBytes(BinLib.SizeOf<BrushSides>() * 6);
                byte[] brushData = reader.ReadBytes(BinLib.SizeOf<Brush>());
                byte[] modelData = reader.ReadBytes(BinLib.SizeOf<Model>());

                collmap.Shader = BinLib.ReadFromByteArray<Shader>(shaderData);
                collmap.BrushSides = BinLib.ReadListFromByteArray<BrushSides>(brushSidesData).ToArray();
                collmap.Brush = BinLib.ReadFromByteArray<Brush>(brushData);
                collmap.Model = BinLib.ReadFromByteArray<Model>(modelData);
            }

            return collmap;
        }
    }
}
