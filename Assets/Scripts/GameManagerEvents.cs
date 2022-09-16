using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     GameManager :
///         collisions behaviors from Player character are defined here
/// </summary>
public partial class GameManager : MonoBehaviour
{
    // pick coin : get index and trigger it
    // @see TriggerableGameObject
    public void OnPickCoin(string id)
    {
        int i = getId(coins, id);
        if (i > -1)
        {
            Score++;
            coins[i].trigerred = true;
            coins[i].gameObject.GetComponent<SphereCollider>().enabled = false;
        }
    }

    // pick bullet : get an ammo with direct removal
    public void OnPickBullet(string id)
    {
        int i = getId(bullets, id);
        if (i > -1)
        {
            HaveAmmo = true;
            bullets[i].gameObject.GetComponent<SphereCollider>().enabled = false;
            Destroy(bullets[i].gameObject, 0.1f);
            bullets.RemoveAt(i);
        } 
    }


    // trigger chest
    public void OnPickChest(string id)
    {
    }

    // body part hitted enviromnent
    public void OnPlayerDie()
    {
        PlayerExplode();
    }

    public void OnPlayerDrawn()
    {
        // avoid multiple water splashes
        if (!playerDrawn)
        {
            GameObject go = Instantiate(FXWaterSplash, Player.transform.position, new Quaternion(-90, 0, 0, -90));
            playerDrawn = true;
        }
    }

    // reached bottom map 
    public void OnPlayerOut()
    {
        PlayerAlive = false;
        Player.SetActive(false);
    }

    // player touchs ground
    public void OnPlayerGrounded(float angle)
    {
        PlayerGrounded = true;
        groundedTime = 0;
        lastContactAngle = angle;
        playerRigidBody.AddRelativeForce(Vector3.back, ForceMode.VelocityChange);
    }

    // Jumping action : add angular velocity and up force
    public void OnPlayerJump()
    {
        Player.transform.Rotate(new Vector3(360 - (lastContactAngle + Player.transform.rotation.x), 0, 0));
        playerRigidBody.angularVelocity = Vector3.left * angularForce;
        playerRigidBody.AddRelativeForce(Vector3.up * jumpForce, ForceMode.Impulse);

        groundedTime = 0;
        PlayerGrounded = false;
    }

    // Firing action : add back force (recoil) use amo and trigger particles
    public void OnPlayerFire()
    {
        GameObject go = Instantiate(ProjectilePrefab, Player.transform.position, Player.transform.rotation);
        go.GetComponent<Rigidbody>().AddRelativeForce(Vector3.back * projectileVelocity, ForceMode.Impulse);
        Destroy(go, 2f);

        HaveAmmo = false;
        CanonParticle.Play();
        playerRigidBody.AddRelativeForce(Vector3.forward * recoilForce, ForceMode.Impulse);
    }

    public void BarrelExplosion(GameObject barrel, bool playerHit=false)
    {
        GameObject go = Instantiate(FXFireExplosion, barrel.transform.position, new Quaternion(-90, 0, 0, -90));
        Destroy(barrel);

        if(playerHit) PlayerExplode();
    }

    public void OnVictory(int n)
    {
        Time.timeScale = 0.7f;
        Destroy(Chest);
        PlayerWon = true;
        for(var i =0; i < n; i++)
        {
            GameObject go = Instantiate(CoinsPrefab, Player.transform.position, Random.rotation);
            go.AddComponent(typeof(Rigidbody));
            go.AddComponent(typeof(SphereCollider));
            go.GetComponent<Rigidbody>().AddForce(new Vector3(0, playerExplosionForce, 0), ForceMode.Impulse);
            Destroy(go, 2f);
        }
       
    }

    // hide player character and instantiate GameObjects parts randomly oriented
    public void PlayerExplode()
    {
        if (PlayerAlive && !PlayerWon)
        {
            Player.SetActive(false);
            PlayerAlive = false;
            HaveAmmo = false;
            PlayerGrounded = false;
            foreach (GameObject part in playerExplosionParts)
            {
                GameObject go = Instantiate(part, Player.transform.position, Random.rotation);
                go.AddComponent(typeof(Rigidbody));
                go.AddComponent(typeof(SphereCollider));
                go.GetComponent<Rigidbody>().AddForce(new Vector3(0, playerExplosionForce, 0), ForceMode.Impulse);
                Destroy(go, 2f);
            }
        }
    }
}
