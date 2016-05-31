using UnityEngine;
using System;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;

using Shooter;

[System.Serializable]
public class _LoginRequestResponse {
    public string handler;
    public string type;
    public int request_id;
    public LoginRequestResponse response;
}

public class SockUtil : MonoBehaviour {
    private int _requestId = 1;
    private Socket _socket;
    private byte[] _buffer = new byte[1024];
    private byte[] _readBuffer = new byte[1024];
    private byte[] _swapBuffer = new byte[1024];
    private int _readIndex = 0;

    private static SockUtil _instance;
    public static SockUtil Instance {
        get {
            if (_instance == null) {
                var obj = new GameObject("SockUtil");
                _instance = obj.AddComponent<SockUtil>();
            }
            return _instance;
        }
    }

    // private SockUtil() {
    void Awake() {
        DontDestroyOnLoad(gameObject);

        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socket.Connect("127.0.0.1", 10240);
    }

    void Update() {
        if (_socket == null) return;

        var readlist = new List<Socket>() { _socket };
        Socket.Select(readlist, null, null, 0);
        if (readlist.Count == 1) {
            var n = _socket.Receive(_readBuffer, _readIndex, (_readBuffer.Length - _readIndex), SocketFlags.None);
            if (n == 0){
                Debug.Log("Socket closed");
            }else{
                _readIndex += n;
                if (_readIndex >= 2){
                    var index = 0;
                    var length = ReadByte128(_readBuffer, ref index);
                    if (length <= (_readIndex - 2)) {
                        // index = 0;
                        var handler = ReadString(_readBuffer, ref index);
                        var info = "Receive:\n";
                        info += "handler: " + handler + "\n";
                        var message = Encoding.UTF8.GetString(_readBuffer, index, (length - index + 2));

                        Array.Copy(_readBuffer, index, _swapBuffer, 0, (_readIndex - index));
                        var tmp = _swapBuffer;
                        _swapBuffer = _readBuffer;
                        _readBuffer = tmp;
                        _readIndex -= index;

                        info += "message.Length: " + message.Length + "\n";
                        info += "message: " + message + "\n";
                        Debug.Log(info);

                        _socket.Close();
                        _socket = null;

                        RecvMessage(handler, message);
                    }
                }
            }
        }
    }

    private void RecvMessage(string handler_, string payload_) {
        if (handler_ == "login_request_response") {
            var loginResponse = JsonUtility.FromJson<_LoginRequestResponse>(payload_) as _LoginRequestResponse;
            var requestId = loginResponse.request_id;
            if (_callbackMap.ContainsKey(requestId)) {
                _callbackMap[requestId].RecvMessage(loginResponse.response);
                _callbackMap.Remove(requestId);
            }
        }
    }

    // TODO: positive only
    private static int WriteByte128(int value_, byte[] buffer_, int index_) {
        value_ &= 0xffff;
        var low_byte = (byte)(value_ & 0x7F);
        var high_byte = (byte)(value_ >> 7);

        buffer_[index_++] = high_byte;
        buffer_[index_++] = low_byte;

        return index_;
    }

    private static int ReadByte128(byte[] buffer_, ref int index_) {
        int high_byte = buffer_[index_++];
        int low_byte = buffer_[index_++];
        return (high_byte << 7) + low_byte;
    }

    private static int WriteString(string value_, byte[] buffer_, int index_) {
        var utf8 = Encoding.UTF8;
        var bytes = utf8.GetBytes(value_);
        var length = bytes.Length;

        index_ = WriteByte128(length, buffer_, index_);
        Array.Copy(bytes, 0, buffer_, index_, length);
        return index_ + length;
    }

    private static string ReadString(byte[] buffer_, ref int index_) {
        var length = ReadByte128(buffer_, ref index_);
        var utf8 = Encoding.UTF8;
        Debug.Log("length: " + length);
        var str = utf8.GetString(buffer_, index_, length);
        index_ += length;
        return str;
    }

    private void SendMessage<T>(T request_, string handler_) {
        var payload = JsonUtility.ToJson(request_);
        var index = 0;
        // index = WriteString(payload, _buffer, index);
        var handler = handler_;
        var bytes = Encoding.UTF8.GetBytes(payload);
        var totalLength = bytes.Length + 2 + handler.Length;
        index = WriteByte128(totalLength, _buffer, index);
        index = WriteString(handler, _buffer, index);
        Array.Copy(bytes, 0, _buffer, index, bytes.Length);
        index += bytes.Length;
        _socket.Send(_buffer, 0, index, SocketFlags.None);
    }

    private Dictionary<int, IReponseHandler> _callbackMap = new Dictionary<int, IReponseHandler>();
    public void SendRequest<TRequest, TResponse>(
            Socket socket_, TRequest request_, Action<TResponse> callback_ = null)
            where TResponse : class {
        var baseRequest = new BaseRequest<TRequest>() {
            type = "request",
            request_id = _requestId,
            handler = "login_request",
            request = request_,
        };
        SendMessage<BaseRequest<TRequest>>(baseRequest, baseRequest.handler);
        _callbackMap.Add(_requestId, new _ResponseHandler<TResponse>(callback_));
        _requestId++;
    }
}


public interface IReponseHandler {
    void RecvMessage(object message_);
}

public class _ResponseHandler<T> : IReponseHandler where T: class{
    private Action<T> _callback;
    public _ResponseHandler(Action<T> callback_) {
        _callback = callback_;
    }

    public void RecvMessage(object message_) {
        if (_callback != null)
            _callback(message_ as T);
    }
}
