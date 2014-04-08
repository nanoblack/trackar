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
				/* TODO
				 * get vessel CoM
				 * compare to this part's coords to determine which side of the vessel it's on
				 * get this part's "forward" vector to determine whether it's facing "forward" or "backward"
				 * set bInvertTrack accordingly
				 * 
				 * this works perfectly in my head
				 * which means it won't work at all in reality
				 */

				Vector3 com = this.vessel.findWorldCenterOfMass (); // what's the difference between local CoM and world CoM?
				Vector3 partPosition = this.part.transform.position; // what's the difference between this and localPosition?

				Debuggar.Message ("SingleTrack in OnStart(): Vessel CoM X = " + com.x.ToString () + " Y = " + com.y.ToString () + " Z = " + com.z.ToString ());
				Debuggar.Message ("SingleTrack in OnStart(): Part X = " + partPosition.x.ToString () + " Y = " + partPosition.y.ToString () + " Z = " + partPosition.z.ToString ());

				if(partPosition.y > com.y)
					SideOfVessel = "Left";
				else if(partPosition.y < com.y)
					SideOfVessel = "Right";

				if (SideOfVessel == "Right")
					bInvertTrack = true;

				TrackInstance = new Track (part.FindModelTransform (SingleTrackRoot), TrackConfig, bInvertTrack);
				Tracks.Add (TrackInstance);
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
						TrackInstance.Torque = torque;

						ConsumeResource (torque);
					}
					TrackRPM = TrackInstance.RPM;
					DispTorque = torque;
				}
				else Debuggar.Error ("SingleTrack in FixedUpdate(): TrackInstance is null");
			}
			base.FixedUpdate ();
		}
	}
}