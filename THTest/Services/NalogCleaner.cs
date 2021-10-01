using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace THTest.Services
{
    public class NalogCleaner : ICleaner
    {
        public readonly TimeSpan frequency = TimeSpan.FromMinutes(5);
        public DateTime lastClean { get; private set; }
        public NalogCleaner()
        {
            lastClean = DateTime.Now;
        }
        public void Clean()
        {
            if (DateTime.Now - lastClean >= frequency)
            {
                NalogBuffer.Init().ClearOldRows();
                lastClean = DateTime.Now;
            }
        }
    }
}
