/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 25/11/2013
 * Time: 16:52
 * 
 */
using System;
using System.Windows.Data;

namespace Plugin.WPF
{
	/// <summary>
	/// Description of MultiConverterBoolToBackground.
	/// </summary>
	public class MultiConverterBoolToBackground:IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool r = false;
			if(values.Length > 0)
			{
				r = true;
			}
			foreach(var o in values)
			{
				try{
					bool t = (bool)o;
					if(!t)
					{
						return false;
					}
				}catch(Exception ex)
				{
					return false;
				}
			}
			return r;
		}
		
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
