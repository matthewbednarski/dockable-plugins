/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 09/10/2013
 * Time: 14:55
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Plugin.WPF
{
	/// <summary>
	/// Description of IPluginMenuItem.
	/// </summary>
	public interface IPluginMenuItem: INotifyPropertyChanged
	{
//		int Position{get;}
		string Label{get;}
		string MenuKey{get;}
		string MenuGroup{get;}
		IList<IPluginMenuItem> MenuItems{get;}
	}
}
