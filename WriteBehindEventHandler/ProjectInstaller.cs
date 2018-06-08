/* 
 * ScaleOut StateServer dedicated write-behind service sample.
 * 
 * Copyright 2013-2018 ScaleOut Software, Inc.
 * 
 * LICENSE AND DISCLAIMER
 * ----------------------
 * This material contains sample programming source code ("Sample Code").
 * ScaleOut Software, Inc. (SSI) grants you a nonexclusive license to compile, 
 * link, run, display, reproduce, and prepare derivative works of 
 * this Sample Code.  The Sample Code has not been thoroughly
 * tested under all conditions.  SSI, therefore, does not guarantee
 * or imply its reliability, serviceability, or function. SSI
 * provides no support services for the Sample Code.
 *
 * All Sample Code contained herein is provided to you "AS IS" without
 * any warranties of any kind. THE IMPLIED WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGMENT ARE EXPRESSLY
 * DISCLAIMED.  SOME JURISDICTIONS DO NOT ALLOW THE EXCLUSION OF IMPLIED
 * WARRANTIES, SO THE ABOVE EXCLUSIONS MAY NOT APPLY TO YOU.  IN NO 
 * EVENT WILL SSI BE LIABLE TO ANY PARTY FOR ANY DIRECT, INDIRECT, 
 * SPECIAL OR OTHER CONSEQUENTIAL DAMAGES FOR ANY USE OF THE SAMPLE CODE
 * INCLUDING, WITHOUT LIMITATION, ANY LOST PROFITS, BUSINESS 
 * INTERRUPTION, LOSS OF PROGRAMS OR OTHER DATA ON YOUR INFORMATION
 * HANDLING SYSTEM OR OTHERWISE, EVEN IF WE ARE EXPRESSLY ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGES.
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Diagnostics;
using System.ServiceProcess;

namespace WriteBehindEventHandler
{
    /// <summary>
    /// This code is called by installation utilities, such as InstallUtil.exe, when installing a Windows
    /// service application. It installs an executable containing classes that extend ServiceBase (like 
    /// our WriteBehindEventService class). 
    /// </summary>
    /// <remarks>
    /// See: "How to: Add Installers to Your Service Application"
    /// http://msdn.microsoft.com/en-us/library/ddhy0byf(v=vs.110).aspx
    /// </remarks>
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            // We'll try to start up the write-behind service immediately after installation:
            try
            {
                ServiceController controller = new ServiceController(this.serviceInstaller.ServiceName);
                controller.Start();
            }
            catch (Exception ex)
            {
                String source = this.serviceInstaller.ServiceName + " Installer";
                String log = "Application";
                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, log);
                }

                EventLog eventLog = new EventLog();
                eventLog.Source = source;

                eventLog.WriteEntry("The service could not be started immediately after the installation. Please try to start the service manually. Error: " + ex.Message, EventLogEntryType.Error);
            }
        }
    }
}
