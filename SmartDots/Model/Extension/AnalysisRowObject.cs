using System.Collections.Generic;
using System.Linq;

namespace SmartDots.Model.Extension
{
    public class AnalysisRowObject : Analysis
    {
        public bool IsAvailableOffline { get; set; }

        public string Parameters
        {
            get
            {
                string result = "";
                if (AnalysisParameters != null)
                {
                    List<string> parameterStrings = AnalysisParameters.Select(x => x.Code).ToList();
                    result = string.Join(",", parameterStrings);
                }
                return result;
            }
        }
    }
}
