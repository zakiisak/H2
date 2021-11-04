using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using FlaskeAutomaten.Items;

namespace FlaskeAutomaten
{
    public class BeverageUi
    {
        public Beverage Beverage { get; private set; }
        public Label attachedLabel { get; private set; }
        public double x { get; set; }
        public double y { get; set; }
        public double dx { get; private set; }

        public BeverageUi(Beverage beverage, Label label, double x, double y, double dx)
        {
            this.Beverage = beverage;
            this.attachedLabel = label;
            this.x = x;
            this.y = y;
            this.dx = dx;
        }

        public void MoveLabel()
        {
            this.x += dx;
            Canvas.SetLeft(attachedLabel, x);
        }
    }
}
