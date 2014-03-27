//=============================================================
// All you do is take, take, takin' off the little that
// You got on, goin' on, I'd rather be eating glass
// Stick hot needles in my eyes, go ahead and tap that ass 
//=============================================================

using System;


namespace Trackar
{
	public class SuspConfigContainer
	{
		public float Travel = 0;
		public float TravelCenter = 0;
		public float Damper = 0;

		public SuspConfigContainer()
		{
			Debuggar.Message ("SuspConfigContainer spawned: empty");
		}

		public SuspConfigContainer(float travel, float travelCenter, float damper)
		{
			Travel = travel;
			TravelCenter = travelCenter;
			Damper = damper;
			Debuggar.Message ("SuspConfigContainer spawned: Travel = " + Travel.ToString () + " TravelCenter = " + TravelCenter.ToString () + " Damper = " + Damper.ToString ());
		}
	}

	public class WheelDummyConfigContainer
	{
		public float BrakingTorque = 0;
		public float RollingResistance = 0;

		public SuspConfigContainer SuspConfig;

		public WheelDummyConfigContainer()
		{
			SuspConfig = new SuspConfigContainer ();
			Debuggar.Message ("WheelDummyConfigContainer spawned: empty");
		}

		public WheelDummyConfigContainer(float brakingTorque, float rollingResistance)
		{
			BrakingTorque = brakingTorque;
			RollingResistance = rollingResistance;
			SuspConfig = new SuspConfigContainer ();
			Debuggar.Message ("WheelDummyConfigContainer spawned: BrakingTorque = " + BrakingTorque.ToString () + " RollingResistance = " + RollingResistance.ToString ());
		}

		public WheelDummyConfigContainer(float brakingTorque, float rollingResistance, SuspConfigContainer suspConfig)
		{
			BrakingTorque = brakingTorque;
			RollingResistance = rollingResistance;
			if (suspConfig != null)
			{
				SuspConfig = suspConfig;
			}
			else
			{
				Debuggar.Error ("Attempted to spawn a WheelDummyConfigContainer with a null SuspConfigContainer");
				SuspConfig = new SuspConfigContainer ();
			}
			Debuggar.Message ("WheelDummyConfigContainer spawned: BrakingTorque = " + BrakingTorque.ToString () + " RollingResistance = " + RollingResistance.ToString ());

		}
	}

	public class ModelConfigContainer
	{
		public string WheelCollider = "WheelCollider";
		public string WheelModel = "RoadWheel";
		public string TrackSurface = "TrackSurface";
		public string Joint = "joint";

		public ModelConfigContainer()
		{
			Debuggar.Message ("ModelConfigContainer spawned: defaults");
		}

		public ModelConfigContainer(string wheelCollider, string wheelModel, string trackSurface, string joint)
		{
			if(wheelCollider != null && wheelCollider != "")
				WheelCollider = wheelCollider;

			if(wheelModel != null && wheelModel != "")
				WheelModel = wheelModel;

			if(trackSurface != null && trackSurface != "")
				TrackSurface = trackSurface;

			if(joint != null && joint != "")
				Joint = joint;

			Debuggar.Message ("ModelConfigContainer spawned: WheelCollider = " + WheelCollider + " WheelModel = " + WheelModel + " TrackSurface = " + TrackSurface + " Joint = " + Joint);
		}
	}

	public class TrackConfigContainer
	{
		public float Width = 0;
		public float Length = 0;

		public ModelConfigContainer ModelConfig;

		public WheelDummyConfigContainer WheelDummyConfig;

		public TrackConfigContainer()
		{
			ModelConfig = new ModelConfigContainer ();
			WheelDummyConfig = new WheelDummyConfigContainer ();
			Debuggar.Message ("TrackConfigContainer spawned: empty");
		}

		public TrackConfigContainer(float width, float length)
		{
			Width = width;
			Length = length;
			ModelConfig = new ModelConfigContainer ();
			WheelDummyConfig = new WheelDummyConfigContainer ();
			Debuggar.Message ("TrackConfigContainer spawned: Width = " + Width.ToString () + " Length = " + Length.ToString ());
		}

		public TrackConfigContainer(float width, float length, ModelConfigContainer modelConfig, WheelDummyConfigContainer wheelDummyConfig)
		{
			Width = width;
			Length = length;
			if (modelConfig != null)
			{
				ModelConfig = modelConfig;
			}
			else
			{
				Debuggar.Error ("Attempted to spawn a TrackConfigContainer with a null ModelConfig");
				ModelConfig = new ModelConfigContainer ();
			}
			if(wheelDummyConfig != null)
			{
				WheelDummyConfig = wheelDummyConfig;
			}
			else
			{
				Debuggar.Error ("Attempted to spawn a TrackConfigContainer with a null WheelDummyConfig");
				WheelDummyConfig = new WheelDummyConfigContainer ();
			}
			Debuggar.Message ("TrackConfigContainer spawned: Width = " + Width.ToString () + " Length = " + Length.ToString ());
		}
	}
}

