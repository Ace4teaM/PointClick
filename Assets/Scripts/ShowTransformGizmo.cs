using UnityEngine;

public class ShowTransformGizmo : MonoBehaviour
{
    #region Gizmo
    [SerializeField] public float gizmoSize = 0.2f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, gizmoSize);
    }
    #endregion
}
