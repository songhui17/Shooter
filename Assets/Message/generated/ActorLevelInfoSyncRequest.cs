using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class ActorLevelInfoSyncRequest {
        public ActorLevelInfo actor_level_info; 
        public override string ToString() {
            var info = "";
            info += "<b>actor_level_info</b>:\n" + actor_level_info + "\n";
            return info;
        }
    }
}
