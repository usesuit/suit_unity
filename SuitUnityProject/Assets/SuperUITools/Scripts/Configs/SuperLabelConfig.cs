using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class SuperLabelConfig : MonoBehaviour
{
    public SuperFont[] localFonts;
    
    //a container can either be a true container or a proxy for a UI class (most commonly buttons)
    public CustomClass[] customLabels;

    private static SuperLabelConfig _instance = null;
    public static Dictionary<string, Font> fonts;
    public static Dictionary<string, Type> labelClasses;
    
    public static SuperLabelConfig instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject config_go = GameObject.Find("SuperConfig");
                if(config_go == null)
                {
                    config_go = new GameObject();
                    config_go.name = "SuperConfig";
                }

                SuperLabelConfig label_config = config_go.GetComponent<SuperLabelConfig>();
                if(label_config == null)
                {
                    label_config = config_go.AddComponent(typeof(SuperLabelConfig)) as SuperLabelConfig;
                    label_config.customLabels = new CustomClass[0];
                    label_config.localFonts = new SuperFont[0];
                }

                _instance = label_config;
            }

            return _instance;
        }
    }

    //if we match bounds exactly the text doesn't render
    public const float TEXT_VERTICAL_PADDING = 2f;
    public static SuperNode ProcessNode(SuperMetaNode root_node, Transform parent, Dictionary<string,object> node)
    {
        GameObject game_object = new GameObject();
        RectTransform rect_transform = game_object.AddComponent(typeof(RectTransform)) as RectTransform;
        SuperLabel super_label = game_object.AddComponent(typeof(SuperLabel)) as SuperLabel;
        Text label = game_object.AddComponent(typeof(Text)) as Text;

        string name = (string)node["name"];
        game_object.name = name;

        List<object> position = node["position"] as List<object>;
        float x = Convert.ToSingle(position[0]);
        float y = Convert.ToSingle(position[1]);

        List<object> size = node["size"] as List<object>;
        float w = Convert.ToSingle(size[0]);
        float h = Convert.ToSingle(size[1]);

        rect_transform.position = new Vector2(x, y);
        rect_transform.sizeDelta = new Vector2(w, h * TEXT_VERTICAL_PADDING);

        super_label.resetX = x;
        super_label.resetY = y;

        label.horizontalOverflow = HorizontalWrapMode.Overflow;

        string font = (string)node["font"];
        if(SuperLabelConfig.GetFont(font) != null)
        {
            label.font = SuperLabelConfig.GetFont(font);
        }else{
            Debug.Log("[WARNING] SuperLabelConfig not able to find " + font + " -- falling back to Arial");
        }

        string text = (string)node["text"];
        label.text = text;

        int font_size = Convert.ToInt32(node["fontSize"]);
        label.fontSize = font_size;

        string font_color_hex = (string)node["color"];
        label.color = HexToColor(font_color_hex);

        if(node.ContainsKey("justification"))
        {
            string alignment = (string)node["justification"];
            if(alignment == "center")
            {
                label.alignment = TextAnchor.MiddleCenter;
            }else if(alignment == "left"){
                label.alignment = TextAnchor.MiddleLeft;
                rect_transform.pivot = new Vector2(0f , 0.5f);
                
                //no reset adjustment needed. setting us to our old position will move us left w/2
            }else if(alignment == "right"){
                label.alignment = TextAnchor.MiddleRight;
                rect_transform.pivot = new Vector2(1f , 0.5f);
                
                //moving the pivot effectively translates us w/2, so we need to move a full with
                super_label.resetX = x + 2;
            }

        }

        super_label.cachedMetadata = node;
        super_label.rootNode = root_node;

        root_node.labels[name] = super_label;

        super_label.transform.SetParent(parent);
        super_label.Reset();

        return super_label;
    }

    public static void RefreshAll()
    {
        RefreshClasses();
        RefreshFonts();
    }

    public static void RefreshClasses()
    {
        labelClasses = new Dictionary<string, Type>();
        foreach(CustomClass custom_label in instance.customLabels)
        {
            Type label_class = Type.GetType(custom_label.scriptName);
            if(label_class == null)
            {
                Debug.Log("[ERROR] " + custom_label.scriptName + " COULD NOT BE FOUND");
                continue;
            }
            labelClasses[custom_label.prefix] = label_class;
        }
    }

    public static void RefreshFonts()
    {
        fonts = new Dictionary<string, Font>();
        foreach(SuperFont entry in instance.localFonts)
        {
            fonts[entry.name] = entry.font;
        }
    }

    public static Font GetFont(string name)
    {
        //todo: should I just refresh this every time? or hook into editor modified events to refresh?
        if(fonts == null)
        {
            RefreshFonts();
        }

        if(fonts.ContainsKey(name))
        {
            return fonts[name];
        }

        return null;
    }

    public static string ColorToHex(Color color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    public static Color HexToColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r,g,b, 255);
    }
}
