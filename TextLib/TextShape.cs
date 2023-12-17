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

        private string text="";

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
                FontSize = Thickness*10,
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
    }
}
