/* 
 * ScaleOut StateServer dedicated write-behind service sample.
 * 
 * Copyright 2013-2023 ScaleOut Software, Inc.
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
using System.ServiceProcess;

namespace Scaleout.Samples.WriteBehindEventHandler
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			// Run with -debug flag the from command line or within Visual Studio.
			// Otherwise, install as a Windows Service using InstallUtil.exe .NET tool or
			// a setup project (Installshield, WiX Toolset, etc.).
			// See: https://msdn.microsoft.com/en-us/library/sd8zc8ha.aspx
			if (args.Length > 0 && args[0].EndsWith("debug", StringComparison.CurrentCultureIgnoreCase))
			{
				// Run from the command line for development/debugging purposes
				var service = new WriteBehindEventService();
				service.Debug(args);
				Console.WriteLine("The Write-Behind Event Handler process is up and running...");
				Console.WriteLine("Hit ENTER to stop.");
				Console.ReadLine();
				service.Stop();
			}
			else
			{
				// Run as Windows service
				ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[]
				{
					new WriteBehindEventService()
				};
				ServiceBase.Run(ServicesToRun);
			}
		}
	}
}
