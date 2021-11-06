using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Numerics
{
    public static class VectorExt
    {
        public static Vector3 FromString(string vecString)
        {
            vecString = vecString.Replace(',', '.');
            string[] splitVec = vecString.Split(' ');

            float X = float.Parse(splitVec[0]);
            float Y = float.Parse(splitVec[1]);
            float Z = float.Parse(splitVec[2]);

            Vector3 vector = new Vector3(X, Y, Z);
            return vector;
        }

        public static string String(this Vector3 vec, string separator = "")
        {
            string vecString = $"{vec.X} {vec.Y} {vec.Z}";
            
            if (string.IsNullOrEmpty(separator) == false)
            {
                vecString = vecString.Replace(" ", separator);
            }

            return vecString;
        }
    }
}
