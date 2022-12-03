using System;

namespace OdinExtra.HelpBox
{
	public class TooltipDetailsAttribute : Attribute
	{
		public string Details { get; }

		public TooltipDetailsAttribute(string details)
		{
			Details = details;
		}
	}
}