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

		[KSPField]
		public float ConsumeResourceRate = 0.25f;
		[KSPField]
		public string ConsumedResource = "ElectricCharge";

		[KSPField(guiActive = Debuggar.bIsDebugMode, guiName = "Cruise Mode")]
		public bool bIsCruiseEnabled = false;
		[KSPField(guiName = "Cruise RPM", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float CruiseTargetRPM = 0;
		private KSPActionGroup CruiseActionGroup;
		protected float CruiseMonitorRPM = 0;

		protected bool bApplyBrakes = false;

		[KSPField(guiActive = true, guiActiveEditor = true, guiName = "Damping") , UI_FloatRange(minValue = 0, maxValue = 15, stepIncrement = 1f)]
		public float SuspensionDampingAdjustment = 0;
		[KSPField(guiActive = true, guiActiveEditor = true, guiName = "Spring") , UI_FloatRange(minValue = 0, maxValue = 100, stepIncrement = 1)]
		public float SuspensionSpringAdjustment = 0;
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
			if (param.type == KSPActionType.Activate)
			{
				if (bIsCruiseEnabled)
					this.vessel.ActionGroups.ToggleGroup (CruiseActionGroup);

				bApplyBrakes = true;
			}
			else
			{
				bApplyBrakes = false;
			}
		}

		[KSPAction("Toggle Cruise Control", KSPActionGroup.None)]
		protected void ToggleCruiseControl(KSPActionParam param)
		{
			if (param.type == KSPActionType.Activate)
			{
				bIsCruiseEnabled = true;
				CruiseTargetRPM = CruiseMonitorRPM;
			}
			else
			{
				bIsCruiseEnabled = false;
				CruiseTargetRPM = 0;
			}
		}

		public virtual void FixedUpdate ()
		{
			if (HighLogic.LoadedSceneIsFlight && this.vessel.isActiveVessel)
			{
				if (TrackConfig != null)
				{
					TrackConfig.WheelDummyConfig.SuspConfig.Damper = SuspensionDampingAdjustment;
					TrackConfig.WheelDummyConfig.SuspConfig.Spring = SuspensionSpringAdjustment;
					TrackConfig.WheelDummyConfig.SuspConfig.Travel = SuspensionTravelAdjustment;
				}
			}
		}

		public virtual void Update ()
		{
		}

		protected bool ConsumeResource(float torque)
		{
			float amountToConsume = Mathf.Abs (torque * ConsumeResourceRate);
			float amountConsumed = this.part.RequestResource (ConsumedResource, amountToConsume);

			return (amountConsumed >= amountToConsume);
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