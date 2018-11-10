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
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SIMSInterface
{
    public class SIMSLocation
    {
        //Private Import to read INI Files
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// Simple mechanism to return the SIMS .Net directory.  Just wraps existing code from Vivo.
        /// </summary>
        public static string SIMSDotNetDirectory
        {
            get
            {
                string rc = "";
                try
                {
                    StringBuilder SIMSFolder = new StringBuilder();
                    string SIMSIniFile = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SIMS.INI");
                    if (File.Exists(SIMSIniFile))
                    {
                        GetPrivateProfileString("Setup", "SIMSDotNetDirectory", "", SIMSFolder, 0xff, SIMSIniFile);
                        if (!string.IsNullOrEmpty(SIMSFolder.ToString()))
                        {
                            return SIMSFolder.ToString();
                        }
                    }

                }
                catch
                {
                    // Almost doesn't matter why!
                }
                return rc;  // Empty string returned on failure
            }
        }
    }
}
