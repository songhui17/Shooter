using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class ActorLevelInfo {
        public int actor_id; 
        public int level_id; 
        public bool passed; 
        public bool star1; 
        public bool star2; 
        public bool star3; 
        public override string ToString() {
            var info = "";
            info += "<b>actor_id</b>:" + actor_id + "\n";
            info += "<b>level_id</b>:" + level_id + "\n";
            info += "<b>passed</b>:" + passed + "\n";
            info += "<b>star1</b>:" + star1 + "\n";
            info += "<b>star2</b>:" + star2 + "\n";
            info += "<b>star3</b>:" + star3 + "\n";
            return info;
        }
    }
}
