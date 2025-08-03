using UnityEngine;
using UnityEditor;
using DiscordRPC;
using System;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

namespace Lachee.DiscordRPC.Editor
{
	[CustomPropertyDrawer(typeof(RichPresenceObject))]
	public class RichPresenceObjectDrawer : PropertyDrawer
	{
		private bool _foldout = true;
		private bool _showJson = true;
		private static readonly float LINE_HEIGHT = EditorGUIUtility.singleLineHeight;
		private static readonly float SPACING = EditorGUIUtility.standardVerticalSpacing;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			var container = property.GetSerializedValue() as RichPresenceObject;
			var presence = container.presence ?? new RichPresence();
			int prevIndentLevel = EditorGUI.indentLevel;

			_foldout = EditorGUILayout.BeginFoldoutHeaderGroup(_foldout, label);
			if (_foldout)
			{
				EditorGUI.indentLevel++;
				try
				{
					presence.Details = EditorGUILayout.TextField("Details", presence.Details);
					presence.DetailsUrl = EditorGUILayout.TextField("┗ Link", presence.DetailsUrl);
					GUILayout.Space(5);

					presence.State = EditorGUILayout.TextField("State", presence.State);
					presence.StateUrl = EditorGUILayout.TextField("┗ Link", presence.StateUrl);
					GUILayout.Space(5);

					presence.StatusDisplay = (StatusDisplayType)EditorGUILayout.EnumPopup("Status Display", presence.StatusDisplay);
					presence.Type = (ActivityType)EditorGUILayout.EnumPopup("Activity Type", presence.Type);
					GUILayout.Space(5);

					// Assets
					DrawAssets(ref presence);
					GUILayout.Space(5);

					// Party
					DrawParty(ref presence);
					GUILayout.Space(5);

					// Timestamps
					DrawTimestamps(ref presence);
					GUILayout.Space(5);

					// Buttons
					DrawButtons(ref presence);
				}
				catch (Exception ex)
				{
					EditorGUILayout.HelpBox($"Error: {ex.Message}", MessageType.Error);
				}
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
			EditorGUI.EndProperty();

			container.presence = presence;
			ApplyProperties(property, container);
			EditorGUI.indentLevel = prevIndentLevel;
		}

