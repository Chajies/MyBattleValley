using Unity.Netcode.Components;
using UnityEngine;

// Syncs a transform with client side changes.
[DisallowMultipleComponent]
public class ClientNetworkTransform : NetworkTransform
{
    // Used to determine who can write to this transform. Owner client only.
    protected override bool OnIsServerAuthoritative() => false;
}
