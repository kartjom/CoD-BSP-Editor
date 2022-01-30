using CoD_BSP_Editor.Libs;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CoD_BSP_Editor.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BrushSides
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] PlaneDistanceUnion;
        public uint MaterialID;

        public uint GetPlaneIndex()
        {
            uint PlaneIndex = BinLib.ReadFromByteArray<uint>(PlaneDistanceUnion);
            return PlaneIndex;
        }

        public void SetPlaneIndex(uint index)
        {
            PlaneDistanceUnion = BinLib.ToByteArray<uint>(index);
        }

        public float GetDistance()
        {
            float Distance = BinLib.ReadFromByteArray<float>(PlaneDistanceUnion);
            return Distance;
        }

        public void SetDistance(float distance)
        {
            PlaneDistanceUnion = BinLib.ToByteArray<float>(distance);
        }

        public override string ToString()
        {
            return $"Shader: {this.MaterialID} | Distance: {this.GetDistance()} | Plane: {this.GetPlaneIndex()}";
        }

        public static int FindBrushSidesStart(int brushIndex)
        {
            if (brushIndex >= MainWindow.bsp.Brushes.Count) return -1;

            int index = 0;
            for (int i = 0; i < brushIndex; i++)
            {
                index += MainWindow.bsp.Brushes[i].Sides;
            }

            return index;
        }

        public static BrushSides[] GetBrushSides(int brushIndex)
        {
            if (brushIndex >= MainWindow.bsp.Brushes.Count) return default;

            List<BrushSides> brushSides = new();
            
            int brushSidesOffset = BrushSides.FindBrushSidesStart(brushIndex);
            for (int i = brushSidesOffset; i < brushSidesOffset + 6; i++)
            {
                brushSides.Add( MainWindow.bsp.BrushSides[i]);
            }

            return brushSides.ToArray();
        }
    }
}
