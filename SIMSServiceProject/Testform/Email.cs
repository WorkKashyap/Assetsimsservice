using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Testform
{
    class Email
    {
        INIFile inif = new INIFile(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini");

        public void SendHtmlFormattedEmail(string subject, string body)

        {

            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "auth.smtp.1and1.co.uk";
            client.EnableSsl = true;
            client.Timeout = 10000;

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("virtualerror@assetforschools.co.uk", "8HJkasdfghdsvftywegv127!");

            MailMessage mm = new MailMessage("virtualerror@assetforschools.co.uk", "virtualerror@assetforschools.co.uk", subject, body);
            mm.IsBodyHtml = true;
            //  mm.AlternateViews.Add(getEmbeddedImage("logo.png", body));
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);

        }
        public AlternateView getEmbeddedImage(String filePath, string body)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();

            string htmlBody = @"<img src='cid:" + res.ContentId + @"'/>";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }
        static string GetIPAddress()
        {
            String address = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                address = stream.ReadToEnd();
            }

            int first = address.IndexOf("Address: ") + 9;
            int last = address.LastIndexOf("</body>");
            address = address.Substring(first, last - first);

            return address;
        }
        public string createEmailBody(string flag, string dfe,string school, string title, string message)

        {

            string body = string.Empty;
            string EmailTemplate="";
            string am = inif.Read("AMStartTime", "AM");
            string pm = inif.Read("PMEndTime", "PM");


            if (flag=="first")
            { 
                     EmailTemplate = AppDomain.CurrentDomain.BaseDirectory + "\\Template.htm";
            }
            if (flag == "login")
            {
                EmailTemplate = AppDomain.CurrentDomain.BaseDirectory + "\\ErrorTemplate.htm";
            }
            if (flag == "server")
            {
                EmailTemplate = AppDomain.CurrentDomain.BaseDirectory + "\\ErrorServer.htm";
            }
            //using streamreader for reading my htmltemplate   

            using (StreamReader reader = new StreamReader(EmailTemplate))

            {

                body = reader.ReadToEnd();

            }

            if(flag=="server")
            {
                body = body.Replace("{DFE}", dfe);
            }

            body = body.Replace("{School}", school); //replacing the required things  

            body = body.Replace("{AMTIME}", am);

            body = body.Replace("{PMTIME}", pm);

            body = body.Replace("{Title}", title);

            body = body.Replace("{IPAddress}", GetIPAddress());

            body = body.Replace("{message}", message);

            return body;

        }
       
    }
}
