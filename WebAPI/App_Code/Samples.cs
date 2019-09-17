using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.App_Code
{

    public class Samples
    {
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


        private string m_SampleNumber = null;
        public string SampleNumber
        {
            get
            {
                return m_SampleNumber;
            }
            set
            {
                m_SampleNumber = value;
            }
        }


        private string m_AphiaID = null;
        public string AphiaID
        {
            get
            {
                return m_AphiaID;
            }
            set
            {
                m_AphiaID = value;
            }
        }

        private string m_SpeciesName = null;
        public string SpeciesName
        {
            get
            {
                return m_SpeciesName;
            }
            set
            {
                m_SpeciesName = value;
            }
        }

        private string m_CatchDate = null;
        public string CatchDate
        {
            get
            {
                return m_CatchDate;
            }
            set
            {
                m_CatchDate = value;
            }
        }


        private string m_ICESStatisticalRectangle = null;
        public string ICESStatisticalRectangle
        {
            get
            {
                return m_ICESStatisticalRectangle;
            }
            set
            {
                m_ICESStatisticalRectangle = value;
            }
        }

        private string m_Lenght = null;
        public string Lenght
        {
            get
            {
                return m_Lenght;
            }
            set
            {
                m_Lenght = value;
            }
        }

        private string m_TypeOfStructure = null;
        public string TypeOfStructure
        {
            get
            {
                return m_TypeOfStructure;
            }
            set
            {
                m_TypeOfStructure = value;
            }
        }

        private string m_PreparationMethod = null;
        public string PreparationMethod
        {
            get
            {
                return m_PreparationMethod;
            }
            set
            {
                m_PreparationMethod = value;
            }
        }



    }
}