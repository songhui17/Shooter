using System;
using System.Collections.Generic;

namespace Shooter
{
    [Serializable]
    public class BotPlayAnimationRequestResponse {

        public override string ToString() {
            var info = "";

            return info;
        }
    }

    [Serializable]
    public class _BotPlayAnimationRequestResponse {
        public string handler;
        public string type;
        public int request_id;
        public BotPlayAnimationRequestResponse response;
    }
}
