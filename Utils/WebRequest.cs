/*
 * Matthew Carl Bednarski <matthew.bednarski@ekr.it>
 * 25/05/2012 - 09.53
 */

using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Windows.Resources;
using System.Xml;
using Microsoft.Web.Services3.Mime;
using StructureMap;

namespace PacketManagerAdminGui.Utils
{
	/// <summary>
	/// Description of WebGet.
	/// </summary>
	public class WebRequest:INotifyPropertyChanged
	{
		public const String GET = "GET";
		public const String POST = "POST";
		
		public const String USES_AUTH = "authenticate";
		public const String USER_NAME = "user";
		public const String PASSWORD = "password";
		
		ISettings _settings;
		ISettings Settings
		{
			get{
				if(_settings == null){
					_settings = StructureMap.ObjectFactory.GetInstance<ISettings>();
					
					if(_settings[USES_AUTH] == null)
					{
						_settings.Values.Add(USES_AUTH,  Boolean.FalseString);
					}
					
					if(_settings[USER_NAME] == null)
					{
						_settings.Values.Add(USER_NAME, "");
					}
					
					if(_settings[PASSWORD] == null)
					{
						_settings.Values.Add(PASSWORD, "");
					}
				}
				return _settings;
			}
		}

		public WebRequest()
		{
			
		}

		public bool Request(string address, string httpmethod = WebRequest.GET, string body = "" , bool use_xop = false,  int timeout = System.Threading.Timeout.Infinite,Boolean expect100Cont = false)
		{
			bool r = false;
			try
			{
				HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(address);
				
				request.Method = httpmethod;
				request.ServicePoint.Expect100Continue = expect100Cont;
				request.KeepAlive = false;
				request.Timeout = timeout;
				if(!string.IsNullOrEmpty(body) &&  !httpmethod.Equals(WebRequest.GET))
				{
					byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
					request.ContentLength = postBytes.Length;
//					request.ContentType = "multipart/mixed;type=text/xml";
					request.ContentType = "text/xml";
					SetRequestContents(request, postBytes);
				}
				if(Settings[USES_AUTH].ToLower().Equals(Boolean.TrueString.ToLower()))
				{
					request.PreAuthenticate = true;
					CredentialCache cache =  new CredentialCache( );
					Uri uri = new Uri(address);
					cache.Add(uri, "Digest",  new NetworkCredential(Settings[USER_NAME], Settings[PASSWORD]));
					request.Credentials = cache;
				}
				using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				{
					if(use_xop)
					{
						try{
							using(Stream s = response.GetResponseStream())
							{
								XopDocument doc = new XopDocument();
								string ss = doc.LoadFromXopPackage(response.Headers[HttpResponseHeader.ContentType], s);
								ss = doc.OuterXml;
								this.Output = ss;
								r = true;
							}
						}catch(XmlException ex)
						{
							Console.WriteLine("Error: {0}", ex.Message);
						}
					}else{
						
						Encoding enc = System.Text.Encoding.UTF8;
						
						using(StreamReader loResponseStream = new StreamReader(response.GetResponseStream(),enc))
						{
							this.Output = loResponseStream.ReadToEnd();
							if(response.StatusCode == HttpStatusCode.OK){
								r = true;
							}
						}
					}
				}
				
			}
			catch (WebException e)
			{
				this.Output = e.StackTrace;
				if (e.Status != WebExceptionStatus.ProtocolError)
				{
					Encoding enc = System.Text.Encoding.UTF8;
					if(e.Response != null)
					{
						using(StreamReader loResponseStream = new StreamReader(e.Response.GetResponseStream(),enc))
						{
							this.Output += loResponseStream.ReadToEnd();
						}
					}
				}
			}
			catch (Exception e)
			{
				this.Output = e.StackTrace;
				r = false;
			}
			return r;
		}
		private bool _gotResponse;
		public bool IsRequestFinished
		{
			get{
				return _gotResponse;
			}
			set{
				if(value != _gotResponse)
				{
					_gotResponse = value;
					this.OnPropertyChanged("IsRequestFinished");
				}
			}
		}
		private String _output;
		public String Output
		{
			get{
				return _output;
			}
			set{
				if(!String.IsNullOrEmpty(value) && value != _output)
				{
					_output = value;
					this.OnPropertyChanged("Output");
				}
			}
		}

		
		public void RequestAsync(string address,  string httpmethod =WebRequest.GET,  string body = "" , int timeout = System.Threading.Timeout.Infinite,Boolean expect100Cont = false)
		{
			try
			{
				HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(address);
				
				request.Method = httpmethod;
				request.ServicePoint.Expect100Continue = expect100Cont;
				request.KeepAlive = false;
				request.Timeout = timeout;
				if(!string.IsNullOrEmpty(body) &&  !httpmethod.ToLower().Equals("get"))
				{
					byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
					request.ContentLength = postBytes.Length;
					SetRequestContents(request, postBytes);
				}
				if(Settings[USES_AUTH].ToLower().Equals(Boolean.TrueString.ToLower()))
				{
					request.PreAuthenticate = true;
					CredentialCache cache =  new CredentialCache( );
					Uri uri = new Uri(address);
					cache.Add(uri, "Digest",  new NetworkCredential(Settings[USER_NAME], Settings[PASSWORD]));
					request.Credentials = cache;
				}
				IAsyncResult result = request.BeginGetResponse(new AsyncCallback(FinishWebRequest), request);
			}
			catch (Exception e)
			{
				this.Output = e.StackTrace;
			}
		}
		public static void SetRequestContents(System.Net.WebRequest request, byte[] postBytes)
		{
			using(Stream stream = request.GetRequestStream())
			{
				stream.Write(postBytes, 0, postBytes.Length);
			}
		}
		void FinishWebRequest(IAsyncResult result)
		{
			try{
				using(HttpWebResponse response = (HttpWebResponse)(result.AsyncState as HttpWebRequest).EndGetResponse(result))
				{
					Encoding enc = System.Text.Encoding.UTF8;
					using(StreamReader loResponseStream = new StreamReader(response.GetResponseStream(),enc))
					{
						this.Output = loResponseStream.ReadToEnd();
					}
				}
			}
			catch (WebException e)
			{
				this.Output = e.StackTrace;
				if (e.Status != WebExceptionStatus.ProtocolError)
				{
					Encoding enc = System.Text.Encoding.UTF8;
					if(e.Response != null)
					{
						using(StreamReader loResponseStream = new StreamReader(e.Response.GetResponseStream(),enc))
						{
							this.Output += loResponseStream.ReadToEnd();
						}
					}
				}
			}
			catch (Exception e)
			{
				this.Output = e.StackTrace;
			}
			this.IsRequestFinished = true;
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		public virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = this.PropertyChanged;
			if (handler != null)
			{
				PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
				handler(this, e);
			}
		}
	}
}
