using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.App_Code
{

    public class SampleImages
    {
        private string m_sampleID = null;
        public string sampleID
        {
            get
            {
                return m_sampleID;
            }
            set
            {
                m_sampleID = value;
            }
        }

        private string m_smartImageID = null;
        public string smartImageID
        {
            get
            {
                return m_smartImageID;
            }
            set
            {
                m_smartImageID = value;
            }
        }

        private string m_smartImageURL = null;
        public string smartImageURL
        {
            get
            {
                return m_smartImageURL;
            }
            set
            {
                m_smartImageURL = value;
            }
        }


    }
}