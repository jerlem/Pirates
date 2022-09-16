using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameManager gameManager;
    private void Awake() => gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Barrel")
        {
            Destroy(other.gameObject);
        }
    }
}
