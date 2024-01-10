
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using IShapeContract;
using System.Drawing;

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
                line.Points.Add(new System.Windows.Point(point.X, point.Y));
            }
            return line;
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
                string[] words = kleString.Split(' ');

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
                    Points.Add(new System.Windows.Point(double.Parse(words[i]), double.Parse(words[i + 1])));
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

        public override string Name => "Line";

    }

}
