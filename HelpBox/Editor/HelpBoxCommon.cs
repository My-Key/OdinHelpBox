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

		public static bool ToggleButton(bool value, SdfIconType icon, Color toggleOnColor, Color toggleOffColor,
			int width = 18, int height = 18, string tooltip = null)
		{
			var rect = EditorGUILayout.GetControlRect(false,
				GUILayoutOptions.ExpandWidth(false).Width(width).Height(height));

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

			// if (EditorGUIUtility.hierarchyMode)
			// 	GUIHelper.PushIndentLevel(EditorGUI.indentLevel - 1);
			
			GUIHelper.PushHierarchyMode(false);
			
			GUIHelper.PushLabelWidth(GUIHelper.BetterLabelWidth - 15f);
			
			SirenixEditorGUI.BeginIndentedHorizontal();

			GUIHelper.PushGUIEnabled(true);

			isVisible.Value = ToggleButton(isVisible.Value,
				isVisible.Value ? SdfIconType.InfoCircleFill : SdfIconType.InfoCircle, TOGGLE_ON_COLOR, TOGGLE_ON_COLOR,
				15, tooltip: buttonTooltip);
			GUIHelper.PopGUIEnabled();

			// if (visible)
			// 	SirenixEditorGUI.BeginBox();

			SirenixEditorGUI.BeginIndentedVertical();

			if (SirenixEditorGUI.BeginFadeGroup(fadeKey, isVisible.Value))
			{
				MessageBox(tooltip, detailed, isVisibleDetailed, fadeDetailedKey);
			}

			SirenixEditorGUI.EndFadeGroup();

			return visible;
		}

		public static void EndHelpBox(bool visible)
		{
			SirenixEditorGUI.EndIndentedVertical();

			SirenixEditorGUI.EndIndentedHorizontal();
			
			GUIHelper.PopLabelWidth();

			GUIHelper.PopHierarchyMode();
			
			// if (EditorGUIUtility.hierarchyMode)
			// 	GUIHelper.PopIndentLevel();

			// if (visible)
			// 	SirenixEditorGUI.EndBox();
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

			SirenixEditorGUI.BeginIndentedHorizontal();

			if (detailMessageIsValid)
			{
				isVisible.Value = ToggleButton(isVisible.Value,
					isVisible.Value ? EditorIcons.TriangleDown : EditorIcons.TriangleRight, Color.white, Color.white,
					tooltip: isVisible.Value ? "Hide details" : "Show details");
			}

			SirenixEditorGUI.BeginIndentedVertical();

			var rect = GUILayoutUtility.GetRect(GUIHelper.TempContent(message), MessageStyle);
			EditorGUI.SelectableLabel(rect, message, MessageStyle);

			if (SirenixEditorGUI.BeginFadeGroup(key, isVisible.Value))
			{
				SirenixEditorGUI.HorizontalLineSeparator(Color.black * 0.25f);
				
				var rectDetails = GUILayoutUtility.GetRect(GUIHelper.TempContent(detailMessage), MessageStyle);
				EditorGUI.SelectableLabel(rectDetails, detailMessage, MessageStyle);
			}

			SirenixEditorGUI.EndFadeGroup();

			SirenixEditorGUI.EndIndentedVertical();

			SirenixEditorGUI.EndIndentedHorizontal();
			SirenixEditorGUI.EndBox();
		}
	}
}