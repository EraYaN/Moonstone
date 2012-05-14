using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace EMP
{
    [Serializable()]
    public class Configuration : ISerializable 
    {
        public List<Entities.Tab> Tabs;
        public Configuration()
        {
            Tabs = new List<Entities.Tab>();
            //add config with default values
            Entities.Setting setting = new Entities.Setting();
            setting.Name = "_Test Blah";//accelerator key is T
            setting.Identifier = "testBlah";
            setting.Value = "Test Value";
            setting.Type = typeof(String);
            Entities.Group group = new Entities.Group();
            group.Settings = new List<Entities.Setting>();
            group.Name = "Other";
            group.Identifier = "other";
            group.Settings.Add(setting);
            Entities.Tab tab = new Entities.Tab();
            tab.Identifier = "general";
            tab.Name = "General";
            tab.Groups = new List<Entities.Group>();
            tab.Groups.Add(group);
            Tabs.Add(tab);
        }
        public Configuration(String ConfigPath)
        {
            //add config with default values and save to configpath
        }
        protected Configuration(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");
            Tabs = (List<Entities.Tab>)info.GetValue("Tabs", typeof(List<Entities.Tab>));            
        }
        public virtual void GetObjectData(
        SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");
            info.AddValue("Tabs", Tabs);           
        }
    }
}
