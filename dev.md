# Point&Click

## Configuration du projet Unity

**Projet Universal Render 2D**

![]()

![version](version.png)

**Gestion des Inputs**

Gestion des binding sur les inputs (clavier/souris/joystick,...)

![](inputs.png)

Attacher la méthode `OnClick` à l'objet `MouseClickController`

![](input_binding.png)

![](input_binding2.png)

**Sprites personnages**

Lors de l'importation des sprites vérifier que celui-ci n'est pas tronqué automatiquement.

Vérifier que le type est bien Full Rect et si celui-ci est quand même tronqué (comme dans l'exemple ci dessous), ouvrir l'éditeur de sprite et ajuster le rectangle pour qu'il prenne toute l'image.

![](sprite_mesh type.png)

**Animation du personnage**

La propriété Has Exit Time permet de stopper l'animation immédiatement avant de passer à l'état suivant.

![](animation_has_exit.png)

Le contrôleur d'animation de base fonctionne pour les animations de personnages à 8 directions

![](C:\Users\aceteam\source\repos\PointClick\Unity\PointClick\animation_base_controler.png)

Pour adapter le contrôleur à d'autres images de sprite et garder le fonctionnement, il faut créer un `Animation Override Controller`

![](animation_override_controler.png)

et changer les groupes de sprite pour chaque direction dans les propriétés

![](C:\Users\aceteam\source\repos\PointClick\Unity\PointClick\animation_override_controler_prop.png)

**Ordre de profondeur des objets**

L'ordre de rendu est définit sur l'axe `Y` ce qui permet d'utiliser la position des objets comme profondeur de champ. Cela permet de simuler un personnage qui passe devant ou derrière un objet.

Note la profondeur est géré par `Layer` donc `Background` ne passera jamais devant `Character`

![animation_override_controler](render transparency axis.png)

![animation_override_controler_prop](render transparency axis_prop.png)

![render transparency axis_scene](render transparency axis_scene.png)

**Déboguer avec VisualStudio**

Attacher le debogeur Unity depuis l'éditeur Visual Studio

![image-20251124181543852](debug.png)

## Scénes

Sous Unity le jeu est composé de différentes **Scènes**. Chaque scène et ses objets peuvent être chargé/déchargé individuellement et en complément des scènes déjà existantes.

**Dans ce projet les scènes se groupées ainsi:**

* La scène de démarrage (toujours active) : `Main`
* Les scènes d'interfaces utilisateurs : `Scenes/UserInterface/*`
* Les scènes de jeux : `Scenes/Game/*`
* Les scènes de transitions : `Scenes/Transitions/*`

![](scenes2.jpg)

**Les scènes sont combinables mais pas dans n'importe quel ordre:**

* `Main` est toujours actif et contient les objets single-instance (`PlayerInput`, `MainCamera`, `Persistant`, ...)
* 1 scène de `UserInterface` à la fois
* 1 scène de `Game` à la fois
* La scène `Transition` est chargé et déchargé automatiquement lors de la transition d'une scène `Game`

**Editeurs**

En mode **Editeur** vous devez donc avoir au moins plusieurs scènes ouverte pour faire fonctionner le jeu: `Main`, 1 scène de `Game` et 1 scène de `UserInterface`.

![](scenes.jpg)

**Handler/Manager/Controller**

Certains objets on besoins d'être relier à des objets de la scène `Main`, les objets étant dans des scènes différentes il existe des objets intermédiaires nommé `Handler` (ex: `GameDataHandler`) ou directement accessible par script (ex: `GameData`).