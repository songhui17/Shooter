public class Blackboard {
    // TODO: bad code
    public bool LastLoadingDone = true;

    private static Blackboard _instance;
    public static Blackboard Instance {
        get { return _instance ?? (_instance = new Blackboard()); }
    }
}
