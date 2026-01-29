using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LuminosityParameters : MonoBehaviour
{
    [Range(0f, 1f)]
    public float globalAlpha = 1f;

    public Color tintColor = Color.white;

    private MaterialPropertyBlock propBlock;
    private SpriteRenderer spriteRenderer;

    public float GlobalAlpha
    {
        get => globalAlpha;
        set => this.globalAlpha = value;
    }

    public Color TintColor
    {
        get => tintColor;
        set => this.tintColor = value;
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propBlock = new MaterialPropertyBlock();
        UpdateProperties();
    }

    void Update()
    {
        UpdateProperties();
    }

    void OnValidate()
    {
        // S'exécute dans l'éditeur quand une valeur est modifiée
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (propBlock == null)
            propBlock = new MaterialPropertyBlock();
        UpdateProperties();
    }

    void UpdateProperties()
    {
        spriteRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_GlobalAlpha", globalAlpha);
        propBlock.SetColor("_TintColor", tintColor);
        spriteRenderer.SetPropertyBlock(propBlock);
    }
}
