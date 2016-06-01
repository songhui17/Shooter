using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class LoginRequestResponse {
        public bool result; 
        public int errno; 
        public override string ToString() {
            var info = "";
            info += "<b>result</b>:" + result + "\n";
            info += "<b>errno</b>:" + errno + "\n";
            return info;
        }
    }

    [Serializable]
    public class _LoginRequestResponse {
        public string handler;
        public string type;
        public int request_id;
        public LoginRequestResponse response;
    }
}
