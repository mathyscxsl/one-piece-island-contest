using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public enum ItemType { Health, Strength, Speed }

    [SerializeField] private ItemType type;
    [SerializeField] private int healthBonus = 20;
    [SerializeField] private int strengthBonus = 5;
    [SerializeField] private float speedBonus = 0.5f;
    [SerializeField] private float pickupRadius = 0.6f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private AudioClip pickupSound;

    private void Update()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, pickupRadius, playerLayer);
        if (hit == null) return;

        PlayerController player = hit.GetComponentInParent<PlayerController>();
        if (player == null) return;

        ApplyBuff(player);
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        Destroy(gameObject);
    }

    private void ApplyBuff(PlayerController player)
    {
        switch (type)
        {
            case ItemType.Health:   player.AddMaxHealth(healthBonus);   break;
            case ItemType.Strength: player.AddDamage(strengthBonus);    break;
            case ItemType.Speed:    player.AddSpeed(speedBonus);        break;
        }
    }
}