		private void DrawAssets(ref RichPresence presence)
		{
			bool isExpanded = EditorGUILayout.BeginToggleGroup("Assets", presence.Assets != null);
			presence.Assets ??= new Assets(); // Ensure Assets is initialized
			if (isExpanded)
			{
				EditorGUI.indentLevel++;
				try
				{
					EditorGUILayout.LabelField("Large", EditorStyles.boldLabel);
					EditorGUI.indentLevel++;
					presence.Assets.LargeImageKey = EditorGUILayout.TextField("Image Key", presence.Assets.LargeImageKey);
					presence.Assets.LargeImageText = EditorGUILayout.TextField("┣ Text", presence.Assets.LargeImageText);
					presence.Assets.LargeImageUrl = EditorGUILayout.TextField("┗ Link", presence.Assets.LargeImageUrl);
					EditorGUI.indentLevel--;

					EditorGUILayout.LabelField("Small", EditorStyles.boldLabel);
					EditorGUI.indentLevel++;
					presence.Assets.SmallImageKey = EditorGUILayout.TextField("Image Key", presence.Assets.SmallImageKey);
					presence.Assets.SmallImageText = EditorGUILayout.TextField("┣ Text", presence.Assets.SmallImageText);
					presence.Assets.SmallImageUrl = EditorGUILayout.TextField("┗ Link", presence.Assets.SmallImageUrl);
					EditorGUI.indentLevel--;
				}
				catch (Exception ex)
				{
					EditorGUILayout.HelpBox($"Error: {ex.Message}", MessageType.Error);
				}
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndToggleGroup();

			if (!isExpanded)
				presence.Assets = null; // Clear Assets if not expanded
		}

		private void DrawParty(ref RichPresence presence)
		{
			bool isExpanded = EditorGUILayout.BeginToggleGroup("Party", presence.Party != null);
			presence.Party ??= new Party(); // Ensure Party is initialized
			if (isExpanded)
			{
				EditorGUI.indentLevel++;
				try
				{
					presence.Party.ID = EditorGUILayout.TextField("Party ID", presence.Party.ID ?? "");
					presence.Party.Size = EditorGUILayout.IntField("Size", presence.Party.Size);
					presence.Party.Max = EditorGUILayout.IntField("Max", presence.Party.Max);

				}
				catch (Exception ex)
				{
					EditorGUILayout.HelpBox($"Error: {ex.Message}", MessageType.Error);
				}
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndToggleGroup();

			if (!isExpanded)
				presence.Party = null; // Clear Party if not expanded
		}

		private void DrawTimestamps(ref RichPresence presence)
		{
			bool isExpanded = EditorGUILayout.BeginToggleGroup("Timestamps", presence.Timestamps != null);
			presence.Timestamps ??= new Timestamps(); // Ensure Timestamps is initialized
			if (isExpanded)
			{
				EditorGUI.indentLevel++;
				try
				{
					// Start Time
					EditorGUILayout.LabelField("Start Time", EditorStyles.boldLabel);
					EditorGUI.indentLevel++;

					DateTime? startTime = presence.Timestamps.Start;
					bool hasStartTime = startTime.HasValue;
					hasStartTime = EditorGUILayout.Toggle("Enable Start Time", hasStartTime);

					if (hasStartTime)
					{
						if (!startTime.HasValue)
							startTime = DateTime.Now;

						string startTimeStr = startTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
						string newStartTimeStr = EditorGUILayout.TextField("Start Time", startTimeStr);

						if (DateTime.TryParse(newStartTimeStr, out DateTime parsedStart))
							presence.Timestamps.Start = parsedStart;

						if (GUILayout.Button("Set to Now"))
							presence.Timestamps.Start = DateTime.Now;
					}
					else
					{
						presence.Timestamps.Start = null;
					}
					EditorGUI.indentLevel--;

					// End Time
					EditorGUILayout.LabelField("End Time", EditorStyles.boldLabel);
					EditorGUI.indentLevel++;

					DateTime? endTime = presence.Timestamps.End;
					bool hasEndTime = endTime.HasValue;
					hasEndTime = EditorGUILayout.Toggle("Enable End Time", hasEndTime);

					if (hasEndTime)
					{
						if (!endTime.HasValue)
							endTime = DateTime.Now.AddMinutes(30); // Default to 30 minutes from now

						string endTimeStr = endTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
						string newEndTimeStr = EditorGUILayout.TextField("End Time", endTimeStr);

						if (DateTime.TryParse(newEndTimeStr, out DateTime parsedEnd))
							presence.Timestamps.End = parsedEnd;

						if (GUILayout.Button("Set to +30 min"))
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
					EditorGUILayout.HelpBox($"Error: {ex.Message}", MessageType.Error);
				}
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndToggleGroup();

			if (!isExpanded)
				presence.Timestamps = null; // Clear Timestamps if not expanded
		}

		private void DrawButtons(ref RichPresence presence)
		{
			bool isExpanded = EditorGUILayout.BeginToggleGroup("Buttons", presence.Buttons != null);
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
							EditorGUILayout.BeginHorizontal();
							GUILayout.FlexibleSpace();
							if (GUILayout.Button("New Button") || buttons.Count == 0)
							{
								buttons.Add(new Button());
							}
							EditorGUILayout.EndHorizontal();
							break;
						}

						Button button = buttons[i];
						EditorGUI.indentLevel++;
						button.Label = EditorGUILayout.TextField("Label", button.Label);
						button.Url = EditorGUILayout.TextField("┗ URL", button.Url);
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();

						if (buttons.Count > 1 && GUILayout.Button("🗑️ Delete"))
						{
							buttons.RemoveAt(i);
						}

						if (i > 0 && GUILayout.Button("↑"))
						{
							var temp = buttons[i - 1];
							buttons[i - 1] = button;
							buttons[i] = temp;
						}

						if (i < buttons.Count - 1 && GUILayout.Button("↓"))
						{
							var temp = buttons[i + 1];
							buttons[i + 1] = button;
							buttons[i] = temp;
						}

						EditorGUILayout.EndHorizontal();
						EditorGUI.indentLevel--;
					}

				}
				catch (Exception ex)
				{
					EditorGUILayout.HelpBox($"Error: {ex.Message}", MessageType.Error);
				}
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndToggleGroup();
			presence.Buttons = isExpanded ? buttons.Where(b => b != null).ToArray() : null;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return 0f; // Let EditorGUILayout handle the height automatically
		}

		private void ApplyProperties(SerializedProperty property, RichPresenceObject richPresenceObject)
		{
			if (richPresenceObject != null)
			{
				string json = richPresenceObject.Serialize();
				property.FindPropertyRelative("_json").stringValue = json;
				property.serializedObject.ApplyModifiedProperties();
			}
		}
	}
}