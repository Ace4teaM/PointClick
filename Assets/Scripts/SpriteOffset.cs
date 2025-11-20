using UnityEngine;

public class SpriteOffset : MonoBehaviour
{
    public Vector3 spriteOffset = new Vector3(0, -0.5f, 0);

    private void OnValidate()
    {
        // exécuté en Éditeur quand une valeur change
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.transform.localPosition = spriteOffset;
        }
    }
}
