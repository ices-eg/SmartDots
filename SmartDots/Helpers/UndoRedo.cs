using System.Collections.Generic;
using SmartDots.Model;
using SmartDots.ViewModel;

namespace SmartDots.Helpers
{
    public interface IUndoRedoCommand
    {
        void Execute();
        void UnExecute();
    }

    public class UndoRedo
    {
        private AgeReadingEditorViewModel editor;

        private Stack<IUndoRedoCommand> _Undocommands = new Stack<IUndoRedoCommand>();
        private Stack<IUndoRedoCommand> _Redocommands = new Stack<IUndoRedoCommand>();

        public UndoRedo(AgeReadingEditorViewModel e)
        {
            editor = e;
        }

        public void Redo()
        {
            if (_Redocommands.Count != 0)
            {
                IUndoRedoCommand command = _Redocommands.Pop();
                command.Execute();
                _Undocommands.Push(command);
                CheckStacks();
                editor.RefreshShapes();
                editor.ShapeChangeFlag = true;
            }
        }

        public void Undo()
        {
            if (_Undocommands.Count != 0)
            {
                IUndoRedoCommand command = _Undocommands.Pop();
                command.UnExecute();
                _Redocommands.Push(command);
                CheckStacks();
                editor.RefreshShapes();
                editor.ShapeChangeFlag = true;
            }
        }

        public void EmptyStacks()
        {
            _Undocommands.Clear();
            _Redocommands.Clear();
            CheckStacks();
        }

        private void CheckStacks()
        {
            editor.CanUndo = _Undocommands.Count > 0;
            editor.CanRedo = _Redocommands.Count > 0;
        }

        #region UndoHelperFunctions

        public void InsertInUnDoRedoForAddLine(Line l, CombinedLine cl, AgeReadingEditorViewModel editor)
        {
            IUndoRedoCommand cmd = new AddLineCommand(l, cl, editor);
            _Undocommands.Push(cmd);
            _Redocommands.Clear();
            CheckStacks();
        }

        public void InsertInUnDoRedoForAddDot(Dot d, CombinedLine cl, AgeReadingEditorViewModel editor)
        {
            IUndoRedoCommand cmd = new AddDotCommand(d, cl, editor);
            _Undocommands.Push(cmd);
            _Redocommands.Clear();
            CheckStacks();
        }

        public void InsertInUnDoRedoForAddMeasure(System.Windows.Shapes.Line l, AgeReadingEditorViewModel editor)
        {
            IUndoRedoCommand cmd = new AddMeasureCommand(l, editor);
            _Undocommands.Push(cmd);
            _Redocommands.Clear();
            CheckStacks();
        }

        public void InsertInUnDoRedoForDelete(CombinedLine cl, AgeReadingEditorViewModel editor)
        {
            IUndoRedoCommand cmd = new DeleteCommand(cl, editor);
            _Undocommands.Push(cmd);
            _Redocommands.Clear();
            CheckStacks();
        }

        public void InsertInUnDoRedoForDelete(System.Windows.Shapes.Line l, AgeReadingEditorViewModel editor)
        {
            IUndoRedoCommand cmd = new DeleteMeasureCommand(l, editor);
            _Undocommands.Push(cmd);
            _Redocommands.Clear();
            CheckStacks();
        }

        public void InsertInUnDoRedoForDeleteDot(Dot d, AgeReadingEditorViewModel editor)
        {
            IUndoRedoCommand cmd = new DeleteDotCommand(d, editor);
            _Undocommands.Push(cmd);
            _Redocommands.Clear();
            CheckStacks();
        }

        public void InsertInUnDoRedoForDeleteMeasure(System.Windows.Shapes.Line l, AgeReadingEditorViewModel editor)
        {
            IUndoRedoCommand cmd = new DeleteMeasureCommand(l, editor);
            _Undocommands.Push(cmd);
            _Redocommands.Clear();
            CheckStacks();
        }
        #endregion
    }

    class AddLineCommand : IUndoRedoCommand
    {
        private Line line;
        private CombinedLine combinedLine;
        private AgeReadingEditorViewModel editor;

