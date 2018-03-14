# VRPhysicsGame
Based on the VR Networked Physics from Glenn Fiedler.

### How to use this:

#### Easy part
* Make sure to run it with Unity 5.5.x
* Make sure the host is an actual Oculus Rift (it's best just to use Rifts)
* When using a Vive, set Unity, Revive Dashboard & Revive Inject to run as administrator

#### Technical part (oculus developer account & quickmatch room & query)
* Create an Oculus Develop account, create a new App, and add the appId in "Oculus Platform -> Edit Settings" as the Rift ID 
  - You can find this under the "Getting Started API" page, in your developer account under "Manage -> [name]"

* Create a new "room pool" under "Platform Services -> Rooms and Matchmaking"
  - The key should be "VRNetwork_Test_Key" (I changed this, the original was "quickmatch")
    - (I intend to change it back again)
  - The mode should be QuickMatch, min 2, max 4 users, None skill pool, and yes to all questions
  - I used 500/200 as acceptable/good ping times
* After creating the pool, select the "..." and hit "Manage Queries"
* Hit "Edit Data Settings" and add a Data Setting with the following setup:
  - key: version
  - type: integer
  - default value: 0
* Add a "quickmatch_query" query, with the following setup:
  - Importance: required
  - their "version" is (=) my "version" (check datasetting)

#### Finally
* Make sure to use a different Oculus account for each device
 * Either set each computer up with different user tokens, or login to Oculus with different accounts

### Known issues:
* Vive clients will disconnect intermittently

### Attributions:
* Sounds effects taken from various Freesound users (id__user__soundname):
  - 19301__starrock__pen6
  - 315725__nabz871__tonal-impact-e
  - 376816__original-sound__impact

## How I added stuff

# Game Events
To figure out how to add things to this, I basically traced the types of packets that are sent (StateUpdate and ServerInfo), which you can find in the PacketSerializer.cs file, and added a new GameEvent packet type. Then I also added an enum for a GameEvent itself, which for now just contains the SCORE event.

## Information structure
Then I decided on a basic structure of the data of a GameEvent:
* the type of GameEvent (byte from the enum)
* a short for the senderId (player index)
* a short for the targetId (player index)
* an array of shorts for the "event value" for all players (like scores)

The last one I added specifically to prevent <i>drift</i> between players that did or did not receive score updates. If a player joined later, and only received "somebody scored", and not absolute values, they wouldn't agree about what the current score values were. I also thought adding a separate "all scores" packet for when clients joined would be too much of a bother for something as simple as sending 4 additional shorts per score event (something that doesn't happen more than a few times per second).

## Read/Write packet functions
Once I had this, I copy pasted the Read/Write packet functions for ServerInfo (currently you can still spot a non-corrected Debug Log in one of them), and edited how the packet was packed/unpacked to coincide with the above structure.

## Implementing in shared class (Common.cs)
To find out what to do next, I checked where the read/write packet functions were called (the ones in PacketSerializer.cs), and ended up in Common.cs. So again, I mirrored the setup I found for ServerInfo packets, and changed what happened to coincide with the new GameEvent information structure.

## Implementing the send/receive events
Then we finally arrive at, "ok, so when do we actually use these packet read/write functions?"

Again, I checked one layer up where these functions were called, and this is where we get to the Guest and Host classes. At this point we need to decide who will be sending what, and to whom. My idea was as follows for the <i>GameEvent.Score</i>:
* Server does all collision testing
* Server handles score locally (incremental + feedback in UI & sfx etc.)
* Server sends all scores, including information about "who scored against who" (sender/target) to <i>all clients</i>
* Clients receive all scores each time anybody scores, and update UI / play sounds, etc.

This made it pretty simple. I would only need to have some kind of "event push" happening on the Server, and then send a packet to all clients. Clients only need to respond to scoring as an incoming packet.

## Implementing Scoring
To implement scoring I added an <i>EventManager</i>, as a static class where events could be triggered globally. This contains an event for when a player scores, which only the Host registers itself to.

The <i>Goal</i> class is attached to BoxColliders that are only present in the Host scene, and when a cube enter this (a layer was added to make sure nothing else was registered), its <i>authority index</i> is compared to the goal's player index. This is quite simple, because:
* The authority index (0 = nobody, 1-4 = playerId 0-3) is "who handled the cube last"
* The goal's playerId is <i>scene static</i>, meaning that player0 (server) always stands on the Blue spot, player1 (a client) always stands on the Red spot, etc.

If the id's don't match (another player's cube entered this goal) a "scored" event is triggered with the two Id's as arguments (cube's id = sender, goal id = target), and a dump of the ScoreManager's score array for all players.

## Host handles score
The Host, who's listeneing for Score Events, receives this message. First it locally updates the score (and plays the necessary SFX), and then it writes a GameEvent package using the functions we wrote earlier, and loops through the client's (I stole this, again, from the ServerInfo packet because the lines of communication of that packet are very similar to this particular GameEvent).

## Client handles packet
When the client receives this packet, it is checked for in the same place where other packets are received in Guest.cs, so adding new packet types requires you to add logic for them here as well. Once we confirm it is a GameEvent, we call the ProcessGameEvent function. There we unpack all of the data, which I've chosen to do with local variables that are sent as <i>ref</i> into the ReadPacket function.

I noticed other functions didn't do this, but they also used classes of which array's would be modified, which I assume prevents them from being "passed by value", and therefore don't require the ref keyword.

Once the data is read, the Client responds to it much the same as the Host does, except that the ScoreManager is not updated incrementally, but all the scores are simply pushed to it.
