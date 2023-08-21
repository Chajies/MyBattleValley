using Unity.Netcode;
using UnityEngine;

public class SpriteSetter : NetworkBehaviour
{
    readonly NetworkVariable<int> spriteIndex = new NetworkVariable<int>();
    public Sprite[] playerSprite;
    SpriteRenderer rend;

    void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        spriteIndex.OnValueChanged += OnValueChanged;
    }
    //
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner) CommitSpriteToNetworkServerRpc((int)OwnerClientId);
        else rend.sprite = playerSprite[(int)OwnerClientId % playerSprite.Length];
    }
    //
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        spriteIndex.OnValueChanged -= OnValueChanged;
    }
    //
    [ServerRpc]
    void CommitSpriteToNetworkServerRpc(int newIndex) => spriteIndex.Value = newIndex;
    void OnValueChanged(int previousIndex, int newIndex) => rend.sprite = playerSprite[newIndex % playerSprite.Length];
}
