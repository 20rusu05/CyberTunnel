using UnityEngine;

/// <summary>
/// Placed at the entrance of the trophy room.
/// When the player walks in, fires all child ParticleSystems once.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ConfettiTrigger : MonoBehaviour
{
    private bool _fired;

    private void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        // Make sure all confetti starts stopped (prefabs may have playOnAwake = true).
        foreach (var ps in GetComponentsInChildren<ParticleSystem>(true))
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_fired) return;
        if (!other.CompareTag("Player")) return;

        _fired = true;

        foreach (var ps in GetComponentsInChildren<ParticleSystem>(true))
            ps.Play();
    }
}
