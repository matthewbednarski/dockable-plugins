/*
 * Matthew Carl Bednarski <matthew.bednarski@ekr.it>
 * 06/04/2012 - 11.17
 */
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Logger;
using Plugin;
using Plugin.WPF;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace Plugin.WPF.Example
{
	/// <summary>
	/// Description of IoCBootstrapper.
	/// </summary>
	public class DIBootstrapper : IBootstrapper
	{
		public DIBootstrapper()
		{
			
		}
		
		
		public static void Bootstrap()
		{
			new DIBootstrapper().BootstrapStructureMap();
		}
		private static bool _hasStarted;

		public void BootstrapStructureMap()
		{
			
			ObjectFactory.Initialize(x =>
			                         {
			                         	x.Scan( scan =>
			                         	       {
			                         	       	scan.AssembliesFromApplicationBaseDirectory();
			                         	       	scan.With(new SingletonConvention<IPluginSettings>());
			                         	       }
			                         	      );
			                         });
			
			
			ObjectFactory.Configure( x => {
			                        	
			                        	x.For<IDropdownTexts>().Singleton().Use<DropdownTexts>();
			                        	
			                        	x.Scan( scan =>
			                        	       {
			                        	       	string path_to_scan = Path.GetDirectoryName(this.GetType().Assembly.Location);
			                        	       	scan.AssembliesFromPath(path_to_scan);
			                        	       	if(Directory.Exists(Path.Combine(path_to_scan, "plugin")))
			                        	       	{
			                        	       		scan.AssembliesFromPath(Path.Combine(path_to_scan, "plugin"));
			                        	       	}

			                        	       	scan.With(new SingletonConvention<IPluginMenuItem>());
			                        	       }
			                        	      );
			                        });
			try{
				var l = ObjectFactory.GetAllInstances<IPluginMenuItem>();
				foreach(var p in l)
				{
					Thread.Sleep(100);
					Debug.WriteLine(p.Label);
				}
			}catch(Exception  ex)
			{
				ex.Message.ToString();
			}
		}

		public static void Restart()
		{
			if (_hasStarted)
			{
				ObjectFactory.ResetDefaults();
			}
			else
			{
				Bootstrap();
				_hasStarted = true;
			}
		}

	}
	internal class SingletonConvention<T>:IRegistrationConvention
	{
		
		public void Process(Type type, Registry  registry)
		{
			if (!type.IsConcrete() || !type.CanBeCreated() || !type.AllInterfaces().Contains(typeof(T)))
			{
				return;
			}
			registry.For(typeof(T)).Singleton().Use(type);
		}
		
	}
	public class RepositoryRegistry : StructureMap.Configuration.DSL.Registry
	{
		public RepositoryRegistry()
		{

		}
	}
}
