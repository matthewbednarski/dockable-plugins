/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 19/11/2013
 * Time: 12:52
 * 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MVVm.Core;

namespace Plugin.WPF
{
	/// <summary>
	/// Description of IDropdownTexts.
	/// </summary>
	public interface IDropdownTexts : IDictionary<string, ObservableCollection<string>>
	{
		ObservableCollection<string> GetDropdownText(string key);
	}
}
