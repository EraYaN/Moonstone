using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace EMP
{
    public class Entities
    {
        public class Tab
        {
            public String Name;
            public String Identifier;
            public List<Group> Groups; 
        }
        public class Group
        {
            public String Name;
            public String Identifier;
            public List<Setting> Settings;
        }
        public class Setting
        {
            public String Name;
            public String Identifier;
            public Object Value;
            public Type Type;
        }
        [Serializable()]
        public class TabHelper : ISerializable
        {
            public String Name;
            public String Identifier;
            public TabHelper()
            {
                Name = "";
                Identifier = "";                
            }
            public TabHelper(String _Name, String _Identifier)
            {
                Name = _Name;
                Identifier = _Identifier;                               
            }
            protected TabHelper(SerializationInfo info, StreamingContext context)
            {
                if (info == null)
                    throw new System.ArgumentNullException("info");

                Name = (String)info.GetValue("Name", typeof(String));
                Identifier = (String)info.GetValue("Identifier", typeof(String));
            }
            public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context)
            {
                if (info == null)
                    throw new System.ArgumentNullException("info");

                info.AddValue("Name", Name);
                info.AddValue("Identifier", Identifier);
            }
        }
        [Serializable()]
        public class GroupHelper : ISerializable
        {
            public String Name;
            public String Identifier;
            public String Tab;
            public GroupHelper()
            {
                Name = "";
                Identifier = "";
                Tab = "";                
            }
            public GroupHelper(String _Name, String _Identifier, String _Tab)
            {
                Name = _Name;
                Identifier = _Identifier;
                Tab = _Tab;                
            }
            protected GroupHelper(SerializationInfo info, StreamingContext context)
            {
                if (info == null)
                    throw new System.ArgumentNullException("info");

                Name = (String)info.GetValue("Name", typeof(String));
                Identifier = (String)info.GetValue("Identifier", typeof(String));
                Tab = (String)info.GetValue("Tab", typeof(String));
            }
            public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context)
            {
                if (info == null)
                    throw new System.ArgumentNullException("info");

                info.AddValue("Name", Name);
                info.AddValue("Identifier", Identifier);
                info.AddValue("Tab", Tab);
                
            }
        }
        [Serializable()]
        public class SettingHelper : ISerializable
        {
            public String Name;
            public String Identifier;
            public String Group;
            public Object Value;
            public Type Type;
            public SettingHelper()
            {
                Name = "";
                Identifier = "";
                Group = "";
                Value = "";
                Type = typeof(String);
            }
            public SettingHelper(String _Name, String _Identifier, String _Group, Object _Value, Type _Type)
            {
                Name = _Name;
                Identifier = _Identifier;
                Group = _Group;
                Value = _Value;
                Type = _Type;
            }
            protected SettingHelper(SerializationInfo info, StreamingContext context)
            {
                if (info == null)
                    throw new System.ArgumentNullException("info");

                Name = (String)info.GetValue("Name", typeof(String));
                Identifier = (String)info.GetValue("Identifier", typeof(String));
                Group = (String)info.GetValue("Group", typeof(String));
                Type = (Type)info.GetValue("Type", typeof(Type));
                Value = (Object)info.GetValue("Value", typeof(Object));
            }
            public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context)
            {
                if (info == null)
                    throw new System.ArgumentNullException("info");

                info.AddValue("Name", Name);
                info.AddValue("Identifier", Identifier);
                info.AddValue("Group", Group);
                info.AddValue("Type", Type);
                info.AddValue("Value", Value);
            }
        }
    }
}
