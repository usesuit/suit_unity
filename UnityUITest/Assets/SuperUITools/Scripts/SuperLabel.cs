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
    
}