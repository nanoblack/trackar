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

		public bool bIsMirror = false;

		public GameObject Root;
		public SkinnedMeshRenderer TrackSurface;
		public Transform TrackTransform;
		public Transform TrackSurfaceTransform;

		public WheelDummyList WheelDummies;

		public ConfigContainer Config;

		public SuspConfig Susp;

		public Track(Transform transform, ConfigContainer configContainer, bool mirror)
		{
			Debuggar.Message ("New Track instantiated");

			Config = configContainer;

			TrackTransform = transform;

			Root = TrackTransform.gameObject;

			WidthScale = Config.TrackWidth;

			Component[] components = Root.GetComponentsInChildren<Component> ();
			Debuggar.Message ("Fetching Components: " + components.Count ().ToString ());

			WheelDummies = new WheelDummyList (components, Config);

			foreach (Component o in components)
			{
				if (o.name.StartsWith (Config.TrackSurfaceName))
				{
					TrackSurface = o as SkinnedMeshRenderer;
					Debuggar.Message ("Found track surface: " + o.name);
				}
			}
			TrackSurfaceTransform = Root.transform.Find (Config.TrackSurfaceName);
			if (TrackSurfaceTransform == null)
				Debuggar.Message ("TrackSurfaceTransform is null!");

			bIsMirror = mirror;
		}

		public void Update()
		{
			WheelDummies.Update ();
			RPM = WheelDummies.RPM;

			float distanceTravelled = (float)((WheelDummies.RealRPM * 2 * Math.PI) / 60) * Time.deltaTime;
			Material trackMaterial = TrackSurface.renderer.material;
			Vector2 textureOffset = trackMaterial.mainTextureOffset;
			textureOffset = textureOffset + new Vector2(-distanceTravelled / Config.TrackLength, 0);
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
				WheelDummies.BrakingTorque = Config.BrakingTorque;
			else
				WheelDummies.BrakingTorque = Config.RollingResistance;
		}

		public void ApplyTorque(float torque)
		{
			WheelDummies.Torque = torque;
		}
	}
}

