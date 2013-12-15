/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 26/11/2013
 * Time: 12:14
 * 
 */
using System;
using System.Collections.Generic;
using Logger;
using StructureMap;

namespace Plugin.WPF.Service
{
	public class ServiceManager:IDisposable
	{
		internal ILogWrapper l;
		static object sync = new object();
		List<IPluginService> _services;
		public List<IPluginService> Services
		{
			get{
				if(_services == null)
				{
					_services = new List<IPluginService>();
				}
				return _services;
			}
		}
		
		public ServiceManager()
		{
			
			try{
				l = StructureMap.ObjectFactory.GetInstance<ILogWrapper>();
			}catch(StructureMapException  ex)
			{
				ex.Message.ToString();
			}
			try{
				var services = StructureMap.ObjectFactory.GetAllInstances<IPluginService>();
				if(services != null)
				{
					foreach(var service in services)
					{
						l.info("Found service \"{0}\"", service.GetType().Name);
						if(service.CanActivate)
						{
							l.info("CanActivate Service \"{0}\"", service.GetType().Name);
							Services.Add(service);
						}
					}
				}
			}catch(StructureMapException  ex)
			{
				ex.Message.ToString();
			}
		}
		public bool ServicesStarted{
			get{
				bool r = false;
				foreach(IPluginService s in Services)
				{
					if(s.IsRunning)
					{
						r = true;
					}
				}
				return r;
			}
		}
		
		protected void Dispose(bool disposing)
		{
			foreach(IPluginService s in Services)
			{
				if(s.IsRunning)
				{
					s.Stop();
				}
			}
			
			foreach(IPluginService s in Services)
			{
				while(s.IsRunning)
				{
					s.Dispose();
				}
			}
		}

		public  void Start(string[] args)
		{
			lock(sync)
			{
				foreach(IPluginService s in Services)
				{
					if(!s.IsRunning)
					{
						s.Start();
					}
				}
				l.info("Service started");
			}
		}

		public  void Stop()
		{
			lock(sync)
			{
				foreach(IPluginService s in Services)
				{
					if(s.IsRunning)
					{
						s.Stop();
					}
				}
				l.info(" Service stopped");
			}
		}
		public  void Restart(string[] args)
		{
			lock(sync)
			{
				l.info("Restarting Services...");
				foreach(IPluginService s in Services)
				{
					if(s.IsRunning)
					{
						s.Stop();
					}
				}
				l.info("Service stopped");
				foreach(IPluginService s in Services)
				{
					if(!s.IsRunning)
					{
						s.Start();
					}
				}
				l.info("Service started");
			}
		}
		
		
		public void Dispose()
		{
			this.Dispose(true);
		}
	}
}
