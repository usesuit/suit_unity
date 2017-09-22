using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
    At some point labels are probably going to be a performance bottleneck.
    TODO: port over Futile's FFont and FLabel to allow hand-tailored bitmap fonts
    and then either adapt the source Text or build a custom Graphic to support
    bitmap fonts (probably with way less bells & whistles)
 */
 /*
    Showerthought: one possible intermediate to full on bitmap fonts would be to
    add a flag to DAMetaNode for moving all labels to top. When moving a label 
    to the top, replace it with a tracking GameObject and every frame synch
    the label's position/rotation/scale to the tracking object in the event
    that the container with the tracking object in it has moved.
  */
public class SuperLabel : SuperNode 
{
    //if we match bounds exactly the text doesn't render... so lets double it!
    public const float TEXT_VERTICAL_PADDING = 2f;
    override public void CreateRectTransform(GameObject game_object, Dictionary<string,object> node)
    {
        base.CreateRectTransform(game_object, node);
        
        RectTransform rect_transform = game_object.GetComponent<RectTransform>();

        List<object> size = node["size"] as List<object>;
        float w = Convert.ToSingle(size[0]);
        float h = Convert.ToSingle(size[1]);

        rect_transform.sizeDelta = new Vector2(w, h*TEXT_VERTICAL_PADDING);
    }
}