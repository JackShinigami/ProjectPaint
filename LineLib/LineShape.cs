
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using IShapeContract;

namespace LineLib
{
    public class LineShape : IShape
    {
        public override UIElement Draw()
        {
            return new Line()
            {
                X1 = Points[0].X,
                Y1 = Points[0].Y,
                X2 = Points[1].X,
                Y2 = Points[1].Y,
                Stroke = Brush,
                StrokeThickness = Thickness,
                StrokeDashArray = DashArray
            };
        }

        public override IShape Clone()
        {
            var line = new LineShape();
            line.Brush = this.Brush;
            line.Thickness = this.Thickness;
            line.DashArray = this.DashArray;
            foreach (var point in this.Points)
            {
                line.Points.Add(new Point(point.X, point.Y));
            }
            return line;
        }

        public override string Name => "Line";

    }

}
