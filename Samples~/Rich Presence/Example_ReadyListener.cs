using Lachee.Discord;
using Lachee.Discord.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiscordRPC.Examples.RichPresence
{
	public class Example_ReadyListener : MonoBehaviour
	{
		void Start()
        {
			// This method is only required for 2018 or below. 
			// Since 2019, Unity can serialize generics
			DiscordManager.current.OnReady.AddListener(OnReady);
        }

		public void OnReady(ReadyEvent evt)
		{
			Debug.Log("Received Ready!");
			evt.user.GetAvatar(DiscordAvatarSize.x1024, (user, texture) =>
			{
				var renderer = GetComponent<Renderer>();
				renderer.material.mainTexture = texture;
			});
		}
	}
}