using IShapeContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Paint
{
    public abstract class DataUndoRedo
    {
        public abstract void Undo();
        public abstract void Redo();
    }

    public class ShapeUndoRedo : DataUndoRedo
    {
        public Layer CurrentLayer { get; set; }
        public IShape NewShape { get; set; }
        public int IndexInLayer { get; set; }
        public Type TypeOfData { get; set; }
        public IShape OldShape { get; set; }

        public enum Type { Add, Remove, Move, Change }

        override public void Undo()
        {
            switch (TypeOfData)
            {
                case Type.Add:
                    CurrentLayer.RemoveShape(IndexInLayer);
                    break;
                case Type.Remove:
                    CurrentLayer.AddShape(IndexInLayer, OldShape.Clone());
                    break;
                case Type.Move:
                    CurrentLayer.ChangeShape(IndexInLayer, OldShape.Clone());
                    break;
                case Type.Change:
                    CurrentLayer.ChangeShape(IndexInLayer, OldShape.Clone());
                    break;
            }
        }

        override public void Redo()
        {
            switch (TypeOfData)
            {
                case Type.Add:
                    CurrentLayer.AddShape(IndexInLayer, NewShape.Clone());
                    break;
                case Type.Remove:
                    CurrentLayer.RemoveShape(IndexInLayer);
                    break;
                case Type.Move:
                    CurrentLayer.ChangeShape(IndexInLayer, NewShape.Clone());
                    break;
                case Type.Change:
                    CurrentLayer.ChangeShape(IndexInLayer, NewShape.Clone());
                    break;
            }
        }
    }

    public class LayerUndoRedo : DataUndoRedo
    {
        public Panel drawingLayout { get; set; }
        public LayerManager layerManager { get; set; }
        public Layer NewLayer { get; set; }
        public Layer OldLayer { get; set; }
        public int IndexInLayout { get; set; }
        public Type TypeOfData { get; set; }

        public enum Type { Add, Remove, MoveUp, MoveDown}
        

        override public void Undo()
        {
            switch(TypeOfData)
            {
                case Type.Add:
                    layerManager.RemoveLayer(NewLayer, drawingLayout);
                    break;
                case Type.Remove:
                    layerManager.AddLayer(OldLayer, drawingLayout, IndexInLayout);
                    break;
                case Type.MoveUp:
                    layerManager.MoveLayerDown(IndexInLayout);
                    break;
                case Type.MoveDown:
                    layerManager.MoveLayerUp(IndexInLayout);
                    break;
            }
        }

        override public void Redo()
        {
            switch (TypeOfData)
            {
                case Type.Add:
                    layerManager.AddLayer(NewLayer, drawingLayout, IndexInLayout);
                    break;
                case Type.Remove:
                    layerManager.RemoveLayer(OldLayer, drawingLayout);
                    break;
                case Type.MoveUp:
                    layerManager.MoveLayerUp(IndexInLayout - 1);
                    break;
                case Type.MoveDown:
                    layerManager.MoveLayerDown(IndexInLayout + 1);
                    break;
            }
        }
    }
}
