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

		[KSPField(guiName = "X position", guiFormat = "F1", guiActiveEditor = Debuggar.bIsDebugMode)]
		public float dbgPartX = 0;
		[KSPField(guiName = "Y position", guiFormat = "F1", guiActiveEditor = Debuggar.bIsDebugMode)]
		public float dbgPartY = 0;
		[KSPField(guiName = "Z position", guiFormat = "F1", guiActiveEditor = Debuggar.bIsDebugMode)]
		public float dbgPartZ = 0;
		[KSPField(isPersistant = true, guiActive = Debuggar.bIsDebugMode, guiActiveEditor = Debuggar.bIsDebugMode, guiName = "bIsTrackLeftSide")]
		public bool bIsTrackLeftSide = true;

		private Callback EditorAttachEventHandler;

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
				Tracks.Add (new Track (part.FindModelTransform (SingleTrackRoot), TrackConfig, bIsMirrorInstance));
			}
			Debuggar.Message ("SingleTrack module successfully started");
		}

		public void OnEditorAttachEvent()
		{
			dbgPartX = this.part.transform.position.x;
			dbgPartY = this.part.transform.position.y;
			dbgPartZ = this.part.transform.position.z;

			if (dbgPartX <= this.part.symmetryCounterparts [0].transform.position.x)
				bIsTrackLeftSide = true;
			else
				bIsTrackLeftSide = false;
		}

		public override void Update()
		{
			if (EditorAttachEventHandler == null)
				EditorAttachEventHandler = new Callback(OnEditorAttachEvent);
			if (!this.part.OnEditorAttach.GetInvocationList().Contains(EditorAttachEventHandler))
				this.part.OnEditorAttach += EditorAttachEventHandler;

			base.Update ();
		}

		public override void FixedUpdate ()
		{

			if(HighLogic.LoadedSceneIsFlight && this.vessel.isActiveVessel)
			{
				if (bIsTrackEnabled)
				{
					foreach (Track track in Tracks)
					{
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
					}
				}
			}
			base.FixedUpdate ();
		}
	}
}