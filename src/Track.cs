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
	public class Track
	{
		public float WidthScale = 1;

		public float RPM = 0;
		public float RealRPM = 0;

		public float Torque = 0;

		public bool bApplyBrakes = false;

		private bool bIsMirror = false;

		private GameObject Root;
		private SkinnedMeshRenderer TrackSurface;
		private Transform TrackTransform;
		private Transform TrackSurfaceTransform;

		private List<WheelDummy> WheelDummies = new List<WheelDummy> ();

		private TrackConfigContainer Config;

		public Track(Transform transform, TrackConfigContainer configContainer, bool mirror)
		{
			Config = configContainer;

			TrackTransform = transform;

			Root = TrackTransform.gameObject;

			WidthScale = Config.Width;

			Component[] components = Root.GetComponentsInChildren<Component> ();

			InitWheelDummyList (components, Config);

			foreach (Component o in components)
			{
				if (o.name.StartsWith (Config.ModelConfig.TrackSurface))
					TrackSurface = o as SkinnedMeshRenderer;
			}

			TrackSurfaceTransform = Root.transform.Find (Config.ModelConfig.TrackSurface);

			if (TrackSurfaceTransform == null)
				Debuggar.Error ("Track: TrackSurfaceTransform is null");

			bIsMirror = mirror;

			Debuggar.Message ("Track: Spawned");
		}

		private void InitWheelDummyList (Component[] components, TrackConfigContainer config)
		{
			Dictionary<int,GameObject> wheelObjects = new Dictionary<int, GameObject>();
			Dictionary<int,WheelCollider> wheelColliders = new Dictionary<int, WheelCollider>();
			Dictionary<int,Transform> suspJoints = new Dictionary<int, Transform>();

			Config = config; // lolcase

			if (components != null)
			{
				foreach (Component o in components)
				{
					if (o.name.StartsWith (Config.ModelConfig.WheelModel) && o is MeshFilter)
					{
						int wheelNumber = Convert.ToInt32 (o.name.Substring (Config.ModelConfig.WheelModel.Length));
						wheelObjects.Add (wheelNumber, o.gameObject);
					}

					if (o.name.StartsWith (Config.ModelConfig.WheelCollider) && o is WheelCollider)
					{
						int wheelNumber = Convert.ToInt32 (o.name.Substring (Config.ModelConfig.WheelCollider.Length));
						wheelColliders.Add (wheelNumber, o as WheelCollider);
					}

					if (o.name.StartsWith (Config.ModelConfig.Joint) && o is Transform)
					{
						int jointNumber = Convert.ToInt32 (o.name.Substring (Config.ModelConfig.Joint.Length));
						suspJoints.Add (jointNumber, o as Transform);
					}
				}
				foreach (KeyValuePair<int, WheelCollider> i in wheelColliders)
				{
					int number = i.Key;
					WheelCollider collider = i.Value;

					SuspConfigContainer suspConfig = Config.WheelDummyConfig.SuspConfig;

					JointSpring spring = collider.suspensionSpring;

					Debuggar.Message ("Track in InitWheelDummyList(): Original collider settings: damper = " + spring.damper.ToString () + " spring = " + spring.spring.ToString () + " center = " + spring.targetPosition.ToString () + " travel = " + collider.suspensionDistance.ToString ());

					spring.damper = suspConfig.Damper;
					spring.spring = suspConfig.Spring;
					spring.targetPosition = suspConfig.TravelCenter;

					collider.suspensionSpring = spring;
					collider.suspensionDistance = suspConfig.Travel;

					Debuggar.Message ("Track in InitWheelDummyList(): Collider now using: damper = " + spring.damper.ToString () + " spring = " + spring.spring.ToString () + " center = " + spring.targetPosition.ToString () + " travel = " + collider.suspensionDistance.ToString ());
					WheelDummies.Add (new WheelDummy (collider, suspJoints [number], wheelObjects [number], Config.WheelDummyConfig));
				}
				Debuggar.Message ("Track in InitWheelDummyList(): " + WheelDummies.Count.ToString () + " WheelDummies");
			}
			else Debuggar.Error ("Track in InitWheelDummyList(): Received null components");
		}

		public void Update()
		{
			if (WheelDummies != null)
			{
				if (WheelDummies.Count != 0)
				{
					/*if (IsOnGround ()) // this whole spot is causing problems
					{
						foreach (WheelDummy wheelDummy in WheelDummies)
						{
							RealRPM = wheelDummy.Collider.rpm * wheelDummy.Collider.radius;
							RPM = Mathf.Abs (RealRPM);
							if (bIsMirror)
								wheelDummy.Rotate (-RealRPM);
							else
								wheelDummy.Rotate (RealRPM);
						}
					}*/
					RealRPM = GetTrackRPM ();
					RPM = Mathf.Abs (RealRPM);
					foreach(WheelDummy wheelDummy in WheelDummies)
					{
						if (bIsMirror)
							wheelDummy.Rotate (-RealRPM);
						else
							wheelDummy.Rotate (RealRPM);
					}
				}
				else Debuggar.Error ("Track in Update(): WheelDummies list is empty");

				// this needs to be done much different
				float distanceTravelled = (float)((RealRPM * 2 * Math.PI) / 60) * Time.deltaTime;
				Material trackMaterial = TrackSurface.renderer.material;
				Vector2 textureOffset = trackMaterial.mainTextureOffset;
				textureOffset = textureOffset + new Vector2 (-distanceTravelled / Config.Length, 0);
				trackMaterial.SetTextureOffset ("_MainTex", textureOffset);
				trackMaterial.SetTextureOffset ("_BumpMap", textureOffset);
			}
			else Debuggar.Error ("Track in Update(): WheelDummies is null");
		}

		// this needs to get the lowest grounded wheel RPM
		public float GetTrackRPM()
		{
			List<float> RPMlist = new List<float> ();
			float i = 0;
			float value = RealRPM; // if no colliders on ground, RPMlist will be empty so use last known RPM as a default value

			foreach(WheelDummy wheelDummy in WheelDummies)
			{
				if(wheelDummy.Collider.isGrounded)
				{
					i = wheelDummy.Collider.rpm * wheelDummy.Collider.radius;
					RPMlist.Add (i);
				}
			}

			if (RPMlist.Count != 0)
			{
				if (IsOnGround ())
				{
					if (RPMlist.Min () >= 0) // RPM are all positive, lowest RPM is lowest number
						value = RPMlist.Min ();
					else if (RPMlist.Max () <= 0) // RPM are all negative, lowest RPM is highest number
						value = RPMlist.Max ();
				}
			}

			return value;
		}

		public bool IsOnGround()
		{
			if (WheelDummies.Count != 0)
			{
				foreach (WheelDummy wheel in WheelDummies)
				{
					if (wheel.Collider.isGrounded)
						return true; // return asap, if one is on ground consider them all on ground
				}
				return false; // this should only happen when ALL wheels in this set are off ground
			}
			else
			{
				Debuggar.Error ("Track in IsOnGround(): WheelDummies list is empty");
				return false;
			}
		}

		public void FixedUpdate ()
		{
			if (WheelDummies != null)
			{
				if (WheelDummies.Count != 0)
				{
					foreach (WheelDummy wheelDummy in WheelDummies)
					{
						wheelDummy.Collider.motorTorque = Torque;

						if(bApplyBrakes)
							wheelDummy.Collider.brakeTorque = Config.WheelDummyConfig.BrakingTorque;
						else
							wheelDummy.Collider.brakeTorque = Config.WheelDummyConfig.RollingResistance; // lets just spam this at it and see if it finally fucking sticks eh?
					}
				}
				else Debuggar.Error ("Track in FixedUpdate(): WheelDummies list is empty");
			}
			else Debuggar.Error ("Track in FixedUpdate(): WheelDummies is null");
		}

		public void UpdateSuspension()
		{
			SuspConfigContainer suspConfig = Config.WheelDummyConfig.SuspConfig;

			foreach (WheelDummy wheelDummy in WheelDummies)
			{
				JointSpring newSpring = wheelDummy.Collider.suspensionSpring;
				newSpring.damper = suspConfig.Damper;
				newSpring.spring = suspConfig.Spring;
				newSpring.targetPosition = suspConfig.TravelCenter;

				wheelDummy.Collider.suspensionSpring = newSpring;

				wheelDummy.Collider.suspensionDistance = suspConfig.Travel;
			}
		}
	}
}