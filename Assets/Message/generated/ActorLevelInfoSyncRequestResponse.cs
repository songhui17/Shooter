using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class ActorLevelInfoSyncRequestResponse {

        public override string ToString() {
            var info = "";

            return info;
        }
    }

    [Serializable]
    public class _ActorLevelInfoSyncRequestResponse {
        public string handler;
        public string type;
        public int request_id;
        public ActorLevelInfoSyncRequestResponse response;
    }
}
