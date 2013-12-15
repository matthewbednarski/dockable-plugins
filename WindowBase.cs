/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 08/10/2013
 * Time: 14:05
 * 
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using System.Windows.Input;
using Logger;
using MVVm.Core;
using Plugin;
using StructureMap.TypeRules;

namespace Plugin.WPF
{
	/// <summary>
	/// Description of WindowBase.
	/// </summary>
	public class WindowBase:System.Windows.Window, IPluginWindow
	{
		
		ISettings _settings;
		public ISettings Settings
		{
			get{
				if(_settings == null){
					_settings = StructureMap.ObjectFactory.GetInstance<ISettings>();
				}
				return _settings;
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
							if(menu.MenuGroup.Equals("")){
								this._childMenus.Add(menu);
							}
						}
					}
				}
				return _childMenus;
			}
		}

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
						StructureMap.ObjectFactory.Configure(x =>
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
		
		public Mediator Mediator
		{
			get{
				return Mediator.Instance;
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
		
		private IList<IPluginUserControl> _controls;
		public IList<IPluginUserControl> Controls
		{
			get{
				if(_controls == null){
					try{
//						_controls = StructureMap.ObjectFactory.GetAllInstances<IPluginUserControl>();
						var tmp = StructureMap.ObjectFactory.GetAllInstances<IPluginMenuItem>();
						foreach(var tmpSingleton in tmp)
						{
							if(tmpSingleton.GetType().AllInterfaces().Contains(typeof(IPluginUserControl)))
							{
								if(_controls == null)
								{
									_controls = new ObservableCollection<IPluginUserControl>();
								}
								_controls.Add(tmpSingleton as IPluginUserControl);
							}
						}
					}catch(Exception ex)
					{
						Debug.WriteLine(ex.Message);
					}
				}
				return _controls;
			}
		}
		
		string _name;
		public new string Name {
			get {
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
		string _label;
		public string Label {
			get {
				if(T == null || this.LabelKey == null){
					_label = this.Name;
				} else {
					_label = T[this.LabelKey].CurrentValue;
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
		string _labelKey;
		public string LabelKey {
			get {
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
		private bool _activate = true;
		public new bool Activate
		{
			get{
				return _activate;
			}
			set{
				_activate = value;
			}
		}
		public WindowBase():base()
		{
			Mediator.Register(this);
		}
		
		private RelayCommand _exitCommand;
		public ICommand ExitCommand
		{
			get
			{
				if (_exitCommand == null)
				{
					_exitCommand = new RelayCommand(param => this.Exit());
				}
				return _exitCommand as ICommand;
			}
		}
		
		private void Exit()
		{
			this.Mediator.NotifyColleagues("Persist", "Closing");
			this.Mediator.NotifyColleagues("CloseWindow", "Closing");
		}
		
//		[MediatorMessageSink("CloseWindow")]
//		void CloseWindow(object dummy)
//		{
//			this.Close();
//		}
		
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
