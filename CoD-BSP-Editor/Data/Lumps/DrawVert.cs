using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CoD_BSP_Editor.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DrawVert
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] Position;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] UV;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] LightmapUV;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] Normal;

        public float Unknown;
    };
}
