using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class KillReportSyncRequest {
        public int actor_id; 
        public KillReport kill_report; 
        public override string ToString() {
            var info = "";
            info += "<b>actor_id</b>:" + actor_id + "\n";
            info += "<b>kill_report</b>:\n" + kill_report + "\n";
            return info;
        }
    }
}
