using Newtonsoft.Json;
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
        public Vector3 BBoxMin;
        public Vector3 BBoxMax;

        public int ShaderIndex;
        public List<Plane> Planes = new();

        [JsonConstructor]
        public BrushVolume()
        {
            // For JSON Serializer
        }

        public BrushVolume(Vector3 min, Vector3 max)
        {
            this.Initialize(min, max);
        }

        public BrushVolume(string start, string end)
        {
            string[] startVec = start.Split(' ');
            float startX = float.Parse(startVec[0]);
            float startY = float.Parse(startVec[1]);
            float startZ = float.Parse(startVec[2]);

            string[] endVec = end.Split(' ');
            float endX = float.Parse(endVec[0]);
            float endY = float.Parse(endVec[1]);
            float endZ = float.Parse(endVec[2]);

            Vector3 min = new Vector3(startX, startY, startZ);
            Vector3 max = new Vector3(endX, endY, endZ);

            this.Initialize(min, max);
        }

        public BrushVolume(float[] distances)
        {
            Vector3 min = new Vector3(distances[0], distances[1], distances[2]);
            Vector3 max = new Vector3(distances[3], distances[4], distances[5]);

            this.Initialize(min, max);
        }

        public BrushVolume(BrushSides[] brushSides)
        {
            float xMin = brushSides[0].GetDistance();
            float yMin = brushSides[3].GetDistance();
            float zMin = brushSides[4].GetDistance();
            Vector3 min = new Vector3(xMin, yMin, zMin);

            float xMax = brushSides[1].GetDistance();
            float yMax = brushSides[2].GetDistance();
            float zMax = brushSides[5].GetDistance();
            Vector3 max = new Vector3(xMax, yMax, zMax);

            this.Initialize(min, max);
        }

        private void Initialize(Vector3 min, Vector3 max)
        {
            BBoxMin = min;
            BBoxMax = max;
            
            Vector3 Size = this.GetSize();
            Vector3 Center = this.GetCenter();

            // Recalculating the diagonal from the center
            BBoxMin = new Vector3(Center.X - Size.X / 2, Center.Y - Size.Y / 2, Center.Z - Size.Z / 2);
            BBoxMax = new Vector3(Center.X + Size.X / 2, Center.Y + Size.Y / 2, Center.Z + Size.Z / 2);
        }

        public BrushSides[] GetBrushSides(int planeIndexOffset = 0)
        {
            BrushSides[] brushSides = new BrushSides[6 + this.Planes.Count];
            float[] Distances = this.GetDistances();

            for (int i = 0; i < 6; i++)
            {
                BrushSides brushSideStruct = new BrushSides();
                brushSideStruct.MaterialID = (ushort)this.ShaderIndex;
                brushSideStruct.SetDistance(Distances[i]);

                brushSides[i] = brushSideStruct;
            }

            for (int i = 0; i < this.Planes.Count; i++)
            {
                BrushSides brushSidePlane = new BrushSides();
                brushSidePlane.MaterialID = (ushort)this.ShaderIndex;

                int planeIndex = i + planeIndexOffset;
                brushSidePlane.SetPlaneIndex((uint)planeIndex);

                brushSides[6 + i] = brushSidePlane;
            }

            return brushSides;
        }

        public float[] GetDistances()
        {
            Vector3 Size = this.GetSize();
            Vector3 Center = this.GetCenter();
            float[] Distances = new float[6];

            Distances[0] = Center.X - (Size.X / 2); // Left
            Distances[1] = Center.X + (Size.X / 2); // Right
            Distances[2] = Center.Y - (Size.Y / 2); // Front
            Distances[3] = Center.Y + (Size.Y / 2); // Back
            Distances[4] = Center.Z - (Size.Z / 2); // Bottom
            Distances[5] = Center.Z + (Size.Z / 2); // Top

            return Distances;
        }

        public Vector3 GetSize()
        {
            float x = Math.Abs(BBoxMin.X - BBoxMax.X);
            float y = Math.Abs(BBoxMin.Y - BBoxMax.Y);
            float z = Math.Abs(BBoxMin.Z - BBoxMax.Z);

            return new Vector3(x, y, z); // Lenght of the brush in all axes
        }

        public Vector3 GetCenter()
        {
            return (this.BBoxMin + this.BBoxMax) / 2;
        }

        public bool ContainsVector(Vector3 origin)
        {
            return
                origin.X > this.BBoxMin.X && origin.X < this.BBoxMax.X &&
                origin.Y > this.BBoxMin.Y && origin.Y < this.BBoxMax.Y &&
                origin.Z > this.BBoxMin.Z && origin.Z < this.BBoxMax.Z;
        }

        public Vector3 MoveByOffset(Vector3 offset)
        {
            Vector3 oldCenter = this.GetCenter();

            Vector3 newMin = this.BBoxMin + offset;
            Vector3 newMax = this.BBoxMax + offset;

            this.Initialize(newMin, newMax);

            return oldCenter;
        }
    }
}
