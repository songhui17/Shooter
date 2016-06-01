using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class GetActorLevelInfoRequestResponse {
        public List<ActorLevelInfo> actor_level_info; 
        public int errno; 
        public override string ToString() {
            var info = "";
            info += "<b>actor_level_info</b>:\n" + actor_level_info + "\n";
            info += "<b>errno</b>:" + errno + "\n";
            return info;
        }
    }

    [Serializable]
    public class _GetActorLevelInfoRequestResponse {
        public string handler;
        public string type;
        public int request_id;
        public GetActorLevelInfoRequestResponse response;
    }
}
