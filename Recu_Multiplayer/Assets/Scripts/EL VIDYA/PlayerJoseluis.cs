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

public class PlayerJoseluis : MonoBehaviourPunCallbacks
{
    Vector2 inputMov;
    float speedDecrease = 3;
    [SerializeField] float speed;
    [SerializeField] float sensibility;
    [SerializeField] float fallDamageThreshold;
    [SerializeField] float fallDeathThreshold;
    [SerializeField] GameObject cameraController;
    Rigidbody _rigidbody;
    float fallVelocity;
    bool damaged;
    [SerializeField] GameObject spawner;

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


    private void Awake()
    {
        if (!photonView.IsMine)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!damaged && photonView.IsMine)
        {
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
            isChargingJump = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (jumpForce > minJump)
            {
                _rigidbody.AddForce((Vector3.up + transform.forward).normalized * jumpForce, ForceMode.Impulse);
                inGround = false;
                inRope = false;
                actualRope = null;
                photonView.RPC("Rope", RpcTarget.All, false);
            }

            isChargingJump = false;
            jumpForce = 0;
        }


        if (isChargingJump)
        {
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
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                LookForward();
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
            LookForward();
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

    }

    private void FixedUpdate()
    {
        if (!inRope)
        {
            if (inGround && !damaged)
            {
                _rigidbody.velocity = transform.right * speed * inputMov.x +
                                      transform.forward * speed * inputMov.y +
                                      transform.up * _rigidbody.velocity.y;

            }
        }
        else
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
            inGround = true;
            fallVelocity = -fallVelocity;
            if (fallVelocity > fallDamageThreshold)
            {
                if (fallVelocity > fallDeathThreshold)
                {
                    Die();
                }
                else
                {
                    Damage();
                }
            }
        }
        else if (other.CompareTag("Rope"))
        {
            inRope= true;
            actualRope = other.gameObject;
            photonView.RPC("Rope", RpcTarget.All, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            inGround = false;
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

    void Revive(PlayerJoseluis target)
    {
        target.damaged = false;
    }

    void Damage()
    {
        damaged = true;
    }


    void Die()
    {
        _rigidbody.velocity = Vector3.zero;
        transform.position = spawner.transform.position;
    }
}
