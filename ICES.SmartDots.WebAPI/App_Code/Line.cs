using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.App_Code
{
    public class Line
    {
        private string m_lineID = null;
        public string lineID
        {
            get
            {
                return m_lineID;
            }
            set
            {
                m_lineID = value;
            }
        }

        private string m_CreateDate = null;
        public string CreateDate
        {
            get
            {
                return m_CreateDate;
            }
            set
            {
                m_CreateDate = value;
            }
        }

        private string m_User = null;
        public string User
        {
            get
            {
                return m_User;
            }
            set
            {
                m_User = value;
            }
        }

        private int? m_LineStartX = null;
        public int? LineStartX
        {
            get
            {
                return m_LineStartX;
            }
            set
            {
                m_LineStartX = value;
            }
        }


        private int? m_LineStartY = null;
        public int? LineStartY
        {
            get
            {
                return m_LineStartY;
            }
            set
            {
                m_LineStartY = value;
            }
        }

        private int? m_LineEndX = null;
        public int? LineEndX
        {
            get
            {
                return m_LineEndX;
            }
            set
            {
                m_LineEndX = value;
            }
        }


        private int? m_LineEndY = null;
        public int? LineEndY
        {
            get
            {
                return m_LineEndY;
            }
            set
            {
                m_LineEndY = value;
            }
        }
    }
}