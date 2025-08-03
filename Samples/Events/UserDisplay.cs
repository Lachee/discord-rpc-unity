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
			string avatarUrl = user.GetAvatarURL();
			avatarImage.texture = await DownloadTextureAsync(avatarUrl);

			// If the user has a decoration, we can also download that
			// Note this is a Animated PNG (APNG), but Unity doesn't support that.
			// If you wish to use a animated image, you will need to use a third-party library.
			decorationImage.enabled = false;
			if (user.AvatarDecoration != null)
			{
				string decorationUrl = user.GetAvatarDecorationURL();
				decorationImage.texture = await DownloadTextureAsync(decorationUrl);
				decorationImage.enabled = true;
			}
		}

		private static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
		private async Task<Texture2D> DownloadTextureAsync(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				Debug.LogError("URL is null or empty");
				return null;
			}

			// A very basic cache to avoid downloading the same texture multiple times.
			// When implementing this in a real project, consider using a more robust caching solution using disk.
			if (textureCache.TryGetValue(url, out Texture2D cachedTexture))
				return cachedTexture;

			using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
			{
				// Send the request and wait for completion
				var operation = request.SendWebRequest();
				while (!operation.isDone)
					await Task.Yield();

#if UNITY_2021_3_OR_NEWER
				if (request.result != UnityWebRequest.Result.Success)
#else
				if (request.isNetworkError || request.isHttpError)
#endif
				{
					Debug.LogError($"Failed to download texture from {url}: {request.error}");
					return null;
				}

				return textureCache[url] = DownloadHandlerTexture.GetContent(request);
			}
		}
	}
}