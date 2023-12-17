using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint
{
    public class UndoRedoManager
    {
        private Stack<DataUndoRedo> undoStack;
        private Stack<DataUndoRedo> redoStack;
        public UndoRedoManager()
        {
            undoStack = new Stack<DataUndoRedo>();
            redoStack = new Stack<DataUndoRedo>();
        }
        public void AddUndoRedo(DataUndoRedo data)
        {
            undoStack.Push(data);
            redoStack.Clear();
        }

        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                DataUndoRedo data = undoStack.Pop();
                data.Undo();
                redoStack.Push(data);
            }
        }

        public void Redo()
        {
            if (redoStack.Count > 0)
            {
                DataUndoRedo data = redoStack.Pop();
                data.Redo();
                undoStack.Push(data);
            }
        }
    }
}
