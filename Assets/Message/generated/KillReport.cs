using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class KillReport {
        public int double_kill; 
        public int triple_kill; 
        public override string ToString() {
            var info = "";
            info += "<b>double_kill</b>:" + double_kill + "\n";
            info += "<b>triple_kill</b>:" + triple_kill + "\n";
            return info;
        }
    }
}
