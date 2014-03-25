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

		public float TrackLength = 15;

		public float TrackThickness = 0.0f;

		[KSPField]
		public float BrakingTorque = 1.0f;
		[KSPField]
		public float RollingResistance = 0.1f;
		[KSPField]
		public FloatCurve TorqueCurve = new FloatCurve();


		[KSPField]
		public string WheelModelName = "RoadWheel";
		[KSPField]
		public string WheelColliderName = "WheelCollider";
		[KSPField]
		public string TrackSurfaceName = "TrackSurface";
		[KSPField]
		public string SuspJointName = "joint";


		public bool bAreBrakesEngaged = false;

		public bool bIsMirrorInstance = false;

		public List<Track> Tracks = new List<Track>();

		[KSPField(guiActive = Debuggar.bIsDebugMode, guiName = "Cruise Mode")]
		public bool bIsCruiseEnabled = false;
		[KSPField(guiName = "Cruise Desired RPM", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float CruiseTargetRPM = 0;
		public KSPActionGroup CruiseActionGroup;

		public ConfigContainer TrackConfig;

		public void BuildTrackConfig()
		{
			TrackConfig.TrackSections = TrackSections;
			TrackConfig.TrackWidth = TrackWidth;
			TrackConfig.TrackThickness = TrackThickness;
			TrackConfig.BrakingTorque = BrakingTorque;
			TrackConfig.RollingResistance = RollingResistance;
			TrackConfig.TrackLength = TrackLength;

			TrackConfig.WheelModelName = WheelModelName;
			TrackConfig.WheelColliderName = WheelColliderName;
			TrackConfig.TrackSurfaceName = TrackSurfaceName;
			TrackConfig.SuspJointName = SuspJointName;

			TrackConfig.bIsDoubleTrackPart = true;

			TrackConfig.LeftTrackRoot = "";
			TrackConfig.RightTrackRoot = "";

			TrackConfig.SingleTrackRoot = "";
		}

		public void DispatchAnimsUpdate()
		{
			foreach(Track track in Tracks)
				track.DoMovementAnims ();
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

			BuildTrackConfig ();
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
					track.EngageBrake ();
			}
			else
			{
				foreach (Track track in Tracks)
					track.ReleaseBrake ();
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
			//DispatchSuspension ();
		}

		public override void OnUpdate ()
		{
			if(HighLogic.LoadedSceneIsFlight && this.vessel.isActiveVessel)
				DispatchAnimsUpdate ();

			if(HighLogic.LoadedSceneIsFlight || HighLogic.LoadedSceneIsEditor)
				//DispatchProceduralUpdate ();

			base.OnUpdate ();
		}

		public override void OnLoad (ConfigNode node)
		{
			//DispatchProceduralUpdate ();
			base.OnLoad (node);
		}

		public override void OnSave (ConfigNode node)
		{
			base.OnSave (node);
		}
	}
}