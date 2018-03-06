using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Server-only object that checks if cubes have passed into it.
/// When a cube hits it, it tells the score manager that it has registered a "goal".
/// There probably needs to be some conflict resolution for when cubes are "caught" by players, but glitch beyond them briefly.
/// </summary>
public class Goal : NetworkBehaviour {

}
