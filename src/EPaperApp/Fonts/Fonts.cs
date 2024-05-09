using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPaperApp
{
    public partial class Fonts
    {
        public Fonts(int width, int height, byte[] fontTable) { 
            Width = width;
            Height = height;
            FontTable = fontTable;
        }
        public byte[] FontTable { get; }
        public int Width { get; }
        public int Height { get; }
    }
}
