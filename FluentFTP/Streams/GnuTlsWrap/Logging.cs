﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GnuTlsWrap {
	public class Logging {

		public static Queue<string> logQueue;
		public static int logQueueMaxSize;
		public static int logMaxLevel;

		private static FtpGnuLib.FtpGnuStream.GnuStreamLogCBFunc logCBFunc;

		public static void Log(string msg) {
			Log(0, msg, true);
		}

		public static void LogNoQueue(string msg) {
			Log(0, msg, false);
		}

		public static void LogGnuFunc(string msg) {
			Log(1, "Interop : " + msg, false);
		}

		public static void Log(int lvl, string msg, bool q) {
			string s = lvl.ToString().PadRight(3) + " " + msg.TrimEnd(new char[] { '\n', '\r' });

			if (q) {
				if (logQueue.Count < logQueueMaxSize) {
					logQueue.Enqueue(s);
				}
				else {
					logQueue.Enqueue(s);
					logQueue.Dequeue();
				}
			}

			if (lvl > logMaxLevel) {
				return;
			}

			if (logCBFunc == null) {
				Console.WriteLine(s);
			}
			else {
				logCBFunc(s);
			}
		}

		private static void Log(int lvl, IntPtr msg) {
			string logMsg = "Internal: " + Marshal.PtrToStringAnsi(msg);
			//Static.Free(msg);
			Log(lvl, logMsg, true);
		}

		// This is the definition of the log call back that the GnuTLS.dll will call.
		// It is set up and defined in the main class FtpGnuStream
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void GnuTlsLogCBFunc(int level, IntPtr message);

		// Avoid garbage collection failure of this callback
		private static GnuTlsLogCBFunc gnuTlsLogCBFunc = Log;

		public Logging(FtpGnuLib.FtpGnuStream.GnuStreamLogCBFunc logCBFunc, int logQueueMaxSize, int logMaxLevel) {
			logQueue = new Queue<string>();

			Logging.logCBFunc = logCBFunc;
			Logging.logQueueMaxSize = logQueueMaxSize;
			Logging.logMaxLevel = logMaxLevel;

			Static.GlobalSetLogFunction(gnuTlsLogCBFunc);
			Static.GlobalSetLogLevel(99);
		}
	}
}
