/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 14/11/2013
 * Time: 15:24
 * 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.Permissions;
using System.Security.RightsManagement;
using System.Windows.Input;
using System.Windows.Threading;
using Plugin;
using Plugin.WPF;
using MVVm.Core;

namespace Plugin.WPF.Example
{
	
//	public class FileMenu:MenuItemBase
//	{
//		public override string MenuGroup
//		{
//			get{
//				return "";
//			}
//		}
//		public  override string MenuKey
//		{
//			get{
//				return UserControlBase.MENU_GROUP_FILE;
//			}
//		}
//		public override ICommand Command
//		{
//			get{
//				return null;
//			}
//		}
//		public FileMenu()
//		{
//			this.Label = "File";
//		}
//	}

	public class ViewMenu: MenuItemBase
	{
		public override string MenuGroup
		{
			get{
				return "";
			}
		}
		public  override string MenuKey
		{
			get{
				return UserControlBase.MENU_GROUP_VIEW;
			}
		}
		public override ICommand Command
		{
			get{
				return null;
			}
		}
		public ViewMenu()
		{
			this.Label = "View";
		}
	}
//	public class ModifyMenu: MenuItemBase
//	{
//		public override string MenuGroup
//		{
//			get{
//				return "";
//			}
//		}
//		public  override string MenuKey
//		{
//			get{
//				return UserControlBase.MENU_GROUP_SETTING;
//			}
//		}
//		public override ICommand Command
//		{
//			get{
//				return null;
//			}
//		}
//		public ModifyMenu()
//		{
//			this.Label = "Modify";
//		}
//	}
}
