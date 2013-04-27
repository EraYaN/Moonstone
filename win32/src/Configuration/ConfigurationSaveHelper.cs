using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace EMP
{
    [Serializable()]
    public class ConfigurationSaveHelper : ISerializable
    {        
        List<Entities.SettingHelper> settings = new List<Entities.SettingHelper>();        
        public List<Entities.SettingHelper> Settings
        {
            get { return settings; }
        }
        public ConfigurationSaveHelper(Configuration config)
        {
            //create save helper from config
            foreach (Entities.Tab tab in config.Tabs)
            {
                foreach (Entities.Group group in tab.Groups)
                {
                    foreach (Entities.Setting setting in group.Settings)
                    {
                        //add setting  
                        settings.Add(new Entities.SettingHelper(setting.Name, setting.Identifier, group.Identifier, setting.Value, setting.Type));                     
                    }
                }
            }
        }
        protected ConfigurationSaveHelper(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            settings = (List<Entities.SettingHelper>)info.GetValue("Settings", typeof(List<Entities.SettingHelper>)); 
        }
        public virtual void GetObjectData(
        SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            info.AddValue("Settings", settings);
        }
    }
}
