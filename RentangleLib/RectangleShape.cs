
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using IShapeContract;

namespace RentangleLib
{
    public class RectangleShape : IShape
    {
        public override UIElement Draw()
        {
            // TODO: can dam bao Diem 0 < Diem 1
            Point start = Points[0];
            Point end = Points[1];
            IShapeContract.CreateRightPointsForDraw.Change(ref start, ref end);
            double width = Math.Abs(end.X - start.X);
            double height = Math.Abs(end.Y - start.Y);

            var element = new System.Windows.Shapes.Rectangle()
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
            var rectangle = new RectangleShape();
            rectangle.Brush = this.Brush;
            rectangle.Thickness = this.Thickness;
            rectangle.DashArray = this.DashArray;
            foreach (var point in this.Points)
            {
                rectangle.Points.Add(new Point(point.X, point.Y));
            }
            return rectangle;
        }

        public override string Name => "Rectangle";
    }

}
