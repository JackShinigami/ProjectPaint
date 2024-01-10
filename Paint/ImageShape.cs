using IShapeContract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Paint
{
    class ImageShape : IShape
    {
        public override string Name => "ImageShape";
        private BitmapImage bitmap;

        public ImageShape(BitmapImage bitmap)
        {
            this.bitmap = bitmap;
        }
        public override IShape Clone()
        {
            IShape clone = new ImageShape(bitmap);
            foreach (var point in this.Points)
            {
                clone.Points.Add(new Point(point.X, point.Y));
            }
            return clone;
        }

        public override UIElement Draw()
        {
            Point start = Points[0];
            Point end = Points[1];
            IShapeContract.CreateRightPointsForDraw.Change(ref start, ref end);
            double width = Math.Abs(end.X - start.X);
            double height = Math.Abs(end.Y - start.Y);
            Image image = new Image()
            {
                Width = width,
                Height = height,
                Source = bitmap,
                Stretch = Stretch.Fill
            };
            Canvas.SetLeft(image, start.X);
            Canvas.SetTop(image, start.Y);
            return image;
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
                if (words[0] != Name)
                    return null;
                Brush = (Brush)new BrushConverter().ConvertFromString(words[1]);
                Thickness = int.Parse(words[2]);
                string[] dashArrayString = words[3].Split(',');
                if (dashArrayString[0] != "")
                    foreach (var dash in dashArrayString)
                    {
                        DashArray.Add(double.Parse(dash));
                    }
                for (int i = 4; i < 8; i += 2)
                {
                    Points.Add(new Point(double.Parse(words[i]), double.Parse(words[i + 1])));
                }

                // the last words are the path to the image
                string uri = "";
                for (int i = 8; i < words.Length; i++)
                {
                    uri += words[i];
                    if (i != words.Length - 1)
                        uri += " ";
                }

                bitmap = new BitmapImage(new Uri(uri.Trim()));

                return this;

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
