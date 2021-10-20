using CoD_BSP_Editor.Libs;
using System.IO;
using System.Numerics;

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

        public static CollmapData CreateTrigger(Vector3 BBoxMin, Vector3 BBoxMax, Shader? Texture = null)
        {
            CollmapData collmap = new CollmapData();
            BrushVolume brush = new BrushVolume(BBoxMin, BBoxMax);

            collmap.Shader = Texture ?? ShaderUtils.Construct("textures/common/trigger", 128, 671088641);
            collmap.Brush = new Brush() { MaterialID = 0, Sides = 6 };
            collmap.BrushSides = new BrushSides[6];

            for (int i = 0; i < 6; i++)
            {
                collmap.BrushSides[i].PlaneDistanceUnion = BinLib.ToByteArray<float>(brush.Distances[i]);
            }

            collmap.Model = new Model()
            {
                Position = new float[6] {
                    brush.BBoxMin.X, brush.BBoxMin.Y, brush.BBoxMin.Z,
                    brush.BBoxMax.X, brush.BBoxMax.Y, brush.BBoxMax.Z
                },
                TrianglesoupsOffset = 0, TrianglesoupsSize = 1,
                CollisionaabbsOffset = 0, CollisionaabbsSize = 0,
                BrushesOffset = 0, BrushesSize = 1
            };

            return collmap;
        }
    }
}
