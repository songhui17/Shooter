using UnityEngine;

namespace Shooter
{
    public class ResponseDeserializer {
        public static object Deserialize(string handler_, string payload_, out int requestId_) {
            
            if (handler_ == "start_level_request_response") {
                var response = JsonUtility.FromJson<_StartLevelRequestResponse>(payload_) as _StartLevelRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "bot_explose_request_response") {
                var response = JsonUtility.FromJson<_BotExploseRequestResponse>(payload_) as _BotExploseRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "get_level_info_request_response") {
                var response = JsonUtility.FromJson<_GetLevelInfoRequestResponse>(payload_) as _GetLevelInfoRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "create_actor_request_response") {
                var response = JsonUtility.FromJson<_CreateActorRequestResponse>(payload_) as _CreateActorRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "use_item_request_response") {
                var response = JsonUtility.FromJson<_UseItemRequestResponse>(payload_) as _UseItemRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

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

            if (handler_ == "update_actor_hp_request_response") {
                var response = JsonUtility.FromJson<_UpdateActorHpRequestResponse>(payload_) as _UpdateActorHpRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "kill_report_sync_request_response") {
                var response = JsonUtility.FromJson<_KillReportSyncRequestResponse>(payload_) as _KillReportSyncRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "bot_play_animation_request_response") {
                var response = JsonUtility.FromJson<_BotPlayAnimationRequestResponse>(payload_) as _BotPlayAnimationRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "bot_transform_sync_request_response") {
                var response = JsonUtility.FromJson<_BotTransformSyncRequestResponse>(payload_) as _BotTransformSyncRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "tower_hp_sync_request_response") {
                var response = JsonUtility.FromJson<_TowerHpSyncRequestResponse>(payload_) as _TowerHpSyncRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "enter_level_request_response") {
                var response = JsonUtility.FromJson<_EnterLevelRequestResponse>(payload_) as _EnterLevelRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "finish_level_request_response") {
                var response = JsonUtility.FromJson<_FinishLevelRequestResponse>(payload_) as _FinishLevelRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "leave_level_request_response") {
                var response = JsonUtility.FromJson<_LeaveLevelRequestResponse>(payload_) as _LeaveLevelRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "get_actor_info_request_response") {
                var response = JsonUtility.FromJson<_GetActorInfoRequestResponse>(payload_) as _GetActorInfoRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "spawn_item_request_response") {
                var response = JsonUtility.FromJson<_SpawnItemRequestResponse>(payload_) as _SpawnItemRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "level0_bot_killed_request_response") {
                var response = JsonUtility.FromJson<_Level0BotKilledRequestResponse>(payload_) as _Level0BotKilledRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "spawn_bot_request_response") {
                var response = JsonUtility.FromJson<_SpawnBotRequestResponse>(payload_) as _SpawnBotRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "login_request_response") {
                var response = JsonUtility.FromJson<_LoginRequestResponse>(payload_) as _LoginRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            if (handler_ == "actor_level_info_sync_request_response") {
                var response = JsonUtility.FromJson<_ActorLevelInfoSyncRequestResponse>(payload_) as _ActorLevelInfoSyncRequestResponse;
                requestId_ = response.request_id;
                return response.response;
            }

            requestId_ = -1;
            return null;
        }
    }

