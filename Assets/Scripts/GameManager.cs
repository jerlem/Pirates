using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     GameManager :
///         Main GameManager declaration 
/// </summary>
public partial class GameManager : MonoBehaviour
{
    #region Game Settings 
    [Header("Game Settings")]
    public int updateFrameRate = 5;         // Only update each N frames
    public float coinsRotationSpeed = 600f;  // rotation speed * frame rate 
    public float smoothTime = 0.3f;          // camera follow smoothness

    public float jumpForce = 10f;            // player rotation while in air
    public float angularForce = 5f;          // player rolling velocity
    public float projectileVelocity = 10f;   // Canon bullets speed
    public float recoilForce = 5f;           // recoil when firing
    public float fallingAngle = 60f;         // max character angle before Falling
    public float fallingTimerTreshold = 0.2f;// delay before triggering OnFall
    public float playerExplosionForce = 7f;  // velocity for player explosion parts 
    public int VictoryCoins = 5;             // granted coins once level finished
    #endregion

    #region User Interface

    // UI
    [Header("User Interface")]
    public Image UI_HaveAmmo;
    public Image UI_PlayerGrounded;
    public Text UI_Score;
    public Button UI_Retry;
    public Button UI_Victory;

    // Ammo property linked to UI_HaveAmmo
    private bool _haveAmmo = false;
    public bool HaveAmmo
    {
        get => _haveAmmo;
        set
        {
            _haveAmmo = value;
            UI_HaveAmmo.enabled = value;
        }
    }

    // PlayerGrounded property linked to UI_PlayerGrounded
    private bool _playerGrounded = false;
    public bool PlayerGrounded
    {
        get => _playerGrounded;
        set
        {
            _playerGrounded = value;
            UI_PlayerGrounded.enabled = value;
        }
    }

    // Golds (score) linked to UI_Score
    private int _score = 0; // TODO serialize
    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            UI_Score.text = _score.ToString();
        }
    }

    // check victory and if player has died 
    // to show up UI_Retry
    private bool _playerAlive = true;
    public bool PlayerAlive
    {
        get => _playerAlive;
        set
        {
            _playerAlive = value;

            if(!_playerWon)
                UI_Retry.gameObject.SetActive(!value);
        }
    }

    // check and show victory (UI_Victory)
    private bool _playerWon = false;
    public bool PlayerWon
    {
        get => _playerWon;
        set
        {
            _playerWon = value;
            UI_Victory.gameObject.SetActive(value);
        }
    }


    #endregion

    #region Declarations

    // Game Managment
    private int frameCount = 0;
    private float groundedTime = 0f;
    private bool ActionPressed = false;
    private float lastContactAngle = 0f;

    [Header("References")]
    // player
    public GameObject savedScore;
    public GameObject Player;
    private Rigidbody playerRigidBody;
    private float playerAngle;
    private bool playerDrawn = false;

    // Player Explosion parts 
    public List<GameObject> playerExplosionParts;

    // coins
    public GameObject CoinsContainer;
    private List<TriggerableGameObject> coins;
    public GameObject CoinsPrefab;

    // Bullets
    public GameObject BulletsContainer;
    private List<TriggerableGameObject> bullets;
    public GameObject ProjectilePrefab;

    // Camera handling
    private Transform mainCam;
    private Vector3 mainCamVelocity = Vector3.zero;

    //particles
    public ParticleSystem CanonParticle;

    // Chest
    public GameObject Chest;

    // Particles FX
    public GameObject FXWaterSplash;
    public GameObject FXFireExplosion;

    #endregion

    #region Entities

    // Look in a GameObject container for references
    // NOTE: games objects in container must have the good type 
    private List<TriggerableGameObject> GetChilds(GameObject container)
    {
        if (container == null) return null;

        List<TriggerableGameObject> childs = new List<TriggerableGameObject>();
        for (int i = 0; i < container.transform.childCount; i++)
        {
            var go = container.transform.GetChild(i).gameObject;
            go.name = container.name + "_" + i.ToString();
            childs.Add(new TriggerableGameObject(go));
        }
        return childs;
    }

    private TriggerableGameObject Find(List<TriggerableGameObject> collection, string name) => collection.Find(t => t.gameObject.name == name);

    private int getId(List<TriggerableGameObject> collection, string name)
    {
        if (Find(collection, name) == null) return -1;

        for (var i = 0; i < collection.Count; i++)
            if (collection[i].gameObject.name == name) return i;
        return -1;
    }

    #endregion

    #region Init

    private void Awake()
    {
        Time.timeScale = 1f;

        // rigid body and parts
        playerRigidBody = Player.GetComponent<Rigidbody>();
        CanonParticle.Stop();

        // camera Intialization
        if (Camera.main != null)
            mainCam = Camera.main.transform;
        else
            Debug.LogError("No camera found");

        // get level Items
        coins = GetChilds(CoinsContainer);
        bullets = GetChilds(BulletsContainer);
    }

    #endregion

    #region Update and Logic

    private void FrameUpdate()
    {
        // Coins Handling
        if(coins != null)
        {
            for (var i = 0; i < coins.Count; i++)
            {
                var currentcoin = coins[i];
                // coins by default are rotating
                // and if picked will roll faster and move up
                if (currentcoin.trigerred == false)
                {
                    currentcoin.gameObject.transform.Rotate(0, coinsRotationSpeed * Time.deltaTime, 0, Space.World);
                }
                else
                {
                    currentcoin.gameObject.transform.Rotate(0, coinsRotationSpeed * 20 * Time.deltaTime, 0, Space.World);
                    currentcoin.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * 5, ForceMode.Impulse);
                }

                // removing coin 
                if (currentcoin.gameObject.transform.position.y > 10f)
                {
                    coins.RemoveAt(i);
                    Destroy(currentcoin.gameObject, 0.1f);
                }
            }
        }

    }

    private void Update()
    {
        // lock x axis for player character
        Player.transform.position = new Vector3(0, Player.transform.position.y, Player.transform.position.z);

        // camera following player
        mainCam.position = Vector3.SmoothDamp(
                mainCam.transform.position,
                new Vector3(mainCam.position.x, mainCam.position.y, Player.transform.position.z),
                ref mainCamVelocity,
                smoothTime
            );

        // Check for a touch or mouse pressed
        ActionPressed = (Input.touchCount > 0 || Input.GetMouseButtonDown(0));

        if (PlayerGrounded)
        {
            if (ActionPressed)
            {
                OnPlayerJump();
            }
            else
            {
                // update angle and check if fall
                playerAngle = Quaternion.Angle(Quaternion.Euler(new Vector3(0, 0, 0)), Player.transform.rotation);
                groundedTime += Time.deltaTime;

                if (playerAngle > fallingAngle && groundedTime > fallingTimerTreshold)
                    OnPlayerDie();
            }
        }
        else
        {
            if (ActionPressed && HaveAmmo)
                OnPlayerFire();
        }

        // frames update
        frameCount++;
        if (frameCount > updateFrameRate)
        {
            FrameUpdate();
            frameCount = 0;
        }

    }

    #endregion 
}