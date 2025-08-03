#if ENABLE_IL2CPP || !NETSTANDARD2_1
#define NATIVE_PIPES
#endif

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
        public static DiscordRpcClient client => current.m_client;

		[Header("Properties")]
		[Tooltip("The ID of the Discord Application. Visit the Discord API to create a new application if nessary.")]
		public string applicationId = "424087019149328395";

		[Tooltip("The Steam App ID. This is a optional field used to launch your game through steam instead of the executable.")]
		public string steamId = "";

		[Tooltip("Registers a custom URI scheme for your game. This is required for the Join / Specate features to work.")]
		public bool registerUriScheme = false;

		[Header("Logging")]
		[Tooltip("Logging level of the Discord IPC connection.")]
		public LogLevel logLevel = LogLevel.Warning;

		[Tooltip("The file to write the logs too in a build. If empty, then the console logger will be used.")]
		public string logFile = "discord.log";

        [Header("State")]
        [SerializeField]
        private User m_user;
        public User user => m_user;
        private UnityLogger m_logger;
        private DiscordRpcClient m_client;

		void Awake() 
        {
            DontDestroyOnLoad(this);

            current = this;
            m_logger = new UnityLogger() {
                Level = logLevel 
            };

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

            m_logger.Level = logLevel;
            client.Invoke();
        }

        public void Initialize()
        {
            if (client != null)
                Deinitialize();

#if NATIVE_PIPES
			INamedPipeClient pipeClient = new Lachee.DiscordRPC.UnityNamedPipes.NativeNamedPipeClient();
#else
            INamedPipeClient pipeClient = new ManagedNamedPipeClient();
#endif

			m_client = new DiscordRpcClient(
                applicationId, 
                logger: m_logger, 
                autoEvents: false, 
                client: pipeClient
            );
			
            if (registerUriScheme)
                m_client.RegisterUriScheme(steamId);

			m_client.OnError += (s, args) => m_logger.Error($"[DRP] Error Occured within the Discord IPC: ({args.Code}) {args.Message}");
			m_client.OnReady += (s, args) =>
            {
				//We have connected to the Discord IPC. We should send our rich presence just incase it lost it.
				m_logger.Info("[DRP] Connection established and received READY from Discord IPC.");
                m_user = args.User;
            };

			m_client.Initialize();
		}

		public void Deinitialize() {
			m_client.Deinitialize();
			m_client.Dispose();
			m_client = null;
        }
    }
}