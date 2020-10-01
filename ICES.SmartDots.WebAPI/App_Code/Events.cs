using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.App_Code
{

    public class OtolithsEvents
    {
        private string m_eventName = null;
        public string eventName
        {
            get
            {
                return m_eventName;
            }
            set
            {
                m_eventName = value;
            }
        }

        private string m_role = null;
        public string role
        {
            get
            {
                return m_role;
            }
            set
            {
                m_role = value;
            }
        }

        private string m_eventID = null;
        public string eventID
        {
            get
            {
                return m_eventID;
            }
            set
            {
                m_eventID = value;
            }
        }

        private string m_startDate = null;
        public string startDate
        {
            get
            {
                return m_startDate;
            }
            set
            {
                m_startDate = value;
            }
        }

        private string m_endDate = null;
        public string endDate
        {
            get
            {
                return m_endDate;
            }
            set
            {
                m_endDate = value;
            }
        }
    }
}