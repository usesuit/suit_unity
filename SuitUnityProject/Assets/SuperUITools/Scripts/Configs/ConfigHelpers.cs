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



[System.Serializable]
public class ContainerReference
{
    public string name;
    public SuperContainer container;

    public ContainerReference(string name, SuperContainer container)
    {
        this.name = name;
        this.container = container;
    }
}

[System.Serializable]
public class LabelReference
{
    public string name;
    public SuperLabel label;

    public LabelReference(string name, SuperLabel label)
    {
        this.name = name;
        this.label = label;
    }
}

[System.Serializable]
public class SpriteReference
{
    public string name;
    public SuperSprite sprite;

    public SpriteReference(string name, SuperSprite sprite)
    {
        this.name = name;
        this.sprite = sprite;
    }
}

[System.Serializable]
public class PlaceholderReference
{
    public string name;
    public Rect rect;

    public PlaceholderReference(string name, Rect rect)
    {
        this.name = name;
        this.rect = rect;
    }
}

[System.Serializable]
public class ButtonReference
{
    public string name;
    public SuperButtonBase button;

    public ButtonReference(string name, SuperButtonBase button)
    {
        this.name = name;
        this.button = button;
    }
}

[System.Serializable]
public class ControlReference
{
    public string name;
    public SuperNode control;

    public ControlReference(string name, SuperNode control)
    {
        this.name = name;
        this.control = control;
    }
}