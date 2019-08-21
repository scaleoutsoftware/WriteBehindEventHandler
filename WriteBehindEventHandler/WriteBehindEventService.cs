/* 
 * ScaleOut StateServer dedicated write-behind service sample.
 * 
 * Copyright 2013-2019 ScaleOut Software, Inc.
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
using System.Diagnostics;
using System.ServiceProcess;

using Soss.Client;
using Scaleout.Samples.Common;

namespace Scaleout.Samples.WriteBehindEventHandler
{
	/// <summary>
	/// A Windows service dedicated to handling write-behind events coming
	/// out of the ScaleOut StateServer service, where objects in the StateServer
	/// store need to be periodically persisted to a database. Using a service is
	/// an alternative to processing write-behind events directly in an ASP.NET app.
	/// </summary>
	public partial class WriteBehindEventService : ServiceBase
	{
		/// <summary>
		/// Public constructor.
		/// </summary>
		public WriteBehindEventService()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Executes when a Start command is sent to the service by the Service Control Manager.
		/// </summary>
		/// <param name="args">Data passed by the start command.</param>
		protected override void OnStart(string[] args)
		{
			try
			{
				// Reading service configuration settings from its configuration file
				Configuration.ReadConfigParams();

				///
				/// Configure this service to receive backing store events
				/// 
				NamedCache cache = CacheFactory.GetCache(Configuration.CACHE_NAME);
				BackingStorePolicy policy = new BackingStorePolicy();

				// We want to receive asynchronous write-behind events from StateServer:
				policy.EnableAsyncOperations = true;

				// Our customized IBackingStore implementation that knows how to write a cached
				// object to the database:
				BackingStoreAdapter adapter = new BackingStoreAdapter();

				// Register to handle backing store events:
				cache.SetBackingStoreAdapter(adapter, policy);
				Logger.WriteMessage(TraceEventType.Information, 1, $"[OnStart] Backing store adapter was successfully registered for handling events in named cache '{Configuration.CACHE_NAME}'");
			}
			catch (Exception ex)
			{
				Logger.WriteError(2, ex);
				throw;
			}
		}

		protected override void OnStop()
		{
		}

		public void Debug(string[] args)
		{
			OnStart(args);
		}
	}
}
