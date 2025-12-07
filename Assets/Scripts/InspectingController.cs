using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InspectingController : MonoBehaviour
{
    /// <summary>
    /// Si true l'utiliseur a cliquer pour déplacer l'objet
    /// Cette propriété est utilisé en décalage avec OnClick et Update pour permettre à Unity de calculer toutes les propriétés d'UI avant l'action (ie: EventSystem.current.IsPointerOverGameObject())
    /// </summary>
    private bool wantInspect = false;
    void Awake()
    {
        GameData.InputClickEvent += OnClick;
    }
    void OnDestroy()
    {
        GameData.InputClickEvent -= OnClick;
    }

    // Cette fonction sera bindée dans Input Action
    internal void OnClick()
    {
        // Vérifie si l'action en cours est "Inspecter"
        if (GameData.action != ActionType.Inspect)
            return;

        wantInspect = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (wantInspect)
        {
            wantInspect = false;
            // Le clic vient de l’UI (Button ou autre)
            if (HoverCursorFlag.HoverFlagType == HoverFlagType.UI)
                return;

            switch (HoverCursorFlag.HoverFlag)
            {
                // Grenier
                case "Porte":
                    SceneTransition.SetTransition(Scenes.BoitesAuSol);
                    break;
                case "Bibliothèque":
                    SceneTransition.SetTransition(Scenes.Bibliotheque);
                    break;
                case "Canapé":
                    break;
                case "Seb":
                    break;
                    // Boites de jeux PC
                case "Full Throttle":
                    break;
                case "Final Fantasy VII":
                    break;
                case "Sim City 2000":
                    break;
                case "Command & Conquer":
                    break;
                case "Serious Sam":
                    break;
                case "Carmagedon":
                    break;
                case "Thief":
                    break;
                case "Hexen II":
                    break;
                case "Halo":
                    break;
                case "C&C Red Alert":
                    break;
                case "Half Life 2":
                    break;
                case "StarCraft":
                    break;
                case "Blade Runner":
                    break;
                case "Riven":
                    break;
                case "Might and Magic VI":
                    break;
                case "Discworld II":
                    break;
                case "Alone In The Dark":
                    break;
                case "Diablo II":
                    break;
                case "Dune":
                    break;
                case "Oddworld":
                    break;
                case "Age Of Empire":
                    break;
                case "Dark Forces":
                    break;
                case "Little Big Adventure":
                    break;
                case "Fallout":
                    break;
                // Boites de jeux PS4
                case "Uncharted 4":
                    break;
                case "The Last of Us":
                    break;
                case "God of War":
                    break;
                case "The Last of Us Part II":
                    break;
                case "Red Dead Redemption II":
                    break;
                case "Bloodborne":
                    break;
                case "Horizon: Zero Down":
                    break;
                case "Grand Theft Auto":
                    break;
                case "Marvel's Spider Man":
                    break;
                case "Metal Gear Solid V":
                    break;
                case "Batman: Arkham Knight":
                    break;
                case "NieR: Automata":
                    break;
                case "Depth Standing":
                    break;
                case "Detroit: Become Human":
                    break;
                case "The Last Guardian":
                    break;
                case "Dark Souls III":
                    break;
                case "Until Dawn":
                    break;
                case "Persona 5":
                    break;
                case "Ghost of Tsushima":
                    break;
                case "inFamous: Second Son":
                    break;
                case "Shadow of the Colosus":
                    break;
                case "Final Fantasy VII: Remake":
                    break;
                case "Resident Evil 2":
                    break;
                case "Life is Strange":
                    break;
                case "Assassin's Creed Origins":
                    break;
                case "Persona 5: Royal":
                    break;
                case "Dragon Age: Inquisition":
                    break;
                case "Assassin's Credd Unity":
                    break;
                case "Ratchet & Clank":
                    break;
                case "Dragon Quest XI":
                    break;
                case "Star Wars: Battlefield":
                    break;
                case "God of War III Remastered":
                    break;
                case "Wolfenstein: The New Order":
                    break;
                case "Doom Eternal":
                    break;
                case "Battlefield 4":
                    break;
                case "Diablo III":
                    break;
                case "Spyro Reignited Trilogy":
                    break;
                case "BioShock: The Collection":
                    break;
                case "The Elder Scrolls V":
                    break;
                case "Rise of the Tomb Raider":
                    break;
                case "Yakuza: Like a Dragon":
                    break;
                case "Eden Rising":
                    break;
                // Boites de jeux Sega
                case "Flashback":
                    break;
                case "Jungle Strike":
                    break;
                case "Last Battle":
                    break;
                case "Roi Lion":
                    break;
                case "Lemmings":
                    break;
                case "Micromachines 2":
                    break;
                case "Mortal Kombat II":
                    break;
                case "Zool":
                    break;
                case "Wiz 'n' Liz":
                    break;
                case "Schtroumpfs":
                    break;
                case "King of the Monsters":
                    break;
                case "James Bond 007 The Duel":
                    break;
                case "G Loc Air Battle":
                    break;
                case "Fifa 97":
                    break;
                case "Marko's Magic Football":
                    break;
                case "Shining Force II":
                    break;
                case "Streets of Rage 3":
                    break;
                case "Shinobi III":
                    break;
                case "Golden Axe II":
                    break;
                case "Sonic the Hedgehog":
                    break;
                case "Golden Axe (import)":
                    break;
                case "Earthworm Jim 2":
                    break;
                case "Sonic the Hedgehog ":
                    break;
                case "Desert Strike":
                    break;
                case "NBA Jam":
                    break;
                case "Alien Storm":
                    break;
                case "After Burner II":
                    break;
                case "Captain America":
                    break;
                case "Valis":
                    break;
            }
        }
    }
}
