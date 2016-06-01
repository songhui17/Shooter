using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class Actor {
        public int actor_id; 
        public string name; 
        public int level; 
        public int gold; 
        public int experience; 
        public override string ToString() {
            var info = "";
            info += "<b>actor_id</b>:" + actor_id + "\n";
            info += "<b>name</b>:" + name + "\n";
            info += "<b>level</b>:" + level + "\n";
            info += "<b>gold</b>:" + gold + "\n";
            info += "<b>experience</b>:" + experience + "\n";
            return info;
        }
    }
}
