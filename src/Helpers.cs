using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


namespace Trackar
{
	// lol lonely ol' forgotten struct with one var
	public struct ProcVars
	{
		public float Width;
	}

	public static class Debuggar
	{
		public const bool bIsDebugMode = false;

		public static void Message(string message)
		{
			//if (bIsDebugMode)
			Debug.Log("Trackar: " + message);
		}

		public static void Error(string message)
		{
			//if (bIsDebugMode)
			Debug.Log ("Trackar GURU MEDITATION: " + message);
		}
	}
}

