using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicScepter
{
    public class MiniObeliskObject
    {
        public string Name { get; set; }
        public int CoordX { get; set; }
        public int CoordY { get; set; }

        public MiniObeliskObject(string name, int x, int y)
        {
            Name = name;
            CoordX = x;
            CoordY = y;
        }
    }
}
