namespace Shooter {
    [System.Serializable]
    public class BaseError
    {
        public string type;
        public int request_id;
        public string handler;
        public string error;
    }
}
