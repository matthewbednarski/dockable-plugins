/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 08/10/2013
 * Time: 14:40
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Diagnostics;
using Plugin;

namespace Plugin.WPF
{
	/// <summary>
	/// Description of TranslationItem.
	/// </summary>
	public class TranslationItem : Dictionary<CultureInfo,string>, ITranslationItem
	{
		
		public TranslationItem(string key):base()
		{
			this._key = key;
		}
		public TranslationItem(string key, string text):base()
		{
			this._key = key;
			this.Add(CultureInfo.CurrentCulture, text);
		}
		public TranslationItem(string key, CultureInfo lang, string text):base()
		{
			this._key = key;
			this.Add(lang, text);
		}
		string _key;
		public string TranslateKey {
			get {
				return _key;
			}
		}
		public string CurrentValue{
			get{
				return this[CultureInfo.CurrentCulture];
			}
		}
		public IDictionary<CultureInfo, string> Item {
			get {
				return this;
			}
		}
		public override string ToString()
		{
			string s = this[CultureInfo.CurrentCulture];
			if(!String.IsNullOrEmpty(s))
			{
				return s;
			}else{
				return TranslateKey;
			}
		}
		
		#region INotifyPropertyChanged Members

		/// <summary>
		/// Raised when a property on this object has a new value.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises this object's PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">The property that has a new value.</param>
		public virtual void OnPropertyChanged(string propertyName)
		{
			
			PropertyChangedEventHandler handler = this.PropertyChanged;
			if (handler != null)
			{
				var e = new PropertyChangedEventArgs(propertyName);
				handler(this, e);
			}
		}

		#endregion // INotifyPropertyChanged Members
	}
}
