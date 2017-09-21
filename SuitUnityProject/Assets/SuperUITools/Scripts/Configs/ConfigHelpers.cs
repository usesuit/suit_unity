using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CustomClass
{
    public string name;
    public string prefix;
    public string scriptName;

    public CustomClass(string name, string prefix, string scriptName)
    {
    	this.name = name;
    	this.prefix = prefix;
    	this.scriptName = scriptName;
    }
}

[System.Serializable]
public class SuperFont
{
    public string name;
    public Font font;
}