using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DiscordRPC;
using DiscordRPC.Logging;
using Lachee.DiscordRPC.Logging;

namespace Lachee.DiscordRPC
{
    public class DiscordManager : MonoBehaviour
    {
        public static DiscordManager current { get; private set; }
        public DiscordRpcClient client { get; private set; }

        public string applicationId = "424087019149328395";

		void Awake() 
        {
            current = this;
        }

        void Start() 
        {
            Initialize();
        }

        void OnDestroy() 
        {
            Deinitialize();
        }

        void Update() 
        {
            if (client == null) 
                return;

            client.Invoke();
        }

        public void Initialize()  {
            if (client != null)
                Deinitialize();

            var logger = new UnityLogger() { Level = LogLevel.Trace };
            client = new DiscordRpcClient(applicationId, logger: logger, autoEvents: false);
            client.Initialize();
        }

        public void Deinitialize() {
            client.Deinitialize();
            client.Dispose();
            client = null;
        }
    }
}