using DiscordRPC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lachee.DiscordRPC
{
	[Serializable]
	public sealed class RichPresenceObject
	{
		[SerializeField, TextArea]
		private string m_json;

		private RichPresence m_presence = null;
		public RichPresence presence
		{
			get
			{
				if (m_presence == null)
					return Deserialize();
				return m_presence;
			}
			set
			{
				m_presence = value;
				Serialize();
			}
		}

		public RichPresenceObject() 
		{
			this.m_presence = null;
		}

		public RichPresenceObject(RichPresence presence) 
		{
			this.presence = presence;
		}

		/// <summary>
		/// Loads the Rich Presence object from the internal serialized state
		/// </summary>
		/// <remarks>This is used in the Editor to serialize/deserialize the object from the discord-rpc-csharp library.</remarks>
		/// <returns></returns>
		public RichPresence Deserialize()
			=> m_presence = Newtonsoft.Json.JsonConvert.DeserializeObject<RichPresence>(m_json) ?? new RichPresence();

		/// <summary>
		/// Saves the Rich PResence object to the internal serialized state
		/// </summary>
		/// <remarks>This is used in the Editor to serialize/deserialize the object from the discord-rpc-csharp library.</remarks>
		/// <returns></returns>
		public string Serialize()
			=> m_json = Newtonsoft.Json.JsonConvert.SerializeObject(m_presence ?? new RichPresence());
		
		public static implicit operator RichPresence(RichPresenceObject obj)
		{
			return obj.presence;
		}

		public static implicit operator RichPresenceObject(RichPresence obj)
		{
			return new RichPresenceObject(obj);
		}
	}
}
