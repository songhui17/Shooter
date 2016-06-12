using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class BotExploseRequestResponse {

        public override string ToString() {
            var info = "";

            return info;
        }
    }

    [Serializable]
    public class _BotExploseRequestResponse {
        public string handler;
        public string type;
        public int request_id;
        public BotExploseRequestResponse response;
    }
}
