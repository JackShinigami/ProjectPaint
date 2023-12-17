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
    }
}
