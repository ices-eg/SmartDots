using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using SmartDots.Helpers;

namespace SmartDots.Model
{
    public class MaturitySample
    {
        public Guid ID { get; set; }
        public string StatusCode { get; set; }
        public string StatusColor { get; set; }
        public int StatusRank { get; set; }
        public dynamic SampleProperties { get; set; }
        public dynamic AnnotationProperties { get; set; }
        public bool IsReadOnly { get; set; }
        public bool AllowApproveToggle { get; set; }
        public bool UserHasApproved { get; set; }
        public DtoFolder Folder { get; set; }
        public ObservableCollection<MaturityFile> Files { get; set; }
        public ObservableCollection<MaturityAnnotation> Annotations { get; set; }

        public StatusIcon Status
        {
            get
            {
                return new StatusIcon((Color)ColorConverter.ConvertFromString(StatusColor), StatusCode, StatusRank);
            }
        }


        public void ConvertDbFiles(List<DtoMaturityFile> dbFiles)
        {
            var result = new ObservableCollection<MaturityFile>();
            foreach (var file in dbFiles)
            {
                var temp = new MaturityFile()
                {
                    ID = file.ID,
                    Path = file.Path,
                    ThumbnailPath = file.ThumbnailPath,
                    IsReadOnly = file.IsReadOnly,
                    Scale = file.Scale,
                };
                
                result.Add(temp);

            }
            Files = result;
        }

        public void ConvertDbAnnotations(List<DtoMaturityAnnotation> dbAnnotations)
        {
            var result = new ObservableCollection<MaturityAnnotation>();
            foreach (var annotation in dbAnnotations)
            {
                var temp = new MaturityAnnotation()
                {
                    ID = annotation.ID,
                    Date = annotation.Date,
                    Comments = annotation.Comments,
                    MaturityQualityID = annotation.MaturityQualityID,
                    MaturityID = annotation.MaturityID,
                    MaturitySampleID = annotation.MaturitySampleID,
                    SexID = annotation.SexID,
                    UserID = annotation.UserID,
                    User = annotation.User,
                    IsApproved = annotation.IsApproved
                };

                result.Add(temp);

            }
            Annotations = result;
        }


    }
}
