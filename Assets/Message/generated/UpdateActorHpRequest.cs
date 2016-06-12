using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class UpdateActorHpRequest {
        public int actor_id; 
        public int hp; 
        public int max_ammo; 
        public int ammo; 
        public override string ToString() {
            var info = "";
            info += "<b>actor_id</b>:" + actor_id + "\n";
            info += "<b>hp</b>:" + hp + "\n";
            info += "<b>max_ammo</b>:" + max_ammo + "\n";
            info += "<b>ammo</b>:" + ammo + "\n";
            return info;
        }
    }
}
