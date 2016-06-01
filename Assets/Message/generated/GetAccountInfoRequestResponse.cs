using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class GetAccountInfoRequestResponse {
        public Account account_info; 
        public int errno; 
        public override string ToString() {
            var info = "";
            info += "<b>account_info</b>:\n" + account_info + "\n";
            info += "<b>errno</b>:" + errno + "\n";
            return info;
        }
    }

    [Serializable]
    public class _GetAccountInfoRequestResponse {
        public string handler;
        public string type;
        public int request_id;
        public GetAccountInfoRequestResponse response;
    }
}
