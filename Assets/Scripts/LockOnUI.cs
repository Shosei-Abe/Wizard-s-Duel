/*
 * Script: LockOnUI.cs
 * Description: Controls the UI marker that follows the locked-on target.
 * * References:
 * - Unity Manual (Camera.WorldToScreenPoint): https://docs.unity3d.com/ScriptReference/Camera.WorldToScreenPoint.html
 * - Unity Manual (RectTransform): https://docs.unity3d.com/ScriptReference/RectTransform.html
 */
using UnityEngine;
using UnityEngine.UI;

public class LockOnUI : MonoBehaviour
{
    public Image markerImage;
    public LockOnSystem lockOnSystem; // target point with camera
    public Camera mainCamera;

    void Update()
    {
        if (lockOnSystem != null && lockOnSystem.IsLockedOn && lockOnSystem.CurrentTarget != null)
        {
            markerImage.gameObject.SetActive(true);

            // transform the position in the world to position on the screen
            // display a bit upside of enemy chest
            Vector3 targetPos = lockOnSystem.CurrentTarget.position + Vector3.up * 0.5f;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(targetPos);

            // display only when enemy is in screen
            if (screenPos.z > 0)
            {
                markerImage.transform.position = screenPos;
            }
        }
        else
        {
            markerImage.gameObject.SetActive(false);
        }
    }
}