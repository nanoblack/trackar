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
	public partial class SingleTrack : BaseTrackModule
	{
		[KSPField]
		public string SingleTrackRoot = "TrackRoot";
		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Track Motor"),
			UI_Toggle(disabledText="Disabled", enabledText="Enabled")]
		public bool bIsTrackEnabled = true;

		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Invert Track Motor"),
			UI_Toggle(disabledText="No", enabledText="Yes")]
		public bool bInvertTrack = true;

		private string SideOfVessel = "left";

		private Track TrackInstance;

		public override void OnStart(StartState state)
		{
			base.OnStart (state);

			if (HighLogic.LoadedSceneIsFlight)
			{
				TrackInstance = new Track (part.FindModelTransform (SingleTrackRoot), TrackConfig, bIsMirrorInstance);
				Tracks.Add (TrackInstance);

				/* TODO
				 * get vessel CoM
				 * compare to this part's coords to determine which side of the vessel it's on
				 * get this part's "forward" vector to determine whether it's facing "forward" or "backward"
				 * set bInvertTrack accordingly
				 * 
				 * this works perfectly in my head
				 * which means it won't work at all in reality
				 */
			}
			Debuggar.Message ("SingleTrack in OnStart(): Module successfully started");
		}

		public override void Update()
		{
			base.Update ();
		}

		public override void FixedUpdate ()
		{

			if(HighLogic.LoadedSceneIsFlight && this.vessel.isActiveVessel)
			{
				float torque = 0;

				if (bIsTrackEnabled)
				{
					if (TrackInstance != null)
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
						TrackInstance.Torque = torque;

						ConsumeResource (torque);
					}
					else Debuggar.Error ("SingleTrack in FixedUpdate(): TrackInstance is null");
				}
			}
			base.FixedUpdate ();
		}
	}
}