#region using Statements
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xaml;

using AvalonDock;
using MVVm.Core;
using StructureMap;

#endregion


namespace Plugin.WPF.Example
{
	/// <summary>
	/// Logica di interazione per MainWindow.xaml
	/// </summary>
	public partial class MainWindow :WindowBase
	{
		public string _layout_base_name;
		public string LayoutBaseFileName
		{
			get{
				if(_layout_base_name == null)
				{
					_layout_base_name = "dock_layout_config.xml";
				}
				return _layout_base_name;
			}
			set{
				if(value != null)
				{
					this._layout_base_name = value;
					this.OnPropertyChanged("LayoutBaseFileName");
				}
			}
		}
		private String  layout_file;
		public string LayoutFile
		{
			get{
				if(!String.IsNullOrEmpty(Settings["repo.dir"]))
				{
					layout_file = Path.Combine(Settings["repo.dir"], this.LayoutBaseFileName);
					try{
						if(!Directory.Exists(Settings["repo.dir"]))
						{
							Directory.CreateDirectory(Settings["repo.dir"]);
						}
					} catch(IOException ex)
					{
						Debug.WriteLine(ex.Message);
					}
					return layout_file;
				} else{
					return Path.Combine( Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), this.LayoutBaseFileName);
				}
			}
		}

		
		private ObservableCollectionWithCurrent<string> _documents;
		public ObservableCollectionWithCurrent<string> Documents
		{
			get{
				if(_documents == null)
				{
					_documents =  new ObservableCollectionWithCurrent<string>();
				}
				return _documents;
			}
		}
		
		public MainWindow()
		{
			try
			{
				InitializeComponent();
				this.DataContext = this;
			}
			catch (XamlParseException ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}
		public void RestoreLayout()
		{
			//throw new NotImplementedException();
			if(File.Exists(this.LayoutFile))
			{
				
				this.dockHost.DockingManager.DeserializationCallback += delegate(object esender, DeserializationCallbackEventArgs ee)
				{

					String name = ee.Name;
					//MessageBox.Show(name);
				};
				try
				{
					this.dockHost.DockingManager.RestoreLayout(this.LayoutFile);
				}
				catch(Exception ex )
				{
					Debug.WriteLine(ex.Message);
				}
			} else {
				foreach(Object o_uc in this.dockHost.Panes)
				{
					if(o_uc is IPluginUserControl)
					{
						var uc = o_uc as IPluginUserControl;
						if(uc.Activate)
						{
							this.dockHost.ShowPanel(uc);
						}
					}
				}
			}
		}
		void DockHost_AvalonDockLoaded1(object sender, EventArgs e)
		{
			this.RestoreLayout();
		}


		[MediatorMessageSink("ShowView.ByKey")]
		void ShowItem(string item_key)
		{
			foreach(IPluginUserControl control in this.Controls)
			{
				if(control.MenuKey.Equals(item_key))
				{
					this.dockHost.ShowPanel(control);
				}
			}
		}
		
		[MVVm.Core.MediatorMessageSink("ShowView")]
		private void ShowViewMessageSink(string item)
		{
			foreach(IPluginUserControl control in this.Controls)
			{
				if(control.Name.Equals(item))
				{
					this.dockHost.ShowPanel(control);
				}
			}
		}
		
		void WindowBase_Closing(object sender, CancelEventArgs e)
		{
			try
			{
				this.dockHost.DockingManager.SaveLayout(this.LayoutFile);
			}
			catch(Exception ex )
			{
				Debug.WriteLine(ex.Message);
			}
			this.ExitCommand.Execute("");
		}
	}
}
