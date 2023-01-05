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
        public bool IsReadOnly { get; set; }
        public bool AllowApproveToggle { get; set; }
        public bool UserHasApproved { get; set; }
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
                    Name = file.Name,
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
                    AnalFinPresenceID = annotation.AnalFinPresenceID,
                    DorsalFinPresenceID = annotation.DorsalFinPresenceID,
                    PelvicFinPresenceID = annotation.PelvicFinPresenceID,
                    DevelopmentStageID = annotation.DevelopmentStageID,
                    LarvaeSampleID = annotation.LarvaeSampleID,
                    UserID = annotation.UserID,
                    User = annotation.User,
                    IsApproved = annotation.IsApproved,
                    LarvaeAnnotationParameterResult = new List<LarvaeAnnotationParameterResult>()

                };

                if (annotation.AnnotationParameterResult != null)
                {
                    foreach (var apr in annotation.AnnotationParameterResult)
                    {
                        var apResult = new LarvaeAnnotationParameterResult();
                        apResult.ID = apr.ID;
                        apResult.LarvaeParameterID = apr.ParameterID;
                        apResult.LarvaeAnnotationID = apr.AnnotationID;
                        apResult.LarvaeFileID = apr.FileID;
                        apResult.Result = apr.Result;
                        apResult.Lines = new List<LarvaeLine>();
                        apResult.Dots = new List<LarvaeDot>();

                        foreach (var line in apr.Lines)
                        {
                            apResult.Lines.Add(new LarvaeLine()
                            {
                                ID = line.ID,
                                //AnnotationParameterResultID = line.AnnotationParameterResultID,
                                LineIndex = line.LineIndex,
                                X1 = line.X1,
                                X2 = line.X2,
                                Y1 = line.Y1,
                                Y2 = line.Y2,
                                Width = line.Width
                            });
                        }

                        foreach (var dot in apr.Dots)
                        {
                            apResult.Dots.Add(new LarvaeDot()
                            {
                                ID = dot.ID,
                                AnnotationParameterResultID = dot.AnnotationParameterResultID,
                                X = dot.X,
                                Y = dot.Y,
                                Width = dot.Width
                            });
                        }

                        temp.LarvaeAnnotationParameterResult.Add(apResult);
                    }

                }

                result.Add(temp);

            }
            Annotations = result;
        }


    }
}
