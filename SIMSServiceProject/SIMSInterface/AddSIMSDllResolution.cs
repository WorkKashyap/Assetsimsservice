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
using System.Reflection;
using System.IO;

namespace SIMSInterface
{
    /// <summary>
    /// A variant of the DLL resolution code from the plugins.  In this case it strictly uses the SIMS .Net folder as the source of SIMS DLLs
    /// </summary>
    public class SIMSDllResolution
    {
        static int resolverCount = 0;
        /// <summary>
        /// Make this function call from Main before much else is done.
        /// </summary>
        public static void AddSIMSDllResolution()
        {
            #region AL get the sims .net assemblies
            if (resolverCount == 0)
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.AssemblyResolve += currentDomain_AssemblyResolve;
                resolverCount++;
            }
            #endregion

        }

        #region AL get the sims .net assemblies
        private static Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //This handler is called only when the common language runtime tries to bind to the assembly and fails.

            //Retrieve the list of referenced assemblies in an array of AssemblyName.
            string strTempAssmbPath = string.Empty;
            string assemblyBeingResolved = args.Name.Substring(0, args.Name.IndexOf(","));

            Assembly objExecutingAssemblies = Assembly.GetExecutingAssembly();
            AssemblyName[] arrReferencedAssmbNames = objExecutingAssemblies.GetReferencedAssemblies();

            //Loop through the array of referenced assembly names.
            if (arrReferencedAssmbNames.Any(strAssmbName => strAssmbName.FullName.Substring(0, strAssmbName.FullName.IndexOf(",")) == assemblyBeingResolved))
            {
                strTempAssmbPath = assemblyPath(assemblyBeingResolved + ".dll");
            }
            Assembly MyAssembly = null;
            //Load the assembly from the specified path. 					
            if (!string.IsNullOrEmpty(strTempAssmbPath))
            {
                MyAssembly = Assembly.LoadFrom(strTempAssmbPath);
            }
            else
            {
                string path = assemblyPath(assemblyBeingResolved + ".dll");
                if (!String.IsNullOrEmpty(path))
                    MyAssembly = Assembly.LoadFrom(path);
            }
            //Return the loaded assembly.
            return MyAssembly;
        }

        private static string assemblyPath(string assemblyFileName)
        {
            string rc = string.Empty;
            string assemblyPathLocal = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), assemblyFileName);


            if (File.Exists(assemblyPathLocal))
                rc = assemblyPathLocal;
            else
            {
                assemblyPathLocal = Path.Combine(SIMSLocation.SIMSDotNetDirectory, assemblyFileName);

                if (File.Exists(assemblyPathLocal))
                {
                    rc = assemblyPathLocal;
                }
                else
                {
                    assemblyPathLocal = Path.Combine(Environment.CurrentDirectory, assemblyFileName);
                    if (File.Exists(assemblyPathLocal))
                    {
                        rc = assemblyPathLocal;
                    }
                }
            }

            return rc;
        }
        #endregion
    }
}
