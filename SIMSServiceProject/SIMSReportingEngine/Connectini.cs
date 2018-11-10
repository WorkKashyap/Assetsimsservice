
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
   
    public class Connectini
    {
        //Constants
        private const string CONNECT = "CONNECT.INI";
        private const string SERVER = "ServerName";
        private const string DATABASE = "DatabaseName";
        private const string REDIRECT = "Redirect";
        private const string SECTION = "SIMSConnection";
        private const string CONNECTIONTYPE = "ConnectionType";
        private static bool _initalised = false;
        private static string _Server = "";
        private static string _Database = "";
        private static string _ConnectionType = "";
        private static int _recursionDepth = 0;
        private static string _redirect = "";
        private static string _errorMessage = "";
        /// <summary>
        /// Read the values from the ini file and be prepared to recurse!
        /// </summary>
        private static void Initialise()
        {
            _initalised = true;
            if (_redirect == "")
            {
                _recursionDepth++;
                if (_recursionDepth > 10)
                {
                    return;  // Give up
                }
                _redirect = Path.Combine( SIMSIni.SIMSDotNetFolder,CONNECT) ;
                Initialise();  // recurse
            }
            _Server = iniFile.Read(_redirect, SECTION, SERVER);
            _Database = iniFile.Read(_redirect, SECTION, DATABASE);
            _ConnectionType = iniFile.Read(_redirect, SECTION, CONNECTIONTYPE);

        }
        /// <summary>
        /// Returns the last error message
        /// </summary>
        public static string ErrorMessage { get { return _errorMessage; } }
        /// <summary>
        /// Returns the SIMS Server Name
        /// </summary>
        public static string Server
        {
            get
            {
                if (!_initalised)
                {
                    Initialise();
                }
                return _Server;
            }
        }
        /// <summary>
        /// Returns the SIMS Database Name
        /// </summary>
        public static string Database
        {
            get
            {
                if (!_initalised)
                {
                    Initialise();
                }
                return _Database;
            }
        }
        /// <summary>
        /// Returns the Connection Type
        /// </summary>
        public static string ConnectionType
        {
            get
            {
                if (!_initalised)
                {
                    Initialise();
                }
                return _ConnectionType;
            }
        }
    }
}
