using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Debug = UnityEngine.Debug;

public class InGameConsole : MonoBehaviour {
    [SerializeField]
    private InputField _commandInputField;

    private enum OPERATION_MODE {
        Command,
        Normal,
    }

    private GameObject _lastSelectedGameObject;
    private OPERATION_MODE _operationMode = OPERATION_MODE.Normal;
    // private CursorLockMode _lastCursorState = CursorLockMode.None;
    [SerializeField]
    private bool _lastCursorVisible = false;
    private OPERATION_MODE OperationMode {
         get { return _operationMode; }
         set {
             Debug.Log(string.Format("Change _operationMode {0} -> {1}",
                                     _operationMode, value));
             if (_operationMode == value) return;
             _operationMode = value;

             switch (OperationMode){
                 case OPERATION_MODE.Normal:
                     {
                         _commandInputField.text = "";
                         Cursor.visible = _lastCursorVisible;
                         // Cursor.lockState = _lastCursorState;
                         // if (_lastSelectedGameObject != null)
                         EventSystem.current.SetSelectedGameObject(_lastSelectedGameObject);
                         _commandInputField.gameObject.SetActive(false);
                         _lastKeyStroke = "";
                     }
                     break;
                 case OPERATION_MODE.Command:
                     {
                         _lastCursorVisible = Cursor.visible;
                         Cursor.visible = true;
                         // _lastCursorState = Cursor.lockState;
                         // Cursor.lockState = CursorLockMode.Confined;
                         _commandInputField.gameObject.SetActive(true);
                         _lastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
                         EventSystem.current.SetSelectedGameObject(_commandInputField.gameObject);
                         _lastKeyStroke = "";
                     }
                     break;
                 default:
                     throw new Exception("Invalid OperationMode: " + OperationMode);
             }
         }
    }

    public static InGameConsole Instance { get; private set; }

    void Awake() {
        if (Instance != null) {
            enabled = false;
            Destroy(this);
            Debug.LogWarning("InGameConsole Instance is not null: " + InGameConsole.Instance);
            return;
        }
        DontDestroyOnLoad(gameObject);
        InGameConsole.Instance = this;

        //TODO
        InGameConsole.Instance.RegisterCommand("st:e",
            StringTable.Instance.Reload, help_:"Reload StringTable");
    }

    private Dictionary<string, Action> _commandDict = new Dictionary<string, Action>();
    private Dictionary<string, string> _helpDict = new Dictionary<string, string>();
    public void RegisterCommand(string key_, Action handler_, string help_ = "") {
        if (key_ == "" || key_ == null || handler_ == null) {
            throw new ArgumentNullException();
        }else if (_commandDict.ContainsKey(key_)) {
            throw new Exception("Duplicate key_: " + key_);
        }

        _commandDict.Add(key_, handler_);
        _helpDict.Add(key_, help_ ?? "");
    }

    private Dictionary<string, List<string>> _macroDict = new Dictionary<string, List<string>>();
    public void RegisterMacro(string key_, List<string> macro_) {
        if (key_ == "" || key_ == null || macro_ == null || macro_.Count == 0){
            throw new ArgumentNullException();
        }else if (_macroDict.ContainsKey(key_)){
            throw new Exception("Duplicate key_: " + key_);
        }
        _macroDict.Add(key_, macro_);
    }

    void Start(){
        _commandInputField.onEndEdit.AddListener(OnInputFieldEndEdit);
        Cursor.visible = _lastCursorVisible;
        OperationMode = OPERATION_MODE.Normal;
    }
    #region Process
    
    // command := name params
    // name := alphanum
    // params := [param sp]*
    // param := string | float | bool
    // string := " [alphanum]+ "
    // bool := true | false
    // float := \d+ '.' \d+

    private enum TOKEN_TYPE{
        Name,
        String,
        Float,
        End,
        Invalid,
    }
    
    private void SkipWhitespace(string text_, ref int idx_){
        for (; idx_ < text_.Length; idx_++){
            if (text_[idx_] != ' ') break;
        }
    }

    private bool IsStartingCharOfName(char value_)
    {
        return value_ == '_' || Char.IsLetter(value_);
    }

    private bool GetFloat(string text_, ref int idx_, out float value_){
        value_ = 0;
        var savedIndex = idx_;
        try {
            var currentChar = text_[idx_];
            if (!Char.IsDigit(currentChar)
                    && currentChar != '+'
                    && currentChar != '-') {
                return false;
            }

            var startIdx = idx_;
            for(; idx_ < text_.Length; idx_++){
                if (text_[idx_] == ' ') break;
            }
            var endIdx = idx_;

            var str = text_.Substring(startIdx, endIdx - startIdx);
            if (float.TryParse(str, out value_)){
                idx_ = savedIndex;
                return true;
            }else{
                return false;
            }
        } catch (IndexOutOfRangeException) {
            idx_ = savedIndex;
            return false;
        }
    }

    private bool GetName(string text_, ref int idx_, out string value_){
        value_ = "";
        var savedIndex = idx_;
        try{
            var currentChar = text_[idx_];
            if (!IsStartingCharOfName(currentChar)){
                return false;
            }

            var startIdx = idx_;
            for(; idx_ < text_.Length; idx_++){
                if (text_[idx_] == ' ') break;
            }
            var endIdx = idx_;

            value_ = text_.Substring(startIdx, endIdx - startIdx);
            return true;
        }catch(IndexOutOfRangeException){
            idx_ = savedIndex;
            return false;
        }
    }

