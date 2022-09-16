using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanonController : MonoBehaviour
{
    private GameManager gameManager;
    private void Awake() => gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Coins":
                gameManager.OnPickCoin(other.gameObject.name);
                break;
            case "Bullets":
                gameManager.OnPickBullet(other.gameObject.name);
                break;
            case "Chest":
                gameManager.OnVictory(gameManager.VictoryCoins);
                break;
            case "Water":
                gameManager.OnPlayerDrawn();
                break;
            case "Bottom":
                gameManager.OnPlayerOut();
                break;
            case "Environment":
                gameManager.OnPlayerGrounded(other.gameObject.transform.rotation.x);
                break;
            case "Barrel":
                gameManager.BarrelExplosion(other.gameObject, true);
                break;
        }
    }
}
