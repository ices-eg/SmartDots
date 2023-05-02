using System.Collections.Generic;
using SmartDots.Model;
using SmartDots.ViewModel;
using Line = System.Windows.Shapes.Line;

namespace SmartDots.Helpers
{

    public class LarvaeUndoRedo
    {
        private LarvaeEditorViewModel editor;

        public Stack<IUndoRedoCommand> _Undocommands = new Stack<IUndoRedoCommand>();
        public Stack<IUndoRedoCommand> _Redocommands = new Stack<IUndoRedoCommand>();

        public LarvaeUndoRedo(LarvaeEditorViewModel e)
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
            editor.UpdateButtons();
        }

        #region UndoHelperFunctions

        public void InsertInUnDoRedoForAddLine(List<LarvaeLine> l, LarvaeEditorViewModel editor)
        {
            IUndoRedoCommand cmd = new AddLarvaeLineCommand(new List<LarvaeLine>(l), editor);
            _Undocommands.Push(cmd);
            _Redocommands.Clear();
            CheckStacks();
        }

        public void InsertInUnDoRedoForAddDot(LarvaeDot d, LarvaeEditorViewModel editor)
        {
            IUndoRedoCommand cmd = new AddLarvaeDotCommand(d, editor);
            _Undocommands.Push(cmd);
            _Redocommands.Clear();
            CheckStacks();
        }

        public void InsertInUnDoRedoForAddCircle(LarvaeCircle c, LarvaeEditorViewModel editor)
        {
            IUndoRedoCommand cmd = new AddLarvaeCircleCommand(c, editor);
            _Undocommands.Push(cmd);
            _Redocommands.Clear();
            CheckStacks();
        }

        public void InsertInUnDoRedoForAddMeasure(System.Windows.Shapes.Line l, LarvaeEditorViewModel editor)
        {
            IUndoRedoCommand cmd = new AddLarvaeMeasureCommand(l, editor);
            _Undocommands.Push(cmd);
            _Redocommands.Clear();
            CheckStacks();
        }

        public void InsertInUnDoRedoForDelete(List<LarvaeLine> l, LarvaeEditorViewModel editor)
        {
            IUndoRedoCommand cmd = new DeleteLarvaeLineCommand(l, editor);
            _Undocommands.Push(cmd);
            _Redocommands.Clear();
            CheckStacks();
        }

        public void InsertInUnDoRedoForDeleteDot(LarvaeDot d, LarvaeEditorViewModel editor)
        {
            IUndoRedoCommand cmd = new DeleteLarvaeDotCommand(d, editor);
            _Undocommands.Push(cmd);
            _Redocommands.Clear();
            CheckStacks();
        }

        public void InsertInUnDoRedoForDeleteCircle(LarvaeCircle c, LarvaeEditorViewModel editor)
        {
            IUndoRedoCommand cmd = new DeleteLarvaeCircleCommand(c, editor);
            _Undocommands.Push(cmd);
            _Redocommands.Clear();
            CheckStacks();
        }

