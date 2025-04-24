using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixed_Partitioning
{
    class Memory_Block
    {
        private static int Auto_ID = 0;
        public int ID;
        public bool Occupied {  get; set; }
        public int Size {  get; set; }
        public Process Process { get; set; }
        public Memory_Block(int Size) 
        {
            ID = Auto_ID;
            this.Size = Size;
            Occupied = false;
            Process = null;
            Auto_ID++;
        }

    }
}
