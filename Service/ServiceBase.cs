/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 26/11/2013
 * Time: 12:18
 * 
 */
using System;
using System.Diagnostics;
using System.Threading;
using Logger;
using MVVm.Core;

namespace Plugin.WPF.Service
{

	public abstract class ServiceBase: MediatorEnabledViewModelBase<object>, IPluginService
	{
		public static bool IsService;
		
		internal ILogWrapper l;
		public String ServiceName{
			get{
				return this.GetType().Name;
			}
		}
		Boolean _canActivate = false;
		public virtual Boolean CanActivate{ get{ return _canActivate;}}
		private Boolean _isRunning;
		public virtual Boolean IsRunning
		{
			get{
				return this.State == ServiceState.Started;
			}
		}
		private Boolean _canStart;
		public virtual Boolean CanStart
		{
			get{
				return _canStart;
			}
			internal set{
				if(_canStart != value)
				{
					_canStart = value;
				}
			}
		}
		public ServiceBase()
		{
			try{
				l = StructureMap.ObjectFactory.GetInstance<ILogWrapper>();
				this._state = ServiceState.Stopped;
			}catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}
		public Boolean IsThread
		{
			get{
				return this._thread == null ? false : true;
			}
		}
		private ServiceState _state;
		public ServiceState State {
			get {
				return _state;
			}
			set {
				l.debug("Service State changed from {0} to {1}.", _state, value);
				_state = value;
				
			}
		}
		
		private Thread _thread;
		public Thread Thrd {
			get {
				return _thread;
			}
			set{
				if(value != null && value is Thread)
				{
					_thread = value;
				}
			}
		}
		
		public virtual void Start()
		{
			this.Start("");
		}
		
		public virtual void Start(string msg)
		{
			if(this.CanStart)
			{
				//Thread.Sleep(6000);
				if(this.Thrd != null)
				{
					l.debug("Service starting...");
					Thrd.Start();
					if(Thrd.IsAlive)
					{
						l.info("Service {0} started...", this.GetType().Name);
						this.State = ServiceState.Started;
					}
				}else{
					l.warn("Service Thread property is null...");
					this.State = ServiceState.Started;
				}
			}else{
				l.Warn("{0} cannot be started.", this.GetType().Name);
			}
		}
		public virtual void Stop()
		{
			this.Stop("");
		}
		public virtual void Stop(String msg)
		{
			if(this.State == ServiceState.Started)
			{
				l.debug("Service stopping...");
				if(Thrd != null)
				{
					Thrd.Join(new TimeSpan(0, 0, 5));
					Debug.WriteLine("Stopping");
				}
				this.State = ServiceState.Stopped;
			}else{
				l.error("Nothing to stop, Service is not up...");
			}
		}
		
		public virtual void Restart()
		{
			
			l.info("Service restarting...");
			this.Stop();
			this.Start();
		}

		
		

	}
}
