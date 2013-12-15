/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 08/10/2013
 * Time: 10:10
 * 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Plugin;
using MVVm.Core;

namespace Plugin.WPF
{
	/// <summary>
	/// Description of IPluginUserControl.
	/// </summary>
	public interface IPluginUserControl: IPlugin,IPluginMenuItem
	{
//		ITranslationItems T{get;}
		Object Container{get; set;}
		bool Visible{get;}
		IDropdownTexts DropdownTexts{get;}
	}
}
