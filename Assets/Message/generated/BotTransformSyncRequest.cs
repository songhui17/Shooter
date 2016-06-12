using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class BotTransformSyncRequest {
        public int bot_id; 
        public Vector3 position; 
        public float rotation; 
        public Vector3 waypoint_position; 
        public override string ToString() {
            var info = "";
            info += "<b>bot_id</b>:" + bot_id + "\n";
            info += "<b>position</b>:\n" + position + "\n";
            info += "<b>rotation</b>:" + rotation + "\n";
            info += "<b>waypoint_position</b>:\n" + waypoint_position + "\n";
            return info;
        }
    }
}
