using System;

public interface IReponseHandler {
    void RecvMessage(object message_, int requestId_);
}

public class _ResponseHandler<T> : IReponseHandler where T: class{
    private Action<T, int> _callbackReq;
    public _ResponseHandler(Action<T, int> callback_) {
        _callbackReq = callback_;
    }

    public void RecvMessage(object message_, int requestId_) {
        if (_callbackReq != null)
            _callbackReq(message_ as T, requestId_);
    }
}

public interface IRequestHandler {
    object RecvMessage(object message_);
}

public class _RequestHandler<TRequest, TResponse> : IRequestHandler where TRequest: class {
    private Func<TRequest, TResponse> _handler;
    public _RequestHandler(Func<TRequest, TResponse> handler_) {
        _handler = handler_;
    }

    public object RecvMessage(object message_) {
        if (_handler != null) {
            return _handler(message_ as TRequest);
        }else{
            return null;
        }
    }
}
