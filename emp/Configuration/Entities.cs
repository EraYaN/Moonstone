using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMP
{
    public class Entities
    {
        [Serializable()]
        public struct Tab
        {
            public String Name;
            public String Identifier;
            public List<Group> Groups; 
        }
        [Serializable()]
        public struct Group
        {
            public String Name;
            public String Identifier;
            public List<Setting> Settings;
        }
        [Serializable()]
        public struct Setting
        {
            public String Name;
            public String Identifier;
            public Object Value;
            public Type Type;
        }
    }
}
