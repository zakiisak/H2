using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaskeAutomaten
{
    public class IdGenerator
    {
        private static int IdCounter;
        public static int GetNextId()
        {
            return IdCounter++;
        }
    }
}
