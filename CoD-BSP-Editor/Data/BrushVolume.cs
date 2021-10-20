using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CoD_BSP_Editor.Data
{
    public class BrushVolume
    {
        public Vector3 Size = new Vector3();
        public Vector3 Center = new Vector3();

        public Vector3 BBoxMin;
        public Vector3 BBoxMax;

        public float[] Distances = new float[6];

        public BrushVolume(Vector3 min, Vector3 max)
        {
            this.Initialize(min, max);
        }

        public BrushVolume(float[] distances)
        {
            Vector3 vec1 = new Vector3(distances[0], distances[1], distances[2]);
            Vector3 vec2 = new Vector3(distances[3], distances[4], distances[5]);

            this.Initialize(vec1, vec2);
        }

        private void Initialize(Vector3 min, Vector3 max)
        {
            if (Vector3.Equals(Vector3.Min(min, max), max))
            {
                (min, max) = (max, min);
            }

            BBoxMin = min;
            BBoxMax = max;

            Center = (BBoxMin + BBoxMax) / 2;

            Size.X = Math.Abs(BBoxMin.X - BBoxMax.X);
            Size.Y = Math.Abs(BBoxMin.Y - BBoxMax.Y);
            Size.Z = Math.Abs(BBoxMin.Z - BBoxMax.Z);

            Distances[0] = Center.X - (Size.X / 2); // Left
            Distances[1] = Center.X + (Size.X / 2); // Right
            Distances[2] = Center.Y - (Size.Y / 2); // Front
            Distances[3] = Center.Y + (Size.Y / 2); // Back
            Distances[4] = Center.Z - (Size.Z / 2); // Bottom
            Distances[5] = Center.Z + (Size.Z / 2); // Top
        }
    }
}
