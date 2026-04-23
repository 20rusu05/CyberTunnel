using UnityEngine;
using System.Collections;

public class TunnelIntro : MonoBehaviour
{
    [SerializeField] private Door entryDoor;
    [SerializeField] private GameObject alarmPanel;
    [SerializeField] private float alarmDuration = 5f;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        if (entryDoor != null)
            entryDoor.Close();

        yield return new WaitForSeconds(0.5f);

        if (alarmPanel != null)
            alarmPanel.SetActive(true);

        yield return new WaitForSeconds(alarmDuration);

        if (alarmPanel != null)
            alarmPanel.SetActive(false);
    }
}
