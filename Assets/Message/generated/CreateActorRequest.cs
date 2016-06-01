using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class CreateActorRequest {
        public string username; 
        public string actor_type; 
        public override string ToString() {
            var info = "";
            info += "<b>username</b>:" + username + "\n";
            info += "<b>actor_type</b>:" + actor_type + "\n";
            return info;
        }
    }
}
