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
			
			if (!isVisible.Value && tooltip != null) 
				buttonTooltip = REMOVE_RICH_TEXT.Replace(tooltip, string.Empty);

			var visible = isVisible.Value;
			
			EditorGUILayout.BeginHorizontal();

			if (EditorGUI.indentLevel == 0 && !EditorGUIUtility.hierarchyMode)
				GUIHelper.PushIndentLevel(EditorGUI.indentLevel + 1);
			else
				GUIHelper.PushIndentLevel(EditorGUI.indentLevel);

			var toggleButtonRect = GUIHelper.GetCurrentLayoutRect();
			toggleButtonRect = GUIHelper.IndentRect(toggleButtonRect);
			toggleButtonRect.xMin -= TOGGLE_BUTTON_WIDTH;
			toggleButtonRect.xMin = Mathf.Max(4, toggleButtonRect.xMin);
			toggleButtonRect.width = TOGGLE_BUTTON_WIDTH;
			toggleButtonRect.height = EditorGUIUtility.singleLineHeight;
			
			GUIHelper.PushHierarchyMode(false);

			SirenixEditorGUI.BeginIndentedVertical();

			if (SirenixEditorGUI.BeginFadeGroup(fadeKey, visible)) 
				MessageBox(tooltip, detailed, isVisibleDetailed, fadeDetailedKey);

			SirenixEditorGUI.EndFadeGroup();
			
			GUIHelper.PushGUIEnabled(true);

			isVisible.Value = ToggleButton(toggleButtonRect, visible,
				visible ? SdfIconType.InfoCircleFill : SdfIconType.InfoCircle, TOGGLE_ON_COLOR, TOGGLE_ON_COLOR,
				tooltip: buttonTooltip);
			
			GUIHelper.PopGUIEnabled();

			return visible;
		}

		public static void EndHelpBox()
		{
			SirenixEditorGUI.EndIndentedVertical();

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
			
			GUIHelper.PushColor(Color.white);

			EditorGUILayout.BeginHorizontal();

			if (detailMessageIsValid)
			{
				isVisible.Value = ToggleButton(isVisible.Value,
					isVisible.Value ? EditorIcons.TriangleDown : EditorIcons.TriangleRight, Color.white, Color.white,
					tooltip: isVisible.Value ? "Hide details" : "Show details");
			}
			else
				isVisible.Value = false;

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
			
			GUIHelper.PopColor();
			
			SirenixEditorGUI.EndBox();
		}
	}
}