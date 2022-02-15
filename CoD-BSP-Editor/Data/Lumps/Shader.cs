using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace CoD_BSP_Editor.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Shader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public char[] Material;

        public uint SurfaceFlag;
        public uint ContentFlag;

        public override string ToString()
        {
            return new string(this.Material).Trim('\0').ToLower();
        }

        public Shader(string material, uint surfaceFlag = 0, uint contentFlag = 0)
        {
            material = material.ToLower();
            StringBuilder matBuilder = new StringBuilder(material).Append('\0', 64 - material.Length);

            Material = matBuilder.ToString().ToCharArray();
            SurfaceFlag = surfaceFlag;
            ContentFlag = contentFlag;
        }
    }
}
