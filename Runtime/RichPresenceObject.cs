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
		private string _json;

		private RichPresence m_presence = null;
		public RichPresence presence
		{
			get
			{
				if (m_presence != null)
					return m_presence;
				return Deserialize();
			}
			set
			{
				m_presence = value;
				Serialize();
			}
		}

		public RichPresence Deserialize()
		{
			return m_presence = Newtonsoft.Json.JsonConvert.DeserializeObject<RichPresence>(_json);
		}

		public string Serialize()
		{
			return _json = Newtonsoft.Json.JsonConvert.SerializeObject(m_presence);
		}

		public static implicit operator RichPresence(RichPresenceObject obj)
		{
			return obj.presence;
		}

		public static implicit operator RichPresenceObject(RichPresence obj)
		{
			return new RichPresenceObject()
			{
				presence = obj
			};
		}
	}
}
