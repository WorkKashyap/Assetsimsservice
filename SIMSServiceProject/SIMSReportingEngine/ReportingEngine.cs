
// ---------------------------------------------------------------------------------
// Sample code provided to Capita SIMS Partners under consultancy (Hereafter 'partners')
// Written by Andy McGowan 14 Aug 12
// 
// Sample code is copyright (c) Capita Group PLC
//
// Capita grants rights to use the code on an 'as is' basis to partners for the purpose
// of interfacing with SIMS .net and is for their own use.  Code may be used in whole or in part for
// the partners's application(s).  Rights are subject to the restrictions below.
//
// This code may not be used for commercial purposes without a partnership agreement with capita
// if the code is provided to schools for their own use then further consent from Capita Group PLC
// must be obtained before the code can be re-used for commercial gain.
//
// No warranty is provided . This header must be retained within the code and Copyright acknowleged as
// Parts (c) Capita Group PLC 2012 in version info.
//
// Code samples may be used to support other partners and no exclusivity is implied herein 
//
// ---------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SIMSReportingEngine
{
    public class ReportingEngine
    {
        #region Private Consts
        private const string CLR_PARAMS = " /report:\"{0}\" /QUIET /ServerName:\"{1}\" /DatabaseName:\"{2}\"";
        private const string CLR_PARAMS_WITH_PARAMS = " /report:\"{0}\" /QUIET /ServerName:\"{1}\" /DatabaseName:\"{2}\" /PARAMFILE:\"{3}\"";
        private const string CLR_USER   = " /USER:{0} /PASSWORD:{1}";
        #endregion
        #region Private Variables
        private static string _errorMessage = "";
        #endregion
        /// <summary>
        /// Returns the latest error message
        /// </summary>
        public static string ErrorMessage { get { return _errorMessage; } }
        #region Load the SIMS Reports
        /// <summary>
        /// Load the report definition via
        /// CommandReportImporter
        /// </summary>
        /// <param name="importFile"></param>
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public static bool Load(string importFile, string Server, string Database, string User, String Password)
        {
            _errorMessage = "";                                                                                 // Reset the error message
            // Build the argument string
            string args = string.Format(CLR_USER, User, Password) + string.Format(CLR_PARAMS, importFile, Server, Database);
            if (System.IO.File.Exists(importFile))
            {
                System.Diagnostics.Process installer = new System.Diagnostics.Process();                            // Create the process
                installer.StartInfo.UseShellExecute = false;                                                        // Set UseShellExecute to false for redirection.
                installer.StartInfo.CreateNoWindow = true;                                                          // Don't flash an unnecessary window
                installer.StartInfo.RedirectStandardOutput = true;                                                  // Redirect standard output
                installer.StartInfo.ErrorDialog = true;                                                             // Tell the user on fail to start
                installer.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;                     // Not too relevent if ou can't see it
                installer.StartInfo.FileName = System.IO.Path.Combine(SIMSIni.SIMSDotNetFolder, "commandreportimporter.exe"); // Command Importer
                installer.StartInfo.Arguments = args;                                                        // Set the command line params
                try
                {
                    installer.Start();                                                                              // Start the command reporter
                }
                catch (Exception ex)                                                                            // If comamnd reporter fails to run an exception is fired.
                {
                    _errorMessage = "Report Import: Error - " + ex.ToString();
                    return false;                                                                            // Give the user a clue what's up...
                }
                string output = installer.StandardOutput.ReadToEnd();                                               // Capture the output
                installer.WaitForExit();                                                                            // Wait for it to finish
                if (output != null)
                {
                    // If it fails the output is of the form
                    //
                    //SIMS.net Command Line Report Importing Utility 7.60
                    //Copyright (C) Capita Education Services 2006. All rights reserved.
                    //Logging into to SIMS.net as user CSS\amcgowan
                    //Locating Report Definition vvvC:\reports\Demo\demo2.rptdef
                    //Report not found


                    if (output.Contains("Report Definition Successfully Imported"))                                                // For arguments sake check for an error from command reporter
                    {
                        //logEntry("Report Import: Import successfull");
                        return true;                                                                             // Tell the user if we have an error 
                    }
                    else
                    // Output is of the form below if it works
                    // 
                    //SIMS.net Command Line Report Importing Utility 7.60
                    //Copyright (C) Capita Education Services 2006. All rights reserved.
                    //Logging into to SIMS.net as user CSS\amcgowan
                    //Locating Report Definition C:\reports\Demo\demo2.rptdef
                    //Report Definition found.
                    //Importing Report Definition C:\reports\Demo\demo2.rptdef
                    //Report Definition Successfully Imported
                    {
                        _errorMessage = "Report Import: Import failed - " + output;
                        return false;
                    }
                }
                else
                {
                    _errorMessage = "Report Import: Import failed - Unknown reason - importer did not respond";
                    return false;
                }
            }
            else
            {
                _errorMessage = "Missing File: " + importFile;
                return false;
            }
        }
        #endregion
        #region RunSIMSReports
        /// <summary>
        /// Call to run a report without a paramdef file
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="Server"></param>
        /// <param name="Database"></param>
        /// <param name="User"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static XmlDocument Run(string reportName, string Server, string Database, string User, String Password)
        {
            return Run(string.Format(CLR_USER, User, Password) + string.Format(CLR_PARAMS, reportName, Server, Database));
        }
        /// <summary>
        /// Call to run a report with a param definition file
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="Server"></param>
        /// <param name="Database"></param>
        /// <param name="User"></param>
        /// <param name="Password"></param>
        /// <param name="ParamFile"></param>
        /// <returns></returns>
        public static XmlDocument Run(string reportName, string Server, string Database, string User, String Password, string ParamFile)
        {
            return Run(string.Format(CLR_USER, User, Password) + string.Format(CLR_PARAMS_WITH_PARAMS, reportName, Server, Database, ParamFile));
        }
        /// <summary>
        /// Main engine to run reports
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static XmlDocument Run(string args)
        {
            _errorMessage = "";                                                                                 // Reset the error message
            System.Diagnostics.Process installer = new System.Diagnostics.Process();                            // Create the process
            installer.StartInfo.UseShellExecute = false;                                                        // Set UseShellExecute to false for redirection.
            installer.StartInfo.CreateNoWindow = true;                                                          // Don't flash an unnecessary window
            installer.StartInfo.RedirectStandardOutput = true;                                                  // Redirect standard output
            installer.StartInfo.ErrorDialog = true;                                                             // Tell the user on fail to start
            installer.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;                     // Not too relevent if ou can't see it
            installer.StartInfo.FileName = System.IO.Path.Combine(SIMSIni.SIMSDotNetFolder, "commandreporter.exe");        // Command Reporter
            installer.StartInfo.Arguments = args;                                                        // Set the arguments
            try
            {
                installer.Start();                                                                              // Start the command reporter
            }
            catch (Exception ex)                                                                                // If comamnd reporter fails to run an exception is fired.
            {
                _errorMessage = ex.Message; 
                return null;
            }
            string output = installer.StandardOutput.ReadToEnd();                                               // Capture the output
            installer.WaitForExit();                                                                            // Wait for it to finish
            if (output != null)
            {
                if (output.StartsWith("<CommandReporterError>"))                                                // For arguments sake check for an error from command reporter
                {
                    _errorMessage = "Report Execution: Error - " + output;
                    return null;
                }
                else
                {
                    // Worked!
                    try
                    {
                        XmlDocument d = new XmlDocument();
                        d.InnerXml = output;
                        return d;                                                                               // Pass back the result of the report
                    }
                    catch (Exception ex)
                    {
                        _errorMessage = "Error: " + ex.Message;
                    }
                    return null;
                }
            }
            _errorMessage = "Report Execution: Error - No impormation back from command reporter";
            return null;
        }
        #endregion
    }
}
