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

		public bool bIsMirror = false;

		public GameObject Root;
		public SkinnedMeshRenderer TrackSurface;
		public Transform TrackTransform;
		public Transform TrackSurfaceTransform;

		public List<WheelDummy> WheelDummies = new List<WheelDummy> ();

		public TrackConfigContainer Config;

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

		public void InitWheelDummyList (Component[] components, TrackConfigContainer config)
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
					Config.WheelDummyConfig.SuspConfig.Damper = collider.suspensionSpring.damper;
					Config.WheelDummyConfig.SuspConfig.Travel = collider.suspensionDistance;
					Config.WheelDummyConfig.SuspConfig.TravelCenter = collider.suspensionSpring.targetPosition;
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
				bool bIsOnGround = IsOnGround ();

				if (WheelDummies.Count != 0)
				{
					foreach (WheelDummy wheelDummy in WheelDummies)
					{
						if (bIsOnGround)
						{
							RealRPM = wheelDummy.Collider.rpm * wheelDummy.Collider.radius;
						}
						RPM = Mathf.Abs (RealRPM);
						if(bIsMirror)
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

		public void AdjustSuspensionDamper(float value)
		{

		}

		public void AdjustSuspensionSpring(float value)
		{

		}

		public void AdjustSuspensionHeight(float value)
		{

		}

		public void AdjustSuspensionTravelMax(float value)
		{

		}
	}
}