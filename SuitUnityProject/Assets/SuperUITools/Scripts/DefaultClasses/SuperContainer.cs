using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//when creating custom SuperContainers, extend SuperContainer and not SuperContainerBase
//descendants of SuperContainerBase are assumed to be controls and not true containers
public class SuperContainer : SuperContainerBase 
{
    //TODO: figure out a cleaner way to do this
    [HideInInspector]
    public bool flattenMe = false;
}
