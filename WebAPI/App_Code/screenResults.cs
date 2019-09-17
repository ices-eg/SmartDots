using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebInterface.App_Code
{
    public class screenResults
    {
        private int? m_NoErrors = null;
        public int? NumberOfErrors
        {
            get
            {
                return m_NoErrors;
            }
            set
            {
                m_NoErrors = value;
            }
        }

        private string m_SessionID = null;
        public string SessionID
        {
            get
            {
                return m_SessionID;
            }
            set
            {
                m_SessionID = value;
            }
        }

        private string m_ScreenResultURL = null;
        public string ScreenResultURL
        {
            get
            {
                return m_ScreenResultURL;
            }
            set
            {
                m_ScreenResultURL = value;
            }
        }
    }
}