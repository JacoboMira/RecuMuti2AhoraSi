using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerJoseluis player;
    bool audioPlayed;
    // Start is called before the first frame update

    private void Awake()
    {
        if (!player.photonView.IsMine)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            Destroy(gameObject);
        }
        else
        {
            if (player.GetComponent<Rigidbody>().velocity.y > -70)
            {
                transform.position = player.transform.position;
                audioPlayed = false;
            }
            else
            {
                transform.LookAt(player.transform.position);
                if (!audioPlayed)
                {
                    audioPlayed = true;
                    player._audioSource.Play();
                    player._animator.SetBool("Falling", true);
                }
            }
        }
    }
}
