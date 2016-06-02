using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class GetLevelInfoRequestResponse {
        public List<LevelInfo> level_info; 
        public int errno; 
        public override string ToString() {
            var info = "";
            info += "<b>level_info</b>:\n" + level_info + "\n";
            info += "<b>errno</b>:" + errno + "\n";
            return info;
        }
    }

    [Serializable]
    public class _GetLevelInfoRequestResponse {
        public string handler;
        public string type;
        public int request_id;
        public GetLevelInfoRequestResponse response;
    }
}
