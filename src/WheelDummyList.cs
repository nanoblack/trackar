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
		public WheelDummyConfigContainer Config;

		public float Torque = 0;
		public float BrakingTorque = 0;

		public float RPM = 0;
		public float RealRPM;

		public WheelDummyList (Component[] components, WheelDummyConfigContainer config, ModelConfigContainer modelConfig)
		{
			// TODO: Dictionaries are a smidge excessive, no?
			Dictionary<int,GameObject> wheelObjects = new Dictionary<int, GameObject>();
			Dictionary<int,WheelCollider> wheelColliders = new Dictionary<int, WheelCollider>();
			Dictionary<int,Transform> suspJoints = new Dictionary<int, Transform>();

			Config = config; // lolcase

			// does this need to be a foreach anymore? surely there are much more efficient ways to do it
			// although does it matter, it's not like it's per-frame
			foreach(Component o in components)
			{
				if (o.name.StartsWith (modelConfig.WheelModel) && o is MeshFilter)
				{
					int wheelNumber = Convert.ToInt32 (o.name.Substring (modelConfig.WheelModel.Length));
					wheelObjects.Add (wheelNumber, o.gameObject);
				}

				if (o.name.StartsWith (modelConfig.WheelCollider) && o is WheelCollider)
				{
					int wheelNumber = Convert.ToInt32 (o.name.Substring (modelConfig.WheelCollider.Length));
					wheelColliders.Add (wheelNumber, o as WheelCollider);
				}

				if (o.name.StartsWith (modelConfig.Joint) && o is Transform)
				{
					int jointNumber = Convert.ToInt32 (o.name.Substring (modelConfig.Joint.Length));
					suspJoints.Add (jointNumber, o as Transform);
				}
			}
			foreach(KeyValuePair<int, WheelCollider> i in wheelColliders)
			{
				int number = i.Key;
				WheelCollider collider = i.Value;
				WheelDummies.Add (new WheelDummy (collider, suspJoints[number], wheelObjects[number], Config));
			}
			Debuggar.Message("WheelDummyList spawned: " + WheelDummies.Count.ToString() + " WheelDummies in this list");
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

