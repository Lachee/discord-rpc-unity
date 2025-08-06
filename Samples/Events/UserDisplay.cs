using DiscordRPC;
using DiscordRPC.Message;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Lachee.DiscordRPC.Samples.Events
{
	public class UserDisplay : MonoBehaviour
	{
		// This is a very simple dirty cache for avatars which will grow memory usage over time.
		// A better solution will be to save the avatar textures to a temporary directory or "{Application.persistentDataPath}/cache/{user.Avatar}.png"
		// then attempt to load that file in case of cache miss rather than storing every downloaded texture in a static array.
		private static Dictionary<string, Texture2D> avatarCache = new Dictionary<string, Texture2D>();

		public Text usernameText;
		public RawImage avatarImage;
		public RawImage decorationImage;

		void Start()
		{
			// Subscribe to the ready. Not using a anonymous function so we can unsubscribe later
			DiscordManager.client.OnReady += OnReady;

			// If this is added after a ready, we can update the display immediately
			if (DiscordManager.client?.CurrentUser != null)
				_ = UpdateUserDisplay(DiscordManager.client.CurrentUser);
		}

		void OnDestroy()
		{
			// Clean Up so we don't try invoke events on a destroyed GameObject
			if (DiscordManager.client != null)
				DiscordManager.client.OnReady -= OnReady;
		}

		private async void OnReady(object sender, ReadyMessage e)
		{
			await UpdateUserDisplay(e.User);
		}

		private async Task UpdateUserDisplay(User user)
		{
			usernameText.text = user.DisplayName;

			// Get the url and download the avatar texture
			Texture2D avatarTexture;
			if (!avatarCache.TryGetValue(user.Avatar, out avatarTexture))
				avatarTexture = avatarCache[user.Avatar] = await user.DownloadAvatarAsync();
			avatarImage.texture = avatarTexture;

			// If the user has a decoration, we can also download that
			// Note this is a Animated PNG (APNG), but Unity doesn't support that.
			// If you wish to use a animated image, you will need to use a third-party library.
			decorationImage.enabled = false;
			if (user.AvatarDecoration != null)
			{
				Texture2D decorationTexture;
				if (!avatarCache.TryGetValue(user.AvatarDecoration.Value.Asset, out decorationTexture))
					decorationTexture = avatarCache[user.AvatarDecoration.Value.Asset] = await user.DownloadAvatarDecorationAsync();
				decorationImage.texture = decorationTexture;
				decorationImage.enabled = true;
			}
		}
	}
}