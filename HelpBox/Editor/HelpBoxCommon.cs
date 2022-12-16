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
			var buttonTooltip = isVisible.Value ? null : tooltip;

			var visible = isVisible.Value;

			GUIHelper.PushHierarchyMode(false);

			SirenixEditorGUI.BeginIndentedHorizontal();

			GUIHelper.PushGUIEnabled(true);

			isVisible.Value = ToggleButton(isVisible.Value,
				isVisible.Value ? SdfIconType.InfoCircleFill : SdfIconType.InfoCircle, TOGGLE_ON_COLOR, TOGGLE_ON_COLOR,
				14, tooltip: buttonTooltip);
			GUIHelper.PopGUIEnabled();

			// if (visible)
			// 	SirenixEditorGUI.BeginBox();

			SirenixEditorGUI.BeginIndentedVertical();

			if (SirenixEditorGUI.BeginFadeGroup(fadeKey, isVisible.Value))
				MessageBox(tooltip, detailed, isVisibleDetailed, fadeDetailedKey);

			SirenixEditorGUI.EndFadeGroup();

			return visible;
		}

		public static void EndHelpBox(bool visible)
		{
			SirenixEditorGUI.EndIndentedVertical();

			SirenixEditorGUI.EndIndentedHorizontal();

			GUIHelper.PopHierarchyMode();

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