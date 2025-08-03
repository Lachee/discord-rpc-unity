using DiscordRPC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lachee.DiscordRPC.Samples.BasicPresence
{
    public class BasicGameManager : MonoBehaviour
    {
        public string details;
        public string state;

		public void Start()
		{
			DiscordManager.client.SetPresence(new RichPresence()
			{
				Details = details,
				State = state,
			});
		}
	}
}