/*
 * Matthew Carl Bednarski <matthew.bednarski@ekr.it>
 * 08/11/2012 - 20.02
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Logger;
using MVVm.Core;
using StructureMap;
using Plugin;

namespace Plugin.WPF
{
	/// <summary>
	/// Description of UserControl.
	/// </summary>
	public abstract class UserControlBase:  System.Windows.Controls.UserControl, IPluginUserControl
	{
		public const String MENU_GROUP_VIEW = "MF_MENU_VIEW";
		public const String MENU_GROUP_FILE ="MF_MENU_FILE";
		public const String MENU_GROUP_SETTING = "MF_MENU_MODIFICA";
		public const String MENU_GROUP_ABOUT = "MF_MENU_ABOUT";
		
		
		ILogWrapper _l;
		protected ILogWrapper l
		{
			get{
				if(_l == null)
				{
					try{
						_l = StructureMap.ObjectFactory.GetInstance<ILogWrapper>();
					}catch(Exception)
					{
						ObjectFactory.Configure(x =>
						                        {
						                        	x.For<ILogWrapper>().Singleton().Use<DebugConsolLogWrapper>();
						                        });
						try{
							_l = StructureMap.ObjectFactory.GetInstance<ILogWrapper>();
						}catch(Exception){}
					}
				}
				return _l;
			}
		}

		private static ITranslationItems _t;
		public ITranslationItems T
		{
			get{
				if(_t ==  null)
				{
					try{
						_t = StructureMap.ObjectFactory.GetInstance<ITranslationItems>();
					}catch(Exception){}
				}
				return  _t;
			}
		}

		private static IPluginSettings _settings;
		protected IPluginSettings Settings
		{
			get{
				if(_settings == null)
				{
					try{
						_settings = ObjectFactory.GetInstance<IPluginSettings>();
					}catch(Exception){}
				}
				return _settings;
			}
		}

		RelayCommand _command;
		public RelayCommand Command
		{
			get{
				if(_command == null)
				{
					_command = new RelayCommand( xx => this.Mediator.NotifyColleagues("ShowView.ByKey", this.MenuKey ), xx => true );
				}
				return _command;
			}
		}
		
		private Object _container;
		public  Object Container
		{
			get{
				return _container;
			}
			set{
				if(value != null && !value.Equals(_container))
				{
					_container = value;
					this.OnPropertyChanged("Container");
				}
			}
		}
		public string MenuKey
		{
			get{
				return this.LabelKey;
			}
		}
		string _menuGroup = MENU_GROUP_VIEW;
		public string MenuGroup
		{
			get{
				return _menuGroup;
			}
			set{
				_menuGroup = value;
				this.OnPropertyChanged("MenuGroup");
			}
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
		
		
		ITranslationItem _labelItem;
		string _label;
		public string Label {
			get {
				if(T == null || this.LabelKey == null){
					_label = this.Name;
				} else {
					_labelItem = T[this.LabelKey];
					_label = _labelItem.CurrentValue;
					_labelItem.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
					{
						if(e.PropertyName.Equals("CurrentValue"))
						{
							Dispatcher.BeginInvoke( DispatcherPriority.Background, (Action)(() => {
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
		string _name;
		public new string Name {
			get {
				if(_name == null)
				{
					_name = this.GetType().Name;
				}
				return _name;
			}
			set{
				if(value != null)
				{
					_name = value;
					this.OnPropertyChanged("Name");
				}
			}
		}
		string _labelKey;
		public string LabelKey {
			get {
				if(_labelKey == null)
				{
					_labelKey = this.GetType().Name;
				}
				return _labelKey;
			}
			set{
				if(value != null)
				{
					_labelKey = value;
					this.OnPropertyChanged("LabelKey");
				}
			}
		}
		
		private string _file;
		public string Location {
			get {
				if(_file == null)
				{
					_file =  this.GetType().Assembly.Location;
				}
				return _file;
			}
		}

		
		private string _iconUri;
		public string IconUri {
			get {
				return _iconUri;
			}
			set{
				if(value != null)
				{
					_iconUri = value;
					this.OnPropertyChanged("IconUri");
				}
			}
		}
		
		private bool _activate;
		public bool Activate
		{
			get{
				return _activate;
			}
			set{
				_activate = value;
				this.OnPropertyChanged("Activate");
			}
		}
		private bool _visible;
		public bool Visible
		{
			get{
				if(this.Activate)
				{
					_visible = true;
				}
				return _visible;
			}
			set{
				_visible = value;
				this.OnPropertyChanged("Visible");
			}
		}
		private static IDropdownTexts _dropdownTexts;
		public IDropdownTexts DropdownTexts
		{
			get{
				if(_dropdownTexts == null)
				{
					try{
						_dropdownTexts = StructureMap.ObjectFactory.GetInstance<IDropdownTexts>();
					}catch(Exception){}
					
				}
				return _dropdownTexts;
			}
		}
		public UserControlBase():base()
		{
			this.Background = Brushes.White;
			this.Mediator.Register(this);
		}
		public Mediator Mediator
		{
			get{
				return Mediator.Instance;
			}
		}
		#region DisplayName

		/// <summary>
		/// Returns the user-friendly name of this object.
		/// Child classes can set this property to a new value,
		/// or override it to determine the value on-demand.
		/// </summary>
		public virtual string DisplayName { get; protected set; }

		#endregion // DisplayName

		#region Debugging Aides

		/// <summary>
		/// Warns the developer if this object does not have
		/// a public property with the specified name. This
		/// method does not exist in a Release build.
		/// </summary>
		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		public void VerifyPropertyName(string propertyName)
		{
			// Verify that the property name matches a real,
			// public, instance property on this object.
			if (TypeDescriptor.GetProperties(this)[propertyName] == null)
			{
				string msg = "Invalid property name: " + propertyName;

				if (this.ThrowOnInvalidPropertyName)
					throw new Exception(msg);
				else
					Debug.Fail(msg);
			}
		}

		/// <summary>
		/// Returns whether an exception is thrown, or if a Debug.Fail() is used
		/// when an invalid property name is passed to the VerifyPropertyName method.
		/// The default value is false, but subclasses used by unit tests might
		/// override this property's getter to return true.
		/// </summary>
		protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

		#endregion // Debugging Aides

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
			this.VerifyPropertyName(propertyName);

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
