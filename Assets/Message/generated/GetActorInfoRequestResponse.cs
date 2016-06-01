using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class GetActorInfoRequestResponse {
        public Actor actor_info; 
        public int errno; 
        public override string ToString() {
            var info = "";
            info += "<b>actor_info</b>:\n" + actor_info + "\n";
            info += "<b>errno</b>:" + errno + "\n";
            return info;
        }
    }

    [Serializable]
    public class _GetActorInfoRequestResponse {
        public string handler;
        public string type;
        public int request_id;
        public GetActorInfoRequestResponse response;
    }
}
