using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using IShapeContract;
namespace TextLib
{
    public class TextShape : IShape
    {
        public override string Name => "Text";

        public override IShape Clone()
        {
            TextShape textShape = new TextShape();
            textShape.Text = this.Text;
            textShape.Brush = this.Brush;
            textShape.Thickness = this.Thickness;
            textShape.DashArray = this.DashArray;
            foreach (var point in this.Points)
            {
                textShape.Points.Add(new Point(point.X, point.Y));
            }
            return textShape;
        }

        private string text = "";

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public override System.Windows.UIElement Draw()
        {
            Point start = Points[0];
            Point end = Points[1];
            IShapeContract.CreateRightPointsForDraw.Change(ref start, ref end);
            double width = Math.Abs(end.X - start.X);
            double height = Math.Abs(end.Y - start.Y);

            Brush borderBrush = Brushes.Gray;
            if (text != "")
                borderBrush = Brushes.Transparent;


            var element = new TextBox()
            {
                Width = width,
                Height = height,
                Background = Brushes.Transparent,
                Text = text,
                BorderBrush = borderBrush,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                FontSize = Thickness * 10,
                FontFamily = new FontFamily("Cambria"),
                Foreground = Brush,
            };
            element.TextChanged += (sender, e) =>
            {
                this.Text = element.Text;
                element.BorderBrush = Brushes.Transparent;
            };

            Canvas.SetLeft(element, start.X);
            Canvas.SetTop(element, start.Y);

            return element;
        }

        public override string ToKleString(int index = 0)
        {

            string kleString = "text";
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
            kleString += Text;
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
                    Points.Add(new Point(double.Parse(words[i]), double.Parse(words[i + 1])));
                }

                if (Points.Count != 2)
                    return null;

                // all words after 7 is text

                string text = "";
                for (int j = 8; j < words.Length; j++)
                {
                    text += words[j] + " ";
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
