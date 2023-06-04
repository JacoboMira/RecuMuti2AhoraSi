using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EmojiManager : MonoBehaviourPunCallbacks
{
    bool showingEmoji;
    PlayerJoseluis player;

    [SerializeField] GameObject emoji1;
    [SerializeField] GameObject emoji2;
    [SerializeField] GameObject emoji3;
    [SerializeField] GameObject emoji4;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip audio1;
    [SerializeField] AudioClip audio2;
    [SerializeField] AudioClip audio3;
    [SerializeField] AudioClip audio4;
    private void Awake()
    {
        player = transform.parent.GetComponent<PlayerJoseluis>();
        transform.parent = null;
    }

    private void Update()
    {
        if (player == null)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position = player.transform.position;
            transform.LookAt(Camera.main.gameObject.transform);

            if (photonView.IsMine)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    photonView.RPC("ShowEmoji", RpcTarget.All, 1);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    photonView.RPC("ShowEmoji", RpcTarget.All, 2);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    photonView.RPC("ShowEmoji", RpcTarget.All, 3);
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    photonView.RPC("ShowEmoji", RpcTarget.All, 4);
                }
            }
        }
    }

    [PunRPC]
    public void ShowEmoji(int emoji)
    {
        if (!showingEmoji)
        {
            showingEmoji = true;
            switch (emoji)
            {
                case 1:
                    emoji1.SetActive(true);
                    _audioSource.clip = audio1;
                    _audioSource.Play();
                    StartCoroutine(HideEmoji(6.2f));
                    break;
                case 2:
                    emoji2.SetActive(true);
                    _audioSource.clip = audio2;
                    _audioSource.Play();
                    StartCoroutine(HideEmoji(32));
                    break;
                case 3:
                    emoji3.SetActive(true);
                    _audioSource.clip = audio3;
                    _audioSource.Play();
                    StartCoroutine(HideEmoji(7));
                    break;
                case 4:
                    emoji4.SetActive(true);
                    _audioSource.clip = audio4;
                    _audioSource.Play();
                    StartCoroutine(HideEmoji(6.5f));
                    break;
            }

        }
    }


    IEnumerator HideEmoji(float duration)
    {
        yield return new WaitForSeconds(duration);
        emoji1.SetActive(false);
        emoji2.SetActive(false);
        emoji3.SetActive(false);
        emoji4.SetActive(false);
        showingEmoji = false;

    }
}
