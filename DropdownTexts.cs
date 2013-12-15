/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 19/11/2013
 * Time: 12:55
 * 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using MVVm.Core;

namespace Plugin.WPF
{
	/// <summary>
	/// Description of DropdownTexts.
	/// </summary>
	public class DropdownTexts : ObservableDictionary<string, ObservableCollection<string>>,  IDropdownTexts
	{
		public DropdownTexts()
		{
			Mediator.Instance.Register(this);
			this.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
			{
				if(e.PropertyName.Equals("Dictionary"))
				{
					e.ToString();
				}
			};
		}
		static object synclock  = new object();
		
		[MediatorMessageSink("SetDropdownTextList")]
		void SetDropdownText(object key_list)
		{
			lock(synclock)
			{
				Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => {
				                                                                             	KeyValuePair<string, ObservableCollection<string>> dropdownText;
				                                                                             	if(key_list is KeyValuePair<string, ObservableCollection<string>>)
				                                                                             	{
				                                                                             		dropdownText = (KeyValuePair<string, ObservableCollection<string>>) key_list;
				                                                                             		if(this.ContainsKey(dropdownText.Key))
				                                                                             		{
				                                                                             			this[dropdownText.Key].Clear();
				                                                                             			foreach(string val in dropdownText.Value)
				                                                                             			{
				                                                                             				this[dropdownText.Key].Add(val);
				                                                                             			}
				                                                                             			this.OnPropertyChanged("Dictionary");
				                                                                             		} else {
				                                                                             			this.Add(dropdownText.Key, dropdownText.Value);
				                                                                             			this.OnPropertyChanged("Dictionary");
				                                                                             		}
				                                                                             	}
				                                                                             }));
			}
		}
		
		public ObservableCollection<string> GetDropdownText(string key)
		{
			lock(synclock)
			{
				if(!this.ContainsKey(key))
				{
					this.Add(key, new ObservableCollection<string>());
				}
				return this[key];
			}
		}
	}
}
