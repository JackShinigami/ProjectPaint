
using System.Windows;
using System.Windows.Media;
using IKleFileContract;

namespace IShapeContract
{
    public abstract class IShape: IKleFile
    {
        public abstract string Name { get; }
        public List<Point> Points { get; set; } = new List<Point>();
        public Brush Brush { get; set; } = Brushes.Black;
        public int Thickness { get; set; } = 1;
        public DoubleCollection DashArray { get; set; } = new DoubleCollection();
        public abstract UIElement Draw();
        public abstract IShape Clone();
    }
}
