using System;

namespace OdinExtra.HelpBox
{
	public class TooltipDetailsAttribute : Attribute
	{
		public string Details { get; }

		/// <summary>
		/// Adds details to tooltip help box. 
		/// </summary>
		/// <param name="details">Details text. Can be a regular string or resolved to method that returns string to display</param>
		public TooltipDetailsAttribute(string details)
		{
			Details = details;
		}
	}
}