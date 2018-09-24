namespace SmartDots.Model
{
    public class DtoSmartdotsSettings
    {
        public bool CanAttachDetachSample { get; set; } 
        public bool CanBrowseFolder { get; set; } 
        public bool UseSampleStatus { get; set; } 
        public bool CanApproveAnnotation { get; set; } 
        public bool RequireAq1ForApproval { get; set; }
        public bool RequireParamForApproval { get; set; }
        public bool AutoMeasureScale { get; set; }
        public bool ScanFolder { get; set; }
        public bool AnnotateWithoutSample { get; set; }
        public bool OpenSocket { get; set; }
        public string EventAlias { get; set; }
        public string SampleAlias { get; set; }
        public float MinRequiredVersion { get; set; }
    }
}
