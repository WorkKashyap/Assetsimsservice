
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
using System.IO;

namespace SIMSReportingEngine
{
    public class SIMSIni
    {
        const string SIMSINI = "SIMS.INI";
        const string SECTION = "Setup";
        const string KEY = "SIMSDotNetDirectory";
        private static string _errorMessage = "";
        /// <summary>
        /// Returns the last error message
        /// </summary>
        public static string ErrorMessage { get { return _errorMessage; } }
        public static string SIMSDotNetFolder
        {
            get
            {
                _errorMessage = "";
                string file = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Windows), SIMSINI);
                if (! File.Exists(file))
                {
                    _errorMessage = "SIMS.INI File is missing: " + file;
                    return "";
                }
                return iniFile.Read(file, SECTION, KEY);
            }
        }

    }
}
