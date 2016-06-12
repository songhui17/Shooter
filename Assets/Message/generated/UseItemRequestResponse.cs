using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class UseItemRequestResponse {

        public override string ToString() {
            var info = "";

            return info;
        }
    }

    [Serializable]
    public class _UseItemRequestResponse {
        public string handler;
        public string type;
        public int request_id;
        public UseItemRequestResponse response;
    }
}
