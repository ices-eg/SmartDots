using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using SmartDots.View;
using Color = System.Drawing.Color;
using MessageBox = System.Windows.Forms.MessageBox;
using AgeReading.Graph;
using SmartDots.Helpers;
using SmartDots.Model;

namespace AgeReading.Graph
{
    public partial class curvePanel : UserControl
    {
        class GraphTemplate
        {
            public readonly Graph.MathFunction Func;
            string _Description;

            public override string ToString()
            {
                return _Description;
            }

            public GraphTemplate(string desc, Graph.MathFunction func)
            {
                Func = func;
                _Description = desc;
            }
        }

        public InteractiveGraphViewer.Tracker m_MouseLocationTracker;
        public InteractiveGraphViewer.FloatingHint m_Hint;

        public AgeReadingView control;
        public int plan;
        public Graph graph;

        public curvePanel() { }

        public curvePanel(AgeReadingView c, int p)
        {
            InitializeComponent();

            control = c;
            plan = p;
        }

        public void Create()
        {
            InitializeComponent();
            BackColor = Color.FromArgb(235, 235, 235);

            //graphViewer.SetPlan(plan);

            m_MouseLocationTracker = graphViewer.CreateTracker(Color.Green);
            m_Hint = graphViewer.CreateFloatingHint();
            graphViewer.SetPlan(plan);
            graphViewer.SetCombinedLine(control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine);
        }

        public int Plan
        {
            get { return plan; }
            set { plan = value; }
        }

        public void MakeGraph()
        {
            graphViewer.ResetGraphs();
            switch (plan)
            {
                case 1:
                    graph = Graph.IterateCombinedLine(CalculateLineBrightness());
                    break;
                case 2:
                    graph = Graph.IterateCombinedLine(CalculateLineReddness());
                    break;
                case 3:
                    graph = Graph.IterateCombinedLine(CalculateLineGrowth());
                    break;
                default:
                    graph = Graph.IterateCombinedLine(CalculateLineBrightness());
                    break;
            }


            //Ensure that we use a unique color for the graph
            //Color graphColor = graphColorProvider1.AllocateColor();

            Color graphColor = Color.FromArgb(0, 114, 198);

            //Set hint based on the function description

            //Add the graph (collection of (X,Y) points) to the viewer
            var displayedGraph = graphViewer.AddGraph(graph, graphColor, 3);

            displayedGraph.DefaultPointMarkingStyle = GraphViewer.DisplayedGraph.PointMarkingStyle.Circle;
            graphViewer.Invalidate();
        }



        GraphViewer.DisplayedGraph.DisplayedPoint m_HighlightedPoint;

