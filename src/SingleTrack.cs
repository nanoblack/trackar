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

		/*[KSPField(guiName = "X position", guiFormat = "F1", guiActiveEditor = Debuggar.bIsDebugMode)]
		public float dbgPartX = 0;
		[KSPField(guiName = "Y position", guiFormat = "F1", guiActiveEditor = Debuggar.bIsDebugMode)]
		public float dbgPartY = 0;
		[KSPField(guiName = "Z position", guiFormat = "F1", guiActiveEditor = Debuggar.bIsDebugMode)]
		public float dbgPartZ = 0;
		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "bIsTrackLeftSide")]
		public bool bIsTrackLeftSide = true;*/

		public Track TrackInstance;

		//private Callback EditorAttachEventHandler;

		public override void OnStart(StartState state)
		{
			base.OnStart (state);

			/*if (this.part.symmetryCounterparts.Count == 1)
			{
				Part counterpart = this.part.symmetryCounterparts [0];
				SingleTrack module = counterpart.Modules["SingleTrack"] as SingleTrack;

				if (module != null)
				{
					if(!bIsMirrorInstance)
					{

						module.bIsMirrorInstance = true;
						Debuggar.Message ("SingleTrack in OnStart(): Setting instance " + Convert.ToString(counterpart.GetInstanceID()) + " as a mirror instance");
					}
				}
				else
					Debuggar.Error ("SingleTrack in OnStart(): Symmetry conterpart module is null");
			}*/

			if (HighLogic.LoadedSceneIsFlight)
			{
				TrackInstance = new Track (part.FindModelTransform (SingleTrackRoot), TrackConfig, bIsMirrorInstance);
				Tracks.Add (TrackInstance);
			}
			Debuggar.Message ("SingleTrack in OnStart(): Module successfully started");
		}

		/*public void OnEditorAttachEvent()
		{
			if (this.part.symmetryCounterparts.Count == 1)
			{
				dbgPartX = this.part.transform.position.x;
				dbgPartY = this.part.transform.position.y;
				dbgPartZ = this.part.transform.position.z;

				if (dbgPartX <= this.part.symmetryCounterparts [0].transform.position.x)
					bIsTrackLeftSide = true;
				else
					bIsTrackLeftSide = false;
			}
		}*/

		public override void Update()
		{
			/*if (EditorAttachEventHandler == null)
				EditorAttachEventHandler = new Callback(OnEditorAttachEvent);
			if (!this.part.OnEditorAttach.GetInvocationList().Contains(EditorAttachEventHandler))
				this.part.OnEditorAttach += EditorAttachEventHandler;*/

			base.Update ();
		}

		public override void FixedUpdate ()
		{

			if(HighLogic.LoadedSceneIsFlight && this.vessel.isActiveVessel)
			{
				if (bIsTrackEnabled)
				{
					if (TrackInstance != null)
					{
						float steer = -2 * this.vessel.ctrlState.wheelSteer;
						float forward = this.vessel.ctrlState.wheelThrottle;

						float torque = 0;


						if (bIsCruiseEnabled && TrackInstance.RPM < CruiseTargetRPM)
						{
							forward = TrackInstance.RPM / CruiseTargetRPM;
						}

						/*if (bIsMirrorInstance && !bIsTrackLeftSide)
						{
							forward *= -1;
						}*/
						if (bInvertTrack)
							forward *= -1;

						torque = (Mathf.Clamp (forward + steer, -1, 1) * TorqueCurve.Evaluate (TrackInstance.RPM));
						TrackInstance.ApplyTorque (torque);
					}
					else Debuggar.Error ("SingleTrack in FixedUpdate(): TrackInstance is null");

					/*foreach (Track track in Tracks)
					{
						// I assume these have to be negated immediately like this because the right side is being claimed as mirror?
						float steer = -2 * this.vessel.ctrlState.wheelSteer;
						float forward = this.vessel.ctrlState.wheelThrottle;

						float torque = 0;


						if (bIsCruiseEnabled && track.RPM < CruiseTargetRPM)
						{
							forward = track.RPM / CruiseTargetRPM;
						}

						if (bIsMirrorInstance && !bIsTrackLeftSide)
						{
							forward *= -1;
						}

						torque = (Mathf.Clamp (forward + steer, -1, 1) * TorqueCurve.Evaluate (track.RPM));
						track.ApplyTorque (torque);
					}*/
				}
			}
			base.FixedUpdate ();
		}
	}
}