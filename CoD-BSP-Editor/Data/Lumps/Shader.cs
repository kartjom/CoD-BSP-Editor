using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CoD_BSP_Editor.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Shader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public char[] Material;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] Flags; // Surface, Content

        public override string ToString()
        {
            return new string(this.Material).Trim('\0').ToLower();
        }

        public static Shader Construct(string _newMaterial, uint _flag1 = 0, uint _flag2 = 0)
        {
            _newMaterial = _newMaterial.ToLower();
            StringBuilder matBuilder = new StringBuilder(_newMaterial).Append('\0', 64 - _newMaterial.Length);

            return new Shader()
            {
                Material = matBuilder.ToString().ToCharArray(),
                Flags = new uint[] { _flag1, _flag2 }
            };
        }
    }
}
