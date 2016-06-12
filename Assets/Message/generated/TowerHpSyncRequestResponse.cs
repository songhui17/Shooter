using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class TowerHpSyncRequestResponse {

        public override string ToString() {
            var info = "";

            return info;
        }
    }

    [Serializable]
    public class _TowerHpSyncRequestResponse {
        public string handler;
        public string type;
        public int request_id;
        public TowerHpSyncRequestResponse response;
    }
}
