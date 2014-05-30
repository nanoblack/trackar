using System;


namespace Trackar
{
	public class SuspConfigContainer
	{
		public float Travel = 0;
		public float TravelCenter = 0;
		public float Spring = 0;
		public float Damper = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="Trackar.SuspConfigContainer"/> class.
		/// </summary>
		public SuspConfigContainer()
		{
			Debuggar.Message ("SuspConfigContainer: Spawned empty");
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Trackar.SuspConfigContainer"/> class.
		/// </summary>
		/// <param name="travel">Travel.</param>
		/// <param name="travelCenter">Travel center.</param>
		/// <param name="damper">Damper.</param>
		/// <param name="spring">Spring.</param>
		public SuspConfigContainer(float travel, float travelCenter, float damper, float spring)
		{
			Travel = travel;
			TravelCenter = travelCenter;
			Damper = damper;
			Spring = spring;

			Debuggar.Message ("SuspConfigContainer: Spawned with Travel = " + Travel.ToString () + " TravelCenter = " + TravelCenter.ToString () + " Damper = " + Damper.ToString () + " Spring = " + Spring.ToString());
		}
	}

	public class WheelDummyConfigContainer
	{
		public float BrakingTorque = 0;
		public float RollingResistance = 0;

		public SuspConfigContainer SuspConfig;

		/// <summary>
		/// Initializes a new instance of the <see cref="Trackar.WheelDummyConfigContainer"/> class.
		/// </summary>
		public WheelDummyConfigContainer()
		{
			SuspConfig = new SuspConfigContainer ();
			Debuggar.Message ("WheelDummyConfigContainer: Spawned empty");
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Trackar.WheelDummyConfigContainer"/> class.
		/// </summary>
		/// <param name="brakingTorque">Braking torque.</param>
		/// <param name="rollingResistance">Rolling resistance.</param>
		public WheelDummyConfigContainer(float brakingTorque, float rollingResistance)
		{
			BrakingTorque = brakingTorque;
			RollingResistance = rollingResistance;

			SuspConfig = new SuspConfigContainer ();
			Debuggar.Message ("WheelDummyConfigContainer: Spawned with BrakingTorque = " + BrakingTorque.ToString () + " RollingResistance = " + RollingResistance.ToString ());
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Trackar.WheelDummyConfigContainer"/> class.
		/// </summary>
		/// <param name="brakingTorque">Braking torque.</param>
		/// <param name="rollingResistance">Rolling resistance.</param>
		/// <param name="suspConfig">SuspConfigContainer.</param>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="Trackar.ModelConfigContainer"/> class.
		/// </summary>
		public ModelConfigContainer()
		{
			Debuggar.Message ("ModelConfigContainer: Spawned with defaults");
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Trackar.ModelConfigContainer"/> class.
		/// </summary>
		/// <param name="wheelCollider">Wheel collider.</param>
		/// <param name="wheelModel">Wheel model.</param>
		/// <param name="trackSurface">Track surface.</param>
		/// <param name="joint">Joint.</param>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="Trackar.TrackConfigContainer"/> class.
		/// </summary>
		public TrackConfigContainer()
		{
			ModelConfig = new ModelConfigContainer ();
			WheelDummyConfig = new WheelDummyConfigContainer ();
			Debuggar.Message ("TrackConfigContainer: Spawned empty");
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Trackar.TrackConfigContainer"/> class.
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="length">Length.</param>
		public TrackConfigContainer(float width, float length)
		{
			Width = width;
			Length = length;

			ModelConfig = new ModelConfigContainer ();
			WheelDummyConfig = new WheelDummyConfigContainer ();
			Debuggar.Message ("TrackConfigContainer: Spawned with Width = " + Width.ToString () + " Length = " + Length.ToString ());
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Trackar.TrackConfigContainer"/> class.
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="length">Length.</param>
		/// <param name="modelConfig">ModelConfigContaner.</param>
		/// <param name="wheelDummyConfig">WheelDummyConfigContainer.</param>
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

