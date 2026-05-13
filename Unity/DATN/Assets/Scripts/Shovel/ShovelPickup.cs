using UnityEngine;

public class ShovelPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Báo SnowManager player đã có shovel
        SnowManager.Instance?.SetHasShovel(true);
        if (SnowManager.Instance != null)
            SnowManager.Instance.player = other.transform;

        // Báo GridPlayer
        GridPlayer player = other.GetComponent<GridPlayer>();
        if (player != null) player.hasShovel = true;

        Debug.Log("✅ Player đã nhặt shovel!");
        Destroy(gameObject); // xóa shovel khỏi scene
    }
}