namespace Shooter
{
    public enum ENUM_SHOOTER_ERROR {
        E_OK = 0,              // success
        E_NO_USERNAME = 1,              // no account with name username
        E_INVALID_PASSWORD = 2,              // invalid password
        E_USER_NOT_LOGINED = 3,              // user has not logined, login required
        E_ACTOR_EXIST = 4,              // actor already bound to account
        E_ACTOR_NOT_CREATED = 5,              // actor_id == -1, no actor bound to account
        E_NO_SUCH_LEVEL = 6,              // invalid level id
        E_LEVEL_NOT_ALLOWED = 7,              // invalid level id
    }
}
