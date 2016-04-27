using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour {
    [SerializeField]
    private int _hp = 3;
    public int HP { 
        get { return _hp; }
        protected set { _hp = value; }
    }

    public bool IsAlive { get { return HP > 0; } }
}
