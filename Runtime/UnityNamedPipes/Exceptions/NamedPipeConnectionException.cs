using System;

namespace Lachee.DiscordRPC.UnityNamedPipes.Exceptions
{
    public class NamedPipeConnectionException : Exception
    {
        internal NamedPipeConnectionException(string message) : base(message) { }
    }
}
