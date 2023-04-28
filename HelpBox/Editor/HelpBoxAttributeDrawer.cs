using OdinExtra.HelpBox;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using UnityEngine;

namespace OdinExtra.Editor.HelpBox
{
	[DrawerPriority(0, 100, 0)]
	public class HelpBoxAttributeDrawer : OdinAttributeDrawer<HelpBoxAttribute>
	{
		private LocalPersistentContext<bool> m_isVisible;
		private LocalPersistentContext<bool> m_isVisibleDetails;

		private ValueResolver<string> m_textResolver;
		private ValueResolver<string> m_detailsTextResolver;

		protected override void Initialize()
		{
			base.Initialize();

			m_isVisible = this.GetPersistentValue<bool>("IsVisible");
			m_isVisibleDetails = this.GetPersistentValue<bool>("IsVisibleDetails");

			m_textResolver = ValueResolver.GetForString(Property, Attribute.Text);
			m_detailsTextResolver = ValueResolver.GetForString(Property, Attribute.Details);
		}

		protected override void DrawPropertyLayout(GUIContent label)
		{
			if (m_textResolver.HasError)
			{
				m_textResolver.DrawError();
				CallNextDrawer(label);
				return;
			}

			var tooltip = m_textResolver.GetValue();

			string details = null;

			if (m_detailsTextResolver != null)
			{
				m_detailsTextResolver.DrawError();
				details = m_detailsTextResolver.GetValue();
			}

			var visible = HelpBoxCommon.BeginHelpBox(tooltip, m_isVisible, this, details, m_isVisibleDetails,
				m_isVisibleDetails);

			CallNextDrawer(label);

			HelpBoxCommon.EndHelpBox();
		}
	}
}