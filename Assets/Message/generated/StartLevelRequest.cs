using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class StartLevelRequest {
        public int actor_id; 
        public int level_id; 
        public override string ToString() {
            var info = "";
            info += "<b>actor_id</b>:" + actor_id + "\n";
            info += "<b>level_id</b>:" + level_id + "\n";
            return info;
        }
    }
}
