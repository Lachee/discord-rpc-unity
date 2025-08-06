using System.Threading.Tasks;
using DiscordRPC;
using UnityEngine;
using UnityEngine.Networking;

namespace Lachee.DiscordRPC
{
    public static class UserExtension
    {
#if !UNITY_2017_1_OR_NEWER
        [System.Obsolete("API requires UNITY_2017_1_OR_NEWER")]
        public static Task<Texture2D> DownloadAvatarAsync(this User user, User.AvatarSize size = User.AvatarSize.x512)
            => Task.FromResult<Texture2D>(null);
            
        [System.Obsolete("API requires UNITY_2017_1_OR_NEWER")]
        public static Task<Texture2D> DownloadAvatarDecorationAsync(this User user)
            => Task.FromResult<Texture2D>(null);
#else
        /// <summary>
        /// Downloads the user's avatar as a Texture2D asynchronously.
        /// </summary>
        /// <remarks>
        /// There is no caching provided. A call to this function will result in a Web Request being made.
        /// </remarks>
        /// <param name="user"></param>
        /// <param name="size">Target size of the texture</param>
        /// <returns></returns>
        public static async Task<Texture2D> DownloadAvatarAsync(this User user, User.AvatarSize size = User.AvatarSize.x512)
            => await DownloadImageAsync(user.GetAvatarURL(User.AvatarFormat.PNG, size));

        /// <summary>
        /// Downloads the user's avatar decoration as a Texture2D asynchronously.
        /// </summary>
        /// <remarks>
        /// There is no caching provided. A call to this function will result in a Web Request being made.
        /// <para>The decoration will not be animated.</para>
        /// </remarks>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task<Texture2D> DownloadAvatarDecorationAsync(this User user)
            => await DownloadImageAsync(user.GetAvatarDecorationURL(User.AvatarFormat.PNG));

        private static async Task<Texture2D> DownloadImageAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("URL is null or empty");
                return null;
            }

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

                return DownloadHandlerTexture.GetContent(request);
            }
        }
#endif
    }
}
