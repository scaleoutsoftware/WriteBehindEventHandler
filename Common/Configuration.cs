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
using System.Configuration;

namespace Scaleout.Samples.Common
{
	public static class Configuration
	{
		public const string CACHE_NAME = "SampleCache";
		public static string CONNECTION_STRING = "Data Source=localhost;Initial Catalog=BackingStoreSample; Integrated Security=True; MultipleActiveResultSets=true";
		public static bool USE_DATABASE = true;

		/// <summary>
		/// The helper method that checks and reads all supported configuration parameters
		/// from the .config file.
		/// </summary>
		public static void ReadConfigParams()
		{
			try
			{
				if (ConfigurationManager.AppSettings["UseDatabase"] != null)
					Scaleout.Samples.Common.Configuration.USE_DATABASE = Convert.ToBoolean(ConfigurationManager.AppSettings["UseDatabase"]);
				if (ConfigurationManager.AppSettings["ConnectionString"] != null)
					Scaleout.Samples.Common.Configuration.CONNECTION_STRING = ConfigurationManager.AppSettings["ConnectionString"];
			}
			catch { }
		}
	}
}
