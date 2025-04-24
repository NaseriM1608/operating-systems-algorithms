using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixed_Partitioning
{
    class Memory
    {
        public List<Memory_Block> _blocks;
        public int Blocks_Num { get; set; }
        public int Total_Size { get; set; }
        public Memory(int blocks_Num, int Total_Size)
        {
            _blocks = new List<Memory_Block>(Blocks_Num);
            this.Total_Size = Total_Size;
            Blocks_Num = blocks_Num;
        }

    }
}
