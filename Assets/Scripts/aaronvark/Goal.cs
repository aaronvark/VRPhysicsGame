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
    Renderer boxVisual;

	private void Start()
	{
        host = FindObjectOfType<Host>();
        StartCoroutine(ConnectiveVisibility());
        boxVisual = GetComponentInChildren<Renderer>();
	}

	private void OnTriggerEnter(Collider other)
	{
        //hit by a cube
        NetworkInfo cubeInfo = other.GetComponent<NetworkInfo>();
        if ( cubeInfo ) {
            int authorityIndex = cubeInfo.GetAuthorityIndex();
            //if not default authority, and not my authority
            if (authorityIndex != 0 && authorityIndex != goalIndex + 1)
            {
                if ( host.IsClientConnected(goalIndex))
                {
                    //register a scored point (from authIndex - 1 (is senderId), to this clientId (same as goalId))
                    EventManager.playerScored(PacketSerializer.GameEvent.SCORE, (ushort)(authorityIndex - 1), (ushort)goalIndex);
                }
            }
        }
	}

    IEnumerator ConnectiveVisibility() {
        while (Application.isPlaying)
        {
            yield return new WaitForSeconds(1f);
            //check once per second if client is connect, activate box visual if it is
            if (host && boxVisual)
            {
                boxVisual.enabled = host.IsClientConnected(goalIndex);
            }
        }
    }
}
