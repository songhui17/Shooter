using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class LevelInfo {
        public int level_id; 
        public string title; 
        public string task1; 
        public string task2; 
        public string task3; 
        public List<String> bonuses; 
        public override string ToString() {
            var info = "";
            info += "<b>level_id</b>:" + level_id + "\n";
            info += "<b>title</b>:" + title + "\n";
            info += "<b>task1</b>:" + task1 + "\n";
            info += "<b>task2</b>:" + task2 + "\n";
            info += "<b>task3</b>:" + task3 + "\n";
            info += "<b>bonuses</b>:\n" + bonuses + "\n";
            return info;
        }
    }
}
