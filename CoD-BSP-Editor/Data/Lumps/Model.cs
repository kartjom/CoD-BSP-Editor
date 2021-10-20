using System.Runtime.InteropServices;

namespace CoD_BSP_Editor.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Model
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public float[] Position;

        public uint TrianglesoupsOffset;
        public uint TrianglesoupsSize;
        public uint CollisionaabbsOffset;
        public uint CollisionaabbsSize;
        public uint BrushesOffset;
        public uint BrushesSize;
    }
}
