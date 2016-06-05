using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class Level0BotKilledRequestResponse {
        public int errno; 
        public override string ToString() {
            var info = "";
            info += "<b>errno</b>:" + errno + "\n";
            return info;
        }
    }

    [Serializable]
    public class _Level0BotKilledRequestResponse {
        public string handler;
        public string type;
        public int request_id;
        public Level0BotKilledRequestResponse response;
    }
}
