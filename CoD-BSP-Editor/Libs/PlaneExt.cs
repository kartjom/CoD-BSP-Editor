using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using CoD_BSP_Editor.Data;

namespace CoD_BSP_Editor.Libs
{
    public static class PlaneExt
    {
        public static Plane[] GetBrushPlanes(int brushIndex)
        {
            if (brushIndex >= MainWindow.bsp.Brushes.Count) return default;
            Brush brush = MainWindow.bsp.Brushes[brushIndex];

            if (brush.Sides <= 6) return new Plane[0];

            List<Plane> planes = new();

            BrushSides[] brushSides = BrushSides.GetBrushSides(brushIndex);
            for (int i = 6; i < brushSides.Length; i++)
            {
                int planeIndex = (int)brushSides[i].GetPlaneIndex();
                Plane plane = MainWindow.bsp.Planes[planeIndex];
                planes.Add(plane);
            }

            return planes.ToArray();
        }
    }
}
