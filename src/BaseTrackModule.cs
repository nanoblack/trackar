using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


namespace Trackar
{
	public class BaseTrackModule : PartModule
	{
		public float TrackWidth = 1;

		public float TrackSections = 4;
		[KSPField]
		public float TrackLength = 15;

		public float TrackThickness = 0.0f;

		[KSPField]
		public float BrakingTorque = 1.0f;
		[KSPField]
		public float RollingResistance = 0.1f;
		[KSPField]
		public FloatCurve TorqueCurve = new FloatCurve();


		[KSPField]
		public string WheelModelName;
		[KSPField]
		public string WheelColliderName;
		[KSPField]
		public string TrackSurfaceName;
		[KSPField]
		public string SuspJointName;


		public bool bAreBrakesEngaged = false;

		public bool bIsMirrorInstance = false;

		public List<Track> Tracks = new List<Track>();

		[KSPField(guiActive = Debuggar.bIsDebugMode, guiName = "Cruise Mode")]
		public bool bIsCruiseEnabled = false;
		[KSPField(guiName = "Cruise Desired RPM", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float CruiseTargetRPM = 0;
		public KSPActionGroup CruiseActionGroup;

		[KSPField(guiActive = Debuggar.bIsDebugMode, guiActiveEditor = Debuggar.bIsDebugMode, guiName = "dbgTargetPosition"), UI_FloatRange(minValue = -4, maxValue = 0, stepIncrement = 0.25f)]
		public float dbgTargetPosition = 0;
		[KSPField(guiActive = Debuggar.bIsDebugMode, guiActiveEditor = Debuggar.bIsDebugMode, guiName = "dbgTravel"), UI_FloatRange(minValue = 0, maxValue = 4, stepIncrement = 0.25f)]
		public float dbgTravel = 0;
		[KSPField(guiActive = Debuggar.bIsDebugMode, guiActiveEditor = Debuggar.bIsDebugMode, guiName = "dbgDamping"), UI_FloatRange(minValue = -4, maxValue = 4, stepIncrement = 0.25f)]
		public float dbgDamping = 0;

		public TrackConfigContainer TrackConfig;

		public void InitBaseTrackModule()
		{
			SuspConfigContainer SuspConfig = new SuspConfigContainer (dbgTravel, dbgTargetPosition, dbgDamping);
			WheelDummyConfigContainer WheelDummyConfig = new WheelDummyConfigContainer (BrakingTorque, RollingResistance, SuspConfig);
			ModelConfigContainer ModelConfig = new ModelConfigContainer (WheelColliderName, WheelModelName, TrackSurfaceName, SuspJointName);

			TrackConfig = new TrackConfigContainer (TrackWidth, TrackLength, ModelConfig, WheelDummyConfig);
		}

		public override void OnStart(StartState state)
		{
			foreach (Transform tempCol in part.FindModelTransforms("sideCollider"))
				GameObject.Destroy (tempCol.gameObject);

			foreach (Transform tempCol in part.FindModelTransforms("temporaryCollider"))
				GameObject.Destroy (tempCol.gameObject);

			foreach (BaseAction action in Actions)
			{
				if (action.guiName.ToString () == "Toggle Cruise Control")
					CruiseActionGroup = action.actionGroup;
			}
			InitBaseTrackModule ();
			Debuggar.Message ("BaseTrackModule module successfully started");
		}

		[KSPAction("Brakes", KSPActionGroup.Brakes)]
		public void Brake(KSPActionParam param)
		{
			if (param.type == KSPActionType.Activate)
			{
				if (bIsCruiseEnabled)
				{
					this.vessel.ActionGroups.ToggleGroup (CruiseActionGroup);
				}

				foreach (Track track in Tracks)
					track.Brakes (true);
			}
			else
			{
				foreach (Track track in Tracks)
					track.Brakes (false);
			}
		}

		[KSPAction("Toggle Cruise Control", KSPActionGroup.None)]
		public void ToggleCruiseControl(KSPActionParam param)
		{
			if (param.type == KSPActionType.Activate)
			{
				bIsCruiseEnabled = true;
				foreach (Track track in Tracks)
					if (CruiseTargetRPM < track.RPM)
						CruiseTargetRPM = track.RPM;
			}
			else
			{
				bIsCruiseEnabled = false;
				CruiseTargetRPM = 0;
			}
		}

		public virtual void FixedUpdate ()
		{
			SuspConfigContainer suspConfig = TrackConfig.WheelDummyConfig.SuspConfig;
			suspConfig.Damper = dbgDamping;
			suspConfig.Travel = dbgTravel;
			suspConfig.TravelCenter = dbgTargetPosition;

			foreach (Track track in Tracks)
			{
				track.FixedUpdate ();
			}
		}

		public virtual void Update ()
		{
			if (HighLogic.LoadedSceneIsFlight && this.vessel.isActiveVessel)
			{
				foreach (Track track in Tracks)
					track.Update ();
			}
		}

		public override void OnLoad (ConfigNode node)
		{
			base.OnLoad (node);
		}

		public override void OnSave (ConfigNode node)
		{
			base.OnSave (node);
		}
	}
}