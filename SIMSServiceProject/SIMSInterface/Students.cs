using System;
using System.Xml;

namespace SIMSInterface
{
    public class Students
    {
        /// <summary>
        /// Use a third party API call to return current student data
        /// NB: Just because the API returns data, there is no implied right to pass all of the data returned to 
        /// and external system
        /// </summary>
        /// <returns></returns>
        public static XmlDocument GetCurrentStudents()
        {
            XmlDocument d = new XmlDocument();
            // Create an instance of the pa
            SIMS.Processes.TPPersonStudent tpps = new SIMS.Processes.TPPersonStudent();
            d.InnerXml = tpps.GetXmlStudents(DateTime.Today);
            return d;

        }
        
    }
}
