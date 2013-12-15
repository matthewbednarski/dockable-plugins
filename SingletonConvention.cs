/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 10/25/2013
 * Time: 19:54
 * 
 */
using System;
using System.Linq;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace Plugin.WPF
{
	/// <summary>
	/// A StructureMap IRegistrationConvention loading all "scanned" implementations of the Interface T as singletons
	/// </summary>
	public class SingletonConvention<T> : IRegistrationConvention
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
}
