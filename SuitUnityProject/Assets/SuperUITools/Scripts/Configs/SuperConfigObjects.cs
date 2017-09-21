using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DefaultClass
{
    public string name;
    public string scriptName;

    public DefaultClass(string name, string scriptName)
    {
        this.name = name;
        this.scriptName = scriptName;
    }
}

[System.Serializable]
public class CustomClass
{
    public string name;
    public string prefix;
    public string scriptName;
}