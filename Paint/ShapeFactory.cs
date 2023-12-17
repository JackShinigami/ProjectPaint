using IShapeContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint
{
    public class ShapeFactory
    {
        public Dictionary<string, IShape> Prototypes =
            new Dictionary<string, IShape>();

        public IShape Create(String choice)
        {
            IShape shape = Prototypes[choice].Clone();
            return shape;
        }
    }
}
