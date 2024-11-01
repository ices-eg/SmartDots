﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Windows.Media;
using SmartDots.Helpers;

namespace SmartDots.Model
{
    public class File : INotifyPropertyChanged
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public File()
        {
            this.Outcomes = new HashSet<Annotation>();
        }

        public System.Guid ID { get; set; }
        //public Nullable<System.Guid> FolderID { get; set; }
        public string Filename { get; set; }

        public string DisplayName { get; set; }

        public string Display => DisplayName ?? Filename;

        public string SampleNumber { get; set; }
        public Nullable<System.Guid> SampleID { get; set; }
        public decimal? Scale { get; set; }

        //public bool GCRecord { get; set; }
        public virtual ICollection<Annotation> Outcomes { get; set; }
        public virtual Sample Sample { get; set; }
        public bool IsReadOnly { get; set; }
        public bool? CanApprove { get; set; }
        public int AnnotationCount { get; set; }
        public string FullFileName { get; set; }
        public string Path { get; set; }
        public StatusIcon Status { get; set; }
        public dynamic SampleProperties { get; set; }



        public ObservableCollection<Annotation> BoundOutcomes { get; set; }

        public void ConvertDbAnnotations(List<DtoAnnotation> dbAnnotations)
        {
            var result = new ObservableCollection<Annotation>();
            foreach (var annotation in dbAnnotations)
            {
                var lines = new List<Line>();
                foreach (var line in annotation.Lines)
                {
                    lines.Add((Line)Helper.ConvertType(line, typeof(Line)));
                }
                var dots = new List<Dot>();
                foreach (var dot in annotation.Dots)
                {
                    dots.Add((Dot)Helper.ConvertType(dot, typeof(Dot)));
                }

                var temp = new Annotation()
                {
                    ID = annotation.ID,
                    IsApproved = annotation.IsApproved,
                    DateCreation = annotation.DateCreation,
                    LabTechnicianID = annotation.LabTechnicianID,
                    LabTechnician = annotation.LabTechnician,
                    Result = annotation.Result,
                    QualityID = annotation.QualityID,
                    ParameterID = annotation.ParameterID,
                    FileID = annotation.FileID,
                    Lines = lines,
                    Dots = dots,
                    Comment = annotation.Comment,
                    IsFixed = annotation.IsFixed,
                    //DynamicProperties = annotation.DynamicProperties
                };
                if (annotation.Nucleus == "Opaque" || annotation.Nucleus == "Translucent") temp.Nucleus = annotation.Nucleus;
                else
                {
                    temp.Nucleus = null;
                }
                if(annotation.Edge == "Opaque" || annotation.Edge == "Translucent") temp.Edge = annotation.Edge;
                else {
                    temp.Edge = null;
                }
                if (temp.Lines.Any())
                    temp.CombinedLines.Add(new CombinedLine() { Lines = lines, Dots = dots });
                result.Add(temp);


            }
            BoundOutcomes = result;
        }

        public void FetchProps(dynamic row)
        {
            if (Global.API.Settings.UseSampleStatus)
            {
                if (Sample != null) SampleID = Sample.Id;
                Status = SampleID != null ? new StatusIcon((Color)ColorConverter.ConvertFromString(Sample?.StatusColor), Sample?.StatusCode, (int)Sample?.StatusRank) : new StatusIcon(Color.FromRgb(200, 200, 200), "No Sample Linked", 0);
                if(row != null) row.Status = Status; 
            }
            

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs("Status"));
                handler(this, new PropertyChangedEventArgs("AnnotationCount"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
