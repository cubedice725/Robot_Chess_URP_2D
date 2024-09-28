using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.VisionOS;
using UnityEngine;

public interface IState
{
    public void Entry();
    public void IStateUpdate();
    public void Exit();
}
