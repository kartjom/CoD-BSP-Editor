using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CoD_BSP_Editor.Data
{
    public class Vertex
    {
        public List<int> VisualVertices { get; set; } = new List<int>();
        public List<int> CollisionVertices { get; set; } = new List<int>();

        public Vertex(Vector3 origin)
        {
            this.FindClosestVisualVertex(origin);
            this.FindDuplicateVisualVertices();

            if (this.IsValid())
            {
                this.FindCollisionVertices();
            }
            else
            {
                this.FindCollisionVertices(origin);
            }
        }

        public Vertex(int visualVertexIndex)
        {
            this.VisualVertices.Add(visualVertexIndex);
            this.FindDuplicateVisualVertices();
            this.FindCollisionVertices();
        }

        public void FindClosestVisualVertex(Vector3 origin)
        {
            // Find closest vert
            float closestVisualVertDistance = Vector3.DistanceSquared(origin, Vec3.FromArray(MainWindow.bsp.DrawVerts[0].Position));
            int visualVertex = 0;
            for (int i = 1; i < MainWindow.bsp.DrawVerts.Count; i++)
            {
                Vector3 drawVertPos = Vec3.FromArray(MainWindow.bsp.DrawVerts[i].Position);

                float newDistance = Vector3.DistanceSquared(origin, drawVertPos);
                if (newDistance < closestVisualVertDistance)
                {
                    closestVisualVertDistance = newDistance;
                    visualVertex = i;
                }
            }

            // Return if distance to visual vert is too big
            if (closestVisualVertDistance > 128 * 128) return;

            this.VisualVertices.Add(visualVertex);
        }

        public void FindDuplicateVisualVertices()
        {
            if (this.IsValid() == false) return;

            // Find duplicate verts
            Vector3 closestVertexPosition = Vec3.FromArray(MainWindow.bsp.DrawVerts[this.VisualVertices[0]].Position);
            for (int i = 0; i < MainWindow.bsp.DrawVerts.Count; i++)
            {
                if (i == this.VisualVertices[0]) continue;

                Vector3 currentVert = Vec3.FromArray(MainWindow.bsp.DrawVerts[i].Position);
                if (Vector3.Equals(closestVertexPosition, currentVert))
                {
                    this.VisualVertices.Add(i);
                }
            }
        }

        public void FindCollisionVertices()
        {
            Vector3 origin = Vec3.FromArray(MainWindow.bsp.DrawVerts[this.VisualVertices[0]].Position);
            this.FindCollisionVertices(origin);
        }
        
        public void FindCollisionVertices(Vector3 origin)
        {
            for (int i = 0; i < MainWindow.bsp.CollisionVerts.Count; i++)
            {
                if (Vector3.DistanceSquared(origin, MainWindow.bsp.CollisionVerts[i]) <= 0.7f)
                {
                    this.CollisionVertices.Add(i);
                }
            }
        }

        public static List<Vertex> FindWithinBounds(Vector3 min, Vector3 max)
        {
            List<Vertex> vertices = new List<Vertex>();
            BrushVolume boundingBox = new BrushVolume(min, max);

            List<Vector3> origins = new List<Vector3>();
            for (int i = 0; i < MainWindow.bsp.DrawVerts.Count; i++)
            {
                Vector3 drawVertPos = Vec3.FromArray(MainWindow.bsp.DrawVerts[i].Position);

                if (boundingBox.ContainsVector(drawVertPos) && origins.Contains(drawVertPos) == false)
                {
                    Vertex vertex = new Vertex(i);

                    vertices.Add(vertex);
                    origins.Add(drawVertPos);
                }
            }

            return vertices;
        }

        public void MoveTo(Vector3 newOrigin)
        {
            for (int i = 0; i < this.VisualVertices.Count; i++)
            {
                DrawVert vert = MainWindow.bsp.DrawVerts[this.VisualVertices[i]];
                vert.Position = newOrigin.ToArray();

                MainWindow.bsp.DrawVerts[this.VisualVertices[i]] = vert;
            }

            for (int i = 0; i < this.CollisionVertices.Count; i++)
            {
                MainWindow.bsp.CollisionVerts[this.CollisionVertices[i]] = newOrigin;
            }
        }
        
        public void MoveByOffset(Vector3 offset)
        {
            for (int i = 0; i < this.VisualVertices.Count; i++)
            {
                DrawVert vert = MainWindow.bsp.DrawVerts[this.VisualVertices[i]];
                
                Vector3 newPos = Vec3.FromArray(vert.Position) + offset;
                vert.Position = newPos.ToArray();

                MainWindow.bsp.DrawVerts[this.VisualVertices[i]] = vert;
            }

            for (int i = 0; i < this.CollisionVertices.Count; i++)
            {
                Vector3 newPos = MainWindow.bsp.CollisionVerts[this.CollisionVertices[i]];
                newPos += offset;

                MainWindow.bsp.CollisionVerts[this.CollisionVertices[i]] = newPos;
            }
        }

        public bool IsValid()
        {
            return VisualVertices.Count > 0 || CollisionVertices.Count > 0;
        }
    }
}
