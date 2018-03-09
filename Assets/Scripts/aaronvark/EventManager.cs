using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void GEvent(PacketSerializer.GameEvent eventType, ushort senderId, ushort targetId);

public static class EventManager {
    public static GEvent playerScored;
}
