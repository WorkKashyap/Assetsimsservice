using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SIMS.Processes.ThirdParty;
namespace SIMSInterface
{
    public class Assessment
    {
        private const string doubleQuote = "\"";
        public static string DefaultSource = "<Please Set Before Use>";
        private static string defaultDest = "";
        public static string DefaultDestination
        {
            get
            {
                if (string.IsNullOrEmpty(defaultDest))
                {
                    defaultDest = SIMSAssessmentMessage.SchoolRefID;
                }
                return defaultDest;
            }
        }
        public static XmlDocument GetNewAspect(string AspectName)
        {
            XmlDocument d = new XmlDocument();
            string assessmentBase =
            #region Sample Aspect
 @"<?xml version=" + doubleQuote + "1.0" + doubleQuote + " encoding=" + doubleQuote + "UTF-8" + doubleQuote + " standalone=" + doubleQuote + "yes" + doubleQuote + "?>" +
@"
 <SIMSAssessmentMessage>
  <Header>\n<MessageType>UPDATE</MessageType>
    <MessageID>390820998F7A485D8310B390D5FE39C6</MessageID>
    <SourceID>MYSUPPLIER</SourceID>
    <DestinationID>A7BCF7D4E4224965A153A3EDA4243601</DestinationID>
    <Status>OK</Status>
  </Header>
  <DataObjects>" +
   "<AssessmentResultComponent RefId=" +doubleQuote + "MYGUID" + doubleQuote +">" +
@"    <Name>PARTNERASPECT</Name>
      <LocalId>MYLOCALID</LocalId>
      <ShortDescription>MYDESCRIPTION</ShortDescription>
      <Description>MYDESCRIPTION Description</Description>
      <StageList />
      <AssessmentResultGradeSetRefId>GRADESETREFID</AssessmentResultGradeSetRefId>
      <MarkSetList />
      <ComponentType>Grade</ComponentType>
      <ResultQualifier>CM</ResultQualifier>
      <AssessmentMethod>TT</AssessmentMethod>
      <SupplierName>MYSUPPLIER</SupplierName>
    </AssessmentResultComponent>
  </DataObjects>
</SIMSAssessmentMessage>
";
                    #endregion
            Guid aspectGuid = System.Guid.NewGuid();  // Give it a unique ID

            string asp = assessmentBase.Replace("MYGUID", CleanGUID(aspectGuid));
            // Put in a recognisable description
            asp = asp.Replace("PARTNERASPECT", AspectName);
            asp = asp.Replace("MYLOCALID", AspectName);
            
            // Must have a supplier match
            asp = asp.Replace("MYSUPPLIER", DefaultSource);
            //if (CleanGUID(gradesetGuid) != "" && CleanGUID(gradesetGuid) != null)
            asp = asp.Replace("GRADESETREFID", CleanGUID(GradeSetCreationGuid));
            asp = asp.Replace("A7BCF7D4E4224965A153A3EDA4243601", SIMS.Processes.ThirdParty.SIMSAssessmentMessage.SchoolRefID);

            // Display the result
            //This is used for populating Aspects
            d.InnerXml = asp;
            
            return d;
        }
        public static void SaveAspect(ref string ErrorInfo, string AspectXML)
        {
            XmlDocument x = new XmlDocument();
            x.LoadXml(AspectXML);
            SIMS.Processes.ThirdParty.SIMSAssessmentMessage s = new SIMSAssessmentMessage(x, SIMSAssessmentMessage.ASSESSMENTRESULTCOMPONENT);
            // Request the import
            s.Import(x);
            ErrorInfo = s.ErrorString;
        }
        public static void SaveGradeSet(ref string ErrorInfo, string GradeSetXML)
        {
            XmlDocument x = new XmlDocument();
            x.LoadXml(GradeSetXML);
            SIMS.Processes.ThirdParty.SIMSAssessmentMessage s = new SIMSAssessmentMessage(x, SIMSAssessmentMessage.ASSESSMENTRESULTGRADESET);
            // Request the import
            s.Import(x);
            ErrorInfo = s.ErrorString;
        }
        public static Guid GradeSetCreationGuid = Guid.Empty;
        public static XmlDocument GetNewGradeSet (string Name)
        {
            XmlDocument d = new XmlDocument();

            string gradeSetBase =
            #region Sample GradeSet
 @"<?xml version=" + doubleQuote + "1.0" + doubleQuote + " encoding=" + doubleQuote + "UTF-8" + doubleQuote + " standalone=" + doubleQuote + "yes" + doubleQuote + "?>" +
@"
 <SIMSAssessmentMessage xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <Header>
    <MessageType>UPDATE</MessageType>
    <MessageID>390820998F7A485D8310B390D5FE39C6</MessageID>
    <SourceID>MYSUPPLIER</SourceID>
    <DestinationID>A7BCF7D4E4224965A153A3EDA4243601</DestinationID>
    <Status>OK</Status>
  </Header>
  <DataObjects>" +
   "\n <AssessmentResultGradeSet RefId=" + doubleQuote + "MYGUID" + doubleQuote + ">" + "\n" +
@"      <Name>PartnerGradeTest</Name>
        <LocalId>PartnerGradeTest</LocalId>
         <Notes>This GradeSet has been created for TPAssessment Sample Application</Notes>            
        <SupplierName>MYSUPPLIER</SupplierName>
        <GradeSets>
           <GradeSet>
              <StartDate>2007-09-01</StartDate>
              <EndDate xsi:nil=" + doubleQuote + "true" + doubleQuote + ">" + "\n" +
              @"              </EndDate>
              <Grades>
                 <Grade>
                    <Title>5</Title>
                    <Description>Level 5</Description>
                    <RankOrder>1</RankOrder>
                    <NumericValue>33.00</NumericValue>
                 </Grade>
                 <Grade>
                   <Title>4</Title>
                   <Description>Level 4</Description>
                   <RankOrder>2</RankOrder>
                   <NumericValue>27.00</NumericValue>
                </Grade>
             </Grades>
          </GradeSet>
      </GradeSets>
    </AssessmentResultGradeSet>
  </DataObjects>
</SIMSAssessmentMessage>
";

            #endregion
            GradeSetCreationGuid = System.Guid.NewGuid();

            string gradesetXml = gradeSetBase.Replace("MYGUID", CleanGUID(GradeSetCreationGuid));
            gradesetXml = gradesetXml.Replace("PartnerGradeTest", "PartnerGradeTest" + new Random().Next());
            // Put in a recongnisable description
            //gradesetXml = gradesetXml.Replace("Name", textBoxGradeSetToCreate.Text);
            // Must have a supplier match
            gradesetXml = gradesetXml.Replace("MYSUPPLIER", DefaultSource);
            //Supply the Home School Ref ID

            gradesetXml = gradesetXml.Replace("A7BCF7D4E4224965A153A3EDA4243601", SIMS.Processes.ThirdParty.SIMSAssessmentMessage.SchoolRefID);

            //This is used for populating GradeSets
            d.InnerXml = gradesetXml;
            return d;
        }
        public static XmlDocument GetNewResult(string StudentID, string AspectID)
        {
            string date = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
            string resultBase =
            #region Sample Result
 @"<?xml version=" + doubleQuote + "1.0" + doubleQuote + " encoding=" + doubleQuote + "UTF-8" + doubleQuote + " standalone=" + doubleQuote + "yes" + doubleQuote + "?>" +
           @"
 <SIMSAssessmentMessage xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <Header>
    <MessageType>UPDATE</MessageType>
    <MessageID>390820998F7A485D8310B390D5FE39C6</MessageID>
    <SourceID>MYSUPPLIER</SourceID>
    <DestinationID>A7BCF7D4E4224965A153A3EDA4243601</DestinationID>
    <Status>OK</Status>
  </Header>
  <DataObjects>" +
              "\n <LearnerAssessmentResult RefId=" + doubleQuote + "MYRESULTGUID" + doubleQuote + " AssessmentComponentRefId =" + doubleQuote + "MYASPECTGUID" + doubleQuote + " LearnerPersonalRefId=" + doubleQuote + "MYLEARNERGUID" + doubleQuote + ">"
              + "\n" +
           @"      <SchoolInfoRefId>A7BCF7D4E4224965A153A3EDA4243601</SchoolInfoRefId>
        <AchievementDate>MYRESULTDATE</AchievementDate>
        <Result>INSERTMYRESULTHERE</Result>   
        <ResultStatus>R</ResultStatus>                 
    </LearnerAssessmentResult>
  </DataObjects>
</SIMSAssessmentMessage>
";
            #endregion

            System.Guid resultGuid = System.Guid.NewGuid();

            string resultXml = resultBase.Replace("MYRESULTGUID", CleanGUID(resultGuid));

            // Must have a supplier match
            resultXml = resultXml.Replace("MYSUPPLIER", DefaultSource);
            //Supply the Home School Ref ID

            resultXml = resultXml.Replace("A7BCF7D4E4224965A153A3EDA4243601", SIMS.Processes.ThirdParty.SIMSAssessmentMessage.SchoolRefID);
            resultXml = resultXml.Replace("MYASPECTGUID", CleanGUIDAsString(AspectID));
            //Get the GUID of a Student and replace the MYLEARNERGUID with that value
            resultXml = resultXml.Replace("MYLEARNERGUID", CleanGUIDAsString(StudentID));
            resultXml = resultXml.Replace("MYRESULTDATE", date);
            XmlDocument d = new XmlDocument();
            d.InnerXml = resultXml;
            return d;
        }
        public static string CleanGUID(System.Guid g)
        {
            string rc = g.ToString();
            rc = rc.Replace("-", "");
            rc = rc.Replace("{", "");
            rc = rc.Replace("}", "");
            return rc.ToUpper();
        }
        private static string CleanGUIDAsString(String strGuid)
        {
            string rc = strGuid.Replace("-", "");
            rc = rc.Replace("{", "");
            rc = rc.Replace("}", "");
            return rc.ToUpper();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ErrorInfo"></param>
        /// <param name="AspectGuids">Either</param>
        /// <param name="StudentGuids">Either</param>
        /// <param name="ResultSetGuids">Either</param>
        /// <param name="Start">Mandatory</param>
        /// <param name="End">Mandatory</param>
        /// <returns></returns>
        public static XmlDocument GetResults(ref string ErrorInfo, string ResultSetGuid, string StudentGuid, string AspectGUID)
        {
            DateTime Start = DateTime.Parse("2016-09-01");
            DateTime End = DateTime.Today;
        
            XmlDocument d = new XmlDocument();
            // Create a request document
            SIMSAssessmentMessage sdo = new SIMSAssessmentMessage();
            Dictionary<string, string> paramOptions = sdo.GetParamOptions(SIMSAssessmentMessage.LEARNERASSESSMENTRESULT);
            // Create and populate the filter
            Dictionary<string, string[]> selections = new Dictionary<string, string[]>();
            // Do we have any r
            if (AspectGUID != "")
            {
                selections.Add("AssessmentComponentRefId", new string[1] { AspectGUID }); 
            }
            if (ResultSetGuid != "")
            {
                selections.Add("AssessmentSessionRefId", new string[1] { ResultSetGuid });
            }
            if (StudentGuid != "")
            {
                selections.Add("LearnerPersonalRefId", new string[1] { StudentGuid});
            }
            // We must have a date range for this call.
            string dRange = Start.ToShortDateString() +";" + End.ToShortDateString();
            selections.Add("AchievementDate", dRange.Split(';'));
            // Make the request
            XmlDocument request = SIMSAssessmentMessage.GenerateRequestDoc(selections, SIMSAssessmentMessage.LEARNERASSESSMENTRESULT, DefaultSource, DefaultDestination);
            XmlDocument data = sdo.Export(request);
            // Display the results
            d.InnerXml = sdo.ToXmlString();
            // Display the errors
            ErrorInfo = sdo.ErrorDocument.InnerXml;
            XmlDocument rc = new XmlDocument();
            if (ErrorInfo == "")
            {
                foreach(XmlNode n in d.SelectNodes("SIMSAssessmentMessage/DataObjects/LearnerAssessmentResult"))
                {
                    if (n["LearnerPersonalRefId"].InnerXml == StudentGuid)
                    {
                        // This is one of the results we need
                        rc.AppendChild(n);

                    }
                }
            }
            return rc;
        }
        public static XmlDocument GetResultSets(ref string ErrorInfo)
        {
            // Create the request
            SIMSAssessmentMessage sdo = new SIMSAssessmentMessage();
            Dictionary<string, string> paramOptions = sdo.GetParamOptions(SIMSAssessmentMessage.LEARNERASSESSMENTRESULTSET);
            // No filter
            Dictionary<string, string[]> selections = new Dictionary<string, string[]>();
            //Actual request
            XmlDocument request = SIMSAssessmentMessage.GenerateRequestDoc(selections, SIMSAssessmentMessage.LEARNERASSESSMENTRESULTSET, DefaultSource, DefaultDestination);
            XmlDocument data = sdo.Export(request);
            // Display the results
            XmlDocument d = new XmlDocument();

            d.InnerXml  = sdo.ToXmlString();
            // Display the errors
            ErrorInfo = sdo.ErrorDocument.InnerXml;
            return d;
        }
        public static XmlDocument GetGradeSets(ref string ErrorInfo)
        {
            // Need to create a request document.
            SIMSAssessmentMessage sdo = new SIMSAssessmentMessage();
            Dictionary<string, string> paramOptions = sdo.GetParamOptions(SIMSAssessmentMessage.ASSESSMENTRESULTGRADESET);
            // This one asks for them all - it is possible to filter them.
            Dictionary<string, string[]> selections = new Dictionary<string, string[]>();
            // Request Grade sets
            XmlDocument request = SIMSAssessmentMessage.GenerateRequestDoc(selections, SIMSAssessmentMessage.ASSESSMENTRESULTGRADESET, DefaultSource, DefaultDestination);
            XmlDocument data = sdo.Export(request);
            // Display response
            XmlDocument d = new XmlDocument();
            d.InnerXml = sdo.ToXmlString();
            // Display errors
            ErrorInfo = sdo.ErrorDocument.InnerXml;
            return d;
        }
        public static XmlDocument GetAspects(ref string ErrorInfo)
        {
            XmlDocument d = new XmlDocument();
            // Need to generate a request for aspects data as follows
            SIMSAssessmentMessage sdo = new SIMSAssessmentMessage();
            Dictionary<string, string> paramOptions = sdo.GetParamOptions(SIMSAssessmentMessage.ASSESSMENTRESULTCOMPONENT);
            // Blank set of dictionary objects are used for full extract - it is possible to filter the request
            // but that type of operation would suggest a 'coversation with the VLE'
            // This is quite possible but developers are reminfed that SIMS is NOT multi threaded!
            Dictionary<string, String[]> selections = new Dictionary<string, String[]>();
            //  OK - We can now ask for the aspects.
            XmlDocument request = SIMSAssessmentMessage.GenerateRequestDoc(selections, SIMSAssessmentMessage.ASSESSMENTRESULTCOMPONENT, DefaultSource, DefaultDestination);
            XmlDocument data = sdo.Export(request);
            // We now have them and will display them in a rich text box.  Clearly VLE code would need to do something more
            // positive - like call their web service.
            d.InnerXml = sdo.ToXmlString();
            // Display the errors (if any)
            ErrorInfo = sdo.ErrorDocument.InnerXml;
            return d;
            
        }
    }
}
