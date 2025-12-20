using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Représente un emplacement dans l'inventaire
/// Le joueur peut inspecter ou utiliser l'item de l'inventaire dans la scène
/// </summary>
[RequireComponent(typeof(Image))]
[ExecuteAlways]// pour afficher le item dans l'éditeur
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
        // S'abonner à l'event global
        GameData.OnInventoryChanged += GameData_OnInventoryChanged;

        if (Application.isPlaying == false)
            Refresh();
    }

    private void OnDisable()
    {
        // Se désabonner pour éviter les fuites
        GameData.OnInventoryChanged -= GameData_OnInventoryChanged;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        Refresh();
    }

    // Update is called once per frame
    void Refresh()
    {
        if (item == null && image.sprite != null)
            image.sprite = null;
        else if (item != null && image.sprite != item.sprite)
            image.sprite = item.sprite;
    }

    private void GameData_OnInventoryChanged(InventoryItem[] obj)
    {
        // copie les données globales dans l'objet local
        if (obj[slotNumber] != item)
        {
            item = obj[slotNumber];
            Refresh();
        }
    }
}
