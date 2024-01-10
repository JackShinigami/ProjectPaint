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
            } else
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

        public override string ToKleString(int index = 0)
        {
            string kleString = "";

            // add name
            kleString += Name + " ";
            // add brush
            kleString += Brush.ToString() + " ";
            // add thickness
            kleString += Thickness.ToString() + " ";
            // add dash array as comma separated string

            for (int i = 0; i < DashArray.Count; i++)
            {
                kleString += DashArray[i].ToString();
                if (i != DashArray.Count - 1)
                    kleString += ",";
            }

            kleString += " ";

            // add points
            foreach (var point in Points)
            {
                kleString += point.X.ToString() + " " + point.Y.ToString() + " ";
            }

            return kleString;
        }

        public override object? FromKleString(string kleString)
        {
            string[] kleStringArray = kleString.Split(' ');
            int index = 0;
            try
            {
                // get name
                if (kleStringArray[index++] != Name)
                {
                    return null;
                }
                // get brush
                BrushConverter brushConverter = new BrushConverter();
                Brush = (Brush)brushConverter.ConvertFromString(kleStringArray[index++]);

                // get thickness
                Thickness = int.Parse(kleStringArray[index++]);

                // get dash array
                string[] dashArrayString = kleStringArray[index++].Split(',');

                if (dashArrayString[0] != "")
                    foreach (var dash in dashArrayString)
                    {
                        DashArray.Add(double.Parse(dash));
                    }

                // get points
                while (index < 8)
                {
                    Points.Add(new Point(double.Parse(kleStringArray[index++]), double.Parse(kleStringArray[index++])));
                }

                if (Points.Count != 2)
                {
                    return null;
                }

                return this;


            }
            catch
            {
                return null;
            }

        }
    }

}
