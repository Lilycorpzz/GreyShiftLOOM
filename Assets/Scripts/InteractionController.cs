using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionController : MonoBehaviour
{
    [Tooltip("Layer mask for objects that can be clicked (set to 'Interactable' layer).")]
    public LayerMask interactableLayer;
    Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
        if (mainCam == null) Debug.LogError("Main Camera not found. Tag your camera as MainCamera.");
    }

    private void Update()
    {
        // Left mouse button clicked (or touch began)
        if (Input.GetMouseButtonDown(0))
        {
            // Ignore clicks over UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2 worldPoint = mainCam.ScreenToWorldPoint(Input.mousePosition);

            // OverlapPoint returns first collider under the point (2D)
            Collider2D col = Physics2D.OverlapPoint(worldPoint, interactableLayer);
            if (col != null)
            {
                CubicleInteractable ci = col.GetComponent<CubicleInteractable>();
                if (ci != null)
                {
                    ci.Interact();
                }
            }
        }
    }
}
