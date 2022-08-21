using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;
using System.Reflection;
using UnityEngine.SceneManagement;

public class Test_UIManager
{
    UIManager uiManager;
    Dictionary<string, MethodInfo> methodsDic = new Dictionary<string, MethodInfo>();
    
    [SetUp]
    public void SetUp()
    {
        //SceneManager.LoadScene("MainScene");
        uiManager = GameObject.Find("Managers").GetComponent<UIManager>();
        var Methods = uiManager.GetType().GetMethods(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        methodsDic.Clear();
        foreach(var method in Methods)
            if(!methodsDic.ContainsKey(method.Name))
                methodsDic.Add(method.Name, method);
        var Fields = uiManager.GetType().GetFields(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
        Assert.IsNotNull(uiManager);
        Assert.IsNotEmpty(methodsDic);
    }

    [Test]
    public void Test_FindObjects()
    {
        foreach(var method in methodsDic)
            if (method.Key == "SetObjects")
                method.Value.Invoke(uiManager, null);

        Assert.IsNotNull(TestUtils.GetField(uiManager, "mainCanvas"));
        Assert.IsNotNull(TestUtils.GetField(uiManager, "actionManager"));
        Assert.IsNotEmpty(TestUtils.GetField(uiManager, "uiPanels") as List<UIPanels>);
        Assert.IsNotEmpty(TestUtils.GetField(uiManager, "btnList") as List<SelfManageButton>);
    }

    [Test]
    public void Test_LinkBtnPnls()
    {
        foreach(var method in methodsDic)
            if(method.Key == "LinkBtnPnls")
                method.Value.Invoke(uiManager, null);
        var btnDict = TestUtils.GetField(uiManager, "btnDict") as SerializableDictionary<SelfManageButton, UIPanels>;
    }
}
