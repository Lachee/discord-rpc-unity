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

		public bool useFileCache = false;

		void Start()
		{
			// Subscribe to the ready. Not using a anonymous function so we can unsubscribe later
			// We directly use the client here as an example, 
			// 	but you can also use DiscordManager.current.onReady.addListener(OnReady);
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
			Texture2D avatarTexture = GetCachedAvatar(user.Avatar);
			if (avatarTexture == null)
			{
				avatarTexture = await user.DownloadAvatarAsync();
				CacheAvatar(user.Avatar, avatarTexture);
			}
			avatarImage.texture = avatarTexture;
			avatarImage.enabled = true;

			// If the user has a decoration, we can also download that
			// Note this is a Animated PNG (APNG), but Unity doesn't support that.
			// If you wish to use a animated image, you will need to use a third-party library.
			decorationImage.enabled = false;
			if (user.AvatarDecoration != null)
			{
				Texture2D decorationTexture = GetCachedAvatar(user.AvatarDecoration.Value.Asset);
				if (decorationTexture == null)
				{
					decorationTexture = await user.DownloadAvatarDecorationAsync();
					CacheAvatar(user.AvatarDecoration.Value.Asset, decorationTexture);
				}

				decorationImage.texture = decorationTexture;
				decorationImage.enabled = true;
			}
		}


		private Texture2D GetCachedAvatar(string avatarId)
		{
			if (avatarCache.TryGetValue(avatarId, out Texture2D texture))
				return texture;

			// Here is an example of a file cache implementation.
			// We store the avatar texture in our persistent data path for repeated runs.
			if (useFileCache)
			{
				string dataPath = GetAvatarCachePath(avatarId);
				if (System.IO.File.Exists(dataPath))
				{
					byte[] fileData = System.IO.File.ReadAllBytes(dataPath);
					texture = new Texture2D(2, 2);
					texture.LoadImage(fileData);
					avatarCache[avatarId] = texture;
					Debug.Log($"Avatar loaded from {dataPath}");
					return texture;
				}
			}

			return null;
		}

		private void CacheAvatar(string avatarId, Texture2D texture)
		{
			// Store it to disk
			if (useFileCache && texture != null)
			{
				string dataPath = GetAvatarCachePath(avatarId);
				System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dataPath));
				System.IO.File.WriteAllBytes(dataPath, texture.EncodeToPNG());
				Debug.Log($"Avatar cached to {dataPath}");
			}

			avatarCache[avatarId] = texture;
		}

		/// <summary>
		/// Gets the path to the file where the avatar will be cached.
		/// </summary>
		/// <remarks>
		/// The avatar is stored as a PNG file in the persistent data path.
		/// </remarks>
		/// <param name="avatarId"></param>
		/// <returns></returns>
		private static string GetAvatarCachePath(string avatarId)
			=> $"{Application.persistentDataPath}/discord/avatar_{avatarId}.png";
	}
}