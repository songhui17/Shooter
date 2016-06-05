
using System;
using Shooter;

public partial class SockUtil {
    public static int Level0BotKilled(Level0BotKilledRequest request_, Action<Level0BotKilledRequestResponse, int> callback_ = null) {
        return SockUtil.Instance.SendRequest<Level0BotKilledRequest, Level0BotKilledRequestResponse>("level0_bot_killed", request_, callback_);
    }

    public static int SpawnBot(SpawnBotRequest request_, Action<SpawnBotRequestResponse, int> callback_ = null) {
        return SockUtil.Instance.SendRequest<SpawnBotRequest, SpawnBotRequestResponse>("spawn_bot", request_, callback_);
    }

    public static int GetActorLevelInfo(GetActorLevelInfoRequest request_, Action<GetActorLevelInfoRequestResponse, int> callback_ = null) {
        return SockUtil.Instance.SendRequest<GetActorLevelInfoRequest, GetActorLevelInfoRequestResponse>("get_actor_level_info", request_, callback_);
    }

    public static int Login(LoginRequest request_, Action<LoginRequestResponse, int> callback_ = null) {
        return SockUtil.Instance.SendRequest<LoginRequest, LoginRequestResponse>("login", request_, callback_);
    }

    public static int EnterLevel(EnterLevelRequest request_, Action<EnterLevelRequestResponse, int> callback_ = null) {
        return SockUtil.Instance.SendRequest<EnterLevelRequest, EnterLevelRequestResponse>("enter_level", request_, callback_);
    }

    public static int FinishLevel(FinishLevelRequest request_, Action<FinishLevelRequestResponse, int> callback_ = null) {
        return SockUtil.Instance.SendRequest<FinishLevelRequest, FinishLevelRequestResponse>("finish_level", request_, callback_);
    }

    public static int GetAccountInfo(GetAccountInfoRequest request_, Action<GetAccountInfoRequestResponse, int> callback_ = null) {
        return SockUtil.Instance.SendRequest<GetAccountInfoRequest, GetAccountInfoRequestResponse>("get_account_info", request_, callback_);
    }

    public static int GetLevelInfo(GetLevelInfoRequest request_, Action<GetLevelInfoRequestResponse, int> callback_ = null) {
        return SockUtil.Instance.SendRequest<GetLevelInfoRequest, GetLevelInfoRequestResponse>("get_level_info", request_, callback_);
    }

    public static int CreateActor(CreateActorRequest request_, Action<CreateActorRequestResponse, int> callback_ = null) {
        return SockUtil.Instance.SendRequest<CreateActorRequest, CreateActorRequestResponse>("create_actor", request_, callback_);
    }

    public static int LeaveLevel(LeaveLevelRequest request_, Action<LeaveLevelRequestResponse, int> callback_ = null) {
        return SockUtil.Instance.SendRequest<LeaveLevelRequest, LeaveLevelRequestResponse>("leave_level", request_, callback_);
    }

    public static int GetActorInfo(GetActorInfoRequest request_, Action<GetActorInfoRequestResponse, int> callback_ = null) {
        return SockUtil.Instance.SendRequest<GetActorInfoRequest, GetActorInfoRequestResponse>("get_actor_info", request_, callback_);
    }

    public static int StartLevel(StartLevelRequest request_, Action<StartLevelRequestResponse, int> callback_ = null) {
        return SockUtil.Instance.SendRequest<StartLevelRequest, StartLevelRequestResponse>("start_level", request_, callback_);
    }

}
