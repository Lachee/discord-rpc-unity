using UnityEngine;
using UnityEditor;
using DiscordRPC;
using System;

namespace Lachee.DiscordRPC.Editor
{
	[CustomPropertyDrawer(typeof(RichPresenceObject))]
	public class RichPresenceObjectDrawer : PropertyDrawer
	{
		private bool _foldout = false;
		private bool _showJson = false;
		private static readonly float LINE_HEIGHT = EditorGUIUtility.singleLineHeight;
		private static readonly float SPACING = EditorGUIUtility.standardVerticalSpacing;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			var jsonProperty = property.FindPropertyRelative("_json");
			var richPresenceObject = GetRichPresenceObject(property);

			// Main foldout
			Rect foldoutRect = new Rect(position.x, position.y, position.width, LINE_HEIGHT);
			_foldout = EditorGUI.Foldout(foldoutRect, _foldout, label, true);

			if (_foldout)
			{
				EditorGUI.indentLevel++;
				float currentY = position.y + LINE_HEIGHT + SPACING;

				try
				{
					var presence = richPresenceObject?.presence;

					if (presence != null)
					{
						// Details
						Rect detailsRect = new Rect(position.x, currentY, position.width, LINE_HEIGHT);
						string newDetails = EditorGUI.TextField(detailsRect, "Details", presence.Details ?? "");
						if (newDetails != (presence.Details ?? ""))
						{
							presence.Details = string.IsNullOrEmpty(newDetails) ? null : newDetails;
							UpdateJson(richPresenceObject, jsonProperty);
						}
						currentY += LINE_HEIGHT + SPACING;

						// State
						Rect stateRect = new Rect(position.x, currentY, position.width, LINE_HEIGHT);
						string newState = EditorGUI.TextField(stateRect, "State", presence.State ?? "");
						if (newState != (presence.State ?? ""))
						{
							presence.State = string.IsNullOrEmpty(newState) ? null : newState;
							UpdateJson(richPresenceObject, jsonProperty);
						}
						currentY += LINE_HEIGHT + SPACING;

						// Assets Section
						EditorGUI.LabelField(new Rect(position.x, currentY, position.width, LINE_HEIGHT), "Assets", EditorStyles.boldLabel);
						currentY += LINE_HEIGHT + SPACING;

						EditorGUI.indentLevel++;

						// Initialize assets if null
						if (presence.Assets == null)
							presence.Assets = new Assets();

						// Large Image
						Rect largeImageRect = new Rect(position.x, currentY, position.width, LINE_HEIGHT);
						string newLargeImageKey = EditorGUI.TextField(largeImageRect, "Large Image Key", presence.Assets.LargeImageKey ?? "");
						if (newLargeImageKey != (presence.Assets.LargeImageKey ?? ""))
						{
							presence.Assets.LargeImageKey = string.IsNullOrEmpty(newLargeImageKey) ? null : newLargeImageKey;
							UpdateJson(richPresenceObject, jsonProperty);
						}
						currentY += LINE_HEIGHT + SPACING;

						// Large Image Text
						Rect largeImageTextRect = new Rect(position.x, currentY, position.width, LINE_HEIGHT);
						string newLargeImageText = EditorGUI.TextField(largeImageTextRect, "Large Image Text", presence.Assets.LargeImageText ?? "");
						if (newLargeImageText != (presence.Assets.LargeImageText ?? ""))
						{
							presence.Assets.LargeImageText = string.IsNullOrEmpty(newLargeImageText) ? null : newLargeImageText;
							UpdateJson(richPresenceObject, jsonProperty);
						}
						currentY += LINE_HEIGHT + SPACING;

						// Small Image
						Rect smallImageRect = new Rect(position.x, currentY, position.width, LINE_HEIGHT);
						string newSmallImageKey = EditorGUI.TextField(smallImageRect, "Small Image Key", presence.Assets.SmallImageKey ?? "");
						if (newSmallImageKey != (presence.Assets.SmallImageKey ?? ""))
						{
							presence.Assets.SmallImageKey = string.IsNullOrEmpty(newSmallImageKey) ? null : newSmallImageKey;
							UpdateJson(richPresenceObject, jsonProperty);
						}
						currentY += LINE_HEIGHT + SPACING;

						// Small Image Text
						Rect smallImageTextRect = new Rect(position.x, currentY, position.width, LINE_HEIGHT);
						string newSmallImageText = EditorGUI.TextField(smallImageTextRect, "Small Image Text", presence.Assets.SmallImageText ?? "");
						if (newSmallImageText != (presence.Assets.SmallImageText ?? ""))
						{
							presence.Assets.SmallImageText = string.IsNullOrEmpty(newSmallImageText) ? null : newSmallImageText;
							UpdateJson(richPresenceObject, jsonProperty);
						}
						currentY += LINE_HEIGHT + SPACING;

						EditorGUI.indentLevel--;

						// Party Section
						EditorGUI.LabelField(new Rect(position.x, currentY, position.width, LINE_HEIGHT), "Party", EditorStyles.boldLabel);
						currentY += LINE_HEIGHT + SPACING;

						EditorGUI.indentLevel++;

						// Initialize party if null
						if (presence.Party == null)
							presence.Party = new Party();

						// Party ID
						Rect partyIdRect = new Rect(position.x, currentY, position.width, LINE_HEIGHT);
						string newPartyId = EditorGUI.TextField(partyIdRect, "Party ID", presence.Party.ID ?? "");
						if (newPartyId != (presence.Party.ID ?? ""))
						{
							presence.Party.ID = string.IsNullOrEmpty(newPartyId) ? null : newPartyId;
							UpdateJson(richPresenceObject, jsonProperty);
						}
						currentY += LINE_HEIGHT + SPACING;

						// Party Size
						Rect partySizeRect = new Rect(position.x, currentY, position.width, LINE_HEIGHT);
						int newPartySize = EditorGUI.IntField(partySizeRect, "Party Size", presence.Party.Size);
						if (newPartySize != presence.Party.Size)
						{
							presence.Party.Size = newPartySize;
							UpdateJson(richPresenceObject, jsonProperty);
						}
						currentY += LINE_HEIGHT + SPACING;

						// Party Max
						Rect partyMaxRect = new Rect(position.x, currentY, position.width, LINE_HEIGHT);
						int newPartyMax = EditorGUI.IntField(partyMaxRect, "Party Max", presence.Party.Max);
						if (newPartyMax != presence.Party.Max)
						{
							presence.Party.Max = newPartyMax;
							UpdateJson(richPresenceObject, jsonProperty);
						}
						currentY += LINE_HEIGHT + SPACING;

						EditorGUI.indentLevel--;


						// JSON Toggle
						Rect jsonToggleRect = new Rect(position.x, currentY, position.width, LINE_HEIGHT);
						_showJson = EditorGUI.Foldout(jsonToggleRect, _showJson, "Raw JSON", true);
						currentY += LINE_HEIGHT + SPACING;

						if (_showJson)
						{
							EditorGUI.indentLevel++;

							// JSON text area (multi-line)
							Rect jsonRect = new Rect(position.x, currentY, position.width, LINE_HEIGHT * 4);
							EditorGUI.BeginChangeCheck();
							string newJson = EditorGUI.TextArea(jsonRect, jsonProperty.stringValue ?? "");
							if (EditorGUI.EndChangeCheck())
							{
								jsonProperty.stringValue = newJson;
								try
								{
									richPresenceObject?.Deserialize();
								}
								catch (Exception ex)
								{
									Debug.LogWarning($"Failed to deserialize JSON: {ex.Message}");
								}
							}
							currentY += LINE_HEIGHT * 4 + SPACING;

							EditorGUI.indentLevel--;
						}
					}
					else
					{
						// No presence data - show create button
						Rect createRect = new Rect(position.x, currentY, position.width, LINE_HEIGHT);
						if (GUI.Button(createRect, "Create New Rich Presence"))
						{
							if (richPresenceObject != null)
							{
								richPresenceObject.presence = new RichPresence();
								UpdateJson(richPresenceObject, jsonProperty);
							}
						}
						currentY += LINE_HEIGHT + SPACING;
					}
				}
				catch (Exception ex)
				{
					// Error handling
					Rect errorRect = new Rect(position.x, currentY, position.width, LINE_HEIGHT);
					EditorGUI.HelpBox(errorRect, $"Error: {ex.Message}", MessageType.Error);
					currentY += LINE_HEIGHT + SPACING;

					// Show raw JSON field for manual editing
					Rect jsonRect = new Rect(position.x, currentY, position.width, LINE_HEIGHT);
					EditorGUI.PropertyField(jsonRect, jsonProperty, new GUIContent("JSON"));
				}

				EditorGUI.indentLevel--;
			}

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!_foldout)
				return LINE_HEIGHT;

