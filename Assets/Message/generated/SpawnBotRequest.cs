using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class SpawnBotRequest {
        public int bot_id; 
        public string bot_type; 
        public Vector3 position; 
        public float rotation; 
        public override string ToString() {
            var info = "";
            info += "<b>bot_id</b>:" + bot_id + "\n";
            info += "<b>bot_type</b>:" + bot_type + "\n";
            info += "<b>position</b>:\n" + position + "\n";
            info += "<b>rotation</b>:" + rotation + "\n";
            return info;
        }
    }
}