        public void InsertInUnDoRedoForDeleteMeasure(List<System.Windows.Shapes.Line> l, LarvaeEditorViewModel editor)
        {
            IUndoRedoCommand cmd = new DeleteLarvaeMeasureCommand(l, editor);
            _Undocommands.Push(cmd);
            _Redocommands.Clear();
            CheckStacks();
        }
        #endregion
    }

    class AddLarvaeLineCommand : IUndoRedoCommand
    {
        private List<LarvaeLine> lines;
        private LarvaeEditorViewModel editor;

        public AddLarvaeLineCommand(List<LarvaeLine> l, LarvaeEditorViewModel e)
        {
            lines = l;
            editor = e;
        }

        #region ICommand Members

        public void Execute()
        {
            
            editor.AddLine(new List<LarvaeLine>(lines));
            
        }

        public void UnExecute()
        {
            editor.RemoveLine(new List<LarvaeLine>(lines));

            //if (editor.LarvaeViewModel.LarvaeOwnAnnotationViewModel.WorkingAnnotation.CombinedLines.Find(x => x.Equals(combinedLine)).Lines.Count == 0)
            //{
            //    editor.LarvaeViewModel.LarvaeOwnAnnotationViewModel.WorkingAnnotation.CombinedLines.Remove(combinedLine);
            //    editor.ActiveCombinedLine = null;
            //}
        }
        #endregion
    }

    class AddLarvaeDotCommand : IUndoRedoCommand
    {
        private LarvaeDot dot;
        private LarvaeEditorViewModel editor;

        public AddLarvaeDotCommand(LarvaeDot d, LarvaeEditorViewModel e)
        {
            dot = d;
            editor = e;
        }

        #region ICommand Members

        public void Execute()
        {
            editor.AddDot(dot);
        }

        public void UnExecute()
        {
            editor.RemoveDot(dot);
        }

        #endregion
    }

    class AddLarvaeCircleCommand : IUndoRedoCommand
    {
        private LarvaeCircle circle;
        private LarvaeEditorViewModel editor;

        public AddLarvaeCircleCommand(LarvaeCircle c, LarvaeEditorViewModel e)
        {
            circle = c;
            editor = e;
        }

        #region ICommand Members

        public void Execute()
        {
            editor.AddCircle(circle);
        }

        public void UnExecute()
        {
            editor.RemoveCircle(circle);
        }
        #endregion
    }


    class AddLarvaeMeasureCommand : IUndoRedoCommand
    {
        private System.Windows.Shapes.Line line;
        private LarvaeEditorViewModel editor;

        public AddLarvaeMeasureCommand(System.Windows.Shapes.Line l, LarvaeEditorViewModel e)
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

    class DeleteLarvaeLineCommand : IUndoRedoCommand
    {
        private List<LarvaeLine> lines;
        private LarvaeEditorViewModel editor;

        public DeleteLarvaeLineCommand(List<LarvaeLine> l, LarvaeEditorViewModel e)
        {
            lines = l;
            editor = e;
        }

        #region ICommand Members

        public void Execute()
        {
            editor.RemoveLine(new List<LarvaeLine>(lines));
            //editor.LarvaeViewModel.LarvaeAnnotationViewModel.WorkingAnnotation.CombinedLines.Remove(combinedLine);
            //editor.ActiveCombinedLine = null;
            //editor.LarvaeViewModel.LarvaeAnnotationViewModel.WorkingAnnotation.IsChanged = true;
            //editor.RefreshShapes();
        }

        public void UnExecute()
        {
            editor.AddLine(new List<LarvaeLine>(lines));
            //editor.LarvaeViewModel.LarvaeAnnotationViewModel.WorkingAnnotation.CombinedLines.Add(combinedLine);
            //editor.ActiveCombinedLine = combinedLine;
            //editor.LarvaeViewModel.LarvaeAnnotationViewModel.WorkingAnnotation.IsChanged = true;
            //editor.RefreshShapes();
        }
        #endregion
    }

    class DeleteLarvaeDotCommand : IUndoRedoCommand
    {
        private LarvaeDot dot;
        private LarvaeEditorViewModel editor;

        public DeleteLarvaeDotCommand(LarvaeDot d, LarvaeEditorViewModel e)
        {
            dot = d;
            editor = e;
        }

        #region ICommand Members

        public void Execute()
        {
            editor.RemoveDot(dot);

            //dot.ParentCombinedLine.Dots.Remove(dot);
            //editor.ActiveCombinedLine = dot.ParentCombinedLine;
            //editor.LarvaeViewModel.LarvaeAnnotationViewModel.WorkingAnnotation.IsChanged = true;
            //editor.RefreshShapes();
        }

        public void UnExecute()
        {
            editor.AddDot(dot);

            //dot.ParentCombinedLine.Dots.Add(dot);
            //editor.ActiveCombinedLine = dot.ParentCombinedLine;
            //editor.LarvaeViewModel.LarvaeAnnotationViewModel.WorkingAnnotation.IsChanged = true;
            //editor.RefreshShapes();
        }
        #endregion
    }

    class DeleteLarvaeCircleCommand : IUndoRedoCommand
    {
        private LarvaeCircle circle;
        private LarvaeEditorViewModel editor;

        public DeleteLarvaeCircleCommand(LarvaeCircle c, LarvaeEditorViewModel e)
        {
            circle = c;
            editor = e;
        }

        #region ICommand Members

        public void Execute()
        {
            editor.RemoveCircle(circle);
        }

        public void UnExecute()
        {
            editor.AddCircle(circle);
        }
        #endregion
    }

    class DeleteLarvaeMeasureCommand : IUndoRedoCommand
    {
        private List<System.Windows.Shapes.Line> lines;
        private LarvaeEditorViewModel editor;

        public DeleteLarvaeMeasureCommand(List<System.Windows.Shapes.Line> l, LarvaeEditorViewModel e)
        {
            lines = l;
            editor = e;
        }

        #region ICommand Members

        public void Execute()
        {
            foreach (var line in lines)
            {
                editor.LarvaeViewModel.LarvaeEditorViewModel.OriginalMeasureShapes.Remove(line);
            }
            
            editor.RefreshMeasures();
        }

        public void UnExecute()
        {
            foreach (var line in lines)
            {
                editor.LarvaeViewModel.LarvaeEditorViewModel.OriginalMeasureShapes.Add(line);
            }
            
            editor.RefreshMeasures();
        }
        #endregion
    }
}
