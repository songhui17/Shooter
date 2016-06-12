using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class KillReportSyncRequestResponse {

        public override string ToString() {
            var info = "";

            return info;
        }
    }

    [Serializable]
    public class _KillReportSyncRequestResponse {
        public string handler;
        public string type;
        public int request_id;
        public KillReportSyncRequestResponse response;
    }
}
