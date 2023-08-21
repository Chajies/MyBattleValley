using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine;

// server-authoritative image enabling and disabling.
// Added reactive checking of value change instead of doing it per-frame
public class NetworkImageActiveStatus : NetworkBehaviour
{
    public NetworkVariable<bool> status = new NetworkVariable<bool>(true);
    Image image;

    void Awake() => image = GetComponent<Image>();
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        status.OnValueChanged += OnStatusChanged;
    }
    //
    [ClientRpc]
    public void SetImageStatusClientRpc(bool newStatus)
    {
        if (IsOwner) CommitStatusToNetworkServerRpc(newStatus);
    }
    //
    [ServerRpc]
    void CommitStatusToNetworkServerRpc(bool newStatus) => status.Value = newStatus;
    void OnStatusChanged(bool prevStatus, bool newStatus) => image.enabled = newStatus;
    void OnDisable() => status.OnValueChanged -= OnStatusChanged;
}
