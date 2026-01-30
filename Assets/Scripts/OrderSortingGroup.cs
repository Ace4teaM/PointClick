using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Ajuste dynamiquement l'index d'ordre dans le Sorting Layer en se basant sur l'axe Y
/// </summary>
[RequireComponent(typeof(SortingGroup))]
public class OrderSortingGroup : MonoBehaviour
{
    public int offset = 0;
    public float precision = 100f;

    SortingGroup sortingGroup;

    void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
    }

    void LateUpdate()
    {
        sortingGroup.sortingOrder =
            offset - Mathf.RoundToInt(transform.position.y * precision);
    }
}
