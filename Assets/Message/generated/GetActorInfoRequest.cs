using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class GetActorInfoRequest {
        public string username; 
        public override string ToString() {
            var info = "";
            info += "<b>username</b>:" + username + "\n";
            return info;
        }
    }
}
