using UnityEngine;

namespace Shooter
{
    public class ResponseDeserializer {
        public static object Deserialize(string handler_, string payload_, out int requestId_) {
            
            if (handler_ == "get_account_info_request_response") {
                var response = JsonUtility.FromJson<_GetAccountInfoRequestResponse>(payload_) as _GetAccountInfoRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "get_actor_level_info_request_response") {
                var response = JsonUtility.FromJson<_GetActorLevelInfoRequestResponse>(payload_) as _GetActorLevelInfoRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "get_actor_info_request_response") {
                var response = JsonUtility.FromJson<_GetActorInfoRequestResponse>(payload_) as _GetActorInfoRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "create_actor_request_response") {
                var response = JsonUtility.FromJson<_CreateActorRequestResponse>(payload_) as _CreateActorRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "login_request_response") {
                var response = JsonUtility.FromJson<_LoginRequestResponse>(payload_) as _LoginRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            requestId_ = -1;
            return null;
        }
    }
}
