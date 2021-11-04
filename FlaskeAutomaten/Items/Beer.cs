using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaskeAutomaten.Items
{
    public class Beer : Beverage
    {
        public Beer(int Id) : base(Id, "Beer", 20) { }
    }
}
