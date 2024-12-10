using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

public class BossRoomScript : MonoBehaviour
{
    [SerializeField] PlayableDirector entrance_cutscene;
    private RoomScript room_script;

    //handling cameras
    [Header("Cameras")]
    [SerializeField] GameObject player;
    [SerializeField] CinemachineVirtualCamera roomcam;
    [SerializeField] CinemachineVirtualCamera finalcam;
    private float base_ortho;

    //flags for cutscenes being playable
    private bool entrance_playable;
    private bool win_playable;

    private void Start()
    {
        room_script = GetComponent<RoomScript>();
        room_script.my_room = new Room(gameObject, DataManager.g, null, 0, 1);

        //cutscene flags
        entrance_playable = true;
        win_playable = true;

        //base
        base_ortho = finalcam.m_Lens.OrthographicSize;
    }

    private void Update()
    {
        if (room_script.active && entrance_playable && !DataManager.g.playerNode.isDoor)
        {
            entrance_playable = false;
            entrance_cutscene.Play();
        }

        if(player.GetComponent<PlayerStats>().special == 3)
        {
            if(roomcam.gameObject.activeInHierarchy)
            {
                roomcam.gameObject.SetActive(false);
                finalcam.gameObject.SetActive(true);
            }

            finalcam.m_Lens.OrthographicSize = base_ortho - player.GetComponent<PlayerSpinAttack>().size;
        }
        else
        {
            if(finalcam.gameObject.activeInHierarchy)
            {
                finalcam.gameObject.SetActive(false);
                roomcam.gameObject.SetActive(true);
            }
        }
    }
}
