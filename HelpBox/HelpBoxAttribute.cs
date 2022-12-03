using System;

namespace OdinExtra.HelpBox
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class HelpBoxAttribute : Attribute
	{
		public string Text { get; private set; }

		public string Details { get; private set; }
		
		/// <summary>
		/// Draws compact help box
		/// </summary>
		/// <param name="text">Main text text. Can be a regular string or resolved to method that returns string to display</param>
		/// <param name="details">Details text. Can be a regular string or resolved to method that returns string to display</param>
		public HelpBoxAttribute(string text, string details = null)
		{
			Text = text;
			Details = details;
		}
	}
}