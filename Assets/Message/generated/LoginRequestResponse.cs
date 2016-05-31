namespace Shooter {
[System.Serializable]
public class LoginRequestResponse
{
    public bool result;
    public int errno;
    public override string ToString()
    {
        var info = "";
        info += string.Format("result: {0}\n", result);
        info += string.Format("errno: {0}\n", errno);
        return info;
    }
}
}