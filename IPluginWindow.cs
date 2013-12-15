/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 08/10/2013
 * Time: 14:03
 * 
 */
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Plugin;

namespace Plugin.WPF
{
	/// <summary>
	/// Description of IWindow.
	/// </summary>
	public interface IPluginWindow: IPlugin
	{
		IList<IPluginUserControl> Controls{get;}
		ICommand ExitCommand{get;}
	}
}
