using UnityEngine;
using System.Collections.Generic;

public class Chapter : DataModelBase {
    private string _id;
    public string ID {
        get { return _id ?? (_id = ""); }
        set { _id = value; OnPropertyChanged("ID"); }
    }

    private string _title;
    public string Title {
        get { return _title ?? (_title = ""); }
        set { _title = value; OnPropertyChanged("Title"); }
    }

    private List<Level> _levelList;
    public List<Level> LevelList {
        get { return _levelList ?? (_levelList = new List<Level>()); }
        set { _levelList = value; OnPropertyChanged("LevelList");
        }
    }
}

