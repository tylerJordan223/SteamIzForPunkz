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
    }

    private void Update()
    {
        if (room_script.active && entrance_playable && !DataManager.g.playerNode.isDoor)
        {
            entrance_playable = false;
            entrance_cutscene.Play();
        }
    }
}
