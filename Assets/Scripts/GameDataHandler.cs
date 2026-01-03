using UnityEngine;

/// <summary>
/// Instance locale permettant d'interagir avec les données globales (static)
/// Peut être instancié dans chaque scène de jeu pour réaliser des bindings
/// </summary>
public class GameDataHandler : MonoBehaviour
{
    public void EnableMove()
    {
        GameData.SelectedInventoryItem = InventoryItem.Empty;
        GameData.action = ActionType.Move;
        GameData.OnActionChange();
    }

    public void EnableInspect()
    {
        GameData.SelectedInventoryItem = InventoryItem.Empty;
        GameData.action = ActionType.Inspect;
        GameData.OnActionChange();
    }

    public void EnableInteract()
    {
        GameData.SelectedInventoryItem = InventoryItem.Empty;
        GameData.action = ActionType.Interact;
        GameData.OnActionChange();
    }

    public void EnableTalk()
    {
        GameData.SelectedInventoryItem = InventoryItem.Empty;
        GameData.action = ActionType.Talk;
        GameData.OnActionChange();
    }

    public void EnableActivate()
    {
        GameData.SelectedInventoryItem = InventoryItem.Empty;
        GameData.action = ActionType.Activate;
        GameData.OnActionChange();
    }
    public void ClickItem1()
    {
        if(GameData.action == ActionType.Activate && GameData.InventoryItems[0] != InventoryItem.Empty)
        {
            GameData.SelectedInventoryItem = GameData.InventoryItems[0];
            GameData.OnSelectedItemChange();
        }
    }
    public void ClickItem2()
    {
        if (GameData.action == ActionType.Activate && GameData.InventoryItems[1] != InventoryItem.Empty)
        {
            GameData.SelectedInventoryItem = GameData.InventoryItems[1];
            GameData.OnSelectedItemChange();
        }
    }
    public void ClickItem3()
    {
        if (GameData.action == ActionType.Activate && GameData.InventoryItems[2] != InventoryItem.Empty)
        {
            GameData.SelectedInventoryItem = GameData.InventoryItems[2];
            GameData.OnSelectedItemChange();
        }
    }
    public void ClickItem4()
    {
        if (GameData.action == ActionType.Activate && GameData.InventoryItems[3] != InventoryItem.Empty)
        {
            GameData.SelectedInventoryItem = GameData.InventoryItems[3];
            GameData.OnSelectedItemChange();
        }
    }
    public void ClickChoice1()
    {
        if (GameData.ShowDialogChoices.Length >= 1)
        {
            GameData.SelectedChoice = GameData.ShowDialogChoices[0];
            GameData.OnSelectedChoiceChange();
        }
    }
    public void ClickChoice2()
    {
        if (GameData.ShowDialogChoices.Length >= 2)
        {
            GameData.SelectedChoice = GameData.ShowDialogChoices[1];
            GameData.OnSelectedChoiceChange();
        }
    }
    public void ClickChoice3()
    {
        if (GameData.ShowDialogChoices.Length >= 3)
        {
            GameData.SelectedChoice = GameData.ShowDialogChoices[2];
            GameData.OnSelectedChoiceChange();
        }
    }
}