        private void graphViewer_MouseMove(InteractiveGraphViewer sender, GraphMouseEventArgs e)
        {
            //What you need to do is store the position of the last point received, and draw an ellipse at every point between the last and the new one.

            //if (m_MouseLocationTracker.X != null)
            //{
            //    for (int i = (int)m_MouseLocationTracker.X; i < e.DataX; i++)
            //    {
            //        d.Location = new Point((int)(combinedLine.ActualPoints[Math.Abs(i)].X - box.dotsize / 2 * box.ZoomPercentage), (int)(combinedLine.ActualPoints[Math.Abs(i)].Y - box.dotsize / 2 * box.ZoomPercentage));
            //        d.Height = (int) (box.dotsize*box.ZoomPercentage);
            //        d.Width = (int)(box.dotsize * box.ZoomPercentage);

            //    }
            //}
            if (control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine != null)
            {
                if (control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Lines.Count > 0)
                {
                    m_MouseLocationTracker.X = e.DataX;
                    m_MouseLocationTracker.Y = e.DataY;
                    m_MouseLocationTracker.Hidden = false;

                    if (m_MouseLocationTracker.X > control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points.Count)
                    {
                        m_MouseLocationTracker.X = control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points.Count - 1;
                    }

                    int x = 0;
                    int y = 0;


                    if (plan != 3)
                    {
                        x = (int)(control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points[Math.Abs((int)m_MouseLocationTracker.X)].X /**  control.AgeReadingViewModel.AgeReadingEditorViewModel.ZoomPercentage*/);
                        y = (int)(control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points[Math.Abs((int)m_MouseLocationTracker.X)].Y /** control.AgeReadingViewModel.AgeReadingEditorViewModel.ZoomPercentage*/);


                    }
                    else
                    {
                        try
                        {
                            int age = (int)(m_MouseLocationTracker.X);
                            double interpolation = m_MouseLocationTracker.X - age;

                            int index1;
                            int index2;
                            if (age == 0)
                            {
                                index1 = 0;
                                index2 =
                                control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points.IndexOf(
                                    control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points.FirstOrDefault(
                                        p => p.Location == control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Dots[0].Location));
                            }
                            else
                            {
                                index1 =
                                control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points.IndexOf(
                                    control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points.FirstOrDefault(
                                        p => p.Location == control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Dots[age - 1].Location));

                                index2 =
                                control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points.IndexOf(
                                    control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points.FirstOrDefault(
                                        p => p.Location == control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Dots[age].Location));
                            }




                            int index = (int)(index1 + (index2 - index1) * interpolation);

                            x = (int)(control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points[index].X * (int)control.AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomPercentage);
                            y = (int)(control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points[index].Y * (int)control.AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomPercentage);
                        }
                        catch (Exception)
                        {

                        }
                    }

                    control.AgeReadingViewModel.AgeReadingEditorView.Tracker.SetLeft(x * control.AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor - 16);
                    control.AgeReadingViewModel.AgeReadingEditorView.Tracker.SetTop(y * control.AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor - 16);

                    if(plan != 3) control.AgeReadingViewModel.AgeReadingEditorView.Tracker.Visibility = Visibility.Visible;

                    m_Hint.Hidden = true;
                    if (m_HighlightedPoint != null)
                    {
                        m_HighlightedPoint.MarkerStyle = GraphViewer.DisplayedGraph.PointMarkingStyle.Square;    //Default
                    }

                    int distanceSquare;
                    var point = graphViewer.FindNearestGraphPoint(e.DataX, e.DataY, false, out distanceSquare);
                    if (point != null && distanceSquare <= 400)
                    {
                        m_HighlightedPoint = point.NearestReferencePoint;
                        point.NearestReferencePoint.MarkerStyle = GraphViewer.DisplayedGraph.PointMarkingStyle.Square;

                        m_Hint.FillColor = Color.FromArgb(200, Color.LightGreen);
                        switch (plan)
                        {
                            case 1:
                                {
                                    m_Hint.Show(e, string.Format("{1}Brightness = {0:f2}", m_HighlightedPoint.Y, point.Graph.Hint));
                                    break;
                                }
                            case 2:
                                {
                                    m_Hint.Show(e, string.Format("{1}Redness = {0:f2}", m_HighlightedPoint.Y, point.Graph.Hint));
                                    break;
                                }
                            case 3:
                                {
                                    m_Hint.Show(e, string.Format("{1}Age = {0:f0}", m_HighlightedPoint.X, point.Graph.Hint));
                                    break;
                                }

                        }
                    }
                    else
                    {
                        if (point != null)
                        {
                            m_Hint.FillColor = Color.FromArgb(200, Color.LightBlue);
                            switch (plan)
                            {
                                case 1:
                                    {
                                        m_Hint.Show(e, string.Format("{1}Brightness (interpolated) = {0:f2}", point.Y, point.Graph.Hint));
                                        break;
                                    }
                                case 2:
                                    {
                                        m_Hint.Show(e, string.Format("{1}Redness (interpolated) = {0:f2}", point.Y, point.Graph.Hint));
                                        break;
                                    }
                                case 3:
                                    {
                                        m_Hint.Show(e, string.Format("{1}Age (interpolated) = {0:f0}", point.X, point.Graph.Hint));
                                        break;
                                    }
                            }
                        }
                    }
                    if (graphViewer.PreviewRectangle.Visible)
                    {
                        graphViewer.PreviewRectangle.X2 = e.DataX;
                        graphViewer.PreviewRectangle.Y2 = e.DataY;
                    }
                }
            }




        }

        private void graphViewer_MouseClick(InteractiveGraphViewer sender, GraphMouseEventArgs e)
        {
            control.AgeReadingViewModel.AgeReadingEditorViewModel.CalculateAge();
        }

