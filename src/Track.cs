//=============================================================
// Back to bring-a-booksville
// We've all been there, what's the next stop?
// I won't sit still 
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

		public bool bIsMirror = false;

		public GameObject Root;
		public SkinnedMeshRenderer TrackSurface;
		public Transform TrackTransform;
		public Transform TrackSurfaceTransform;

		public WheelDummyList WheelDummies;

		public TrackConfigContainer Config;

		//public SuspConfig Susp;

		public Track(Transform transform, TrackConfigContainer configContainer, bool mirror)
		{
			Config = configContainer;

			TrackTransform = transform;

			Root = TrackTransform.gameObject;

			WidthScale = Config.Width;

			Component[] components = Root.GetComponentsInChildren<Component> ();
			//Debuggar.Message ("Fetching Components: " + components.Count ().ToString ());

			WheelDummies = new WheelDummyList (components, Config.WheelDummyConfig, Config.ModelConfig);

			foreach (Component o in components)
			{
				if (o.name.StartsWith (Config.ModelConfig.TrackSurface))
				{
					TrackSurface = o as SkinnedMeshRenderer;
					//Debuggar.Message ("Found track surface: " + o.name);
				}
			}
			TrackSurfaceTransform = Root.transform.Find (Config.ModelConfig.TrackSurface);
			if (TrackSurfaceTransform == null)
				Debuggar.Error ("TrackSurfaceTransform is null");

			bIsMirror = mirror;
			Debuggar.Message ("Track spawned");
		}

		public void Update()
		{
			WheelDummies.Update ();
			RPM = WheelDummies.RPM;

			float distanceTravelled = (float)((WheelDummies.RealRPM * 2 * Math.PI) / 60) * Time.deltaTime;
			Material trackMaterial = TrackSurface.renderer.material;
			Vector2 textureOffset = trackMaterial.mainTextureOffset;
			textureOffset = textureOffset + new Vector2(-distanceTravelled / Config.Length, 0);
			trackMaterial.SetTextureOffset("_MainTex", textureOffset);
			trackMaterial.SetTextureOffset("_BumpMap", textureOffset);
		}

		public void FixedUpdate ()
		{
			WheelDummies.FixedUpdate ();
		}

		public void Brakes(bool active)
		{
			if (active)
				WheelDummies.BrakingTorque = Config.WheelDummyConfig.BrakingTorque;
			else
				WheelDummies.BrakingTorque = Config.WheelDummyConfig.RollingResistance;
		}

		public void ApplyTorque(float torque)
		{
			WheelDummies.Torque = torque;
		}
	}
}

