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
			Debuggar.Message ("SuspConfigContainer: Spawned empty");
		}

		public SuspConfigContainer(float travel, float travelCenter, float damper)
		{
			Travel = travel;
			TravelCenter = travelCenter;
			Damper = damper;

			Debuggar.Message ("SuspConfigContainer: Spawned with Travel = " + Travel.ToString () + " TravelCenter = " + TravelCenter.ToString () + " Damper = " + Damper.ToString ());
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
			Debuggar.Message ("WheelDummyConfigContainer: Spawned empty");
		}

		public WheelDummyConfigContainer(float brakingTorque, float rollingResistance)
		{
			BrakingTorque = brakingTorque;
			RollingResistance = rollingResistance;

			SuspConfig = new SuspConfigContainer ();
			Debuggar.Message ("WheelDummyConfigContainer: Spawned with BrakingTorque = " + BrakingTorque.ToString () + " RollingResistance = " + RollingResistance.ToString ());
		}

		public WheelDummyConfigContainer(float brakingTorque, float rollingResistance, SuspConfigContainer suspConfig)
		{
			BrakingTorque = brakingTorque;
			RollingResistance = rollingResistance;

			if (suspConfig != null)
				SuspConfig = suspConfig;
			else
				SuspConfig = new SuspConfigContainer ();

			Debuggar.Message ("WheelDummyConfigContainer: Spawned with BrakingTorque = " + BrakingTorque.ToString () + " RollingResistance = " + RollingResistance.ToString ());
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
			Debuggar.Message ("ModelConfigContainer: Spawned with defaults");
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

			Debuggar.Message ("ModelConfigContainer: Spawned with WheelCollider = " + WheelCollider + " WheelModel = " + WheelModel + " TrackSurface = " + TrackSurface + " Joint = " + Joint);
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
			Debuggar.Message ("TrackConfigContainer: Spawned empty");
		}

		public TrackConfigContainer(float width, float length)
		{
			Width = width;
			Length = length;

			ModelConfig = new ModelConfigContainer ();
			WheelDummyConfig = new WheelDummyConfigContainer ();
			Debuggar.Message ("TrackConfigContainer: Spawned with Width = " + Width.ToString () + " Length = " + Length.ToString ());
		}

		public TrackConfigContainer(float width, float length, ModelConfigContainer modelConfig, WheelDummyConfigContainer wheelDummyConfig)
		{
			Width = width;
			Length = length;

			if (modelConfig != null)
				ModelConfig = modelConfig;
			else
				ModelConfig = new ModelConfigContainer ();

			if(wheelDummyConfig != null)
				WheelDummyConfig = wheelDummyConfig;
			else
				WheelDummyConfig = new WheelDummyConfigContainer ();

			Debuggar.Message ("TrackConfigContainer: Spawned with Width = " + Width.ToString () + " Length = " + Length.ToString ());
		}
	}
}

