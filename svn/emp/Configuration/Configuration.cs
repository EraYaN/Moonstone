using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace EMP
{
	public enum Setting
	{
		libraryPath = 1,
		iTunesMediaXMLPath = 2,	
	}
	public class Configuration
	{
		public List<Entities.Tab> Tabs;
		public Configuration()
		{
			Tabs = new List<Entities.Tab>();
			//add config with default values

			//settings
			Entities.Setting s_libraryLocation = new Entities.Setting();
			s_libraryLocation.Name = "_Library Path"; //accelerator key is T
			s_libraryLocation.Identifier = "libraryPath"; //case sensitive
			s_libraryLocation.Value = @"library\EMPLibrary.emplib";
			s_libraryLocation.Type = typeof(String);

			Entities.Setting s_iTunesMedia = new Entities.Setting();
			s_iTunesMedia.Name = "_iTunes Media XML Path"; //accelerator key is T
			s_iTunesMedia.Identifier = "iTunesMediaXMLPath"; //case sensitive
			s_iTunesMedia.Value = "";
			s_iTunesMedia.Type = typeof(String);

			//groups
			Entities.Group g_library = new Entities.Group();
			g_library.Settings = new List<Entities.Setting>();
			g_library.Name = "Library";
			g_library.Identifier = "library";
			//claim settings
			g_library.Settings.Add(s_libraryLocation);
			
			Entities.Group g_iTunesMedia = new Entities.Group();
			g_iTunesMedia.Settings = new List<Entities.Setting>();
			g_iTunesMedia.Name = "iTunes Media";
			g_iTunesMedia.Identifier = "iTunesMedia";
			//claim settings
			g_iTunesMedia.Settings.Add(s_iTunesMedia);
		
			//tabs
			Entities.Tab t_general = new Entities.Tab();
			t_general.Identifier = "general";
			t_general.Name = "General";
			t_general.Groups = new List<Entities.Group>();
			//claim groups
			t_general.Groups.Add(g_library);

			Entities.Tab t_iTunes = new Entities.Tab();
			t_iTunes.Identifier = "iTunes";
			t_iTunes.Name = "iTunes";
			t_iTunes.Groups = new List<Entities.Group>();
			//claim groups
			t_iTunes.Groups.Add(g_iTunesMedia);

			Tabs.Add(t_general);
			Tabs.Add(t_iTunes);
		}
		public void LoadConfigurationHelper(ConfigurationSaveHelper savedhelper)
		{
			//recreate config from helper.            
			foreach (Entities.SettingHelper settinghelper in savedhelper.Settings)
			{
				foreach (Entities.Tab tab in Tabs)
				{
					foreach(Entities.Group group in tab.Groups){
						if (group.Identifier == settinghelper.Group)
						{
							foreach(Entities.Setting setting in group.Settings){
								if(setting.Identifier==settinghelper.Identifier){
									int tabindex = Tabs.IndexOf(tab);
									int groupindex = tab.Groups.IndexOf(group);
									int settingindex = group.Settings.IndexOf(setting);
									Tabs[tabindex].Groups[groupindex].Settings[settingindex].Value = settinghelper.Value;                       
								}
							}
						}
					}
				}
			}
		}
		public Object GetSetting(Setting setting)
		{
			return GetSetting(setting.ToString());
		}
		public Object GetSetting(String identifier)
		{
			//TODO linq
			foreach (Entities.Tab tab in Tabs)
			{
				foreach (Entities.Group group in tab.Groups)
				{
					foreach (Entities.Setting setting in group.Settings)
					{
						if (setting.Identifier == identifier)
						{
							return setting.Value;
						}
					}
				}
			}
			throw new ArgumentException("Setting not found.", identifier);
		}
		public Boolean SetSetting(Setting setting, Object value)
		{
			return SetSetting(setting.ToString(), value);
		}
		public Boolean SetSetting(String identifier, Object value)
		{
			//TODO linq
			foreach (Entities.Tab tab in Tabs)
			{
				foreach (Entities.Group group in tab.Groups)
				{
					foreach (Entities.Setting setting in group.Settings)
					{
						if (setting.Identifier == identifier)
						{
							setting.Value = value;
							return true;
						}
					}
				}
			}
			throw new ArgumentException("Setting not found.", identifier);			
		}
	}
}
