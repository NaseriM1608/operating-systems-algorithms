using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixed_Partitioning
{
    class Process
    {
        private static int Auto_ID = 0;
        public int ID;
        public int Size { get; set; }
        public bool Allocated { get; set; }
        public double ResidencyTime { get; set; }
        public Process(int Size)
        {
            this.ID = Auto_ID;
            Allocated = false;
            this.Size = Size;
            ResidencyTime = DetermineResidencyTime(this.Size);
            Auto_ID++;
        }
        double DetermineResidencyTime(int ProcessSize)
        {
            return 0.3 * ProcessSize + 10;
        }
    }
}
