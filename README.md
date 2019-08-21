# ScaleOut write-behind event handling service sample


ScaleOut StateServer can be configured to automatically update a database with changes made to objects in the store. This sample project illustrates how to write a dedicated, long-running Windows Service process that handles these write-behind events for objects that reside in the ScaleOut StateServer service.

There are several advantages of handling write-behind events in a dedicated process that runs locally on each host StateServer host instead of handling these events in your main client application (which is often hosted in an IIS worker process):

 - Improved performance by reducing network usage: The ScaleOut service automatically routes events to the local event handling process.
 - Predictable process lifetime: IIS's w3wp.exe processes are designed to be ephemeral. For example, IIS might decide to stop the worker process because it has been idle for too long, causing missed events if another remote client isn't available to take over.
 - It protects your web app: Generally, you only want your w3wp.exe process to be concerned with handling web requests. Event handling logic is run on a thread that's outside of the web request pipeline, so an unhandled exception in your event handling code would bring down the whole w3wp.exe worker process.
 - It matches the availability/scalability model of hosts in the SOSS cluster: the count/lifetime of event handling services is the same as the count/lifetime of SOSS hosts. You don't need to be concerned about having enough instances of your main client application running to handle backing store events.
 - The code to handle write-behind events typically has a connection open to an application's database. It's a best practice to not access a database from application's web tier--a more secure approach is to access the database from ScaleOut hosts, which must run in secured network subnet and will be closer to database location.

## Usage

The WriteBehindEventHandler sample consists of three Visual Studio projects: the first one is the Windows service project itself, which should be installed and run locally on each ScaleOut server to capture and process write-behind events. The second project is a client test application that adds/updates objects to the ScaleOut service to trigger write-behind events. You can observe the changes triggered by this application either via the SQL Profiler or by looking at the log file produced by the service. The third project is a shared C# library that contains classes used by both the write-behind event handling service and the client application.

 - Specify the name of your named cache in the Configuration's `CACHE_NAME` field.
 - Specify connection string to your application database in the app.config configuration file.
 - Create the sample table and two stored procedures by running the SQL statements defined in the WriteBehindEventHandler_DatabaseObjects.sql file.
 - Customize the `WriteBehindEventHandler.BackingStoreAdapter` class methods with your custom write-behind event handling logic.
 - Modify the properties of ProjectInstaller.cs to customize the behavior of Windows service--for example, the name of the Windows Service and the account that it runs under can be changed in the VS designer.
 - Use [installutil.exe](https://msdn.microsoft.com/en-us/library/sd8zc8ha.aspx) to install the service on each of your hosts running the ScaleOut StateServer service.
   - Note that the executable can be started with a `-debug` argument during development/debugging if you need to run the process from the command line or from Visual Studio.

## Prerequisites

 - .NET 4.7.2 or higher.
 - [ScaleOut StateServer](https://www.scaleoutsoftware.com/products/stateserver/), [ScaleOut ComputeServer](https://www.scaleoutsoftware.com/products/computeserver/) or [ScaleOut StreamServer](https://www.scaleoutsoftware.com/products/streamserver/) (an instance of this service should be run locally on every ScaleOut host).
