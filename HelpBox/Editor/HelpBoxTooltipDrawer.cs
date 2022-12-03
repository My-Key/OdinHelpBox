using OdinExtra.HelpBox;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using UnityEngine;

namespace OdinExtra.Editor.HelpBox
{
	[DontApplyToListElements]
	[DrawerPriority(0, 100, 0)]
	public sealed class HelpBoxTooltipDrawer<T> : OdinValueDrawer<T>
	{
		private LocalPersistentContext<bool> m_isVisible;
		private LocalPersistentContext<bool> m_isVisibleDetails;

		private ValueResolver<string> m_detailsTextResolver;

		protected override void Initialize()
		{
			base.Initialize();

			m_isVisible = this.GetPersistentValue<bool>("IsVisible");
			m_isVisibleDetails = this.GetPersistentValue<bool>("IsVisibleDetails");

			var detailsAttribute = Property.GetAttribute<TooltipDetailsAttribute>();

			if (detailsAttribute != null)
				m_detailsTextResolver = ValueResolver.GetForString(Property, detailsAttribute.Details);
		}

		protected override void DrawPropertyLayout(GUIContent label)
		{
			var hasTooltip = label != null && !string.IsNullOrWhiteSpace(label.tooltip);

			if (!hasTooltip)
			{
				CallNextDrawer(label);
				return;
			}

			var tooltip = label.tooltip;
			label.tooltip = null;

			string details = null;

			if (m_detailsTextResolver != null)
			{
				m_detailsTextResolver.DrawError();
				details = m_detailsTextResolver.GetValue();
			}

			var visible = HelpBoxCommon.BeginHelpBox(tooltip, m_isVisible, this, details, m_isVisibleDetails,
				m_isVisibleDetails);

			CallNextDrawer(label);

			HelpBoxCommon.EndHelpBox(visible);
		}
	}
}