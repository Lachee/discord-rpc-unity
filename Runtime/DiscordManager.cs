#if ENABLE_IL2CPP || !NETSTANDARD2_1
#define NATIVE_PIPES
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiscordRPC;
using DiscordRPC.Logging;
using Lachee.DiscordRPC.Logging;
using DiscordRPC.IO;

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

        public void Initialize()
        {
            if (client != null)
                Deinitialize();

            var logger = new UnityLogger() { Level = LogLevel.Trace };

            INamedPipeClient pipeClient;
#if NATIVE_PIPES
            pipeClient = new Lachee.DiscordRPC.UnityNamedPipes.NativeNamedPipeClient();
#else
            pipeClient = new ManagedNamedPipeClient();
#endif

            client = new DiscordRpcClient(applicationId, logger: logger, autoEvents: false, client: pipeClient);
            client.Initialize();
            client.SetPresence(new RichPresence() { Details = "Hello from Unity" });
        }

        public void Deinitialize() {
            client.Deinitialize();
            client.Dispose();
            client = null;
        }
    }
}