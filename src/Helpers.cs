//=============================================================
// Straw dumb blonde, brown licorice candy girl I is
// Dot connector, paint a picture
// I'm not in it, never will be 
//=============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


namespace Trackar
{
	public struct ConfigContainer
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

		public string LeftTrackRoot;
		public string RightTrackRoot;

		public string SingleTrackRoot;

		public SuspConfig Suspension;
	}

	public class SuspConfig
	{
		public float Travel;
		public float TravelCenter;
		public float Damper;
	}

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
	}
}

