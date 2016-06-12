using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class UseItemRequest {
        public string item_type; 
        public override string ToString() {
            var info = "";
            info += "<b>item_type</b>:" + item_type + "\n";
            return info;
        }
    }
}
