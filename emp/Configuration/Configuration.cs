using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace EMP
{
    public class Configuration
    {
        public List<Entities.Tab> Tabs;
        public Configuration()
        {
            Tabs = new List<Entities.Tab>();
            //add config with default values
            //settings
            Entities.Setting setting = new Entities.Setting();
            setting.Name = "_Test Blah"; //accelerator key is T
            setting.Identifier = "testBlah"; //case sensitive
            setting.Value = "Test";
            setting.Type = typeof(String);
            Entities.Setting setting2 = new Entities.Setting();
            setting2.Name = "T_est 5"; //accelerator key is e
            setting2.Identifier = "test3"; //case sensitive
            setting2.Value = "Test 5";
            setting2.Type = typeof(String);
            Entities.Setting settingToF = new Entities.Setting();
            settingToF.Name = "_Oma True or False"; //accelerator key is O
            settingToF.Identifier = "omaToF"; //case sensitive
            settingToF.Value = true;
            settingToF.Type = typeof(Boolean);
            //groups
            Entities.Group group = new Entities.Group();
            group.Settings = new List<Entities.Setting>();
            group.Name = "Other";
            group.Identifier = "other";
            //claim settings
            group.Settings.Add(setting);
            group.Settings.Add(setting2);
            group.Settings.Add(settingToF);
            //tabs
            Entities.Tab tab = new Entities.Tab();
            tab.Identifier = "general";
            tab.Name = "General";
            tab.Groups = new List<Entities.Group>();
            //claim groups
            tab.Groups.Add(group);
            Tabs.Add(tab);
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
    }
}
