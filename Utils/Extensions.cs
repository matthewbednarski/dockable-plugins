/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 31/10/2013
 * Time: 21:08
 * 
 */
using System;
using System.CodeDom;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace Plugin.WPF
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public static class Extensions
	{
		public static String xmlObjectToString( this Object obj, bool omitXmlDeclaration = false, string defaultprefix = "", string defaultnamespace = "")
		{
			
			String XmlizedString = null;
			using(MemoryStream memoryStream = new MemoryStream())
			{
				XmlSerializer xs = new XmlSerializer(obj.GetType(), defaultnamespace);
				XmlSerializerNamespaces xsnss = null;
				if(!String.IsNullOrEmpty(defaultnamespace))
				{
					xsnss = new XmlSerializerNamespaces();
					xsnss.Add(defaultprefix, defaultnamespace);
				}
				XmlWriterSettings writerSettings = new XmlWriterSettings();
				writerSettings.OmitXmlDeclaration = omitXmlDeclaration;
				writerSettings.Encoding = new UTF8Encoding(false);

				using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, writerSettings))
				{
					if(xsnss != null){
						xs.Serialize(xmlWriter, obj, xsnss);
					}else{
						xs.Serialize(xmlWriter, obj);
					}

				}
				//memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
				XmlizedString = memoryStream.ToArray().UTF8ByteArrayToString();
			}
			return XmlizedString;
		}
		
		public static T DeserializeObject<T>(this String pXmlizedString, string defaultNamespace)
		{
			object o = new object();
			try
			{
				XmlSerializer xs = new XmlSerializer(typeof(T), defaultNamespace);
				MemoryStream memoryStream = new MemoryStream(pXmlizedString.StringToUTF8ByteArray());
				
				XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
				using(XmlTextReader xmlTextReader = new XmlTextReader(memoryStream))
				{
					xmlTextReader.Normalization = false;
					o = xs.Deserialize(xmlTextReader);
				}
				if(o is T)
				{
					return (T)o;
				}
			}
			catch (System.InvalidOperationException ex)
			{
				Console.WriteLine(ex.Message);
			}
			return default(T);
			
		}
		public static T Deserialize<T>(this String pXmlizedString, string defaultNamespace)
		{
			return pXmlizedString.DeserializeObject<T>(defaultNamespace);
		}
		public static T Deserialize<T>(this String pXmlizedString)
		{
			return pXmlizedString.DeserializeObject<T>("");
		}
		
		public static String UTF8ByteArrayToString(this Byte[] characters)
		{
			UTF8Encoding encoding = new UTF8Encoding();

			String constructedString = encoding.GetString(characters);
			return (constructedString);
		}
		public static Byte[] StringToUTF8ByteArray(this String pXmlString)
		{
			UTF8Encoding encoding = new UTF8Encoding();
			Byte[] byteArray = encoding.GetBytes(pXmlString);
			return byteArray;
		}

		public static String OpenFileDialogImages( int filterIndex, out int lastFilterIndex,  string cd_in, out String cd)
		{
			string imagesFilter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|Icon Files (*.ico)|*.ico|Exe Files (*.exe)|*.exe|All Files (*.*)|*.*";
			
			lastFilterIndex = filterIndex;
			cd = cd_in;
			String r = "";
			OpenFileDialog ofd = new OpenFileDialog();
			if(cd.Equals(String.Empty))
			{
				cd =  Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			}
			ofd.Filter = imagesFilter;
			ofd.FilterIndex = filterIndex;
			ofd.InitialDirectory = cd;
			bool? res = ofd.ShowDialog();
			if(res != null  && res.Equals(true))
			{
				cd = Path.GetDirectoryName(ofd.FileName);
				r= ofd.FileName;
				lastFilterIndex = GetLastFilterIndex(lastFilterIndex, Path.GetExtension(r));
			}
			return r;
		}
		private static int GetLastFilterIndex(int lastFilterIndex, string ext)
		{
			if(ext.Equals(".jpeg"))
			{
				return 1;
			} else if( ext.Equals(".png"))
			{
				return 2;
			} else if( ext.Equals(".jpg"))
			{
				return 3;
			} else if( ext.Equals(".gif"))
			{
				return 3;
			} else if( ext.Equals(".exe"))
			{
				
				return 6;
			} else if( ext.Equals(".ico"))
			{
				return 5;
			}
			return lastFilterIndex;
		}
		
		public static Boolean FileSafeDelete(this string file)
		{
			Boolean r = true;
			try {
				if(File.Exists(file))
				{
					File.Delete(file);
				}
			} catch(IOException ex)
			{
				r = false;
				Debug.WriteLine(ex.Message);
			}
			return r;
		}
		public static Boolean StreamSafeClose(this IDisposable stream)
		{
			Boolean r = false;
			try{
				stream.Dispose();
				r = true;
			}catch(Exception ex){
				//TODO: catch only necessary exceptions
				Debug.WriteLine(ex.Message);
				r = false;
			}finally
			{
				if(stream != null)
				{
					stream.Dispose();
				}
			}
			return r;
		}
	}
}
