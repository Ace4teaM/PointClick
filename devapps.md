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