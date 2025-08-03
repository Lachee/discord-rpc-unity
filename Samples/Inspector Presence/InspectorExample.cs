using DiscordRPC;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lachee.DiscordRPC;

namespace Lachee.DiscordRPC.Samples.InspectorPresence
{
	public class InspectorExample : MonoBehaviour
	{
		public RichPresenceObject defaultPresence;

		public List<RichPresenceObject> presenceSelector = new List<RichPresenceObject>();

		public int selectedIndex = -1;

		public void NextPresence()
		{
			selectedIndex++;
			if (selectedIndex >= presenceSelector.Count)
				selectedIndex = 0;

			defaultPresence = presenceSelector[selectedIndex];
			DiscordManager.client.SetPresence(defaultPresence.presence);
		}

		public void PreviousPresence()
		{
			selectedIndex--;
			if (selectedIndex < 0)
				selectedIndex = presenceSelector.Count - 1;

			defaultPresence = presenceSelector[selectedIndex];
			DiscordManager.client.SetPresence(defaultPresence.presence);
		}
	}
}