using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SIMSInterface
{
    public class School
    {
        public static XmlDocument SchoolDetails()
        {
            StringBuilder b = new StringBuilder();
            b.Append("<School>");
            b.AppendFormat("<Name>{0}</Name>", SIMS.Entities.Cache.CurrentSchool.SchoolName);
            b.AppendFormat("<DFENumber>{0}</DFENumber>", SIMS.Entities.Cache.CurrentSchool.DCSFNumber);
            b.AppendFormat("<Head>{0}</Head>", SIMS.Entities.Cache.CurrentSchool.HeadTeacher);
            b.AppendFormat("<Phase>{0}</Phase>", SIMS.Entities.Cache.CurrentSchool.SchoolPhase);
            

            b.Append("</School>");

            XmlDocument d = new XmlDocument();
            d.InnerXml = b.ToString();
            return d;
        }

        public static string DCSFNumber()
        {
            return SIMS.Entities.Cache.CurrentSchool.DCSFNumber;
        }
    }
}
