using Lachee.Discord;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DiscordRPC.Examples.RichPresence
{
	public class Example_RichPresence : MonoBehaviour
	{

		[Header("Details")]
		public InputField inputDetails, inputState;

		[Header("Time")]
		public Toggle inputStartTime;
		public InputField inputEndTime;

		[Header("Images")]
		public InputField inputLargeKey;
		public InputField inputLargeTooltip;
		public InputField inputSmallKey;
		public InputField inputSmallTooltip;


		public Presence presence;

		private void Start()
		{
			//Update the values
			UpdateFields(presence);

			//Register to a presence change
			DiscordManager.current.OnPresence.AddListener((message) =>
			{
				Debug.Log("Received a new presence! Current App: " + message.applicationID + ", " + message.name);
				UpdateFields(presence);
			});
		}

		public void SendPresence()
		{
			UpdatePresence();
			DiscordManager.current.SetPresence(presence);
		}

		public void UpdateFields(Presence presence)
		{
			if (presence == null)
            {
				return;
            }

			this.presence = presence;
			inputState.text = presence.state;
			inputDetails.text = presence.details;


			inputLargeTooltip.text = presence.largeAsset.tooltip;
			inputLargeKey.text = presence.largeAsset.image;

			inputSmallTooltip.text = presence.smallAsset.tooltip;
			inputSmallKey.text = presence.smallAsset.image;
		}

		public void UpdatePresence()
		{
			presence.state = inputState.text;
			presence.details = inputDetails.text;
			presence.startTime = inputStartTime.isOn ? new Timestamp(Time.realtimeSinceStartup) : Timestamp.Invalid;

			presence.largeAsset = new Asset()
			{
				image = inputLargeKey.text,
				tooltip = inputLargeTooltip.text
			};
			presence.smallAsset = new Asset()
			{
				image = inputSmallKey.text,
				tooltip = inputSmallTooltip.text
			};

			float duration = float.Parse(inputEndTime.text);
			presence.endTime = duration > 0 ? new Timestamp(Time.realtimeSinceStartup + duration) : Timestamp.Invalid;
		}
	}
}