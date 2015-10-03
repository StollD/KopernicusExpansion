﻿using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

using Kopernicus;

namespace KopernicusExpansion
{
	public class KopernicusExpansionLogger : Logger
	{
		public static string LogDirectory
		{
			get{ return KSPUtil.ApplicationRootPath + "/Logs/KopernicusExpansion/"; }
		}

		public KopernicusExpansionLogger(string loggerName = "KopernicusExpansion", string extension = ".log")
		{
			try
			{
				string logFile = KopernicusExpansionLogger.LogDirectory + loggerName + extension;

				//manually set loggerStream using reflection
				var loggerStreamField = typeof(Logger).GetField ("loggerStream", BindingFlags.Instance | BindingFlags.NonPublic);
				loggerStreamField.SetValue (this, new StreamWriter (File.Create (logFile)));

				base.Log ("KopernicusExpansion logger created");
				base.Log ("");
			}
			catch (Exception e)
			{
				Utils.LogError ("Error creating KopernicusExpansionLogger:");
				Debug.LogException (e);
			}
		}

		public static void InitializeKopernicusExpansionLoggers()
		{
			try
			{
				//create new directory
				if(!Directory.Exists (LogDirectory))
					Directory.CreateDirectory (LogDirectory);

				//manually set isInitialized
				var isInitializedField = typeof(Logger).GetField ("isInitialized", BindingFlags.Static | BindingFlags.NonPublic);
				isInitializedField.SetValue (true, null);
			}
			catch(Exception e)
			{
				Utils.LogError ("Error initializing KopernicusExpansionLoggers, using backup init:");
				Debug.LogException (e);

				//backup, in case it fails
				Logger.Initialize ();
			}
		}
	}
}

