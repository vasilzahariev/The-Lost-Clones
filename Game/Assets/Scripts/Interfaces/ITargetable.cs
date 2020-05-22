using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This interface is for all objects / enemies that can be targeted
/// </summary>
public interface ITargetable
{
    Transform GetLookAt(); // This gets the position and rotation that the camera and the player have to look at
}
