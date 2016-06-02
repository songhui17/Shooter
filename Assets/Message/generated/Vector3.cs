using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class Vector3 {
        public float x; 
        public float y; 
        public float z; 
        public override string ToString() {
            var info = "";
            info += "<b>x</b>:" + x + "\n";
            info += "<b>y</b>:" + y + "\n";
            info += "<b>z</b>:" + z + "\n";
            return info;
        }
    }
}
