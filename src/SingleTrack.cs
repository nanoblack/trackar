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
	public class SingleTrack : BaseTrackModule
	{
		[KSPField]
		public string SingleTrackRoot = "TrackRoot";
		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Motor"),
			UI_Toggle(disabledText="Disabled", enabledText="Enabled")]
		public bool bIsTrackEnabled = true;

		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Invert"),
			UI_Toggle(disabledText="No", enabledText="Yes")]
		public bool bInvertTrack = false;
		[KSPField(guiName = "RPM", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float TrackRPM = 0;
		[KSPField(guiName = "Torque", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float DispTorque = 0;

		[KSPField(guiActive = Debuggar.bIsDebugMode, guiName = "Side")]
		private string SideOfVessel = "Left";

		private Track TrackInstance;

		public override void OnStart(StartState state)
		{
			base.OnStart (state);

			if (HighLogic.LoadedSceneIsFlight)
			{
				CheckSide ();

				TrackInstance = new Track (part.FindModelTransform (SingleTrackRoot), TrackConfig, bInvertTrack);
			}
			Debuggar.Message ("SingleTrack in OnStart(): Module successfully started");
		}

		/// <summary>
		/// Find side of vessel this module's part is on and set SideOfVessel and bInvertTrack if necessary.
		/// </summary>
		private void CheckSide()
		{
			if (this.vessel != null && this.part != null)
			{
				Vector3 com = this.vessel.findWorldCenterOfMass ();
				Vector3 partPosition = this.part.transform.position;

				Debuggar.Message ("SingleTrack in OnStart(): Vessel CoM X = " + com.x.ToString () + " Y = " + com.y.ToString () + " Z = " + com.z.ToString ());
				Debuggar.Message ("SingleTrack in OnStart(): Part X = " + partPosition.x.ToString () + " Y = " + partPosition.y.ToString () + " Z = " + partPosition.z.ToString ());

				if (partPosition.y > com.y)
					SideOfVessel = "Left";
				else if (partPosition.y < com.y)
					SideOfVessel = "Right";

				if (SideOfVessel == "Right")
					bInvertTrack = true;
			}
		}

		public override void Update()
		{
			base.Update ();

			if (HighLogic.LoadedSceneIsFlight)
			{
				if (this.vessel != null)
				{
					if (this.vessel.isActiveVessel)
					{
						if (TrackInstance != null)
							TrackInstance.Update ();
					}
				}
			}
		}

		public override void FixedUpdate ()
		{

			if(HighLogic.LoadedSceneIsFlight && this.vessel.isActiveVessel)
			{
				float torque = 0;

				if (TrackInstance != null)
				{
					if (bIsTrackEnabled)
					{

						float steer = -2 * this.vessel.ctrlState.wheelSteer;
						float forward = this.vessel.ctrlState.wheelThrottle;

						if (bIsCruiseEnabled && TrackInstance.RPM < CruiseTargetRPM)
						{
							forward = TrackInstance.RPM / CruiseTargetRPM;
						}

						if (bInvertTrack)
							forward *= -1;

						torque = (Mathf.Clamp (forward + steer, -1, 1) * TorqueCurve.Evaluate (TrackInstance.RPM));

						bool bIsResourceAvailable = ConsumeResource (torque);

						if (!bIsResourceAvailable)
							torque = 0;
						
						TrackInstance.Torque = torque;

					}
					TrackRPM = TrackInstance.RPM;
					DispTorque = torque;

					TrackInstance.UpdateSuspension ();

					TrackInstance.bApplyBrakes = bApplyBrakes;

					TrackInstance.FixedUpdate ();

					CruiseMonitorRPM = TrackRPM;
				}
				else Debuggar.Error ("SingleTrack in FixedUpdate(): TrackInstance is null");
			}
			base.FixedUpdate ();
		}
	}
}