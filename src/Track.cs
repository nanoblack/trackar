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
	public class Track
	{
		public float WidthScale = 1;

		public float RPM = 0.0f;
		public float RealRPM = 0.0f;

		public bool bIsMirror = false;

		public GameObject Root;
		public SkinnedMeshRenderer TrackSurface;
		public Transform TrackTransform;
		public Transform TrackSurfaceTransform;

		public List<WheelDummy> WheelDummies = new List<WheelDummy>();

		public ConfigContainer Config;

		public SuspConfig Susp;

		public Track(Transform transform, ConfigContainer configContainer, bool mirror)
		{
			Debuggar.Message ("New Track instantiated");

			Config = configContainer;

			TrackTransform = transform;

			Root = TrackTransform.gameObject;

			WidthScale = Config.TrackWidth;

			Component[] components = Root.GetComponentsInChildren<Component>();
			Debuggar.Message ("Fetching Components: " + components.Count().ToString());

			Dictionary<int,GameObject> wheelObjects = new Dictionary<int, GameObject>();
			Dictionary<int,WheelCollider> wheelColliders = new Dictionary<int, WheelCollider>();
			Dictionary<int,Transform> suspJoints = new Dictionary<int, Transform>();

			foreach(Component o in components)
			{
				if (o.name.StartsWith (Config.WheelModelName) && o is MeshFilter)
				{
					int wheelNumber = Convert.ToInt32 (o.name.Substring (Config.WheelModelName.Length));
					Debuggar.Message ("Building wheelObjects: name " + o.name + " ID " + wheelNumber.ToString());
					wheelObjects.Add (wheelNumber, o.gameObject);
				}

				if (o.name.StartsWith (Config.WheelColliderName) && o is WheelCollider)
				{
					int wheelNumber = Convert.ToInt32 (o.name.Substring (Config.WheelColliderName.Length));
					Debuggar.Message ("Building wheelColliders: name " + o.name + " ID " + wheelNumber.ToString());
					wheelColliders.Add (wheelNumber, o as WheelCollider);
				}

				if (o.name.StartsWith (Config.SuspJointName) && o is Transform)
				{
					int jointNumber = Convert.ToInt32 (o.name.Substring (Config.SuspJointName.Length));
					Debuggar.Message ("Building suspJoints: name " + o.name + " ID " + jointNumber.ToString());
					suspJoints.Add (jointNumber, o as Transform);
				}

				if (o.name.StartsWith (Config.TrackSurfaceName))
				{
					TrackSurface = o as SkinnedMeshRenderer;
					Debuggar.Message ("Found track surface: " + o.name);
				}
			}

			foreach(KeyValuePair<int, WheelCollider> i in wheelColliders)
			{
				int number = i.Key;
				WheelCollider collider = i.Value;

				collider.enabled = true;

				collider.brakeTorque = Config.RollingResistance;

				Debuggar.Message ("Instantiating WheelDummy " + number.ToString());
				WheelDummies.Add (new WheelDummy (collider, suspJoints[number], wheelObjects[number]));
			}

			TrackSurfaceTransform = Root.transform.Find (Config.TrackSurfaceName);
			if (TrackSurfaceTransform == null)
				Debuggar.Message ("TrackSurfaceTransform is null!");

			bIsMirror = mirror;
		}

		public void PhysUpdate()
		{
			foreach (WheelDummy wheel in WheelDummies)
			{
				wheel.Susp = Susp;
				wheel.PhysUpdate ();
			}
		}

		public void Update()
		{
			float distanceTravelled = (float)((RealRPM * 2 * Math.PI) / 60) * Time.deltaTime;
			Material trackMaterial = TrackSurface.renderer.material;
			Vector2 textureOffset = trackMaterial.mainTextureOffset;
			textureOffset = textureOffset + new Vector2(-distanceTravelled / Config.TrackLength, 0);
			trackMaterial.SetTextureOffset("_MainTex", textureOffset);
			trackMaterial.SetTextureOffset("_BumpMap", textureOffset);

			foreach(WheelDummy wheel in WheelDummies)
			{
				if (Config.bIsDoubleTrackPart)
				{
					if (bIsMirror)
						wheel.Rotate (RealRPM);
					else
						wheel.Rotate (-RealRPM);
				}
				else
					wheel.Rotate (RealRPM);
			}
		}

		public void EngageBrake()
		{
			foreach (WheelDummy wheel in WheelDummies)
				wheel.Collider.brakeTorque = Config.BrakingTorque;
		}

		public void ReleaseBrake()
		{
			foreach (WheelDummy wheel in WheelDummies)
				wheel.Collider.brakeTorque = Config.RollingResistance;
		}

		public void ApplyTorque(float torque)
		{
			foreach (WheelDummy wheelDummy in WheelDummies)
			{
				wheelDummy.Collider.motorTorque = torque;
				if (wheelDummy.Collider.isGrounded)
				{
					RealRPM = wheelDummy.Collider.rpm * wheelDummy.Collider.radius;
					RPM = Mathf.Abs (RealRPM);
				}
			}
		}
	}
}

