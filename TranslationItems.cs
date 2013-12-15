/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 08/10/2013
 * Time: 14:46
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using EKRPE.Author.GestioneStringhe;
using MVVm.Core;
using Plugin;

namespace Plugin.WPF
{
	/// <summary>
	/// Description of TranslationItems.
	/// </summary>
	public class TranslationItems: Dictionary<string, ITranslationItem>, ITranslationItems, INotifyPropertyChanged
	{
		/// <summary>
		/// imposta la lingua che si userà per le traduzioni
		/// </summary>
		//private CultureInfo _currentCulture;
		public CultureInfo CurrentCulture
		{
			get{
				return Strings.Container.CurrentCulture;
			}
			set{
				Strings.Container.CurrentCulture = value;
//				if(value != null && value is CultureInfo && value != CultureInfo.CurrentCulture)
//				{
//					System.Threading.Thread.CurrentThread.CurrentCulture = value;
//					Strings.Container.Refresh();
//
//					Mediator.Instance.NotifyColleagues("EKRPE.Author.GestioneStringhe.Translations.Refresh", "doit");
//				}
			}
		}
		[MediatorMessageSink("EKRPE.Author.GestioneStringhe.Translations.Refresh")]
		void Refreshem(string verb)
		{
			//this.OnPropertyChanged("[]");
			foreach(var item in this)
			{
				item.Value.OnPropertyChanged("CurrentValue");
			}
		}
		public TranslationItems():base()
		{
			Mediator.Instance.Register(this);
			foreach(var p in Strings.Container.Translations)
			{
				ITranslationItem item = null;
				if(this.ContainsKey(p.Key))
				{
					item = this[p.Key];
					
				}else{
					item = new TranslationItem(p.Key);
					this.Add(p.Key, item);
				}
				foreach(var pval in p.Value.Translations)
				{
					if(!item.ContainsKey(pval.Key))
					{
						item.Add(pval.Key, pval.Value);
					}
				}
				
			}
		}
		
		public IDictionary<string, ITranslationItem> Items {
			get {
				return this;
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
