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
using System.Data;
using System.Data.SqlClient;

using Soss.Client;
using System.IO;
using Scaleout.Samples.Common;
using System.Diagnostics;

namespace WriteBehindEventHandler
{
    /// <summary>
    /// A custom IBackingStore implementation that knows how to persist and retrieve cached objects to/from a DB.
    /// </summary>
    /// <remarks>
	/// See: http://static.scaleoutsoftware.com/docs/soss_CacheAPIdoc/html/T_Soss_Client_IBackingStore.htm
    /// </remarks>
    class BackingStoreAdapter : IBackingStore
    {
		// Sample uses one table and two stored procedures defined in the WriteBehindEventHandler_DatabaseObjects.sql file.
		// The USE_DATABASE and CONNECTION_STRING properties are defined in the Configuration class, that is part of
		// the Common project and can be overwritten by corresponding settings in the app.config file of the WriteBehindEventHandler service.
		#region IBackingStore Members

		/// <summary>
		/// Persists an object to the backing store. 
		/// </summary>
		/// <param name="id">The StateServer identifier of the object to persist.</param>
		/// <param name="value">The cached object to be written to the backing store.</param>
		public void Store(CachedObjectId id, object value)
        {
			// The value parameter is the object that's been stored in the cache and needs to be written to the DB.
			// In this example, the incoming value will be of the SampleObject type.

			try
			{
				if (Configuration.USE_DATABASE)
				{
					SampleObject saObj = value as SampleObject;
					if (saObj != null)
					{
						using (SqlConnection conn = new SqlConnection(Configuration.CONNECTION_STRING))
						{
							conn.Open();

							SqlCommand cmd = new SqlCommand("AddOrUpdate", conn);
							cmd.CommandType = CommandType.StoredProcedure;
							cmd.Parameters.Add(new SqlParameter("@ID", Convert.ToInt32(id.GetStringKey())));
							cmd.Parameters.Add(new SqlParameter("@IntVal", saObj.IntVal));
							cmd.Parameters.Add(new SqlParameter("@DoubleVal", saObj.DoubleVal));
							cmd.Parameters.Add(new SqlParameter("@StringVal", saObj.StringVal));

							// Execute the command
							cmd.ExecuteNonQuery();
						}
					}
				}

				Logger.WriteMessage(TraceEventType.Information, 10, $"The Store method is called for object {id.ToString()} with value {value.ToString()}");
			}
			catch (Exception ex)
			{
				Logger.WriteMessage(TraceEventType.Error, 11, $"Exception occurred in the Store method: {ex.ToString()}");
			}                        
        }


        /// <summary>
        /// Removes an object from the backing store. 
        /// </summary>
        /// <param name="id">The StateServer identifier of the object to delete from the backing store.</param>
        public void Erase(CachedObjectId id)
        {
			// Write-behind will call this Erase implementation if an object is removed from the named cache.
			// If you want the object removed from the DB in that situation, this is the place to do it:

			try
			{
				if (Configuration.USE_DATABASE)
				{
					using (SqlConnection conn = new SqlConnection(Configuration.CONNECTION_STRING))
					{
						conn.Open();

						SqlCommand cmd = new SqlCommand("Delete", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add(new SqlParameter("@ID", Convert.ToInt32(id.GetStringKey())));

						// Execute the command
						cmd.ExecuteNonQuery();
					}
				}

				Logger.WriteMessage(TraceEventType.Information, 12, $"The Erase method is called for object {id.GetStringKey()}");
			}
			catch (Exception ex)
			{
				Logger.WriteMessage(TraceEventType.Error, 13, $"Exception occurred in the Erase method: {ex.ToString()}");
			}
		}

        /// <summary>
        /// Loads an object from the backing store. Return null if there is no value in the backing store for the specified id. 
        /// </summary>
        /// <param name="id">The identifier of the object to retrieve from the backing store.</param>
        /// <returns>The object from the backing store that is to be loaded into ScaleOut StateServer, or null if the object was not found.</returns>
        public object Load(CachedObjectId id)
        {
			// This Load method won't ever be invoked because we aren't performing Refresh-Ahead in this sample. 
			// For read-through and/or refresh-ahead approaches, this method should:
			// 1. Obtain data from the database for specified object's Id.
			// 2. Create and return object back to application.

			return null;
        }


        /// <summary>
        /// Provides a policy object to be used when a Load(CachedObjectId) operation inserts an object into the named cache.
        /// If the method returns null then the named cache's DefaultCreatePolicy will be used. 
        /// </summary>
        /// <param name="id">The identifier of the object being loaded into the cache.</param>
        /// <returns>
        /// The CreatePolicy to be used when inserting the specified object into the cache,
        /// or null if the named cache's DefaultCreatePolicy is to be used.
        /// </returns>
        public CreatePolicy GetCreatePolicy(CachedObjectId id)
        {
			// This GetCreatePolicy method is only called when we're handling read-through events.
			// If we did need to implement it, however, returning null causes read-through to use
			// the NamedCache's default create policy when adding an object to the cache.
			return null;

			// Here is how you can create and return CreatePolicy class instance with object specific characteristics:
			//
			// var policy = new CreatePolicy();
			// policy.BackingStoreMode = BackingStoreAsyncPolicy.WriteBehind;
			// policy.BackingStoreInterval = TimeSpan.FromSeconds(30);
			// policy.Timeout = new TimeSpan(0, 5, 0); // 5 minutes
			// return policy;
        }
        #endregion
    }
}
