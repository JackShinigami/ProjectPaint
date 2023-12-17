using IShapeContract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace Paint
{
    public class Layer : INotifyPropertyChanged
    {
        public Canvas _canvas { get; set; }
        public BitmapSource LayerImage { get; set; }
        public List<IShape> Shapes { get; set; }
        public bool IsVisible { get; set; } = true;

        public event PropertyChangedEventHandler? PropertyChanged;

        public BitmapSource GetBitmap()
        {
            _canvas.Visibility = Visibility.Visible;
            System.Windows.Size size = new System.Windows.Size(_canvas.ActualWidth, _canvas.ActualHeight);
            if (size.IsEmpty)
                return null;

            RenderTargetBitmap result = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual drawingvisual = new DrawingVisual();
            using (DrawingContext context = drawingvisual.RenderOpen())
            {
                context.DrawRectangle(new VisualBrush(_canvas), null, new Rect(new System.Windows.Point(), size));
            }
            result.Render(drawingvisual);
            LayerImage = result;
            _canvas.Visibility = Visibility.Hidden;
            return result;
        }
        public void RemoveShape(IShape shape)
        {
            Shapes.Remove(shape);
            _canvas.Children.Remove(shape.Draw());
            GetBitmap();
        }
        public void AddShape(IShape shape)
        {
            Shapes.Add(shape);
            _canvas.Children.Add(shape.Draw());
            GetBitmap();
        }

        public void AddShape(int index, IShape shape)
        {
            Shapes.Insert(index, shape);
            _canvas.Children.Insert(index, shape.Draw());
            GetBitmap();
        }

        public void RemoveShape(int index)
        {
            Shapes.RemoveAt(index);
            _canvas.Children.RemoveAt(index);
            GetBitmap();
        }
        
        public void ChangeShape(int index, IShape shape)
        {
            Shapes[index] = shape;
            _canvas.Children.RemoveAt(index);
            _canvas.Children.Insert(index, shape.Draw());
            GetBitmap();
        }
    }
}
