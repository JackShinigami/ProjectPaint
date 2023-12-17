using IShapeContract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Paint
{
    public class LayerManager
    {
        public BindingList<Layer> Layers { get; set; }
        

        public LayerManager()
        {
            Layers = new BindingList<Layer>();
        }

        public void AddLayer(double Width, double Height, Panel DrawingLayout)
        {
            Layer newLayer = new Layer()
            {
                _canvas = new Canvas()
                {
                    Background = Brushes.Transparent,
                    Visibility = Visibility.Hidden,
                    Width = Width,
                    Height = Height
                },
                Shapes = new List<IShape>(),
                IsVisible = true
            };

            DrawingLayout.Children.Add(newLayer._canvas);

            DrawingLayout.UpdateLayout();
            newLayer.GetBitmap();
            Layers.Add(newLayer);
        }

        public void AddLayer(Layer layer, Panel DrawingLayout, int layerIndex)
        {
            DrawingLayout.Children.Insert(layerIndex, layer._canvas);
            DrawingLayout.UpdateLayout();
            Layers.Insert(layerIndex, layer);
        }

        public void RemoveLayer(Layer layer, Panel DrawingLayout)
        {
            if (Layers.Count == 1)
                return;

            Layers.Remove(layer);
            DrawingLayout.Children.Remove(layer._canvas);
            DrawingLayout.UpdateLayout();
        }

        public void MoveLayerUp(int layerIndex)
        {
            if (layerIndex < Layers.Count - 1)
            {
                Layer temp = Layers[layerIndex];
                int tempIndex = layerIndex;
                Layers.Remove(temp);
                Layers.Insert(tempIndex + 1, temp);
            }
        }

        public void MoveLayerDown(int layerIndex)
        {
            if (layerIndex > 0)
            {
                Layer temp = Layers[layerIndex];
                int tempIndex = layerIndex;
                Layers.Remove(temp);
                Layers.Insert(tempIndex - 1, temp);
            }
        }
    }
}
