//=============================================================
// We mash up the place, turn up the bass
// And mek dem all have fun
// A-we ablaze the fire, make it bun dem
//=============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Trackar
{
	public class WheelDummyList
	{
		public List<WheelDummy> WheelDummies = new List<WheelDummy>();
		public ConfigContainer Config;

		public float Torque = 0;
		public float BrakingTorque = 0;

		public float RPM = 0;
		public float RealRPM;

		public WheelDummyList (Component[] components, ConfigContainer config)
		{
			// TODO: Dictionaries are a smidge excessive, no?
			Dictionary<int,GameObject> wheelObjects = new Dictionary<int, GameObject>();
			Dictionary<int,WheelCollider> wheelColliders = new Dictionary<int, WheelCollider>();
			Dictionary<int,Transform> suspJoints = new Dictionary<int, Transform>();

			Config = config; // lolcase

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
			}
			foreach(KeyValuePair<int, WheelCollider> i in wheelColliders)
			{
				int number = i.Key;
				WheelCollider collider = i.Value;

				collider.enabled = true;

				collider.brakeTorque = Config.RollingResistance;

				Debuggar.Message ("Instantiating WheelDummy " + number.ToString());
				WheelDummies.Add (new WheelDummy (collider, suspJoints[number], wheelObjects[number], Config.Suspension));
			}
		}

		public bool IsOnGround()
		{
			foreach (WheelDummy wheel in WheelDummies)
			{
				if (wheel.Collider.isGrounded)
					return true; // return asap, if one is on ground consider them all on ground
			}
			return false; // this should only happen when ALL wheels in this set are off ground
		}

		public void Update()
		{
			bool bIsOnGround = IsOnGround ();

			foreach (WheelDummy wheelDummy in WheelDummies)
			{
				if (bIsOnGround)
				{
					RealRPM = wheelDummy.Collider.rpm * wheelDummy.Collider.radius;
					RPM = Mathf.Abs (RealRPM);
				}
				wheelDummy.Collider.motorTorque = Torque;
				wheelDummy.Collider.brakeTorque = BrakingTorque;
				wheelDummy.Rotate (RealRPM);
			}
		}
		public void FixedUpdate()
		{
			foreach (WheelDummy wheelDummy in WheelDummies)
			{
				wheelDummy.Collider.motorTorque = Torque;
				wheelDummy.Collider.brakeTorque = BrakingTorque;
			}
		}
	}
}