    public class RequestHandlerDispatcher {
        public static bool HandleRequest(SockUtil sockUtil_, IRequestHandler h_, string handler_, string payload_) {
            
            if (handler_ == "use_item_request") {
                var request = JsonUtility.FromJson<BaseRequest<UseItemRequest>>(payload_) as BaseRequest<UseItemRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as UseItemRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _UseItemRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_UseItemRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "get_actor_level_info_request") {
                var request = JsonUtility.FromJson<BaseRequest<GetActorLevelInfoRequest>>(payload_) as BaseRequest<GetActorLevelInfoRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as GetActorLevelInfoRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _GetActorLevelInfoRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_GetActorLevelInfoRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "kill_report_sync_request") {
                var request = JsonUtility.FromJson<BaseRequest<KillReportSyncRequest>>(payload_) as BaseRequest<KillReportSyncRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as KillReportSyncRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _KillReportSyncRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_KillReportSyncRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "bot_play_animation_request") {
                var request = JsonUtility.FromJson<BaseRequest<BotPlayAnimationRequest>>(payload_) as BaseRequest<BotPlayAnimationRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as BotPlayAnimationRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _BotPlayAnimationRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_BotPlayAnimationRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "get_account_info_request") {
                var request = JsonUtility.FromJson<BaseRequest<GetAccountInfoRequest>>(payload_) as BaseRequest<GetAccountInfoRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as GetAccountInfoRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _GetAccountInfoRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_GetAccountInfoRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "get_level_info_request") {
                var request = JsonUtility.FromJson<BaseRequest<GetLevelInfoRequest>>(payload_) as BaseRequest<GetLevelInfoRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as GetLevelInfoRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _GetLevelInfoRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_GetLevelInfoRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "create_actor_request") {
                var request = JsonUtility.FromJson<BaseRequest<CreateActorRequest>>(payload_) as BaseRequest<CreateActorRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as CreateActorRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _CreateActorRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_CreateActorRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "actor_level_info_sync_request") {
                var request = JsonUtility.FromJson<BaseRequest<ActorLevelInfoSyncRequest>>(payload_) as BaseRequest<ActorLevelInfoSyncRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as ActorLevelInfoSyncRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _ActorLevelInfoSyncRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_ActorLevelInfoSyncRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "start_level_request") {
                var request = JsonUtility.FromJson<BaseRequest<StartLevelRequest>>(payload_) as BaseRequest<StartLevelRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as StartLevelRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _StartLevelRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_StartLevelRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "leave_level_request") {
                var request = JsonUtility.FromJson<BaseRequest<LeaveLevelRequest>>(payload_) as BaseRequest<LeaveLevelRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as LeaveLevelRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _LeaveLevelRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_LeaveLevelRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "spawn_item_request") {
                var request = JsonUtility.FromJson<BaseRequest<SpawnItemRequest>>(payload_) as BaseRequest<SpawnItemRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as SpawnItemRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _SpawnItemRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_SpawnItemRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "level0_bot_killed_request") {
                var request = JsonUtility.FromJson<BaseRequest<Level0BotKilledRequest>>(payload_) as BaseRequest<Level0BotKilledRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as Level0BotKilledRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _Level0BotKilledRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_Level0BotKilledRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "update_actor_hp_request") {
                var request = JsonUtility.FromJson<BaseRequest<UpdateActorHpRequest>>(payload_) as BaseRequest<UpdateActorHpRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as UpdateActorHpRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _UpdateActorHpRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_UpdateActorHpRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "bot_transform_sync_request") {
                var request = JsonUtility.FromJson<BaseRequest<BotTransformSyncRequest>>(payload_) as BaseRequest<BotTransformSyncRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as BotTransformSyncRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _BotTransformSyncRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_BotTransformSyncRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "login_request") {
                var request = JsonUtility.FromJson<BaseRequest<LoginRequest>>(payload_) as BaseRequest<LoginRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as LoginRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _LoginRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_LoginRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "bot_explose_request") {
                var request = JsonUtility.FromJson<BaseRequest<BotExploseRequest>>(payload_) as BaseRequest<BotExploseRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as BotExploseRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _BotExploseRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_BotExploseRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "enter_level_request") {
                var request = JsonUtility.FromJson<BaseRequest<EnterLevelRequest>>(payload_) as BaseRequest<EnterLevelRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as EnterLevelRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _EnterLevelRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_EnterLevelRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "tower_hp_sync_request") {
                var request = JsonUtility.FromJson<BaseRequest<TowerHpSyncRequest>>(payload_) as BaseRequest<TowerHpSyncRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as TowerHpSyncRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _TowerHpSyncRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_TowerHpSyncRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "finish_level_request") {
                var request = JsonUtility.FromJson<BaseRequest<FinishLevelRequest>>(payload_) as BaseRequest<FinishLevelRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as FinishLevelRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _FinishLevelRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_FinishLevelRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "spawn_bot_request") {
                var request = JsonUtility.FromJson<BaseRequest<SpawnBotRequest>>(payload_) as BaseRequest<SpawnBotRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as SpawnBotRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _SpawnBotRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_SpawnBotRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            if (handler_ == "get_actor_info_request") {
                var request = JsonUtility.FromJson<BaseRequest<GetActorInfoRequest>>(payload_) as BaseRequest<GetActorInfoRequest>;
                var requestId = request.request_id;
                var requireResponse = request.require_response;
                var ret = h_.RecvMessage(request.request) as GetActorInfoRequestResponse;
                if (requireResponse) {
                    var xxx_response = new _GetActorInfoRequestResponse() {
                        handler = string.Format("{0}_response", handler_),
                        type = "response",
                        request_id = requestId,
                        response = ret,
                    };
                    sockUtil_.SendMessage<_GetActorInfoRequestResponse>(xxx_response, xxx_response.handler);
                }
                return true;
            }

            return false;
        }
    }
}
