//=============================================================
// We mash up the place, turn up the bass
// And mek dem all have fun
// A-we ablaze the fire, make it bun dem
//=============================================================

using System;
using System.Collections.Generic;

using UnityEngine;


namespace Trackar
{
	public class WheelDummy
	{
		// I can't even remember why I made these read only properties but it looks a bit silly to me now
		public WheelCollider Collider { get { return _collider; } }
		public Transform Joint { get { return _joint; } }
		public GameObject WheelModel { get { return _model; } }

		public SuspConfigContainer Susp;

		private WheelCollider _collider;
		private Transform _joint;
		private GameObject _model;

		public WheelDummy(WheelCollider col, Transform joint, GameObject model, WheelDummyConfigContainer config)
		{
			// if any of these are tripped, the WheelDummy will be basically useless, but that's better than it being definitely useless with NullReferences Everywhere
			if (col == null)
			{
				Debuggar.Error ("WheelDummy: Attempted spawn with null WheelCollider");
				col = new WheelCollider ();
			}
			if(joint == null)
			{
				Debuggar.Error ("WheelDummy: Attempted spawn with null Joint");
				joint = new GameObject ().transform; // lol gg unity
			}
			if(model == null)
			{
				Debuggar.Error ("WheelDummy: Attempted spawn with null Model");
				model = new GameObject ();
			}
			if(config == null)
			{
				Debuggar.Error ("WheelDummy: Attempted spawn with null WheelDummyConfigContainer");
				config = new WheelDummyConfigContainer ();
			}

			this._collider = col;
			this._joint = joint;
			this._model = model;
			this.Susp = config.SuspConfig;

			Collider.brakeTorque = config.RollingResistance;
			Collider.enabled = true;

			Debuggar.Message ("WheelDummy: Spawned");
		}

		public void Rotate(float rpm)
		{
			float deg = (rpm / 60) * 360;
			float delta = deg * Time.deltaTime;
			float rot = delta / Collider.radius;
			if(WheelModel != null)
				WheelModel.transform.Rotate(Vector3.right, rot);
			else
				Debuggar.Error("WheelDummy in Rotate(): WheelModel is null");
		}

		// WHEN AM I GOING TO EVER HAVE SOME SORT OF FUNCTIONAL SUSPENSION UP IN HURR
		/*public void PhysUpdate()
		{
			Collider.suspensionDistance = Susp.Travel;
			JointSpring spring = Collider.suspensionSpring;
			spring.targetPosition = Susp.TravelCenter;
			spring.damper = Susp.Damper;
			Collider.suspensionSpring = spring; // UGH this feels hacky

			//WheelModel.transform.position = Collider.transform.position; // temp hack for possibly getting some visual feedback for suspension
			// ^ the results of this make no sense to me, but are not at all what is intended
		}*/
	}
}

