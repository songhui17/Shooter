using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Shooter;

public class TestLoginRequest {

    [Test]
    public void EditorTest()
    {
        //Arrange
        var gameObject = new GameObject();

        //Act
        //Try to rename the GameObject
        var newGameObjectName = "My game object";
        gameObject.name = newGameObjectName;

        //Assert
        //The object has a new name
        Assert.AreEqual(newGameObjectName, gameObject.name);
    }


    [Test]
    public void UsageTest() {
        var loginRequest = new BaseRequest<LoginRequest>(){
             type = "",
             request_id = 1,
             handler = "",
             request = new LoginRequest() {
                 username = "\u59d3\u540d", // "主宰",
                 password = "abc",
             }
        };
        var info = "UsageTest\n";
        var text = JsonUtility.ToJson(loginRequest);
        info += "1) ecoded:\n" + text + "\n";
        var decoded = JsonUtility.FromJson(text, typeof(BaseRequest<LoginRequest>))
            as BaseRequest<LoginRequest>;
        text = JsonUtility.ToJson(decoded);
        Debug.Log(info);
        info += "2) ecoded:\n" + text + "\n";
        Assert.AreEqual(decoded.request.username, loginRequest.request.username);
        Assert.AreEqual(decoded.request.password, loginRequest.request.password);
        Assert.AreEqual(decoded, loginRequest);
    }
}
