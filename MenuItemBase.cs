/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 15/11/2013
 * Time: 10:13
 * 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;
using Plugin;
using MVVm.Core;

namespace Plugin.WPF
{
	/// <summary>
	/// Description of MenuBase.
	/// </summary>
	public abstract class MenuItemBase: MediatorEnabledViewModel<object>, IPluginMenuItem
	{
		public abstract string MenuGroup
		{
			get;
		}
		public  abstract string MenuKey
		{
			get;
		}
		public abstract  ICommand Command
		{
			get;
		}
		
		static IList<IPluginMenuItem> _allMenus;
		IList<IPluginMenuItem> _childMenus;
		public IList<IPluginMenuItem> MenuItems
		{
			get{
				if(_childMenus == null)
				{
					if(_allMenus == null)
					{
						try{
							_allMenus = StructureMap.ObjectFactory.GetAllInstances<IPluginMenuItem>();
						}catch(Exception){}
						
					}
					if(_allMenus != null)
					{
						_childMenus = new ObservableCollection<IPluginMenuItem>();
						foreach(IPluginMenuItem menu in _allMenus)
						{
							if(menu.MenuGroup.Equals(this.MenuKey)){
								this._childMenus.Add(menu);
							}
						}
					}
				}
				return _childMenus;
			}
		}
		
		private static ITranslationItems _t;
		public ITranslationItems T
		{
			get{
				if(_t ==  null)
				{
					try{
					}catch(Exception){}
				}
				return  _t;
			}
		}
		ITranslationItem _labelItem;
		string _label;
		public string Label {
			get {
				if(T == null || this.MenuKey == null){
					if(_label == null)
					{
						_label = this.GetType().Name;
					}
				} else {
					_labelItem = T[this.MenuKey];
					_label = _labelItem.CurrentValue;
					_labelItem.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
					{
						if(e.PropertyName.Equals("CurrentValue"))
						{
							Dispatcher.CurrentDispatcher.BeginInvoke( DispatcherPriority.Background, (Action)(() => {
							                                                                                  	_label = _labelItem.CurrentValue;
							                                                                                  	this.OnPropertyChanged("Label");
							                                                                                  }));
						}
					};
				}
				return _label;
			}
			set{
				if(value != null)
				{
					_label = value;
					this.OnPropertyChanged("Label");
				}
			}
		}
		public MenuItemBase():base()
		{
			
		}
	}
}
