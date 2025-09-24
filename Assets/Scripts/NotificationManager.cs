using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public TextMeshProUGUI notificationText; // assign a UI Text (or TMP_Text) in inspector
    public float displayTime = 3f;

    private Coroutine showRoutine;

    private void Awake()
    {
        if (notificationText != null) notificationText.gameObject.SetActive(false);
    }

    public void ShowNotification(string message)
    {
        if (showRoutine != null) StopCoroutine(showRoutine);
        showRoutine = StartCoroutine(ShowRoutine(message));
    }

    private IEnumerator ShowRoutine(string message)
    {
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        notificationText.gameObject.SetActive(false);
        showRoutine = null;
    }
}
