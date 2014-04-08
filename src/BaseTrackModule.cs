//=============================================================
// UNSTABLE
//=============================================================

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
		public float TrackLength;

		public float TrackThickness = 0.0f;

		[KSPField]
		public float BrakingTorque;
		[KSPField]
		public float RollingResistance;
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

		[KSPField]
		public float SuspensionDamper;
		[KSPField]
		public float SuspensionSpring;
		[KSPField]
		public float SuspensionHeight;

		//private bool bAreBrakesEngaged = false;

		protected bool bIsMirrorInstance = false; // TODO: does this serve any useful purpose at the moment?

		[KSPField]
		public float ConsumeResourceRate = 0.25f;
		[KSPField]
		public string ConsumedResource = "ElectricCharge";

		protected List<Track> Tracks = new List<Track>();

		[KSPField(guiActive = Debuggar.bIsDebugMode, guiName = "Cruise Mode")]
		public bool bIsCruiseEnabled = false;
		[KSPField(guiName = "Cruise RPM", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float CruiseTargetRPM = 0;
		private KSPActionGroup CruiseActionGroup;

		//[KSPField(guiName = "Suspension Damping", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float dbgSuspensionDamping = 0;
		[KSPField(guiActive = true, guiActiveEditor = true, guiName = "Damping") , UI_FloatRange(minValue = 0, maxValue = 15, stepIncrement = 1f)]
		public float SuspensionDampingAdjustment = 0;

		//[KSPField(guiName = "Suspension Spring", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float dbgSuspensionSpring = 0;
		[KSPField(guiActive = true, guiActiveEditor = true, guiName = "Spring") , UI_FloatRange(minValue = 0, maxValue = 100, stepIncrement = 1)]
		public float SuspensionSpringAdjustment = 0;

		//[KSPField(guiName = "Suspension Target Position", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float dbgSuspensionTargetPos = 0;
		//[KSPField(guiActive = true, guiActiveEditor = true, guiName = "Target Position Adjust") , UI_FloatRange(minValue = 0, maxValue = 1, stepIncrement = 0.1f)]
		public float SuspensionTargetPosAdjustment = 0;

		//[KSPField(guiName = "Suspension Travel", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float dbgSuspensionTravel = 0;
		[KSPField(guiActive = true, guiActiveEditor = true, guiName = "Height") , UI_FloatRange(minValue = 0, maxValue = 2, stepIncrement = 0.1f)]
		public float SuspensionTravelAdjustment = 0;

		protected TrackConfigContainer TrackConfig;

		protected void InitBaseTrackModule()
		{
			SuspConfigContainer SuspConfig = new SuspConfigContainer (SuspensionHeight, 0, SuspensionDamper, SuspensionSpring);

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

			SuspensionDampingAdjustment = SuspensionDamper;
			SuspensionSpringAdjustment = SuspensionSpring;
			SuspensionTravelAdjustment = SuspensionHeight;

			Debuggar.Message ("BaseTrackModule in OnStart(): Module successfully started");
		}

		[KSPAction("Brakes", KSPActionGroup.Brakes)]
		protected void Brake(KSPActionParam param)
		{
			if (Tracks != null)
			{
				if (Tracks.Count != 0)
				{
					if (param.type == KSPActionType.Activate)
					{
						if (bIsCruiseEnabled)
						{
							this.vessel.ActionGroups.ToggleGroup (CruiseActionGroup);
						}

						foreach (Track track in Tracks)
							track.bApplyBrakes = true;
					} else
					{
						foreach (Track track in Tracks)
							track.bApplyBrakes = false;
					}
				}
				else Debuggar.Error ("BaseTrackModule in Brake(): Tracks list empty");
			}
			else Debuggar.Error ("BaseTrackModule in Brake(): Tracks list is null");
		}

		[KSPAction("Toggle Cruise Control", KSPActionGroup.None)]
		protected void ToggleCruiseControl(KSPActionParam param)
		{
			if (Tracks != null)
			{
				if (Tracks.Count != 0)
				{
					if (param.type == KSPActionType.Activate)
					{
						bIsCruiseEnabled = true;
						foreach (Track track in Tracks)
							if (CruiseTargetRPM < track.RPM)
								CruiseTargetRPM = track.RPM;
					} else
					{
						bIsCruiseEnabled = false;
						CruiseTargetRPM = 0;
					}
				}
				else Debuggar.Error ("BaseTrackModule in ToggleCruiseControl(): Tracks list empty");
			}
			else Debuggar.Error ("BaseTrackModule in ToggleCruiseControl(): Tracks list is null");
		}

		[KSPEvent(guiActive = true, guiName = "Update Suspension")]
		protected void UpdateSuspension()
		{
			if (Tracks != null)
			{
				if (Tracks.Count != 0)
				{
					TrackConfig.WheelDummyConfig.SuspConfig.Damper = SuspensionDampingAdjustment;
					TrackConfig.WheelDummyConfig.SuspConfig.Spring = SuspensionSpringAdjustment;
					TrackConfig.WheelDummyConfig.SuspConfig.TravelCenter = SuspensionTargetPosAdjustment;
					TrackConfig.WheelDummyConfig.SuspConfig.Travel = SuspensionTravelAdjustment;

					foreach (Track track in Tracks)
						track.UpdateSuspension ();
				}
				else Debuggar.Error ("BaseTrackModule in UpdateSuspension(): Tracks list empty");
			}
			else Debuggar.Error ("BaseTrackModule in UpdateSuspension(): Tracks list is null");
		}

		public virtual void FixedUpdate ()
		{
			if (HighLogic.LoadedSceneIsFlight)
			{
				SuspConfigContainer suspConfig = TrackConfig.WheelDummyConfig.SuspConfig;

				dbgSuspensionDamping = suspConfig.Damper;
				dbgSuspensionSpring = suspConfig.Spring;
				dbgSuspensionTravel = suspConfig.Travel;
				dbgSuspensionTargetPos = suspConfig.TravelCenter;

				if (Tracks != null)
				{
					if (Tracks.Count != 0)
					{
						foreach (Track track in Tracks)
						{
							track.FixedUpdate ();
						}
					}
					else Debuggar.Error ("BaseTrackModule in FixedUpdate(): Tracks list empty");
				}
				else Debuggar.Error ("BaseTrackModule in FixedUpdate(): Tracks list is null");
			}
		}

		public virtual void Update ()
		{
			if (HighLogic.LoadedSceneIsFlight)
			{
				if (this.vessel != null)
				{
					if (this.vessel.isActiveVessel)
					{
						if (Tracks != null)
						{
							if (Tracks.Count != 0)
							{
								foreach (Track track in Tracks)
									track.Update ();
							} else
								Debuggar.Error ("BaseTrackModule in Update(): Tracks list empty");
						} else
							Debuggar.Error ("BaseTrackModule in Update(): Tracks list is null");
					}
				}
				else Debuggar.Error("BaseTrackModule in Update(): this.vessel is null FOR WHY SQUAD");
			}
		}

		protected void ConsumeResource(float torque)
		{
			float amount = Mathf.Abs (torque * ConsumeResourceRate);
			this.part.RequestResource (ConsumedResource, amount);
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