//=============================================================
// Nope can't be me, get your freak on, you can suck it
// They've all seen it and you liked it, now he wants it 
// Cotton candy suga high
//=============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


namespace Trackar
{
	/*public struct ConfigContainer
	{
		public float TrackWidth;
		public float TrackSections;
		public float TrackLength;
		public float TrackThickness;

		public float BrakingTorque;
		public float RollingResistance;

		public string WheelColliderName;
		public string WheelModelName;
		public string TrackSurfaceName;
		public string SuspJointName;

		public bool bIsDoubleTrackPart;

		//public string LeftTrackRoot;
		//public string RightTrackRoot;

		//public string SingleTrackRoot;

		public SuspConfig Suspension;
	}*/

	public struct ProcVars
	{
		public float Width;
	}

	public static class Debuggar
	{
		public const bool bIsDebugMode = true;

		public static void Message(string message)
		{
			if (bIsDebugMode)
				Debug.Log("Trackar: " + message);
		}

		public static void Error(string message)
		{
			if (bIsDebugMode)
				Debug.Log ("Trackar GURU MEDITATION: " + message);
		}
	}
}