        public AddLineCommand(Line l, CombinedLine cl, AgeReadingEditorViewModel e)
        {
            line = l;
            combinedLine = cl;
            editor = e;
        }

        #region ICommand Members

        public void Execute()
        {
            if (editor.AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Find(x => x.Equals(combinedLine)) == null)
            {
                editor.AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Add(combinedLine);
            }
            editor.AddLine(line);
        }

        public void UnExecute()
        {
            editor.RemoveLine(line);

            if (editor.AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Find(x => x.Equals(combinedLine)).Lines.Count == 0)
            {
                editor.AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Remove(combinedLine);
                editor.ActiveCombinedLine = null;
            }
        }
        #endregion
    }

    class AddDotCommand : IUndoRedoCommand
    {
        private Dot dot;
        private CombinedLine combinedLine;
        private AgeReadingEditorViewModel editor;

        public AddDotCommand(Dot d, CombinedLine cl, AgeReadingEditorViewModel e)
        {
            dot = d;
            combinedLine = cl;
            editor = e;
        }

        #region ICommand Members

        public void Execute()
        {
            editor.AddDot(combinedLine, dot);
        }

        public void UnExecute()
        {
            editor.RemoveDot(combinedLine, dot);
        }

        #endregion
    }

    class AddMeasureCommand : IUndoRedoCommand
    {
        private System.Windows.Shapes.Line line;
        private AgeReadingEditorViewModel editor;

        public AddMeasureCommand(System.Windows.Shapes.Line l, AgeReadingEditorViewModel e)
        {
            line = l;
            editor = e;
        }

        #region ICommand Members

        public void Execute()
        {
            editor.OriginalMeasureShapes.Add(line);
            editor.RefreshShapes();
        }

        public void UnExecute()
        {
            editor.OriginalMeasureShapes.Remove(line);
            editor.RefreshShapes();
        }

        #endregion
    }

    class DeleteCommand : IUndoRedoCommand
    {
        private CombinedLine combinedLine;
        private AgeReadingEditorViewModel editor;

        public DeleteCommand(CombinedLine cl, AgeReadingEditorViewModel e)
        {
            combinedLine = cl;
            editor = e;
        }

        #region ICommand Members

        public void Execute()
        {
            editor.AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Remove(combinedLine);
            editor.ActiveCombinedLine = null;
            editor.AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsChanged = true;
            editor.RefreshShapes();
        }

        public void UnExecute()
        {
            editor.AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.CombinedLines.Add(combinedLine);
            editor.ActiveCombinedLine = combinedLine;
            editor.AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsChanged = true;
            editor.RefreshShapes();
        }
        #endregion
    }

    class DeleteDotCommand : IUndoRedoCommand
    {
        private Dot dot;
        private AgeReadingEditorViewModel editor;

        public DeleteDotCommand(Dot d, AgeReadingEditorViewModel e)
        {
            dot = d;
            editor = e;
        }

        #region ICommand Members

        public void Execute()
        {
            dot.ParentCombinedLine.Dots.Remove(dot);
            editor.ActiveCombinedLine = dot.ParentCombinedLine;
            editor.AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsChanged = true;
            editor.RefreshShapes();
        }

        public void UnExecute()
        {
            dot.ParentCombinedLine.Dots.Add(dot);
            editor.ActiveCombinedLine = dot.ParentCombinedLine;
            editor.AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.IsChanged = true;
            editor.RefreshShapes();
        }
        #endregion
    }

    class DeleteMeasureCommand : IUndoRedoCommand
    {
        private System.Windows.Shapes.Line line;
        private AgeReadingEditorViewModel editor;

        public DeleteMeasureCommand(System.Windows.Shapes.Line l, AgeReadingEditorViewModel e)
        {
            line = l;
            editor = e;
        }

        #region ICommand Members

        public void Execute()
        {
            editor.AgeReadingViewModel.AgeReadingEditorViewModel.OriginalMeasureShapes.Remove(line);
            editor.RefreshMeasures();
        }

        public void UnExecute()
        {
            editor.AgeReadingViewModel.AgeReadingEditorViewModel.OriginalMeasureShapes.Add(line);
            editor.RefreshMeasures();
        }
        #endregion
    }
}
