using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class StringTable
{
    #region Fields

    string _file = "Config/StringTable.csv";


    Dictionary<string, string> _dict;

    #endregion

    #region Properties

    private static StringTable _instance;
    public static StringTable Instance {
        get { return _instance ?? (_instance = new StringTable()); }
    }

    public static string Value(string key_, object content_ = null) {
        return Instance.Get(key_, content_);
    }

    #endregion

    #region Constructors

    StringTable() {
        Reload();
    }

    public void Reload() {
        _dict = new Dictionary<string, string>();
        if (!File.Exists(_file)) {
            Debug.LogWarning(string.Format("StringTable _file:{0} doesnt exists", _file));
            return;
        }

        var reader = new StreamReader(_file);

        string line = null;
        int lineNumber = 0;
        while ((line = reader.ReadLine()) != null) {
            lineNumber++;
            if (line.StartsWith("#")) continue;

            var index = line.IndexOf(':');
            if (index < 1) continue;

            var key = line.Substring(0, index).Trim();
            var value = line.Substring(index + 1);

            if (!_dict.ContainsKey(key))
            {
                _dict.Add(key, value);
            }
            else
            {
                Debug.Log(string.Format("duplicate key {0} ({1})", line, lineNumber));
            }
        }
        reader.Close();
    }

    #endregion

    #region Methods

    public string this[string key_] {
        get {
            if (_dict.ContainsKey(key_)) {
                return _dict[key_];
            } else {
                return key_;
            }
        }
    }

    public string Get(string key_, object content_) {
        string value = this[key_];
        return Translate(value, content_);
    }

    private string Translate(string str_, object content_) {
        int startIdx = 0;

        string resultString = "";

        while (startIdx < str_.Length)
        {
            var idx = str_.IndexOf("{$", startIdx);

            if (idx > 0)
            {
                resultString += str_.Substring(startIdx, idx - startIdx);

                var closingIdx = str_.IndexOf("}", idx);

                if (closingIdx > 0)
                {
                    string path = str_.Substring(idx + 2, closingIdx - (idx + 2));
                    try
                    {
                        var tokens = path.Split('.');
                        object content = content_;
                        foreach (var token in tokens)
                        {
                            var type = content.GetType();
                            var propInfo = type.GetProperty(token);
                            content = propInfo.GetValue(content, null);
                        }
                        resultString += content.ToString();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("Failed to resolve path: " + path + "\n" + ex);
                    }

                    finally
                    {
                        startIdx = closingIdx + 1;
                    }
                }
                else
                {
                    resultString += str_.Substring(idx, 2);
                    startIdx = idx + 2;
                }
            }
            else
            {
                resultString += str_.Substring(startIdx);
                break;
            }
        }

        return resultString;
    }

    #endregion
}

