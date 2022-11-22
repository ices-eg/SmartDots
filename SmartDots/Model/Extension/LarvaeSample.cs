using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using SmartDots.Helpers;

namespace SmartDots.Model
{
    public class LarvaeSample
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
        public ObservableCollection<LarvaeFile> Files { get; set; }
        public ObservableCollection<LarvaeAnnotation> Annotations { get; set; }

        public StatusIcon Status
        {
            get
            {
                return new StatusIcon((Color)ColorConverter.ConvertFromString(StatusColor), StatusCode, StatusRank);
            }
        }


        public void ConvertDbFiles(List<DtoLarvaeFile> dbFiles)
        {
            var result = new ObservableCollection<LarvaeFile>();
            foreach (var file in dbFiles)
            {
                var temp = new LarvaeFile()
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

        public void ConvertDbAnnotations(List<DtoLarvaeAnnotation> dbAnnotations)
        {
            var result = new ObservableCollection<LarvaeAnnotation>();
            foreach (var annotation in dbAnnotations)
            {
                var temp = new LarvaeAnnotation()
                {
                    ID = annotation.ID,
                    Date = annotation.Date,
                    Comments = annotation.Comments,
                    LarvaeQualityID = annotation.LarvaeQualityID,
                    LarvaeID = annotation.LarvaeID,
                    LarvaeSampleID = annotation.LarvaeSampleID,
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
