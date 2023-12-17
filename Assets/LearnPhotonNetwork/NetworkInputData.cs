using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON0 = 0x01;
    public const byte MOUSEBUTTON1 = 0x02;
    public NetworkButtons buttons;

    public Vector3 direction;
}
