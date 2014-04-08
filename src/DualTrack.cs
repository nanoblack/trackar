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
	public class DualTrack : BaseTrackModule
	{
		[KSPField]
		public string LeftTrackRoot = "LeftTrackRoot";
		[KSPField]
		public string RightTrackRoot = "RightTrackRoot";

		[KSPField(guiActiveEditor = true, guiActive = true, guiName = "Left Track")]
		public string LeftTrackContextLabel = ""; // this feels ugly
		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Motor"),
			UI_Toggle(disabledText="Disabled", enabledText="Enabled")]
		public bool bIsLeftTrackEnabled = true;
		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Invert"),
			UI_Toggle(disabledText="No", enabledText="Yes")]
		public bool bInvertLeftTrack = false;
		[KSPField(guiName = "RPM", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float LeftTrackRPM = 0;
		[KSPField(guiName = "Torque", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float LeftTorque = 0;

		[KSPField(guiActiveEditor = true, guiActive = true, guiName = "Right Track")]
		public string RightTrackContextLabel = "";
		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Motor"),
			UI_Toggle(disabledText="Disabled", enabledText="Enabled")]
		public bool bIsRightTrackEnabled = true;
		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Invert"),
			UI_Toggle(disabledText="No", enabledText="Yes")]
		public bool bInvertRightTrack = false;
		[KSPField(guiName = "RPM", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float RightTrackRPM = 0;
		[KSPField(guiName = "Torque", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float RightTorque = 0;

		private Track LeftTrack;
		private Track RightTrack;

		public override void OnStart(StartState state)
		{
			base.OnStart (state);

			if (HighLogic.LoadedSceneIsFlight)
			{
				LeftTrack = new Track (part.FindModelTransform (LeftTrackRoot), TrackConfig, false);
				Tracks.Add (LeftTrack); // can this List be done away with yet? ugh

				RightTrack = new Track (part.FindModelTransform (RightTrackRoot), TrackConfig, true);
				Tracks.Add (RightTrack);
			}
			Debuggar.Message ("DualTrack in OnStart(): module successfully started");
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
						if (RightTrack != null && LeftTrack != null)
						{
							RightTrack.Update ();
							LeftTrack.Update ();
						}
					}
				}
			}
		}

		public override void FixedUpdate ()
		{
			base.FixedUpdate ();

			if (HighLogic.LoadedSceneIsFlight && this.vessel.isActiveVessel)
			{
				if (LeftTrack != null && RightTrack != null)
				{
					float steer = 2 * this.vessel.ctrlState.wheelSteer;
					float leftForward = this.vessel.ctrlState.wheelThrottle;
					float rightForward = this.vessel.ctrlState.wheelThrottle;

					LeftTrackRPM = LeftTrack.RPM;
					RightTrackRPM = RightTrack.RPM;

					LeftTorque = 0;
					RightTorque = 0;

					if (bIsLeftTrackEnabled)
					{
						if (bIsCruiseEnabled && LeftTrack.RPM < CruiseTargetRPM)
							leftForward = LeftTrack.RPM / CruiseTargetRPM;

						if (bInvertLeftTrack)
							leftForward *= -1;

						LeftTorque = (Mathf.Clamp (leftForward - steer, -1, 1) * TorqueCurve.Evaluate (LeftTrack.RPM));
					}

					if (bIsRightTrackEnabled)
					{
						if (bIsCruiseEnabled && RightTrack.RPM < CruiseTargetRPM)
							rightForward = RightTrack.RPM / CruiseTargetRPM;

						if (bInvertRightTrack)
							rightForward *= -1;

						RightTorque = (Mathf.Clamp (rightForward + steer, -1, 1) * TorqueCurve.Evaluate (RightTrack.RPM));
					}

					if ((LeftTrackRPM > RightTrackRPM && RightTrackRPM >= 100) && steer == 0)
						LeftTorque -= LeftTorque * (Mathf.Clamp(LeftTrackRPM - RightTrackRPM, 0, 1));
					else if ((RightTrackRPM > LeftTrackRPM && LeftTrackRPM >= 100) && steer == 0)
						RightTorque -= RightTorque * (Mathf.Clamp(RightTrackRPM - LeftTrackRPM, 0, 1));


					bool bIsResourceAvailable = ConsumeResource (LeftTorque + RightTorque);

					if (!bIsResourceAvailable)
					{
						LeftTorque = 0;
						RightTorque = 0;
					}

					LeftTrack.Torque = LeftTorque;
					RightTrack.Torque = RightTorque;

					LeftTrack.UpdateSuspension ();
					RightTrack.UpdateSuspension ();

					LeftTrack.FixedUpdate ();
					RightTrack.FixedUpdate ();
				}
				else
				{
					if (LeftTrack == null)
						Debuggar.Error ("DualTrack in FixedUpdate(): LeftTrack is null");
					if (RightTrack == null)
						Debuggar.Error ("DualTrack in FixedUpdate(): RightTrack is null");
				}
			}
		}
	}
}