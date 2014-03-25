//=============================================================
// Nope can't be me, get your freak on, you can suck it
// They've all seen it and you liked it, now he wants it 
// Cotton candy suga high
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
						Debuggar.Message ("Instance " + Convert.ToString(counterpart.GetInstanceID()) + " has already been set as a mirror");
					}
				}
				else
					Debuggar.Message ("Symmetry conterpart with no corresponding module!");
			}

			if (HighLogic.LoadedSceneIsFlight)
			{
				TrackConfig.SingleTrackRoot = SingleTrackRoot;
				Debuggar.Message ("Instantiating new Track: " + SingleTrackRoot);
				Tracks.Add (new Track (part.FindModelTransform (SingleTrackRoot), TrackConfig, bIsMirrorInstance));
			}
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