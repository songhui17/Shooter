using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class SpawnItemRequest {
        public string item_type; 
        public Vector3 position; 
        public override string ToString() {
            var info = "";
            info += "<b>item_type</b>:" + item_type + "\n";
            info += "<b>position</b>:\n" + position + "\n";
            return info;
        }
    }
}