    private object GetNextToken(string text_, ref int idx_, out TOKEN_TYPE tokenType_){
        if (idx_ >= text_.Length)
        {
            tokenType_ = TOKEN_TYPE.End;
            return null;
        }

        SkipWhitespace(text_, ref idx_);
        tokenType_ = TOKEN_TYPE.Invalid;

        string nameValue;
        if (GetName(text_, ref idx_, out nameValue)){
            tokenType_ = TOKEN_TYPE.Name;
            return nameValue;
        }

        float floatValue;
        if (GetFloat(text_, ref idx_, out floatValue)){
            tokenType_ = TOKEN_TYPE.Float;
            return floatValue;
        }

        SyntaxError(text_, idx_);
        return null;
    }

    private void SyntaxError(string text_, int idx_) {
        var info = (string.Format(
            "Syntax Error\ntext_: {0}\nidx_: {1}\ntext_.Substring: {2}",
            text_, idx_, text_.Substring(idx_)));
        throw new Exception(info);
    }

    private void CreateHelpMessage() {
        var info = "";
        info += "h[elp]\n        :Print this help message.\n\n";
        info += "hvim\n        :Print this help message and open in gvim.\n\n";
        info += "echo\n        :Debug.Log(\"echo\");\n\n";
        info += "[no]cur\n        :Toggle cursor\n\n";
        foreach (var key_ in _commandDict.Keys) {
            info += string.Format("{0}\n        :{1}\n\n", key_, _helpDict[key_]);
        }
        Debug.Log(info);
        TextWriter writer = null;
        try {
            Debug.Log("Write help message to help.txt");
            writer = new StreamWriter("help.txt");
            writer.Write(info);
        }catch{
            Debug.Log("Failed to write help message to help.txt");
        }
        if (writer != null){
            writer.Close();
        }
    }

    private void Process(string command_) { 
        var text = command_;
        var idx = 0;
        
        TOKEN_TYPE tokenType;
        object result;
        result = GetNextToken(text, ref idx, out tokenType);
        _lastCommand = command_;
        if (tokenType == TOKEN_TYPE.Name) {
            var method = result  as string;
            // assert method != null || method != ""
            switch (method){
                case "h":
                case "help":
                    {
                        CreateHelpMessage();
                    }
                    break;
                case "hvim":
                    {
                        CreateHelpMessage();
                        try {
                            var process = new Process();
                            var startInfo = process.StartInfo;
                            startInfo.CreateNoWindow = true;
                            startInfo.FileName = "gvim";
                            startInfo.Arguments = "--remote-silent help.txt";
                            process.Start();
                        }catch(Exception e){
                            Debug.LogError(e);
                        }
                    }
                    break;
                case "echo":
                    {
                        Debug.Log("echo");
                    }
                    break;
                case "nocur":
                    {
                        _lastCursorVisible = false;
                    }
                    break;
                case "cur":
                    {
                        _lastCursorVisible = true;
                    }
                    break;
                default:
                    {
                        ProcessDefault(method);
                    }
                    break;
            }
        }else{
            SyntaxError(text, idx);
        }
    }

    void ProcessDefault(string method) {
        if (_commandDict.ContainsKey(method)) {
            _commandDict[method]();
        }else if (_macroDict.ContainsKey(method)) {
            var commandList = _macroDict[method];
            foreach (var command2_ in commandList) {
                try {
                    Process(command2_);
                }catch (Exception e){
                    Debug.LogWarning(e);
                }
            }
        }else {
            Debug.Log(string.Format("Method: {0} is not implemented", method));
        }
    }
    #endregion 


    #region MonoBehaviour

    private string _lastCommand = "";
    private string _lastKeyStroke = "";
    private string _lastCharacter = "";
    public void OnInputFieldEndEdit(string value_)
    {
        Debug.Log("OnInputFieldEndEdit: " + value_);
        _lastKeyStroke = value_;
        try{
            Process(_lastKeyStroke);
        }catch(Exception e){
            Debug.Log(e);
        }

        OperationMode = OPERATION_MODE.Normal;
    }

    private float _lastDotProcessTime = 0;
    private float _processDuration = 0.1f;
    void UpdateNormal() {
        if (Input.GetKeyDown(KeyCode.Semicolon)) {
            OperationMode = OPERATION_MODE.Command;
            return;
        }
        
        if (Input.GetKey(KeyCode.Period)
                && Time.realtimeSinceStartup - _lastDotProcessTime > _processDuration){
            // Debug.Log(_lastCommand);
            _lastDotProcessTime = Time.realtimeSinceStartup;
            try{
                Process(_lastCommand);
            }catch(Exception e){
                Debug.Log(e);
            }
        }
    }

    void UpdateCommand() {
        if (Input.GetKeyDown(KeyCode.Q)){
            OperationMode = OPERATION_MODE.Normal;
            return;
        }
    }

    void Update() {
        switch (OperationMode){
            case OPERATION_MODE.Normal:
                UpdateNormal();
                break;
            case OPERATION_MODE.Command:
                UpdateCommand();
                break;
            default:
                throw new Exception("Invalid OperationMode: " + OperationMode);
        }
    }
    #endregion
}
