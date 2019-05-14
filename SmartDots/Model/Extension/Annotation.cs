using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows;

namespace SmartDots.Model
{
    public class Annotation : INotifyPropertyChanged
    {
        private List<CombinedLine> combinedLines = new List<CombinedLine>();
        private User labTechnician;

        public Annotation()
        {
            this.Dots = new HashSet<Dot>();
            this.Lines = new HashSet<Line>();
        }

        public System.Guid ID { get; set; }
        public Nullable<System.Guid> AnalysisID { get; set; }
        public Nullable<System.Guid> SampleID { get; set; }
        public Nullable<System.Guid> ParameterID { get; set; }
        public System.Guid FileID { get; set; }
        public Nullable<System.Guid> QualityID { get; set; }
        public int? Result { get; set; }
        public System.DateTime DateCreation { get; set; }
        public Nullable<System.Guid> LabTechnicianID { get; set; }
        public bool IsApproved { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsFixed { get; set; }
        //public Image FixedIcon
        //{
        //    get
        //    {
        //        if (IsFixed) return new Bitmap(Application.GetResourceStream(new Uri("Resources/pin-16.png", UriKind.RelativeOrAbsolute)).Stream);
        //        return null;
        //    }
        //}

        public string Nucleus { get; set; }
        public string Edge { get; set; }

        public string Comment { get; set; }
        public bool IsChanged { get; set; }


        public virtual ICollection<Dot> Dots { get; set; }
        public virtual ICollection<Line> Lines { get; set; }
        public virtual Quality Quality { get; set; }

        public List<CombinedLine> CombinedLines
        {
            get { return combinedLines; }
            set { combinedLines = value; }
        }

        public List<Quality> Qualities { get; set; }
        public List<DtoAnnotationProperty> DynamicProperties { get; set; }


        public string QualityGuid
        {
            get { return QualityID.ToString(); }
        }

        //public List<Parameter> Parameters { get; set; }

        //public bool IsHistoric()
        //{
        //    return !HasDots() && Result != 0;
        //}

        public string LabTechnician { get; set; }

        public string PinPicture
        {
            get
            {
                if (IsFixed)
                {
                    return "/SmartDots;component/Resources/pin-16.png";
                }
                return null;
            }
        }

        public string ApprovedPicture
        {
            get
            {
                if (IsApproved)
                {
                    return "/SmartDots;component/Resources/ok-16.png";
                }
                return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //public bool HasDots()
        //{
        //    return CombinedLines.Any(combinedLine => combinedLine.Dots.Any());
        //}

        public void SetAge(int? age)
        {
            Result = age;
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs("Result"));
            }
        }

        public void CalculateAge()
        {
            
            if (HasAqNoAge())
            {
                SetAge(null);
                return;
            }
            int age = 0;
            foreach (
                CombinedLine combinedLine in CombinedLines)
            {
                if (combinedLine.Dots.Count > age)
                {
                    age = combinedLine.Dots.Count;
                }
            }
            SetAge(age);
        }

        public bool HasAqNoAge()
        {
            return Quality != null && Quality.Code.ToLower().Contains("aq3") && Quality.Code.ToLower().Contains("noage");
        }

        //public bool IsValidOutcome()
        //{
        //    var aq1Id = Guid.Parse("0BA0FC71-3EC4-424F-A6BA-A158FE8E3157");
        //    return IsHistoric() || QualityID == aq1Id;
        //}
    }
}
