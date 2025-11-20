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