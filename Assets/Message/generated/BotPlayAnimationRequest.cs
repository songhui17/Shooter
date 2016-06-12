using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class BotPlayAnimationRequest {
        public int bot_id; 
        public string animation_clip; 
        public override string ToString() {
            var info = "";
            info += "<b>bot_id</b>:" + bot_id + "\n";
            info += "<b>animation_clip</b>:" + animation_clip + "\n";
            return info;
        }
    }
}
