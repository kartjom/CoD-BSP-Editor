using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CoD_BSP_Editor.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct Shader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        [JsonConverter(typeof(ShaderMaterialConverter))]
        public char[] Material;

        public uint SurfaceFlag;
        public uint ContentFlag;

        [JsonConstructor]
        public Shader(string material, uint surfaceFlag = 0, uint contentFlag = 0)
        {
            material = material.ToLower();
            StringBuilder matBuilder = new StringBuilder(material).Append('\0', 64 - material.Length);

            Material = matBuilder.ToString().ToCharArray();
            SurfaceFlag = surfaceFlag;
            ContentFlag = contentFlag;
        }

        public override string ToString()
        {
            return new string(this.Material).Trim('\0').ToLower();
        }
    }

    public class ShaderMaterialConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            char[] material = (char[]) value;
            string materialString = new string(material).Trim('\0').ToLower();

            writer.WriteValue(materialString);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(char[]);
        }
    }
}
