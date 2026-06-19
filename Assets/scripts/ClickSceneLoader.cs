using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ClickSceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName = "Standard Demo";

    private bool isLoading;

    private void OnMouseDown()
    {
        LoadTargetScene();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0) || IsPointerOverUi())
        {
            return;
        }

        Camera camera = Camera.main != null ? Camera.main : FindObjectOfType<Camera>();
        if (camera == null)
        {
            return;
        }

        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide)
            && hit.collider != null
            && hit.collider.gameObject == gameObject)
        {
            LoadTargetScene();
        }
    }

    private void LoadTargetScene()
    {
        if (isLoading)
        {
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' is not enabled in Build Settings.");
            return;
        }

        isLoading = true;
        ReleaseCursorForInteractiveScene();
        SceneManager.LoadScene(sceneName);
    }

    private static void ReleaseCursorForInteractiveScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private static bool IsPointerOverUi()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
