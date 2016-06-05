using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class FinishLevelRequest {
        public bool win; 
        public List<String> bonuses; 
        public override string ToString() {
            var info = "";
            info += "<b>win</b>:" + win + "\n";
            info += "<b>bonuses</b>:\n" + bonuses + "\n";
            return info;
        }
    }
}
