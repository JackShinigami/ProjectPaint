
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using IShapeContract;

namespace EllipseLib
{
    public class EllipseShape : IShape
    {
        public override UIElement Draw()
        {
            // TODO: can dam bao Diem 0 < Diem 1
            Point start = Points[0];
            Point end = Points[1];
            IShapeContract.CreateRightPointsForDraw.Change(ref start, ref end);
            double width = Math.Abs(Points[1].X - Points[0].X);
            double height = Math.Abs(Points[1].Y - Points[0].Y);

            var element = new Ellipse()
            {
                Width = width,
                Height = height,
                Stroke = Brush,
                StrokeThickness = Thickness,
                StrokeDashArray = DashArray

            };
            Canvas.SetLeft(element, start.X);
            Canvas.SetTop(element, start.Y);

            return element;
        }

        public override IShape Clone()
        {
            var ellipse = new EllipseShape();
            ellipse.Brush = this.Brush;
            ellipse.Thickness = this.Thickness;
            ellipse.DashArray = this.DashArray;
            foreach (var point in this.Points)
            {
                ellipse.Points.Add(new Point(point.X, point.Y));
            }
            return ellipse;
        }

        public override string Name => "Ellipse";
    }
}