			float height = LINE_HEIGHT + SPACING; // Foldout line

			try
			{
				var richPresenceObject = GetRichPresenceObject(property);
				var presence = richPresenceObject?.presence;

				if (presence != null)
				{
					// Basic fields: Details, State
					height += (LINE_HEIGHT + SPACING) * 2;

					// Assets section: Label + 4 fields
					height += (LINE_HEIGHT + SPACING) * 5;

					// Party section: Label + 3 fields
					height += (LINE_HEIGHT + SPACING) * 4;

					// Instance
					height += LINE_HEIGHT + SPACING;

					// JSON toggle
					height += LINE_HEIGHT + SPACING;

					if (_showJson)
					{
						height += (LINE_HEIGHT * 4) + SPACING; // Multi-line JSON area
					}
				}
				else
				{
					// Create button
					height += LINE_HEIGHT + SPACING;
				}
			}
			catch
			{
				// Error case: error box + JSON field
				height += (LINE_HEIGHT + SPACING) * 2;
			}

			return height;
		}

		private RichPresenceObject GetRichPresenceObject(SerializedProperty property)
		{
			var target = property.serializedObject.targetObject;
			var field = target.GetType().GetField(property.propertyPath,
				System.Reflection.BindingFlags.Instance |
				System.Reflection.BindingFlags.Public |
				System.Reflection.BindingFlags.NonPublic);

			return field?.GetValue(target) as RichPresenceObject;
		}

		private void UpdateJson(RichPresenceObject richPresenceObject, SerializedProperty jsonProperty)
		{
			if (richPresenceObject != null)
			{
				string json = richPresenceObject.Serialize();
				jsonProperty.stringValue = json;
				jsonProperty.serializedObject.ApplyModifiedProperties();
			}
		}
	}
}