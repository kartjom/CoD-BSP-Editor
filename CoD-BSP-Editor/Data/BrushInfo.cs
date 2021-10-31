using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CoD_BSP_Editor.Data
{
    public class BrushInfo
    {
        public int Index;
        public int SidesOffset;
        public int SidesCount;
        public Vector3 Center;

        public BrushInfo(int index, int offset, int count)
        {
            Index = index;
            SidesOffset = offset;
            SidesCount = count;

            this.CalculateCenterFromSides();
        }

        public void CalculateCenterFromSides()
        {
            List<BrushSides> brushSides = this.GetSides();

            float xMin = brushSides[0].GetDistance();
            float yMin = brushSides[3].GetDistance();
            float zMin = brushSides[4].GetDistance();
            Vector3 BBoxMin = new Vector3(xMin, yMin, zMin);

            float xMax = brushSides[1].GetDistance();
            float yMax = brushSides[2].GetDistance();
            float zMax = brushSides[5].GetDistance();
            Vector3 BBoxMax = new Vector3(xMax, yMax, zMax);

            Center = (BBoxMin + BBoxMax) / 2;
        }

        public Brush GetBrush()
        {
            Brush brush = MainWindow.bsp.Brushes[this.Index];
            return brush;
        }

        public BrushSides GetSideByIndex(int index)
        {
            int sideIndex = this.SidesOffset + index;
            BrushSides side = MainWindow.bsp.BrushSides[sideIndex];

            return side;
        }

        public List<BrushSides> GetSides()
        {
            List<BrushSides> sides = MainWindow.bsp.BrushSides.GetRange(this.SidesOffset, this.SidesCount);
            return sides;
        }

        public string GetCenterString()
        {
            string center = $"{Center.X}| {Center.Y}| {Center.Z}";
            center = center.Replace(',', '.');
            center = center.Replace('|', ',');

            return center;
        }
    }
}
