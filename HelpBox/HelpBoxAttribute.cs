using System;

namespace OdinExtra.HelpBox
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class HelpBoxAttribute : Attribute
	{
		public string Text { get; private set; }

		public string Details { get; private set; }

		public HelpBoxAttribute(string text, string details = null)
		{
			Text = text;
			Details = details;
		}
	}
}