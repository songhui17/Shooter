using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class TowerHpSyncRequest {
        public int tower_id; 
        public int hp; 
        public override string ToString() {
            var info = "";
            info += "<b>tower_id</b>:" + tower_id + "\n";
            info += "<b>hp</b>:" + hp + "\n";
            return info;
        }
    }
}
