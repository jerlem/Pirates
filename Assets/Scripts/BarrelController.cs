using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelController : MonoBehaviour
{
    private GameManager gameManager;
    private void Awake() => gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullets" || other.gameObject.tag == "Player") {
            Instantiate(gameManager.FXFireExplosion, other.gameObject.transform.position, new Quaternion(-90, 0, 0, -90));
            Destroy(other.gameObject);
        }
    }
}
