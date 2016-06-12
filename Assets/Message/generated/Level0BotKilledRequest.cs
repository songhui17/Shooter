using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class Level0BotKilledRequest {
        public int bot_id; 
        public override string ToString() {
            var info = "";
            info += "<b>bot_id</b>:" + bot_id + "\n";
            return info;
        }
    }
}
