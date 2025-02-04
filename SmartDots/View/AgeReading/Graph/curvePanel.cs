﻿using System;
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
            BackColor = Color.FromArgb(255, 255, 255);

            //graphViewer.SetPlan(plan);

            m_MouseLocationTracker = graphViewer.CreateTracker(Color.Green);
            m_Hint = graphViewer.CreateFloatingHint();
            graphViewer.SetPlan(plan);
            graphViewer.SetAnnotations(control.AgeReadingViewModel.AgeReadingAnnotationViewModel?.SelectedAnnotations);
        }

        public int Plan
        {
            get { return plan; }
            set { plan = value; }
        }

        public void MakeGraph()
        {
            var annotationsWithValues = new List<Tuple<Annotation, List<decimal>>>();
            graphViewer.ResetGraphs();
            switch (plan)
            {
                case 1:
                    annotationsWithValues = CalculateLineBrightness();
                    break;
                case 2:
                    annotationsWithValues = CalculateLineReddness();
                    break;
                case 3:
                    annotationsWithValues = CalculateLineGrowth();
                    break;

            }

            foreach (var keyvaluepair in annotationsWithValues)
            {
                if (keyvaluepair.Item1.CombinedLines.Any() &&
                    keyvaluepair.Item1.CombinedLines[0].Lines.Any())
                {
                    var g = Graph.IterateCombinedLine(keyvaluepair.Item2);
                    g.Annotation = keyvaluepair.Item1;
                    var displayedGraph = graphViewer.AddGraph(g, keyvaluepair.Item1.CombinedLines[0].Lines[0].SystemColor, 2);
                    displayedGraph.DefaultPointMarkingStyle = GraphViewer.DisplayedGraph.PointMarkingStyle.Circle;
                }
                
            }

            //use the color of the line
            Color graphColor = Color.FromArgb(0, 114, 198);

            if(annotationsWithValues.Count == 1 && annotationsWithValues[0].Item1.CombinedLines[0].Lines.Any())
            {
                graphColor = annotationsWithValues[0].Item1.CombinedLines[0].Lines[0].SystemColor;
            }

            var brightness = (graphColor.R * 0.299f + graphColor.G * 0.587f + graphColor.B * 0.114f) / 256f;
            if (brightness > 0.5)
            {
                graphViewer.BackColor = Color.FromArgb(20, 20, 20);
            }
            else
            {
                graphViewer.BackColor = Color.FromArgb(255, 255, 255);
            }

            graphViewer.Invalidate();

        }


        GraphViewer.DisplayedGraph.DisplayedPoint m_HighlightedPoint;

        private void graphViewer_MouseMove(InteractiveGraphViewer sender, GraphMouseEventArgs e)
        {
            //if (m_MouseLocationTracker.X != null)
            //{
            //    for (int i = (int)m_MouseLocationTracker.X; i < e.DataX; i++)
            //    {
            //        d.Location = new Point((int)(combinedLine.ActualPoints[Math.Abs(i)].X - box.dotsize / 2 * box.ZoomPercentage), (int)(combinedLine.ActualPoints[Math.Abs(i)].Y - box.dotsize / 2 * box.ZoomPercentage));
            //        d.Height = (int) (box.dotsize*box.ZoomPercentage);
            //        d.Width = (int)(box.dotsize * box.ZoomPercentage);

            //    }
            //}

            if (graphViewer.PreviewRectangle.Visible)
            {
                graphViewer.PreviewRectangle.X2 = e.DataX;
                graphViewer.PreviewRectangle.Y2 = e.DataY;
            }


            if (control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine != null || plan == 3)
            {
                m_MouseLocationTracker.X = e.DataX;
                m_MouseLocationTracker.Y = e.DataY;
                m_MouseLocationTracker.Hidden = false;
                if (plan != 3 && control.AgeReadingViewModel.AgeReadingEditorViewModel?.ActiveCombinedLine?.Lines?.Count > 0)
                {
                    

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

                            x = control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points[index].X * (int)control.AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor;
                            y = control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points[index].Y * (int)control.AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor;
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    control.AgeReadingViewModel.AgeReadingEditorView.Tracker.SetLeft(x * control.AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor - 16);
                    control.AgeReadingViewModel.AgeReadingEditorView.Tracker.SetTop(y * control.AgeReadingViewModel.AgeReadingStatusbarViewModel.ZoomFactor - 16);

                    if (plan != 3) control.AgeReadingViewModel.AgeReadingEditorView.Tracker.Visibility = Visibility.Visible;

                    m_Hint.Hidden = true;
                    if (m_HighlightedPoint != null)
                    {
                        m_HighlightedPoint.MarkerStyle = GraphViewer.DisplayedGraph.PointMarkingStyle.Square;    //Default
                    }

                    int distanceSquare;
                    var point = graphViewer.FindNearestGraphPoint(e.DataX, e.DataY, false, out distanceSquare);
                    if (point != null)
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
                            //case 3:
                            //    {
                            //        m_Hint.Show(e, string.Format("{1}Age = {0:f0}", m_HighlightedPoint.X, point.Graph.Hint));
                            //        break;
                            //    }

                        }
                    }
                    //else
                    //{
                    //    if (point != null)
                    //    {
                    //        m_Hint.FillColor = Color.FromArgb(200, Color.LightBlue);
                    //        switch (plan)
                    //        {
                    //            case 1:
                    //                {
                    //                    m_Hint.Show(e, string.Format("{1}Brightness (interpolated) = {0:f2}", point.Y, point.Graph.Hint));
                    //                    break;
                    //                }
                    //            case 2:
                    //                {
                    //                    m_Hint.Show(e, string.Format("{1}Redness (interpolated) = {0:f2}", point.Y, point.Graph.Hint));
                    //                    break;
                    //                }
                    //        }
                    //    }
                    //}
                    
                }
                else if(plan == 3) // plan == 3
                {
                    if(graphViewer.DisplayedGraphs.Count() > 1)
                    {
                        return;
                    }
                    m_Hint.Hidden = true;
                    int distanceSquare;
                    var point = graphViewer.FindNearestGraphPoint(e.DataX, e.DataY, false, out distanceSquare);

                    if (point != null)
                    {
                        m_Hint.FillColor = Color.FromArgb(200, Color.LightGreen);
                        point.NearestReferencePoint.MarkerStyle = GraphViewer.DisplayedGraph.PointMarkingStyle.Square;
                        GraphViewer.DisplayedGraph.DisplayedPoint p = graphViewer.DisplayedGraphs.FirstOrDefault().FindPoint(e.DataX, e.DataY,25);

                        m_Hint.FillColor = Color.FromArgb(200, Color.LightGreen);
                        if(p != null)
                        {
                            m_Hint.Show(e, string.Format("{1}Age = {0:f0}", p.X, point.Graph?.Hint));

                        }
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

                if (control.AgeReadingViewModel.AgeReadingSampleViewModel.Sample == null || control.AgeReadingViewModel.AgeReadingFileViewModel.SelectedFile.IsReadOnly) return;
                if (e.Button == MouseButtons.Left && (ModifierKeys & Keys.Control) == Keys.None && control.AgeReadingViewModel.AgeReadingEditorViewModel.Mode == EditorModeEnum.DrawDot)
                {
                    if (control.AgeReadingViewModel.AgeReadingEditorViewModel.CanDrawDot)
                    {
                        Dot d = new Dot()
                        {
                            ID = Guid.NewGuid(),
                            X = control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points[(int)m_MouseLocationTracker.X].X,
                            Y = control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine.Points[(int)m_MouseLocationTracker.X].Y,
                            Width = (int)control.AgeReadingViewModel.AgeReadingEditorViewModel.DotWidth,
                            Color = control.AgeReadingViewModel.AgeReadingEditorViewModel.DotColor.ToString(),
                            ParentCombinedLine = control.AgeReadingViewModel.AgeReadingEditorViewModel.ActiveCombinedLine,
                            AnnotationID = control.AgeReadingViewModel.AgeReadingAnnotationViewModel.WorkingAnnotation.ID,
                            DotShape = control.AgeReadingViewModel.AgeReadingEditorViewModel.DotShape,
                            DotType = control.AgeReadingViewModel.AgeReadingEditorViewModel.DotType
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

        private List<Tuple<Annotation, List<decimal>>> CalculateLineBrightness()
        {
            return control.AgeReadingViewModel.AgeReadingEditorViewModel.LineBrightness;
        }

        private List<Tuple<Annotation, List<decimal>>> CalculateLineReddness()
        {
            return control.AgeReadingViewModel.AgeReadingEditorViewModel.LineRedness;
        }

        private List<Tuple<Annotation, List<decimal>>> CalculateLineGrowth()
        {
            return control.AgeReadingViewModel.AgeReadingEditorViewModel.LineGrowth;
        }
    }
}
