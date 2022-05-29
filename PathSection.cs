using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vstupniukol
{
    public struct PathSection
    {
        public int Position { get; set; }
        public int Lower { get; set; } 
        public int Higher { get; set; }

       public PathSection(int lower,int higher,int position)
        {
            Position = position;
            Lower = lower;
            Higher = higher;
        }
    }

    public class TranslatedPath
    {
        public PathSection[] Horizontals { get; set; }
        public PathSection[] Verticals { get; set; }

        //public Point StartPoint { get; set; }
        //public bool IsStartpointVertical { get; set; }

        //public Point EndPoint { get; set; }
        //public bool IsEndpointVertical { get; set; }
    }
}
