using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     TriggerableGameObject:
///         embeded GameObject with a bool representing a triggered value
/// </summary>
public class TriggerableGameObject : IComparable<GameObject>
{

    public GameObject gameObject;
    public bool trigerred;

    public TriggerableGameObject(GameObject NewGameObject, bool Trigerred = false)
    {
        gameObject = NewGameObject;
        trigerred = Trigerred;
    }

    public int CompareTo(GameObject other)
    {
        if (other == null) return -1;
        return (other.name == gameObject.name) ? 1 : 0;
    }
}