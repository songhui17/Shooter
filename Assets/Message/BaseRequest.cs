namespace Shooter {
    [System.Serializable]
    public class BaseRequest<T>
    {
        public string type;
        public int request_id;
        public bool require_response;
        public string handler;
        public T request;
    }
}
