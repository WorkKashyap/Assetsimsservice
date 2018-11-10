using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace SIMSInterface
{
    public class photos
    {
        public static void GetPhotos(int PersonID, int a_p_id)
        {
            SIMS.Processes.StudentBrowseProcess studentBrowse = new SIMS.Processes.StudentBrowseProcess();

         
             SIMS.Entities.StudentSummarys students = studentBrowse.GetStudents(
                "Current"
                , SIMS.Entities.Cache.WildcardAny     // Any for free 
                                                      // text filter
                , SIMS.Entities.Cache.WildcardAny
                , studentBrowse.RegistrationGroupAny.Code   // Any
                , SIMS.Entities.Cache.WildcardAny
                , studentBrowse.HouseAny.Code       // Any house
                , studentBrowse.TierAny.Code            // Any Tier
                , DateTime.Now                  // Effective Date
                , false);                   // Photos
            // int max = 10;
            foreach (SIMS.Entities.StudentSummary st in students)
            {
                if (PersonID == st.PersonID)
                {
                    SIMS.Entities.IIDEntity identity = new SIMS.Entities.Person(st.ID);
                    SIMS.Processes.EditStudentInformation edStudInfo = new SIMS.Processes.EditStudentInformation();
                    edStudInfo.Load(identity, System.DateTime.Now);
                    Image bitmap = (Bitmap)edStudInfo.Student.Photo;

                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                    bitmap.Save(Path.Combine(@"c:\temp", a_p_id + ".bmp"));
                    // if (max-- < 0)
                    // break; // demo only
                }
            }
        }
}
}
