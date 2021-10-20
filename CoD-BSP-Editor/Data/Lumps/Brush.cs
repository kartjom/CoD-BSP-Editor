using System.Runtime.InteropServices;

namespace CoD_BSP_Editor.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Brush
    {
        public ushort Sides;
        public ushort MaterialID;
    }
}
