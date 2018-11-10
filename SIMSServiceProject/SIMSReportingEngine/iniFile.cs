
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
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;


namespace SIMSReportingEngine
{
    class iniFile
    {
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        private static string _errorMessage = "";
        public static string ErrorMessage { get { return _errorMessage; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_fileName">The name of the ini file</param>
        /// <param name="_section">The ini file Section</param>
        /// <param name="_key">The required key</param>
        /// <returns>Will return "" if it fails</returns>
        public static string Read(string _fileName, string _section, string _key)
        {
            StringBuilder rc = new StringBuilder();
            try
            {
                if (File.Exists(_fileName))
                {
                    GetPrivateProfileString(_section, _key, "", rc, 1024, _fileName);
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
            return rc.ToString();
        }
        
    }
}
