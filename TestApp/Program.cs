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
using System.Threading;

using Soss.Client;
using Scaleout.Samples.Common;

namespace Scaleout.Samples.TestApp
{
	/// <summary>
	/// Test client application that simulates how a client app might insert and modify an 
	/// object in the StateServer cache. If running, the WriteBehind service project should 
	/// pick up changes to the object made by this app and persist them to a DB.
	/// </summary>
	class Program
	{
		/// <summary>
		/// Entry point to the application
		/// </summary>
		static void Main(string[] args)
		{
			SampleObject obj = null;
			int objectKey = 12345;
			int iteration = 0;
			string[] counting = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" };
			int objectTimeoutInSeconds = counting.Length; // 10 seconds

			NamedCache cache = CacheFactory.GetCache(Configuration.CACHE_NAME);

			// Unlike read-through and write-through operations (which are enabled on an entire NamedCache through the backingStorePolicy parameter), 
			// the asynchronous write-behind and refresh-ahead operations must be enabled on an object-by-object basis when they are first created 
			// in the ScaleOut StateServer cache. We use the BackingStoreMode property below to enable write-behind operation for an object. 
			// We're setting this property on the DefaultCreatePolicy level, so all objects added to this named cache will inherit this setting.
			// Note that the BackingStoreInterval property controls the interval between asynchronous backing store events.
			cache.DefaultCreatePolicy.BackingStoreMode = BackingStoreAsyncPolicy.WriteBehind;
			cache.DefaultCreatePolicy.BackingStoreInterval = TimeSpan.FromSeconds(5);

			cache.DefaultCreatePolicy.Timeout = TimeSpan.FromSeconds(objectTimeoutInSeconds);
			cache.DefaultCreatePolicy.IsAbsoluteTimeout = true;

			// Perform a few add/read operations
			while (iteration < 4)
			{
				int year = DateTime.Now.Year + iteration;
				double fraction = (DateTime.Now.Month < 10) ? ((double)DateTime.Now.Month/10) : ((double)DateTime.Now.Month/100);

				obj = new SampleObject(year, ((double)year + fraction), $"iteration {iteration}");
				// Add or Update object to the store
				cache.Insert(objectKey.ToString(), obj, cache.DefaultCreatePolicy, updateIfExists: true, lockAfterInsert: false);

				Thread.Sleep(2000);
				// Reading object from the store
				obj = cache.Retrieve(objectKey.ToString(), false) as SampleObject;
				if (obj != null)
					Console.WriteLine($"Added and retrieved the sample object {obj.ToString()} with key {objectKey}");

				Thread.Sleep(2000);
				iteration++;
			}

			// Waiting for our test object to expire
			Console.WriteLine($"Wait for {objectTimeoutInSeconds} seconds to make sure the object is expired, counting...");
			foreach (var countItem in counting)
			{
				Console.WriteLine(countItem);
				Thread.Sleep(1000);
			}

			Console.WriteLine("Reading object from the store:");
			obj = cache.Retrieve(objectKey.ToString(), acquireLock: false) as SampleObject;
			if (obj == null)
				Console.WriteLine("\tObject is null as expected, re-adding it back.");
			else
				Console.WriteLine($"\tObject is still present, key: {obj.ToString()}, updating it.");

			// Final step - re-adding it back to observe a pair of write-behind events associated with the object (store and erase) one more time
			obj = new SampleObject(988, 988.6, $"June, 988");
			cache.Insert(objectKey.ToString(), obj, cache.DefaultCreatePolicy, updateIfExists: true, lockAfterInsert: false);

			Console.WriteLine($"\nThe sample object {obj.ToString()} was re-added back to the cache.");
			Console.WriteLine($"You should still observe 2 backing store events fired:\n   one is for adding object back and another one is for removing it due to its expiration in {objectTimeoutInSeconds} seconds.");
			Console.WriteLine("\nPress Enter to finish the application.");
			Console.ReadLine();
		}
	}
}
