using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class Account {
        public string name; 
        public int actor_id; 
        public override string ToString() {
            var info = "";
            info += "<b>name</b>:" + name + "\n";
            info += "<b>actor_id</b>:" + actor_id + "\n";
            return info;
        }
    }
}
