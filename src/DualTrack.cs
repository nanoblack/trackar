//=============================================================
// All you do is take, take, takin' off the little that
// You got on, goin' on, I'd rather be eating glass
// Stick hot needles in my eyes, go ahead and tap that ass 
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

		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Left Track Motor"),
			UI_Toggle(disabledText="Disabled", enabledText="Enabled")]
		public bool bIsLeftTrackEnabled = true;
		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Right Track Motor"),
			UI_Toggle(disabledText="Disabled", enabledText="Enabled")]
		public bool bIsRightTrackEnabled = true;

		public Track LeftTrack;
		public Track RightTrack;

		[KSPField(guiName = "Left Track RPM", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float LeftTrackRPM = 0;
		[KSPField(guiName = "Right Track RPM", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float RightTrackRPM = 0;
		[KSPField(guiName = "Revmatch Error", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float RevmatchError = 0;

		[KSPField(guiName = "Left Track Torque", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float LeftTorque = 0;
		[KSPField(guiName = "Right Track Torque", guiFormat = "F1", guiActive = Debuggar.bIsDebugMode)]
		public float RightTorque = 0;

		public override void OnStart(StartState state)
		{
			base.OnStart (state);

			if (HighLogic.LoadedSceneIsFlight)
			{
				TrackConfig.LeftTrackRoot = LeftTrackRoot;
				TrackConfig.RightTrackRoot = RightTrackRoot;

				Debuggar.Message ("Instantiating new Track: " + LeftTrackRoot);
				LeftTrack = new Track (part.FindModelTransform (LeftTrackRoot), TrackConfig, true);
				Tracks.Add (LeftTrack);

				Debuggar.Message ("Instantiating new Track: " + RightTrackRoot);
				RightTrack = new Track (part.FindModelTransform (RightTrackRoot), TrackConfig, false);
				Tracks.Add (RightTrack);
			}
		}

		public override void FixedUpdate ()
		{

			base.FixedUpdate ();

			if(HighLogic.LoadedSceneIsFlight && this.vessel.isActiveVessel)
			{
				float steer = 2 * this.vessel.ctrlState.wheelSteer;
				float forward = this.vessel.ctrlState.wheelThrottle;

				LeftTrackRPM = LeftTrack.RPM;
				RightTrackRPM = RightTrack.RPM;

				RevmatchError = LeftTrackRPM - RightTrackRPM;

				LeftTorque = 0;
				RightTorque = 0;

				if (bIsLeftTrackEnabled)
				{
					if (bIsCruiseEnabled && LeftTrack.RPM < CruiseTargetRPM)
					{
						forward = LeftTrack.RPM / CruiseTargetRPM;
					}
					LeftTorque = (Mathf.Clamp (forward - steer, -1, 1) * TorqueCurve.Evaluate (LeftTrack.RPM));
					LeftTrack.ApplyTorque (LeftTorque);
				}

				if(bIsRightTrackEnabled)
				{
					if (bIsCruiseEnabled && RightTrack.RPM < CruiseTargetRPM)
					{
						forward = RightTrack.RPM / CruiseTargetRPM;
					}
					RightTorque = (Mathf.Clamp (forward + steer, -1, 1) * TorqueCurve.Evaluate (RightTrack.RPM));
					RightTrack.ApplyTorque (RightTorque);
				}
			}
		}
	}
}