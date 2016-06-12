using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class BotTransformSyncRequestResponse {

        public override string ToString() {
            var info = "";

            return info;
        }
    }

    [Serializable]
    public class _BotTransformSyncRequestResponse {
        public string handler;
        public string type;
        public int request_id;
        public BotTransformSyncRequestResponse response;
    }
}
