using System;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Game/InventoryItem")]
[Serializable]
public class InventoryItem : ScriptableObject
{
    public static InventoryItem Empty; // initialisé par BootGame
    /// <summary>
    /// Nom de l'objet
    /// </summary>
    public string label;
    /// <summary>
    /// Description de l'objet
    /// </summary>
    public string description;
    /// <summary>
    /// Image de l'objet
    /// </summary>
    public Sprite sprite;
    internal Image image;


    void Awake()
    {
        if (sprite != null)
        {
            image = new Image();
            image.sprite = sprite;
        }
    }
}