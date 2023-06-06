using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Photon.Pun;
using Quaternion = UnityEngine.Quaternion;

public class PlayerJoseluis : MonoBehaviourPunCallbacks
{
    Vector2 inputMov;
    float speedDecrease = 3;
    [SerializeField] float speed;
    [SerializeField] float torque;
    [SerializeField] float sensibility;
    [SerializeField] float fallDamageThreshold;
    [SerializeField] float fallDeathThreshold;
    [SerializeField] GameObject cameraController;
    [SerializeField] GameObject pointer;
    [SerializeField] GameObject playerMesh;
    Rigidbody _rigidbody;
    float fallVelocity;
    bool damaged;
    [SerializeField] GameObject spawner;
    public List<PlayerJoseluis> playersToHeal = new List<PlayerJoseluis>();
    public EmojiManager emojiManager;
    public AudioSource _audioSource;
    public AudioSource audioFortnite;
    public Animator _animator;

    [Header("------------- JUMP -------------")]
    [Space(10)]

    [SerializeField] float minJump;
    [SerializeField] float maxJump;
    [SerializeField] private float chargeSpeed;
    float jumpForce;
    bool isChargingJump;
    bool inGround = true;
    bool inRope;
    GameObject actualRope;

    #region geters
    public void SetSpawner(Spawner spawner)
    {
        this.spawner = spawner.gameObject;
    }
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Die();
        }

        if (!damaged && photonView.IsMine && inGround || !damaged && photonView.IsMine && inRope)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                photonView.RPC("HealPlayers", RpcTarget.All);
            }

            MoveInputs();

            ChargeJump();
        }
        if (_rigidbody.velocity.y < 0)
        {
            fallVelocity = _rigidbody.velocity.y;
        }
        if (photonView.IsMine)
        {
            MoveCamera();
        }

    }

    void LookAtWalkDirection()
    {
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(pointer.transform.position.x, pointer.transform.position.y, pointer.transform.position.z) - transform.position);
        playerMesh.transform.rotation = Quaternion.Slerp(playerMesh.transform.rotation, targetRotation, torque * Time.deltaTime);
        playerMesh.transform.eulerAngles = new Vector3(0, playerMesh.transform.eulerAngles.y, 0);
    }


    void LookForward()
    {
        transform.eulerAngles = new Vector3(0, cameraController.transform.eulerAngles.y, 0);
    }
    void MoveCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");
        Camera.main.transform.parent.localEulerAngles = new Vector3(Camera.main.transform.parent.localEulerAngles.x + mouseY * sensibility * Time.deltaTime, Camera.main.transform.parent.localEulerAngles.y + mouseX * sensibility * Time.deltaTime, 0);

    }
    void ChargeJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && inGround || Input.GetKeyDown(KeyCode.Space) && inRope)
        {
            _animator.SetBool("ChargingJump", true);
            isChargingJump = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (jumpForce > minJump)
            {
                _animator.SetBool("Jumping", true);
                _animator.SetBool("ChargingJump", false);
                _rigidbody.AddForce((Vector3.up + transform.forward).normalized * jumpForce, ForceMode.Impulse);
                inGround = false;
                inRope = false;
                actualRope = null;
                photonView.RPC("Rope", RpcTarget.All, false);
            }
            else
            {

                _animator.SetBool("ChargingJump", false);
            }

            isChargingJump = false;
            jumpForce = 0;
        }


        if (isChargingJump)
        {
            LookForward();
            inputMov = Vector2.zero;

            jumpForce += chargeSpeed * Time.deltaTime;
            if (jumpForce > maxJump)
            {
                jumpForce = maxJump;
            }
        }
    }
    void MoveInputs()
    {
        if (!inRope)
        {
            if (Input.GetKey(KeyCode.A) && inGround || Input.GetKey(KeyCode.D) && inGround)
            {
                LookAtWalkDirection();
                inputMov.x = Input.GetAxisRaw("Horizontal");
            }
            else if (inputMov.x != 0)
            {
                if (inputMov.x <= 0.2f && inputMov.x >= -0.2f)
                {
                    inputMov.x = 0;
                }

                if (inputMov.x != 0 && inputMov.x > 0)
                {
                    inputMov.x -= speedDecrease * Time.deltaTime;
                }

                if (inputMov.x != 0 && inputMov.x < 0)
                {
                    inputMov.x += speedDecrease * Time.deltaTime;
                }
            }
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            LookAtWalkDirection();
            inputMov.y = Input.GetAxisRaw("Vertical");
        }
        else if (inputMov.y != 0)
        {
            if (inputMov.y <= 0.2f && inputMov.y >= -0.2f)
            {
                inputMov.y = 0;
            }

            if (inputMov.y != 0 && inputMov.y > 0)
            {
                inputMov.y -= speedDecrease * Time.deltaTime;
            }

            if (inputMov.y != 0 && inputMov.y < 0)
            {
                inputMov.y += speedDecrease * Time.deltaTime;
            }
        }
        if(inputMov.y == 0 && inputMov.x == 0)
        {
            _animator.SetBool("Walking", false);
        }
        else
        {
            _animator.SetBool("Walking", true);
        }

    }



    private void FixedUpdate()
    {
        if (!inRope)
        {
            if (inGround && !damaged)
            {
                Vector3 right = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized;
                Vector3 forward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
                _rigidbody.velocity = right * speed * inputMov.x +
                                      forward * speed * inputMov.y +
                                      transform.up * _rigidbody.velocity.y;

            }
        }
        else if(!damaged)
        {
            transform.position = new Vector3(actualRope.transform.localPosition.x, transform.position.y, actualRope.transform.localPosition.z);

            _rigidbody.velocity = transform.right * 0 +
                                  transform.forward * 0 +
                                  transform.up * speed * inputMov.y;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            _animator.SetBool("Jumping", false);
            inGround = true;
            _rigidbody.velocity = Vector3.zero;
            fallVelocity = -fallVelocity;
            if (fallVelocity > fallDamageThreshold)
            {
                if (fallVelocity > fallDeathThreshold)
                {
                    Die();
                }
                else
                {
                    photonView.RPC("Damage", RpcTarget.All);
                }
            }
        }
        else if (other.CompareTag("Rope"))
        {
            inRope = true;
            actualRope = other.gameObject;
            photonView.RPC("Rope", RpcTarget.All, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            inGround = false;
            _animator.SetBool("Jumping", true);
        }
        else if (other.CompareTag("Rope"))
        {

            inRope = false;
            actualRope = null;
            photonView.RPC("Rope", RpcTarget.All, false);
        }
    }

    [PunRPC]
    void Rope(bool isInRope)
    {
        _rigidbody.useGravity = !isInRope;
        if (!isInRope)
        {
            actualRope = null;
        }
    }

    [PunRPC]
    public void HealPlayers()
    {
        foreach (PlayerJoseluis player in playersToHeal)
        {
            player.Revive();
        }   

        playersToHeal.Clear();
    }

    [PunRPC]
    public void Revive()
    {
        damaged = false;
        _animator.SetBool("Dead", false);

    }
    [PunRPC]
    void Damage()
    {
        audioFortnite.Play();
        _rigidbody.velocity = Vector3.zero;
        damaged = true;
        _animator.SetBool("Dead", true);
    }


    public void Die()
    {
        _animator.SetBool("Falling", false);
        damaged = true;
        _animator.SetBool("Dead", false);
        _rigidbody.velocity = Vector3.zero;
        transform.position = spawner.transform.position;
        photonView.RPC("Revive", RpcTarget.All);
    }

}
