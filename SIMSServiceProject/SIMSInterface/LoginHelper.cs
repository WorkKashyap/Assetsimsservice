
// ---------------------------------------------------------------------------------
// Sample code provided to Capita SIMS Partners under consultancy (Hereafter 'partners')
// Written by Andy McGowan 30 Sep 2011
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
// Parts (c) Capita Group PLC 2011 in version info.
//
// Code samples may be used to support other partners and no exclusivity is implied herein 
//
// ---------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SIMSInterface
{
    /// <summary>
    /// Login helper class provides login and logout for SIMS.
    /// </summary>
    public class LoginHelper
    {
        private static bool mIsLoggedInToSIMS = false;
        private static string mSIMSUser = "";
        private static string mSIMSPassword = "";
        private static string mSIMSServer = "";
        private static string mSIMSDatabase = "";
        public static int mSignature = 0;
        private static string mErrorMessage = "";   // Used to pass exceptions back - latest only
        private static SIMS.Processes.Login mLoginProcess = null;
        /// <summary>
        /// SIMS Login function
        /// </summary>
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns>True if successful/</returns>
        public static bool SIMSlogin(string server, string database, string user, string password)
        {
            mSIMSServer = server;
            mSIMSDatabase = database;
            if (string.IsNullOrEmpty(user))
            {
                mSIMSUser = Environment.UserDomainName + "\\" + Environment.UserName;

            }
            else
            {
                mSIMSUser = user;
            }
            mSIMSPassword = password;
            if (mIsLoggedInToSIMS)
            {
                Logout();
            }
            mIsLoggedInToSIMS = false;
            try
            {
                //	
                // Reference required is LoginProcesses.dll
                //
                // Attempt to login
                if (mSIMSServer == "")
                {
                    mLoginProcess = new SIMS.Processes.Login();
                }
                else
                {
                    mLoginProcess = new SIMS.Processes.Login(mSIMSServer, mSIMSDatabase);
                }
                //mLoginProcess.

                mSignature = mLoginProcess.GetDatabaseSignature(mSIMSUser, mSIMSPassword, "TPA1", true);
                mLoginProcess.Init(mSignature, mSIMSUser, mSIMSPassword);
           
                //
                // References required are 
                //		Cache.dll
                //		CacheProcesses.dll
                //		AbstractProcesses.dll
                //		BaseInterfaces.dll
                //
                SIMS.Entities.DatabaseMode dm = new SIMS.Entities.DatabaseMode(false);
                SIMS.Entities.Cache.CurrentDatabase.DatabaseMode = dm;

                // It is essential that the third party conenction flag is set to avoide access being denied due to licensing.
                SIMS.Entities.Cache.ThirdPartyLogin = true;

                // If we get to this point we have been successful and can then build upon it.
              
                mIsLoggedInToSIMS = true;
              

            }
            catch (Exception ex)
            {
                WriteToFile(mErrorMessage);
                mErrorMessage = ex.Message;
            
            }
            return mIsLoggedInToSIMS;
        }
        /// <summary>
        /// If the login fails, this will return the exception
        /// </summary>
        public static string ErrorMessage { get { return mErrorMessage; } }
        /// <summary>
        /// It is essential that logout is called to avoid leaving cached information in the database and to 
        /// ensure that reports of who is logged in are as accurate as possible.
        /// That said, don't keep logging in and out, it is an expensive process.
        /// </summary>
        public static void Logout()
        {
            mIsLoggedInToSIMS = false;
            try
            {
                if (mSignature != 0)
                    mLoginProcess.Logout();
            }
            catch (Exception ex)
            {
                mErrorMessage = ex.Message;

            }
            mSignature = 0;
            mLoginProcess = null;
            return;

        }

        public static void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + System.DateTime.Now.Date.ToString("dd/MM/yyyy").Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

    }
}
