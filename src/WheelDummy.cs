using System;
using System.Collections.Generic;

using UnityEngine;


namespace Trackar
{
	public class WheelDummy
	{
		public WheelCollider Collider;
		public Transform Joint;
		public GameObject WheelModel;

		public SuspConfigContainer Susp;

		/// <summary>
		/// Initializes a new instance of the <see cref="Trackar.WheelDummy"/> class.
		/// </summary>
		/// <param name="col">Col.</param>
		/// <param name="joint">Joint.</param>
		/// <param name="model">Model.</param>
		/// <param name="config">WheelDummyConfigContainer.</param>
		public WheelDummy(WheelCollider col, Transform joint, GameObject model, WheelDummyConfigContainer config)
		{
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

			Collider = col;
			Joint = joint;
			WheelModel = model;
			Susp = config.SuspConfig;

			Collider.enabled = true;
			Collider.brakeTorque = config.RollingResistance;
			Debuggar.Message ("WheelDummy: brakeTorque is " + Collider.brakeTorque.ToString () + " RollingResistance is " + config.RollingResistance.ToString ());

			Debuggar.Message ("WheelDummy: Spawned");
		}

		/// <summary>
		/// Rotate wheel model for the specified rpm.
		/// </summary>
		/// <param name="rpm">RPM.</param>
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
	}
}

