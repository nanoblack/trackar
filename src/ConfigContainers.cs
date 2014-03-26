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
		public float Travel;
		public float TravelCenter;
		public float Damper;

		public SuspConfigContainer()
		{
			Travel = 0;
			TravelCenter = 0;
			Damper = 0;
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
		public float BrakingTorque;
		public float RollingResistance;

		public SuspConfigContainer SuspConfig;

		public WheelDummyConfigContainer()
		{
			BrakingTorque = 0;
			RollingResistance = 0;
			SuspConfig = new SuspConfigContainer ();
			Debuggar.Message ("WheelDummyConfigContainer spawned: empty");
		}

		public WheelDummyConfigContainer(float brakingTorque, float rollingResistance)
		{
			BrakingTorque = brakingTorque;
			RollingResistance = rollingResistance;
			SuspConfig = new SuspConfigContainer ();
			Debuggar.Message ("WheelDummyConfigContainer spawned: BrakingTorque = " + BrakingTorque.ToString () + " RollingResistance = " + RollingResistance.ToString () + " SuspConfig = EMPTY");
		}

		public WheelDummyConfigContainer(float brakingTorque, float rollingResistance, SuspConfigContainer suspConfig)
		{
			BrakingTorque = brakingTorque;
			RollingResistance = rollingResistance;
			SuspConfig = suspConfig;
			Debuggar.Message ("WheelDummyConfigContainer spawned: BrakingTorque = " + BrakingTorque.ToString () + " RollingResistance = " + RollingResistance.ToString () + " SuspConfig = ASSIGNED");
		}
	}

	public class ModelConfigContainer
	{
		public string WheelCollider;
		public string WheelModel;
		public string TrackSurface;
		public string Joint;

		public ModelConfigContainer()
		{
			WheelCollider = "WheelCollider";
			WheelModel = "WheelModel";
			TrackSurface = "TrackSurface";
			Joint = "Joint";
			Debuggar.Message ("ModelConfigContainer spawned: defaults");
		}

		public ModelConfigContainer(string wheelCollider, string wheelModel, string trackSurface, string joint)
		{
			WheelCollider = wheelCollider;
			WheelModel = wheelModel;
			TrackSurface = trackSurface;
			Joint = joint;
			Debuggar.Message ("ModelConfigContainer spawned: WheelCollider = " + WheelCollider + " WheelModel = " + WheelModel + " TrackSurface = " + TrackSurface + " Joint = " + Joint);
		}
	}

	public class TrackConfigContainer
	{
		public float Width;
		public float Length;

		public ModelConfigContainer ModelConfig;

		public WheelDummyConfigContainer WheelDummyConfig;

		public TrackConfigContainer()
		{
			Width = 0;
			Length = 0;
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
			Debuggar.Message ("TrackConfigContainer spawned: Width = " + Width.ToString () + " Length = " + Length.ToString () + " ModelConfig = DEFAULT WheelDummyConfig = EMPTY");
		}

		public TrackConfigContainer(float width, float length, ModelConfigContainer modelConfig, WheelDummyConfigContainer wheelDummyConfig)
		{
			Width = width;
			Length = length;
			ModelConfig = modelConfig;
			WheelDummyConfig = wheelDummyConfig;
			Debuggar.Message ("TrackConfigContainer spawned: Width = " + Width.ToString () + " Length = " + Length.ToString () + " ModelConfig = ASSIGNED WheelDummyConfig = ASSIGNED");
		}
	}
}

