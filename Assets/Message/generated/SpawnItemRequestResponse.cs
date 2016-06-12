using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class SpawnItemRequestResponse {

        public override string ToString() {
            var info = "";

            return info;
        }
    }

    [Serializable]
    public class _SpawnItemRequestResponse {
        public string handler;
        public string type;
        public int request_id;
        public SpawnItemRequestResponse response;
    }
}
