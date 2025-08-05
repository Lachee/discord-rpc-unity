using DiscordRPC;
using UnityEngine;

namespace Lachee.DiscordRPC
{
	/// <summary>
	/// Sets the Discord Rich Presence.
	/// </summary>
    public class SetRichPresence : MonoBehaviour
    {
        private DiscordRpcClient client => DiscordManager.client;

		[Tooltip("Sets the presence on Start")]
		public bool onStart = true;

		[Tooltip("The presence")]
		public RichPresenceObject presence;

		public void Start()
		{
			if (onStart)
			{
				Apply();
			}
		}

		/// <summary>
		/// Applies the current presence.
		/// </summary>
        public void Apply() 
		{
			if (client == null)
			{
				Debug.LogError("Cannot set Rich Presence  because there is no DiscordRpcClient.", this);
				return;
			}

			client.SetPresence(presence);
		}
	}
}