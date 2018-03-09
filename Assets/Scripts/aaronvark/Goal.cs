using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Server-only object that checks if cubes have passed into it.
/// When a cube hits it, it tells the score manager that it has registered a "goal".
/// There probably needs to be some conflict resolution for when cubes are "caught" by players, but glitch beyond them briefly.
/// </summary>
public class Goal : MonoBehaviour {
    //set in the scene view
    public int goalIndex = -1;
    Host host;

	private void Start()
	{
        host = FindObjectOfType<Host>();
	}

	private void OnTriggerExit(Collider other)
	{
        //hit by a cube
        NetworkInfo cubeInfo = other.GetComponent<NetworkInfo>();
        if ( cubeInfo ) {
            int authorityIndex = cubeInfo.GetAuthorityIndex();
            //if not default authority, and not my authority
            if (authorityIndex != 0 && authorityIndex != goalIndex + 1)
            {
                //check if this player is currently connected
                if (host.IsClientConnected(goalIndex))
                {
                    //register a scored point (from authIndex - 1 (is senderId), to this clientId (same as goalId))
                    EventManager.playerScored(PacketSerializer.GameEvent.SCORE, (ushort)(authorityIndex - 1), (ushort)goalIndex);
                }
            }
        }
	}
}
