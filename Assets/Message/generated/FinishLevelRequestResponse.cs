using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class FinishLevelRequestResponse {
        public int errno; 
        public override string ToString() {
            var info = "";
            info += "<b>errno</b>:" + errno + "\n";
            return info;
        }
    }

    [Serializable]
    public class _FinishLevelRequestResponse {
        public string handler;
        public string type;
        public int request_id;
        public FinishLevelRequestResponse response;
    }
}
