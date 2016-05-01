using UnityEngine;
using System.Collections.Generic;

public class Level : DataModelBase {
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

    private string _description;
    public string Description {
        get { return _description ?? (_description = ""); }
        set { _description = value; OnPropertyChanged("Description"); }
    }

    private List<string> _taskDescriptionList;
    public List<string> TaskDescriptionList {
        get { return _taskDescriptionList ?? (_taskDescriptionList = new List<string>()); }
        set {
            _taskDescriptionList = value;
            OnPropertyChanged("TaskDescriptionList");
        }
    }

    private string _scene;
    public string Scene {
        get { return _scene ?? (_scene = ""); }
        set { _scene = value; OnPropertyChanged("Scene"); }
    }

    // TODO:
    public Chapter Chapter;

    public override string ToString(){
        var info = "";
        info += "ID: " + ID + "\n";
        info += "Title: " + Title + "\n";
        info += "Description: " + Description + "\n";
        info += "TaskDescriptionList.Count: " + TaskDescriptionList.Count + "\n";
        info += "Scene: " + Scene + "\n";
        info += "Chapter.ID: " + Chapter.ID + "\n";
        return info;
    }
}
