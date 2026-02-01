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

        var anim = GameObject.Find("Inventory")?.GetComponent<Animator>();
        anim?.ResetTrigger("Show");
        anim?.SetTrigger("Hide");
    }

    public void EnableInspect()
    {
        GameData.SelectedInventoryItem = InventoryItem.Empty;
        GameData.action = ActionType.Inspect;
        GameData.OnActionChange();

        var anim = GameObject.Find("Inventory")?.GetComponent<Animator>();
        anim?.ResetTrigger("Show");
        anim?.SetTrigger("Hide");
    }

    public void EnableInteract()
    {
        GameData.SelectedInventoryItem = InventoryItem.Empty;
        GameData.action = ActionType.Interact;
        GameData.OnActionChange();

        var anim = GameObject.Find("Inventory")?.GetComponent<Animator>();
        anim?.ResetTrigger("Show");
        anim?.SetTrigger("Hide");
    }

    public void EnableTalk()
    {
        GameData.SelectedInventoryItem = InventoryItem.Empty;
        GameData.action = ActionType.Talk;
        GameData.OnActionChange();

        var anim = GameObject.Find("Inventory")?.GetComponent<Animator>();
        anim?.ResetTrigger("Show");
        anim?.SetTrigger("Hide");
    }

    public void EnableActivate()
    {
        GameData.SelectedInventoryItem = InventoryItem.Empty;
        GameData.action = ActionType.Activate;
        GameData.OnActionChange();

        var anim = GameObject.Find("Inventory")?.GetComponent<Animator>();
        anim?.ResetTrigger("Hide");
        anim?.SetTrigger("Show");
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
    public void ClickChoice4()
    {
        if (GameData.ShowDialogChoices.Length >= 4)
        {
            GameData.SelectedChoice = GameData.ShowDialogChoices[3];
            GameData.OnSelectedChoiceChange();
        }
    }
    public void StartGame()
    {
        GameData.StartGame();
    }
}
