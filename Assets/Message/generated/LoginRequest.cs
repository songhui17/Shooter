using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class LoginRequest {
        public string username; 
        public string password; 
        public override string ToString() {
            var info = "";
            info += "<b>username</b>:" + username + "\n";
            info += "<b>password</b>:" + password + "\n";
            return info;
        }
    }
}
