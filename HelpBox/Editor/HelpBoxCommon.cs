using System;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace OdinExtra.Editor.HelpBox
{
	public static class HelpBoxCommon
	{
		private static readonly Color TOGGLE_OFF_COLOR = new Color(0.4f, 0.9f, 1.0f, 0.5f);
		private static readonly Color TOGGLE_ON_COLOR = new Color(0.25f, 1.0f, 1.0f);

		private static readonly Color BG_COLOR = new Color(0.5f, 1.0f, 1.0f);
		
		private static readonly Regex REMOVE_RICH_TEXT = new Regex("<.*?>", RegexOptions.Singleline);
		
		
		private const float TOGGLE_BUTTON_WIDTH = 15f;

		public static bool ToggleButton(bool value, EditorIcon icon, Color toggleOnColor, Color toggleOffColor,
			int width = 18, int height = 18, string tooltip = null)
		{
			var rect = EditorGUILayout.GetControlRect(false,
				GUILayoutOptions.ExpandWidth(false).Width(width).Height(height));

			var current = GUI.Toggle(rect, value, GUIHelper.TempContent(null, null, tooltip), GUIStyle.none);

			if (current != value)
				GUIHelper.RemoveFocusControl();

			if (Event.current.type == EventType.Repaint)
			{
				float drawSize = Mathf.Min(rect.height, rect.width);
				var color = value ? toggleOnColor : toggleOffColor;
				GUIHelper.PushColor(color);
				icon.Draw(rect, drawSize);
				GUIHelper.PopColor();
			}

			return current;
		}

		public static bool ToggleButton(Rect rect, bool value, SdfIconType icon, Color toggleOnColor, Color toggleOffColor, string tooltip = null)
		{
			var current = GUI.Toggle(rect, value, GUIHelper.TempContent(null, null, tooltip), GUIStyle.none);

			if (current != value)
				GUIHelper.RemoveFocusControl();

			if (Event.current.type == EventType.Repaint)
			{
				var color = value ? toggleOnColor : toggleOffColor;
				GUIHelper.PushColor(color);
				SdfIcons.DrawIcon(rect, icon);
				GUIHelper.PopColor();
			}

			return current;
		}

		public static bool BeginHelpBox(string tooltip, LocalPersistentContext<bool> isVisible, object fadeKey,
			string detailed, LocalPersistentContext<bool> isVisibleDetailed, object fadeDetailedKey)
		{
			string buttonTooltip = null;
			
			if (!isVisible.Value) 
				buttonTooltip = REMOVE_RICH_TEXT.Replace(tooltip, string.Empty);

			var visible = isVisible.Value;
			
			EditorGUILayout.BeginHorizontal();
			
			var offset = TOGGLE_BUTTON_WIDTH;
			var height = EditorGUIUtility.singleLineHeight;

			if (EditorGUI.indentLevel == 0 && !EditorGUIUtility.hierarchyMode)
			{
				GUIHelper.PushIndentLevel(EditorGUI.indentLevel + 1);
			}
			else
			{
				GUIHelper.PushIndentLevel(EditorGUI.indentLevel);
				offset = 0f;
			}

			GUIHelper.PushGUIEnabled(true);
			
			var toggleButtonRect = new Rect();
			toggleButtonRect = GUIHelper.GetCurrentLayoutRect();
			toggleButtonRect.xMin = GUIHelper.CurrentIndentAmount - offset;
			toggleButtonRect.width = TOGGLE_BUTTON_WIDTH;
			toggleButtonRect.height = height;

			isVisible.Value = ToggleButton(toggleButtonRect, isVisible.Value,
				isVisible.Value ? SdfIconType.InfoCircleFill : SdfIconType.InfoCircle, TOGGLE_ON_COLOR, TOGGLE_ON_COLOR,
				tooltip: buttonTooltip);
			GUIHelper.PopGUIEnabled();
			
			GUIHelper.PushHierarchyMode(false);

			EditorGUILayout.BeginVertical();

			if (SirenixEditorGUI.BeginFadeGroup(fadeKey, isVisible.Value))
			{
				MessageBox(tooltip, detailed, isVisibleDetailed, fadeDetailedKey);
			}

			SirenixEditorGUI.EndFadeGroup();

			return visible;
		}

		public static void EndHelpBox()
		{
			EditorGUILayout.EndVertical();

			GUIHelper.PopHierarchyMode();
			
			GUIHelper.PopIndentLevel();

			EditorGUILayout.EndHorizontal();
		}

		private static GUIStyle MESSSAGE_BOX;

		public static GUIStyle MessageStyle =>
			MESSSAGE_BOX ??= new GUIStyle("Label")
			{
				margin = new RectOffset(4, 4, 2, 2),
				richText = true,
				wordWrap = true
			};

		private static void MessageBox(string message, string detailMessage, LocalPersistentContext<bool> isVisible,
			object key)
		{
			bool detailMessageIsValid = !string.IsNullOrWhiteSpace(detailMessage);

			GUIHelper.PushColor(BG_COLOR);
			SirenixEditorGUI.BeginBox();
			GUIHelper.PopColor();

			EditorGUILayout.BeginHorizontal();

			if (detailMessageIsValid)
			{
				isVisible.Value = ToggleButton(isVisible.Value,
					isVisible.Value ? EditorIcons.TriangleDown : EditorIcons.TriangleRight, Color.white, Color.white,
					tooltip: isVisible.Value ? "Hide details" : "Show details");
			}

			EditorGUILayout.BeginVertical();

			var rect = GUILayoutUtility.GetRect(GUIHelper.TempContent(message), MessageStyle);
			EditorGUI.SelectableLabel(rect, message, MessageStyle);

			if (SirenixEditorGUI.BeginFadeGroup(key, isVisible.Value))
			{
				SirenixEditorGUI.HorizontalLineSeparator(Color.black * 0.25f);
				
				var rectDetails = GUILayoutUtility.GetRect(GUIHelper.TempContent(detailMessage), MessageStyle);
				EditorGUI.SelectableLabel(rectDetails, detailMessage, MessageStyle);
			}

			SirenixEditorGUI.EndFadeGroup();

			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();
			SirenixEditorGUI.EndBox();
		}
	}
}