        private void graphViewer_MouseDown(InteractiveGraphViewer sender, GraphMouseEventArgs e)
        {
            

                if (control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine != null && control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Lines.Count > 0 && Plan != 3)
                {
                if ((ModifierKeys & Keys.Control) != Keys.None)
                {
                    graphViewer.PreviewRectangle.X1 = e.DataX;
                    graphViewer.PreviewRectangle.Y1 = e.DataY;
                    graphViewer.PreviewRectangle.X2 = e.DataX;
                    graphViewer.PreviewRectangle.Y2 = e.DataY;
                    graphViewer.PreviewRectangle.Visible = true;
                }
                //if (e.Button == MouseButtons.Middle)
                //{
                //    graphViewer.CreateOrCloseFullscreenClone();
                //}

                if (control.AgeReadingViewModel.AgeReadingSampleViewModel.Sample == null || control.AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly ) return;
                if (e.Button == MouseButtons.Left && (ModifierKeys & Keys.Control) == Keys.None && control.AgeReadingViewModel.AgeReadingEditorViewModel.Mode == EditorModeEnum.DrawDot)
                {
                    if (control.AgeReadingViewModel.AgeReadingEditorViewModel.CanDrawDot)
                    {
                        Dot d = new Dot()
                        {
                            ID =  Guid.NewGuid(),
                            X = control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points[(int)m_MouseLocationTracker.X].X,
                            Y =control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points[(int)m_MouseLocationTracker.X].Y,
                            Width = (int) control.AgeReadingViewModel.AgeReadingEditorViewModel.DotWidth,
                            Color = control.AgeReadingViewModel.AgeReadingEditorViewModel.DotColor.ToString(),
                            ParentCombinedLine = control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine,
                            AnnotationID = control.AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.ID,
                            DotShape = control.AgeReadingViewModel.AgeReadingEditorViewModel.DotShape
                        };
                        control.AgeReadingViewModel.AgeReadingEditorViewModel.AddDot(d.ParentCombinedLine, d);
                        control.AgeReadingViewModel.AgeReadingEditorViewModel.UndoRedo.InsertInUnDoRedoForAddDot(d, d.ParentCombinedLine, control.AgeReadingViewModel.AgeReadingEditorViewModel);
                        //control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.AddDot(new Dot() { X = control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points[(int)m_MouseLocationTracker.X].X, Y = control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points[(int)m_MouseLocationTracker.X].Y, Width = control.AgeReadingViewModel.AgeReadingEditorViewModel.DotWidth, Color = new SolidColorBrush(control.AgeReadingViewModel.AgeReadingEditorViewModel.DotColor), ParentLine = control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine }); //todo

                        //box.canvas.Shapes.Clear();
                        graphViewer.Invalidate();
                    }
                    
                }
            }
        }

        private void graphViewer_MouseUp(InteractiveGraphViewer sender, GraphMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if ((e.Button == MouseButtons.Left && graphViewer.PreviewRectangle.Visible))
                {
                    graphViewer.ForceNewBounds(graphViewer.PreviewRectangle.MinX, graphViewer.PreviewRectangle.MaxX,
                        graphViewer.PreviewRectangle.MinY, graphViewer.PreviewRectangle.MaxY);

                }
                graphViewer.PreviewRectangle.Visible = false;
            }
            else
            {
                graphViewer.ForceCustomBounds = false;
            }

        }

        private void graphViewer_MouseLeave(object sender, EventArgs e)
        {
            m_MouseLocationTracker.Hidden = true;
            if (m_HighlightedPoint != null)
                m_HighlightedPoint.MarkerStyle = GraphViewer.DisplayedGraph.PointMarkingStyle.Undefined;    //Default
            m_Hint.Hidden = true;
            control.AgeReadingViewModel.AgeReadingEditorView.Tracker.Visibility = Visibility.Hidden;
        }

        private List<decimal> CalculateLineBrightness()
        {
            return control.AgeReadingViewModel.AgeReadingEditorViewModel.LineBrightness;
        }

        private List<decimal> CalculateLineReddness()
        {
            return control.AgeReadingViewModel.AgeReadingEditorViewModel.LineRedness;
        }

        private List<decimal> CalculateLineGrowth()
        {
            return control.AgeReadingViewModel.AgeReadingEditorViewModel.LineGrowth;
        }
    }
}
