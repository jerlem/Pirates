using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    public GameObject Rail;

    private void Update()
    {
        Rail.gameObject.transform.Rotate(0, -30 * Time.deltaTime, 0, Space.Self);
    }

}
