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
using System.IO;
using System.Diagnostics;

namespace Scaleout.Samples.Common
{
	/// <summary>
	/// Light-weight logger class associated with the WriteBehindEventService
	/// TraceSource object defined in the app.config file of the 
	/// WriteBehindEventHandler service.
	/// </summary>
	public class Logger
	{
		// Trace source associated with the WriteBehindEventHandler service:
		static TraceSource _tSource = new TraceSource("WriteBehindEventService", defaultLevel: SourceLevels.Information);

		public static void WriteMessage(TraceEventType eType, int msgId, string message)
		{
			_tSource.TraceEvent(eType, msgId, message);
			Debug.WriteLine(message);
		}

		public static void WriteError(int msgId, Exception ex)
		{
			_tSource.TraceData(TraceEventType.Error, msgId, ex);
			Debug.WriteLine($"Exception: {ex.Message}");
		}

		public static void Log(string logMessage, TextWriter w)
		{
			w.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssK")}] {logMessage}");
		}
	}
}
