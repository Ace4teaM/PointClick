using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Représente un emplacement dans l'inventaire
/// Le joueur peut inspecter ou utiliser l'item de l'inventaire dans la scène
/// </summary>
[RequireComponent(typeof(Image))]
public class InventorySlot : MonoBehaviour
{
    /// <summary>
    /// Image d'affichage
    /// </summary>
    private Image image;
    /// <summary>
    /// item contenu, null si aucun
    /// </summary>
    public InventoryItem item;
    /// <summary>
    /// emplacement de l'item
    /// </summary>
    public int slotNumber;

    void OnEnable()
    {
        image = gameObject.GetComponent<Image>();
    }

    void Awake()
    {
        // S'abonner à l'event global
        GameData.OnInventoryChanged += GameData_OnInventoryChanged;
    }

    void OnDestroy()
    {
        // Se désabonner pour éviter les fuites
        GameData.OnInventoryChanged -= GameData_OnInventoryChanged;
    }

    void Start()
    {
        Refresh();
    }

    void Update()
    {
    }

    // Update is called once per frame
    void Refresh()
    {
        // copie les données globales dans l'objet local
        if (GameData.InventoryItems[slotNumber] != item)
        {
            item = GameData.InventoryItems[slotNumber];

            if (item == null && image.sprite != null)
                image.sprite = null;
            else if (item != null && image.sprite != item.sprite)
                image.sprite = item.sprite;
        }
    }

    private void GameData_OnInventoryChanged(InventoryItem[] obj)
    {
        Refresh();
    }
}
