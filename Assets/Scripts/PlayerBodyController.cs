using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyController : MonoBehaviour
{
    private GameManager gameManager;
    private void Awake() => gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Environment" ||
            collision.gameObject.tag == "DeathCollider"  ||
            collision.gameObject.tag == "Barrel")
        {
            gameManager.OnPlayerDie();
        }
    }

}
