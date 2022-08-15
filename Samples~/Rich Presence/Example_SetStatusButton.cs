using Lachee.Discord;
using Lachee.Discord.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiscordRPC.Examples.RichPresence
{
	public class Example_SetStatusButton : MonoBehaviour
	{
		public UnityEngine.UI.Button button;
		public UnityEngine.UI.InputField input;

		void Start()
        {
			// This method is only required for 2018 or below. 
			// Since 2019, Unity can serialize generics
			DiscordManager.current.OnReady.AddListener(OnReady);
			button.onClick.AddListener(OnClick);
			button.interactable = false;
		}

		public void OnReady(ReadyEvent evt)
		{
			button.interactable = true;
		}

		public void OnClick()
        {
			var presence = DiscordManager.current.CurrentPresence;
			presence.details = input.text;
			DiscordManager.current.SetPresence(presence);
        }
	}
}