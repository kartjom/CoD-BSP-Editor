using CoD_BSP_Editor.Libs;
using System.IO;
using System.Numerics;

namespace CoD_BSP_Editor.Data
{
    public class CollmapData
    {
        public Shader[] Shaders;
        public BrushSides[] BrushSides;
        public Brush[] Brushes;
        public Model Model;

        public static CollmapData ReadFromByteArray(byte[] collmapData)
        {
            CollmapData collmap = new CollmapData();

            using (BinaryReader reader = new BinaryReader(new MemoryStream(collmapData)))
            {
                int shaderCount = reader.ReadInt32();
                int brushSidesNum = reader.ReadInt32();
                int brushNum = reader.ReadInt32();

                byte[] shaderData = reader.ReadBytes(BinLib.SizeOf<Shader>() * shaderCount);
                byte[] brushSidesData = reader.ReadBytes(BinLib.SizeOf<BrushSides>() * brushSidesNum);
                byte[] brushData = reader.ReadBytes(BinLib.SizeOf<Brush>() * brushNum);
                byte[] modelData = reader.ReadBytes(BinLib.SizeOf<Model>());

                collmap.Shaders = BinLib.ReadListFromByteArray<Shader>(shaderData).ToArray();
                collmap.BrushSides = BinLib.ReadListFromByteArray<BrushSides>(brushSidesData).ToArray();
                collmap.Brushes = BinLib.ReadListFromByteArray<Brush>(brushData).ToArray();
                collmap.Model = BinLib.ReadFromByteArray<Model>(modelData);
            }

            return collmap;
        }

        public static CollmapData CreateBrush(Vector3 BBoxMin, Vector3 BBoxMax, Shader? Texture = null)
        {
            CollmapData collmap = new CollmapData();
            BrushVolume brush = new BrushVolume(BBoxMin, BBoxMax);

            collmap.Shaders = new Shader[1];
            collmap.Shaders[0] = Texture ?? ShaderUtils.Construct("textures/common/trigger", 128, 671088641);
            
            collmap.Brushes = new Brush[1];
            collmap.Brushes[0] = new Brush() { MaterialID = 0, Sides = 6 };
            
            collmap.BrushSides = new BrushSides[6];

            for (int i = 0; i < 6; i++)
            {
                collmap.BrushSides[i].PlaneDistanceUnion = BinLib.ToByteArray<float>(brush.Distances[i]);
            }

            collmap.Model = new Model()
            {
                BBoxMin = new float[3] { brush.BBoxMin.X, brush.BBoxMin.Y, brush.BBoxMin.Z },
                BBoxMax = new float[3] { brush.BBoxMax.X, brush.BBoxMax.Y, brush.BBoxMax.Z },
                TrianglesoupsOffset = 0, TrianglesoupsSize = 0,
                CollisionaabbsOffset = 0, CollisionaabbsSize = 0,
                BrushesOffset = 0, BrushesSize = 1
            };

            return collmap;
        }
    }
}
