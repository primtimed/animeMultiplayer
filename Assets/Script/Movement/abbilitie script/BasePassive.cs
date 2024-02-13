using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Passive", menuName = "Passive")]
public class BasePassive : ScriptableObject
{
    public enum Passive
    {
        None,
        WallRun,
        DubbleJump
    }

    public Passive _passive;

    public void Open(GameObject obj)
    {
        //obj.GetComponent<Movement>()._passive = _passive;
    }
}
