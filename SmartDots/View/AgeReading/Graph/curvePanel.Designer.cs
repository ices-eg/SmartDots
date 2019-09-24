namespace AgeReading.Graph
{
    public partial class curvePanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            AgeReading.Graph.ScaledViewerBase.GridSettings gridSettings1 = new AgeReading.Graph.ScaledViewerBase.GridSettings();
            AgeReading.Graph.ScaledViewerBase.GridSettings gridSettings2 = new AgeReading.Graph.ScaledViewerBase.GridSettings();
            this.graphViewer = new AgeReading.Graph.InteractiveGraphViewer();
            this.SuspendLayout();
            // 
            // graphViewer
            // 
            this.graphViewer.AdditionalPadding = new System.Windows.Forms.Padding(0);
            this.graphViewer.DefaultXFormat = "{0:f2}";
            this.graphViewer.DefaultYFormat = "{0:f2}";
            this.graphViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphViewer.EmbeddedLegend = false;
            this.graphViewer.ForceCustomBounds = false;
            this.graphViewer.Legend = null;
            this.graphViewer.Location = new System.Drawing.Point(0, 0);
            this.graphViewer.Name = "graphViewer";
            this.graphViewer.Size = new System.Drawing.Size(891, 358);
            this.graphViewer.TabIndex = 13;
            gridSettings1.ProportionalToTransformedScale = true;
            gridSettings1.ShowGridLines = false;
            gridSettings1.ShowLabels = false;
            gridSettings1.TransformLabelValues = true;
            this.graphViewer.XGrid = gridSettings1;
            gridSettings2.ProportionalToTransformedScale = true;
            gridSettings2.ShowGridLines = true;
            gridSettings2.ShowLabels = false;
            gridSettings2.TransformLabelValues = true;
            this.graphViewer.YGrid = gridSettings2;
            this.graphViewer.MouseMove += new AgeReading.Graph.GraphMouseEventHandler(this.graphViewer_MouseMove);
            this.graphViewer.MouseDown += new AgeReading.Graph.GraphMouseEventHandler(this.graphViewer_MouseDown);
            this.graphViewer.MouseUp += new AgeReading.Graph.GraphMouseEventHandler(this.graphViewer_MouseUp);
            this.graphViewer.MouseClick += new AgeReading.Graph.GraphMouseEventHandler(this.graphViewer_MouseClick);
            this.graphViewer.MouseLeave += new System.EventHandler(this.graphViewer_MouseLeave);
            // 
            // curvePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.graphViewer);
            this.Name = "curvePanel";
            this.Size = new System.Drawing.Size(891, 358);
            this.ResumeLayout(false);

        }

        #endregion

        public InteractiveGraphViewer graphViewer;
    }
}
