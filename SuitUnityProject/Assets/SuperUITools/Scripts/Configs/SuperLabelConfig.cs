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

    public static void ProcessNode(SuperMetaNode root_node, Transform parent, Dictionary<string,object> node)
    {
        ProcessNode(root_node, parent, node, null);
    }

    public static void ProcessNode(SuperMetaNode root_node, Transform parent, Dictionary<string,object> node, GameObject maybe_recycled_node)
    {
        string name = (string)node["name"];
        string label_type = name.Split('_')[0];

        if(labelClasses.ContainsKey(label_type))
        {
            object[] args = new object[4];
            args[0] = root_node;
            args[1] = parent;
            args[2] = node;
            args[3] = maybe_recycled_node;
            labelClasses[label_type].GetMethod("ProcessNode").Invoke(null, args);
            return;
        }

        bool is_paragraph = false;
        if(label_type == "paragraph")
        {
            Debug.Log("GOT PARAGRAPH TEXT: " + name);
            is_paragraph = true;
        }

        GameObject game_object = maybe_recycled_node;
        SuperLabel label = null;
        Text ui_text = null;

        if(game_object == null)
        {
            game_object = new GameObject();
            label = game_object.AddComponent(typeof(SuperLabel)) as SuperLabel;
            ui_text = game_object.AddComponent(typeof(Text)) as Text;
        }else{
            label = game_object.GetComponent<SuperLabel>();
            ui_text = game_object.GetComponent<Text>();
        }
        
        label.CreateRectTransform(game_object, node);

        label.name = name;
        label.hierarchyDescription = "LABEL";
        
        if(is_paragraph)
        {
            ui_text.horizontalOverflow = HorizontalWrapMode.Wrap;
        }else{
            ui_text.horizontalOverflow = HorizontalWrapMode.Overflow;
        }
        ui_text.verticalOverflow = VerticalWrapMode.Overflow;

        string font = (string)node["font"];
        if(SuperLabelConfig.GetFont(font) != null)
        {
            ui_text.font = SuperLabelConfig.GetFont(font);
        }else{
            Debug.Log("[WARNING] SuperLabelConfig not able to find " + font + " -- falling back to Arial");
        }

        //by default text shouldn't gobble clicks -- we typically embed text in other controls
        ui_text.raycastTarget = false;

        string text = (string)node["text"];
        ui_text.text = text;

        int font_size = Convert.ToInt32(node["fontSize"]);
        ui_text.fontSize = font_size;

        string font_color_hex = (string)node["color"];
        ui_text.color = HexToColor(font_color_hex);

        if(node.ContainsKey("justification"))
        {
            RectTransform rect_transform = label.GetComponent<RectTransform>();

            string alignment = (string)node["justification"];
            if(alignment == "center")
            {
                if(is_paragraph)
                {
                    ui_text.alignment = TextAnchor.UpperCenter;    
                }else{
                    ui_text.alignment = TextAnchor.MiddleCenter;
                }
                
            }else if(alignment == "left"){
                if(is_paragraph)
                {
                    ui_text.alignment = TextAnchor.UpperLeft;    
                }else{
                    ui_text.alignment = TextAnchor.MiddleLeft;
                }
                
                rect_transform.pivot = new Vector2(0f , 0.5f);
            }else if(alignment == "right"){
                if(is_paragraph)
                {
                    ui_text.alignment = TextAnchor.MiddleRight;    
                }else{
                    ui_text.alignment = TextAnchor.UpperRight;
                }
                
                rect_transform.pivot = new Vector2(1f , 0.5f);
            }

        }

        label.cachedMetadata = node;
        label.rootNode = root_node;

        root_node.labelReferences.Add(new LabelReference(name, label));

        label.transform.SetParent(parent);
        label.Reset();

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
