
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
            try
            {
                string[] words = kleString.Split(' ', StringSplitOptions.None);

                // check name
                if (words[0] != Name)
                    return null;

                // get brush
                Brush = (Brush)new BrushConverter().ConvertFromString(words[1]);

                // get thickness
                Thickness = int.Parse(words[2]);

                // get dash array
                DashArray = new DoubleCollection();
                string[] dashArrayString = words[3].Split(',');
                if (dashArrayString[0] != "")
                    foreach (var dash in dashArrayString)
                    {
                        DashArray.Add(double.Parse(dash));
                    }

                // get points
                for (int i = 4; i < 8; i += 2)
                {
                    Points.Add(new Point(double.Parse(words[i]), double.Parse(words[i + 1])));
                }

                if (Points.Count != 2)
                    return null;

                return this;

            }
            catch
            {
                return null;
            }
        }

        public override string Name => "Ellipse";
    }
}
