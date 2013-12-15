/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 01/11/2013
 * Time: 08:09
 * 
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using MVVm.Core;

namespace Plugin.WPF
{
	/// <summary>
	/// Description of Settings.
	/// </summary>
	public class Settings : MediatorEnabledViewModelBase<object>, Plugin.
		ISettings
	{
		public const string SETTINGS_DIR_KEY = "SETTINGS_DIR";
		public string SETTINGS_DIR{
			get {
				return  Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
		}
		public const string SETTINGS_FILE_KEY = "SETTINGS_FILE";
		
		public string this[string index]
		{
			get{
				string r = null;
				if(this.Values.ContainsKey(index))
				{
					r = this.Values[index];
				}
				return r;
			}
		}

		private Dictionary<string,string> _settings;
		public IDictionary<string, string> Values
		{
			get {
				if(_settings == null)
				{
					_settings  = new Dictionary<string, string>();
				}
				return _settings;
			}
		}
		public Settings():base()
		{
			if(!Directory.Exists(SETTINGS_DIR)){
				try{
					Directory.CreateDirectory(SETTINGS_DIR);
				}catch(IOException ex){
					
				}
			}
			this.Values.Add(SETTINGS_DIR_KEY, SETTINGS_DIR);
			this.Values.Add(SETTINGS_FILE_KEY, Path.Combine(this[SETTINGS_DIR_KEY], "settings.txt"));
		}
		public Boolean HasSetting(String setting)
		{
			Boolean r = false;
			if(!String.IsNullOrEmpty(this[setting]))
			{
				r = true;
			}
			return r;
		}
		public void Load(string path = "")
		{
			//this.Mediator.NotifyColleagues("Settings.Load", path);
			this.LoadSettings(path);
		}
		public void Save(string path = "")
		{
//			this.Mediator.NotifyColleagues("Settings.Save", path);
			this.SaveSettings(path);
		}
		[MediatorMessageSinkAttribute("Settings.Load")]
		private void LoadSettings(string path = "")
		{
			string file = this[SETTINGS_FILE_KEY];
			if(!String.IsNullOrEmpty(path))
			{
				file = path;
			} 
			if(File.Exists(file))
			{
				String settings = "";
				FileStream fs = null;
				BufferedStream bs =  null;
				try{
					fs = new FileStream(file, FileMode.Open);
					bs = new BufferedStream(fs);
					using(StreamReader sr = new StreamReader(bs))
					{
						int line_num = 0;
						string line;
						while ((line = sr.ReadLine()) != null)
						{
							if(line_num > 1){
								String[] sett = line.Split(":".ToCharArray(), 2);
								if(sett != null && sett.Length == 2){
									string key = sett[0].Trim();
									string val = sett[1].Trim();
									if(!key.Equals(SETTINGS_DIR_KEY)
									   && !key.Equals(SETTINGS_FILE_KEY)){
										if(!this.Values.ContainsKey(key)){
											this.Values.Add(key, val);
										} else {
											this.Values[key] = val;
										}
									}
								} else{
									if(!String.IsNullOrWhiteSpace(line))
									{
										if(!this.Values.ContainsKey("#" + line)){
											this.Values.Add("#" + line, "");
										}
									}
								}
							}
							line_num++;
						}
					}
				}
				catch(IOException ex)
				{
					Debug.WriteLine(ex.Message);
				} finally{
					bs.StreamSafeClose();
					fs.StreamSafeClose();
				}
			}
		}
		[MediatorMessageSinkAttribute("Settings.Save")]
		private void SaveSettings(string path = "")
		{
			string file = this[SETTINGS_FILE_KEY];
			if(!String.IsNullOrEmpty(path))
			{
				file = path;
			} else{
				if(this[SETTINGS_FILE_KEY].FileSafeDelete())
				{
					
				}
			}
			using(FileStream fs = new FileStream(file, FileMode.Create))
			{
				using(BufferedStream bs = new BufferedStream(fs))
				{
					using(StreamWriter sw = new StreamWriter(bs))
					{
						sw.WriteLine();
						sw.WriteLine("# Settings File");
						
						foreach(var kvp in this.Values)
						{
							if(!kvp.Key.Equals(SETTINGS_DIR_KEY)
							   && !kvp.Key.Equals(SETTINGS_FILE_KEY)){
								if(!String.IsNullOrWhiteSpace(kvp.Value))
								{
									sw.Write(kvp.Key);
									sw.Write(":\t");
									sw.WriteLine(kvp.Value);
								}else{
									sw.WriteLine(kvp.Key);
								}
							}
						}
					}
				}
			}
		}
	}
}
