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

		public Track(Transform transform, TrackConfigContainer configContainer, bool mirror)
		{
			Config = configContainer;

			TrackTransform = transform;

			Root = TrackTransform.gameObject;

			WidthScale = Config.Width;

			Component[] components = Root.GetComponentsInChildren<Component> ();

			WheelDummies = new WheelDummyList (components, Config.WheelDummyConfig, Config.ModelConfig);

			// this can probably be slimmed down a bit, a full blown foreach isn't needed here anymore
			// i mean come on, we're looking for ONE COMPONENT but looking through ALL OF THEM
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

		public void Update()
		{
			if (WheelDummies != null)
			{
				WheelDummies.Update ();
				RPM = WheelDummies.RPM;

				// this needs to be done much different
				float distanceTravelled = (float)((WheelDummies.RealRPM * 2 * Math.PI) / 60) * Time.deltaTime;
				Material trackMaterial = TrackSurface.renderer.material;
				Vector2 textureOffset = trackMaterial.mainTextureOffset;
				textureOffset = textureOffset + new Vector2 (-distanceTravelled / Config.Length, 0);
				trackMaterial.SetTextureOffset ("_MainTex", textureOffset);
				trackMaterial.SetTextureOffset ("_BumpMap", textureOffset);
			}
			else Debuggar.Error ("Track in Update(): WheelDummies is null");
		}

		public void FixedUpdate ()
		{
			if (WheelDummies != null)
			{
				WheelDummies.FixedUpdate ();
			}
			else Debuggar.Error ("Track in FixedUpdate(): WheelDummies is null");
		}

		// I think this can be done a bit better
		public void Brakes(bool active)
		{
			if (WheelDummies != null)
			{
				if (active)
					WheelDummies.BrakingTorque = Config.WheelDummyConfig.BrakingTorque;
				else
					WheelDummies.BrakingTorque = Config.WheelDummyConfig.RollingResistance;
			}
			else Debuggar.Error ("Track in Brakes(): WheelDummies is null");
		}

		// as can this
		public void ApplyTorque(float torque)
		{
			if (WheelDummies != null)
				WheelDummies.Torque = torque;
			else
				Debuggar.Error ("Track in ApplyTorque(): WheelDummies is null");
		}
	}
}

