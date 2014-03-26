//=============================================================
// Break your bones when you come down
// You're a one trick mind trick pony
// Who's next to hop on the ride ride ride... 
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

		public override void OnStart(StartState state)
		{
			base.OnStart (state);

			if (this.part.symmetryCounterparts.Count == 1)
			{
				Part counterpart = this.part.symmetryCounterparts [0];
				SingleTrack module = counterpart.Modules["SingleTrack"] as SingleTrack;

				if (module != null)
				{
					if(!bIsMirrorInstance)
					{
						module.bIsMirrorInstance = true;
						Debuggar.Message ("Setting instance " + Convert.ToString(counterpart.GetInstanceID()) + " as a mirror instance");
					}
					else
					{
						Debuggar.Message ("Instance " + Convert.ToString(counterpart.GetInstanceID()) + " is already claimed as this module's mirror instance");
					}
				}
				else
					Debuggar.Error ("Symmetry conterpart with no corresponding module");
			}

			if (HighLogic.LoadedSceneIsFlight)
			{
				//Debuggar.Message ("Instantiating new Track: " + SingleTrackRoot);
				Tracks.Add (new Track (part.FindModelTransform (SingleTrackRoot), TrackConfig, bIsMirrorInstance));
			}
			Debuggar.Message ("SingleTrack module successfully started");
		}

		public override void FixedUpdate ()
		{
			base.FixedUpdate ();

			if(HighLogic.LoadedSceneIsFlight && this.vessel.isActiveVessel)
			{
				if (bIsTrackEnabled)
				{
					foreach (Track track in Tracks)
					{
						float steer = -2 * this.vessel.ctrlState.wheelSteer;
						float forward = -this.vessel.ctrlState.wheelThrottle;

						float torque = 0;


						if (bIsCruiseEnabled && track.RPM < CruiseTargetRPM)
						{
							forward = track.RPM / CruiseTargetRPM;
						}

						if (track.bIsMirror)
						{
							forward *= -1;
						}

						torque = (Mathf.Clamp (forward + steer, -1, 1) * TorqueCurve.Evaluate (track.RPM));
						track.ApplyTorque (torque);
					}
				}
			}
		}
	}
}