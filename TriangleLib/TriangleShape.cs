using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using IShapeContract;


namespace TriangleLib
{
    public class TriangleShape : IShape
    {


        public override string Name => "Triangle";

        public override UIElement Draw()
        {

            Point start = Points[0];
            Point end = Points[1];

            bool isSwapped = start.Y > end.Y;

            IShapeContract.CreateRightPointsForDraw.Change(ref start, ref end);

            double width = Math.Abs(end.X - start.X);
            double height = Math.Abs(end.Y - start.Y);
            Point point1 = new Point(start.X + width / 2, start.Y);
            Point point2 = new Point(start.X, start.Y + height);
            Point point3 = new Point(start.X + width, start.Y + height);

            Point point4 = new Point(start.X + width / 2, start.Y + height);
            Point point5 = new Point(start.X + width, start.Y);
            Point point6 = new Point(start.X, start.Y);

            var element = new System.Windows.Shapes.Polygon()
            {
                Stroke = Brush,
                StrokeThickness = Thickness,
                StrokeDashArray = DashArray
            };
         
            if (!isSwapped)
            {
                element.Points.Add(point1);
                element.Points.Add(point2);
                element.Points.Add(point3);
            }
            else
            {
                element.Points.Add(point4);
                element.Points.Add(point5);
                element.Points.Add(point6);
            }

            return element;
        }


        public override IShape Clone()
        {
            var triangle = new TriangleShape
            {
                Brush = this.Brush,
                Thickness = this.Thickness,
                DashArray = this.DashArray
            };

            foreach (var point in this.Points)
            {
                triangle.Points.Add(new Point(point.X, point.Y));
            }
            return triangle;
        }

    }

}
