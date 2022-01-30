using System.Numerics;
using System.Runtime.InteropServices;

namespace CoD_BSP_Editor.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Model
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] BBoxMin;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] BBoxMax;

        public uint TrianglesoupsOffset;
        public uint TrianglesoupsSize;
        public uint CollisionaabbsOffset;
        public uint CollisionaabbsSize;
        public uint BrushesOffset;
        public uint BrushesSize;

        public void SetBoundaries(Vector3 min, Vector3 max)
        {
            this.BBoxMin = min.ToArray();
            this.BBoxMax = max.ToArray();
        }
    }
}
