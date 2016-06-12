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
        public int max_hp; 
        public int hp; 
        public int max_ammo; 
        public int ammo; 
        public override string ToString() {
            var info = "";
            info += "<b>actor_id</b>:" + actor_id + "\n";
            info += "<b>name</b>:" + name + "\n";
            info += "<b>level</b>:" + level + "\n";
            info += "<b>gold</b>:" + gold + "\n";
            info += "<b>experience</b>:" + experience + "\n";
            info += "<b>max_hp</b>:" + max_hp + "\n";
            info += "<b>hp</b>:" + hp + "\n";
            info += "<b>max_ammo</b>:" + max_ammo + "\n";
            info += "<b>ammo</b>:" + ammo + "\n";
            return info;
        }
    }
}
