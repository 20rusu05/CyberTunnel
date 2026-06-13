using UnityEngine;

public class LavaRespawnZone : MonoBehaviour
{
    [SerializeField] private Vector3 respawnPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc == null) return;

        AudioManager.Instance?.PlayLavaDeath();

        cc.enabled = false;
        other.transform.position = respawnPosition;
        cc.enabled = true;
    }
}
