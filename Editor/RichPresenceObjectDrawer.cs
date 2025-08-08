using DiscordRPC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Lachee.DiscordRPC.Editor
{
	[CustomPropertyDrawer(typeof(RichPresenceObject))]
	public class RichPresenceObjectDrawer : PropertyDrawer
	{
		private bool _foldout = true;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			var presence = GetRichPresence(property);
			int prevIndentLevel = EditorGUI.indentLevel;

			Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
			_foldout = EditorGUI.BeginFoldoutHeaderGroup(rect, _foldout, label);
			if (_foldout)
			{
				EditorGUI.indentLevel++;
				try
				{
					presence.Details = EditorGUI.TextField(NEXT(ref rect), "Details", presence.Details);
					presence.DetailsUrl = EditorGUI.TextField(NEXT(ref rect), "┗ Link", presence.DetailsUrl);
					SPACE(ref rect, 5f);

					presence.State = EditorGUI.TextField(NEXT(ref rect), "State", presence.State);
					presence.StateUrl = EditorGUI.TextField(NEXT(ref rect), "┗ Link", presence.StateUrl);
					SPACE(ref rect, 5f);

					presence.StatusDisplay = (StatusDisplayType)EditorGUI.EnumPopup(NEXT(ref rect), "Status Display", presence.StatusDisplay);
					presence.Type = (ActivityType)EditorGUI.EnumPopup(NEXT(ref rect), "Activity Type", presence.Type);
					SPACE(ref rect, 5f);

					// Assets
					DrawAssets(ref rect, ref presence);

					// Party
					DrawParty(ref rect, ref presence); 

					// Timestamps
					DrawTimestamps(ref rect, ref presence);

					// Buttons
					DrawButtons(ref rect, ref presence);
				}
				catch (Exception ex)
				{
					EditorGUI.HelpBox(NEXT(ref rect), $"Error: {ex.Message}", MessageType.Error);
				}
				EditorGUI.indentLevel--;
			}
			EditorGUI.EndFoldoutHeaderGroup();
			EditorGUI.EndProperty();
			EditorGUI.indentLevel = prevIndentLevel;

			SetRichPresence(property, presence);
		}

		private void DrawAssets(ref Rect rect, ref RichPresence presence)
		{
			bool isExpanded = EditorGUI.Toggle(NEXT(ref rect), "Assets", presence.Assets != null);
			presence.Assets = presence.Assets ?? new Assets(); // Ensure Assets is initialized
			if (isExpanded)
			{
				EditorGUI.indentLevel++;
				try
				{
					EditorGUI.LabelField(NEXT(ref rect), "Large", EditorStyles.boldLabel);
					EditorGUI.indentLevel++;
					presence.Assets.LargeImageKey = EditorGUI.TextField(NEXT(ref rect), "Image Key", presence.Assets.LargeImageKey);
					presence.Assets.LargeImageText = EditorGUI.TextField(NEXT(ref rect), "┣ Text", presence.Assets.LargeImageText);
					presence.Assets.LargeImageUrl = EditorGUI.TextField(NEXT(ref rect), "┗ Link", presence.Assets.LargeImageUrl);
					EditorGUI.indentLevel--;

					EditorGUI.LabelField(NEXT(ref rect), "Small", EditorStyles.boldLabel);
					EditorGUI.indentLevel++;
					presence.Assets.SmallImageKey = EditorGUI.TextField(NEXT(ref rect), "Image Key", presence.Assets.SmallImageKey);
					presence.Assets.SmallImageText = EditorGUI.TextField(NEXT(ref rect), "┣ Text", presence.Assets.SmallImageText);
					presence.Assets.SmallImageUrl = EditorGUI.TextField(NEXT(ref rect), "┗ Link", presence.Assets.SmallImageUrl);
					EditorGUI.indentLevel--;
				}
				catch (Exception ex)
				{
					EditorGUI.HelpBox(NEXT(ref rect), $"Error: {ex.Message}", MessageType.Error);
				}
				EditorGUI.indentLevel--;
			}

			if (!isExpanded)
				presence.Assets = null; // Clear Assets if not expanded
		}

		private void DrawParty(ref Rect rect, ref RichPresence presence)
		{
			bool isExpanded = EditorGUI.Toggle(NEXT(ref rect), "Party", presence.Party != null);
			presence.Party = presence.Party ?? new Party(); // Ensure Party is initialized
			if (isExpanded)
			{
				EditorGUI.indentLevel++;
				try
				{
					presence.Party.ID = EditorGUI.TextField(NEXT(ref rect), "Party ID", presence.Party.ID ?? "");
					presence.Party.Size = EditorGUI.IntField(NEXT(ref rect), "Size", presence.Party.Size);
					presence.Party.Max = EditorGUI.IntField(NEXT(ref rect), "Max", presence.Party.Max);

				}
				catch (Exception ex)
				{
					EditorGUI.HelpBox(NEXT(ref rect), $"Error: {ex.Message}", MessageType.Error);
				}
				EditorGUI.indentLevel--;
			}

			if (!isExpanded)
				presence.Party = null; // Clear Party if not expanded
		}

		private void DrawTimestamps(ref Rect rect, ref RichPresence presence)
		{
			bool isExpanded = EditorGUI.Toggle(NEXT(ref rect),"Timestamps", presence.Timestamps != null);
			presence.Timestamps = presence.Timestamps ?? new Timestamps(); // Ensure Timestamps is initialized
			if (isExpanded)
			{
				EditorGUI.indentLevel++;
				try
				{
					// Start Time
					EditorGUI.LabelField(NEXT(ref rect), "Start Time", EditorStyles.boldLabel);
					EditorGUI.indentLevel++;

					DateTime? startTime = presence.Timestamps.Start;
					bool hasStartTime = startTime.HasValue;
					hasStartTime = EditorGUI.Toggle(NEXT(ref rect), "Enable Start Time", hasStartTime);

					if (hasStartTime)
					{
						if (!startTime.HasValue)
							startTime = DateTime.Now;

						string startTimeStr = startTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
						string newStartTimeStr = EditorGUI.TextField(NEXT(ref rect), "Start Time", startTimeStr);

						if (DateTime.TryParse(newStartTimeStr, out DateTime parsedStart))
							presence.Timestamps.Start = parsedStart;

						if (GUI.Button(NEXT(ref rect), "Set to Now"))
							presence.Timestamps.Start = DateTime.Now;
					}
					else
					{
						presence.Timestamps.Start = null;
					}
					EditorGUI.indentLevel--;

					// End Time
					EditorGUI.LabelField(NEXT(ref rect), "End Time", EditorStyles.boldLabel);
					EditorGUI.indentLevel++;

					DateTime? endTime = presence.Timestamps.End;
					bool hasEndTime = endTime.HasValue;
					hasEndTime = EditorGUI.Toggle(NEXT(ref rect), "Enable End Time", hasEndTime);

					if (hasEndTime)
					{
						if (!endTime.HasValue)
							endTime = DateTime.Now.AddMinutes(30); // Default to 30 minutes from now

						string endTimeStr = endTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
						string newEndTimeStr = EditorGUI.TextField(NEXT(ref rect), "End Time", endTimeStr);

						if (DateTime.TryParse(newEndTimeStr, out DateTime parsedEnd))
							presence.Timestamps.End = parsedEnd;

						if (GUI.Button(NEXT(ref rect), "Set to +30 min"))
							presence.Timestamps.End = DateTime.Now.AddMinutes(30);
					}
					else
					{
						presence.Timestamps.End = null;
					}
					EditorGUI.indentLevel--;
				}
				catch (Exception ex)
				{
					EditorGUI.HelpBox(NEXT(ref rect), $"Error: {ex.Message}", MessageType.Error);
				}
				EditorGUI.indentLevel--;
			}

			if (!isExpanded)
				presence.Timestamps = null; // Clear Timestamps if not expanded
		}

		private void DrawButtons(ref Rect rect, ref RichPresence presence)
		{
			bool isExpanded = EditorGUI.Toggle(NEXT(ref rect), "Buttons", presence.Buttons != null);
			List<Button> buttons = new List<Button>(presence.Buttons ?? Array.Empty<Button>());			
			if (isExpanded)
			{
				EditorGUI.indentLevel++;
				try
				{
					for (int i = 0; i < 2; i++)
					{
						if (i >= buttons.Count)
						{
							if (GUI.Button(NEXT(ref rect), "New Button") || buttons.Count == 0)
							{
								buttons.Add(new Button());
							}
							break;
						}

						Button button = buttons[i];
						EditorGUI.indentLevel++;
						button.Label = EditorGUI.TextField(NEXT(ref rect), "Label", button.Label);
						button.Url = EditorGUI.TextField(NEXT(ref rect), "┗ URL", button.Url);

						var btnArea = NEXT(ref rect);
						btnArea.xMin += EditorGUIUtility.labelWidth;
						if (buttons.Count > 1 && GUI.Button(new Rect(btnArea.x + btnArea.width - 25 - 110, btnArea.y, 100, btnArea.height), "🗑️ Delete"))
						{
							buttons.RemoveAt(i);
						}
						if (i > 0 && GUI.Button(new Rect(btnArea.x + btnArea.width - 25, btnArea.y, 25, btnArea.height), "↑"))
						{
							var temp = buttons[i - 1];
							buttons[i - 1] = button;
							buttons[i] = temp;
						} 
						else if (i < buttons.Count - 1 && GUI.Button(new Rect(btnArea.x + btnArea.width - 25, btnArea.y, 25, btnArea.height), "↓"))
						{
							var temp = buttons[i + 1];
							buttons[i + 1] = button;
							buttons[i] = temp;
						}

						EditorGUI.indentLevel--;
					}

				}
				catch (Exception ex)
				{
					EditorGUILayout.HelpBox($"Error: {ex.Message}", MessageType.Error);
				}
				EditorGUI.indentLevel--;
			}
			presence.Buttons = isExpanded ? buttons.Where(b => b != null).ToArray() : null;
		}

		private static Rect SPACE(ref Rect position, float space) {
			position.y = position.y + space;
			return position;
		}
		private static Rect NEXT(ref Rect position)
			=> SPACE(ref position, EditorGUIUtility.singleLineHeight + 1f);


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var presence = GetRichPresence(property);
			float height = EditorGUIUtility.singleLineHeight + 1f; // Header foldout

			if (!_foldout)
				return height;

			// Basic fields
			height += (EditorGUIUtility.singleLineHeight + 1f) * 2; // Details + Details URL
			height += 5f; // Space
			height += (EditorGUIUtility.singleLineHeight + 1f) * 2; // State + State URL
			height += 5f; // Space
			height += (EditorGUIUtility.singleLineHeight + 1f) * 2; // Status Display + Activity Type
			height += 5f; // Space

			// Assets section
			height += EditorGUIUtility.singleLineHeight + 1f; // Assets toggle
			if (presence.Assets != null)
			{
				height += (EditorGUIUtility.singleLineHeight + 1f) * 7; // Large label + 3 fields + Small label + 3 fields
			}

			// Party section
			height += EditorGUIUtility.singleLineHeight + 1f; // Party toggle
			if (presence.Party != null)
			{
				height += (EditorGUIUtility.singleLineHeight + 1f) * 3; // Party ID, Size, Max
			}

			// Timestamps section
			height += EditorGUIUtility.singleLineHeight + 1f; // Timestamps toggle
			if (presence.Timestamps != null)
			{
				// Start Time section
				height += EditorGUIUtility.singleLineHeight + 1f; // Start Time label
				height += EditorGUIUtility.singleLineHeight + 1f; // Enable Start Time toggle

				if (presence.Timestamps.Start.HasValue)
				{
					height += (EditorGUIUtility.singleLineHeight + 1f) * 2; // Start time field + Set to Now button
				}

				// End Time section
				height += EditorGUIUtility.singleLineHeight + 1f; // End Time label
				height += EditorGUIUtility.singleLineHeight + 1f; // Enable End Time toggle

				if (presence.Timestamps.End.HasValue)
				{
					height += (EditorGUIUtility.singleLineHeight + 1f) * 2; // End time field + Set to +30 min button
				}
			}

			// Buttons section
			height += EditorGUIUtility.singleLineHeight + 1f; // Buttons toggle
			if (presence.Buttons != null)
			{
				int buttonCount = presence.Buttons.Length;
				if (buttonCount == 0)
				{
					height += EditorGUIUtility.singleLineHeight + 1f; // "New Button" button
				}
				else
				{
					// Each button has: Label + URL + control buttons row
					height += buttonCount * (EditorGUIUtility.singleLineHeight + 1f) * 3;

					// Add space for "New Button" if we have less than 2 buttons
					if (buttonCount < 2)
					{
						height += EditorGUIUtility.singleLineHeight + 1f;
					}
				}
			}

			return height + EditorGUIUtility.singleLineHeight;
		}

		private void SetRichPresence(SerializedProperty property, RichPresence presence) 
		{
			var jsonProperty = property.FindPropertyRelative("m_json");
			jsonProperty.stringValue = Newtonsoft.Json.JsonConvert.SerializeObject(presence);
			jsonProperty.serializedObject.ApplyModifiedProperties();
		}

		private RichPresence GetRichPresence(SerializedProperty property) 
		{
			var jsonProperty = property.FindPropertyRelative("m_json");
			return Newtonsoft.Json.JsonConvert.DeserializeObject<RichPresence>(jsonProperty.stringValue) ?? new RichPresence();
		}
	}
}