using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEditor.EditorTools;
using System.Linq;
using UnityEngine.InputSystem;

/// <summary>
/// Implémernte un outil dédié pour éviter de mixer les outils de bases (move/pan/scale) avec les points d'éditions de chemins
/// </summary>
[EditorTool("Path Editor Tool")]
public class PathEditorTool : EditorTool
{
    public PathNode beginNode = null;
    public Texture2D icon;

    const float hoverThreshold = 5f; // precision au survol (en pixels)

    // Icone dans la barre d'outils
    public override GUIContent toolbarIcon => new GUIContent()
    {
        image = icon,
        text = "Path Editor",
        tooltip = "Edit path points"
    };
    public static Vector3 MouseToWorldPosition(Event e)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        // Plan horizontal Y = 0 par exemple
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }

    // L'outil n'apparaît que si l'objet sélectionné est compatible
    public override bool IsAvailable()
    {
        return Selection.activeGameObject?.GetComponent<PathFinder>() != null;
    }

    // Demande d'activation par l'utilisateur
    public override void OnActivated()
    {
        PathFinder path = Selection.activeGameObject?.GetComponent<PathFinder>();
        if (path == null) return;

        path.editMode = true;
    }

    // Demande de désactivation par l'utilisateur
    public override void OnWillBeDeactivated()
    {
        PathFinder path = Selection.activeGameObject?.GetComponent<PathFinder>();
        if (path == null) return;

        path.editMode = false;
    }

    // Dessin de l'éditeur
    public override void OnToolGUI(EditorWindow window)
    {
        Event e = Event.current;

        // Vérification que la fenêtre est une SceneView
        if (!(window is SceneView)) return;

        // Récupérer l'objet sélectionné
        if (Selection.activeGameObject == null) return;

        PathFinder path = Selection.activeGameObject.GetComponent<PathFinder>();
        if (path == null || !path.editMode) return;

        Handles.color = Color.yellow;

        bool moveHandle = Keyboard.current.leftCtrlKey.isPressed == false && Keyboard.current.leftAltKey.isPressed == false;
        for (int i = 0; i < path.nodes.Count; i++)
        {
            // Position globale
            Vector3 worldPos = path.transform.TransformPoint(path.nodes[i].position);

            // Handle déplaçable
            if (moveHandle)
            {
                Vector3 newWorldPos = Handles.PositionHandle(worldPos, Quaternion.identity);

                // Met à jour la position si modifiée
                if (worldPos != newWorldPos)
                {
                    Undo.RecordObject(path, "Move Path Point");
                    path.nodes[i].position = path.transform.InverseTransformPoint(newWorldPos);
                    EditorUtility.SetDirty(path);
                }
            }
        }

        // detecte le segment sous le curseur
        bool hoverSegment = false;
        bool hoverPoint = false;
        PathNode hoverNodeStart = null;
        PathNode hoverNodeEnd = null;
        foreach (var node in path.nodes)
        {
            float pointDistance = HandleUtility.DistanceToCircle(path.transform.position + node.position, path.gizmoSize);

            if (pointDistance < hoverThreshold)
            {
                hoverNodeStart = node;
                hoverNodeEnd = null;
                hoverPoint = true;
                break;
            }
        }

        foreach (var link in path.links)
        {
            var from = path.nodes[link.from];
            var to = path.nodes[link.to];

            float distance = HandleUtility.DistanceToLine(path.transform.position + from.position, path.transform.position + to.position);

            bool isHovering = distance < hoverThreshold;

            if (isHovering)
            {
                hoverNodeStart = from;
                hoverNodeEnd = to;
                hoverSegment = true;
                break;
            }
        }

        // ajout/supression des points
        if (Keyboard.current.leftCtrlKey.isPressed)
        {
            Rect rect = new Rect(Event.current.mousePosition, Vector2.one * 20);
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
            SceneView.RepaintAll();

            // Actions au curseur
            if (hoverSegment)
            {
                Handles.color = Color.red;
                Handles.DrawLine(path.transform.position + hoverNodeStart.position, path.transform.position + hoverNodeEnd.position);
                SceneView.RepaintAll();

                // Clic gauche, ajoute un point au segment
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    Vector3 worldPos = MouseToWorldPosition(e) - path.transform.position;

                    var n = new PathNode();
                    n.position = worldPos;

                    path.InsertNodeBetween(n, hoverNodeStart, hoverNodeEnd);

                    // Empêche Unity de propager l'événement
                    e.Use();
                }
            }
            else if (hoverPoint)
            {
                Handles.color = Color.red;
                Handles.DrawWireDisc(path.transform.position + hoverNodeStart.position, Vector3.forward, path.gizmoSize);
                SceneView.RepaintAll();

                // Clic gauche, ajoute une connexion entre les points
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    if(beginNode == null)
                        beginNode = hoverNodeStart;
                    else
                    {
                        path.LinkNodes(beginNode, hoverNodeStart);

                        beginNode = hoverNodeStart;
                    }

                    // Empêche Unity de propager l'événement
                    e.Use();
                }
            }
            else
            {
                // Clic gauche, ajoute une connexion entre les points
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    Vector3 worldPos = MouseToWorldPosition(e) - path.transform.position;

                    var n = new PathNode();
                    n.position = worldPos;

                    path.AddNode(n);

                    // Empêche Unity de propager l'événement
                    e.Use();
                }
            }

            if (beginNode != null)
            {
                Handles.color = Color.green;
                Handles.DrawWireDisc(path.transform.position + beginNode.position, Vector3.forward, path.gizmoSize);
                SceneView.RepaintAll();
            }
        }
        else
        {
            beginNode = null;
        }

        // ajout/supression des connections
        if (Keyboard.current.leftAltKey.isPressed)
        {
            Rect rect = new Rect(Event.current.mousePosition, Vector2.one * 20);
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.ArrowMinus);

            // Actions au curseur
            if (hoverSegment)
            {
                Handles.color = Color.red;
                Handles.DrawLine(path.transform.position + hoverNodeStart.position, path.transform.position + hoverNodeEnd.position);
                SceneView.RepaintAll();

                // Clic gauche, supprime un segment
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    Vector3 worldPos = MouseToWorldPosition(e) - path.transform.position;

                    path.RemoveLink(hoverNodeStart, hoverNodeEnd);

                    // Empêche Unity de propager l'événement
                    e.Use();
                }
            }
            if (hoverPoint)
            {
                Handles.color = Color.red;
                Handles.DrawWireDisc(path.transform.position + hoverNodeStart.position, Vector3.forward, path.gizmoSize);
                SceneView.RepaintAll();

                // Clic gauche, supprime un point 
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    path.RemoveNode(hoverNodeStart);

                    // Empêche Unity de propager l'événement
                    e.Use();
                }
            }
        }
    }
}

[CustomEditor(typeof(PathFinder))]
public class PathEditor : Editor
{
    /// <summary>
    /// Implémente les boutons dans le panneau d'édition "Inspector"
    /// </summary>
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathFinder path = (PathFinder)target;

        if (GUILayout.Button(path.editMode ? "Disable Edit Mode" : "Enable Edit Mode"))
        {
            // Activer
            if(path.editMode == false)
                ToolManager.SetActiveTool<PathEditorTool>();
            else
                ToolManager.RestorePreviousTool();
            SceneView.RepaintAll(); // rafraîchit la scene
        }

        if (GUILayout.Button("Update paths"))
        {
            path.dirtyNeighbors = true;
            path.FixIt();
            path.UpdateCache();
        }
    }
}

[System.Serializable]
public class PathNode
{
    /// position du noeud
    public Vector3 position;

    #region Cache
    /// voisins directs accessibles dans cette direction
    [NonSerialized] internal List<PathNode> neighbors = new List<PathNode>();

    [NonSerialized] internal float gCost; // coût depuis le départ
    [NonSerialized] internal float hCost; // heuristique vers la cible
    internal float fCost => gCost + hCost;

    [NonSerialized] internal PathNode parent;
    #endregion
}

[System.Serializable]
public class EdgePath
{
    public int from;
    public int to;
}

/// <summary>
/// Implémente les données du chemin et la logique de recherche du chemin le plus adapté lors d'un déplacement d'un point A au point B
/// </summary>
public class PathFinder : MonoBehaviour
{
    internal bool editMode = false; // active/désactive l'éditeur
    internal bool dirtyNeighbors = true; // met à jour les connecteurs en cache (PathNode)

    /// <summary>
    /// Liste des points de passages
    /// </summary>
    [SerializeField, HideInInspector]
    public List<PathNode> nodes = new();
    /// <summary>
    /// Liste des chemins entre les points de passages
    /// </summary>
    [SerializeField, HideInInspector]
    internal List<EdgePath> links = new();

    #region Gizmo
    [SerializeField] public float gizmoSize = 0.2f;


    private void DrawArrow(Vector3 start, Vector3 end)
    {
        float headAngle = 20f;             // angle de la pointe
        float headLength = 0.10f;          // longueur de la pointe

        Vector2 dir = (end - start).normalized;

        // Ligne principale
        Gizmos.DrawLine(start, end);

        // Pointe de flèche
        Vector3 right = Quaternion.Euler(0, 0, headAngle) * -dir;
        Vector3 left = Quaternion.Euler(0, 0, -headAngle) * -dir;
        Gizmos.DrawLine(end, end + right * headLength);
        Gizmos.DrawLine(end, end + left * headLength);
    }

    private void OnDrawGizmosSelected()
    {
        if (nodes != null)
        {
            Gizmos.color = Color.orange;
            foreach (var node in nodes)
            {
                Gizmos.DrawWireSphere(this.transform.position + node.position, gizmoSize);
            }
            foreach (var link in links)
            {
                var from = nodes[link.from];
                var to = nodes[link.to];
                Gizmos.DrawLine(this.transform.position + from.position, this.transform.position + to.position);
            }
        }
    }
    #endregion

    public PathFinder()
    {
        PathNode a = new PathNode { position = new Vector3(0, 0, 0) };
        PathNode b = new PathNode { position = new Vector3(0.1f, 0, 0) };
        PathNode c = new PathNode { position = new Vector3(0.1f, 0.1f, 0) };
        PathNode d = new PathNode { position = new Vector3(0.2f, 0, 0) };

        nodes = new List<PathNode>(
        new[]{
            a,b,c,d
        });

        // connexions multiples
        LinkNodes(a,b);
        LinkNodes(b,c);
        LinkNodes(c,d);
        LinkNodes(d,a);
    }

    /// <summary>
    /// Vérifie que les liens existes pour tous les noeuds
    /// </summary>
    internal void FixIt()
    {
        foreach (var link in links.ToArray())
        {
            if (link.from >= nodes.Count || link.to >= nodes.Count)
            {
                Debug.Log($"Supprime le lien ${link.from} => ${link.to}");
                links.Remove(link);
                dirtyNeighbors = true;
            }
        }
    }

    public void AddNode(PathNode node)
    {
        nodes.Add(node);
    }

    public void RemoveNode(PathNode node)
    {
        var startIndex = nodes.IndexOf(node);
        // supprime le noeud
        nodes.Remove(node);
        // supprime les liens
        foreach(var link in links.Where(p => p.from == startIndex || p.to == startIndex).ToArray())
        {
            links.Remove(link);
        }
        // décale les index des liens suivants
        foreach (var link in links)
        {
            if (link.from >= startIndex)
                link.from--;
            if (link.to >= startIndex)
                link.to--;
        }
        // invalide le cache
        dirtyNeighbors = true;
    }

    public void InsertNodeBetween(PathNode node, PathNode a, PathNode b)
    {
        // supprime le lien existant
        var link = links.FirstOrDefault(p => (p.from == nodes.IndexOf(a) && p.to == nodes.IndexOf(b)) || (p.from == nodes.IndexOf(b) && p.to == nodes.IndexOf(a)));
        if (link != null)
            links.Remove(link);
        // ajoute le noeud
        nodes.Add(node);
        // ajoute les 2 liens
        links.Add(new EdgePath{ from = nodes.IndexOf(node), to = nodes.IndexOf(a) });
        links.Add(new EdgePath{ from = nodes.IndexOf(node), to = nodes.IndexOf(b) });
        // invalide le cache
        dirtyNeighbors = true;
    }

    public void RemoveLink(PathNode a, PathNode b)
    {
        foreach (var link in links.Where(p => (p.from == nodes.IndexOf(a) && p.to == nodes.IndexOf(b)) || (p.from == nodes.IndexOf(b) && p.to == nodes.IndexOf(a))).ToArray())
        {
            links.Remove(link);
        }
        dirtyNeighbors = true;
    }

    public void LinkNodes(PathNode a, PathNode b)
    {
        links.Add(new EdgePath { from = nodes.IndexOf(a), to = nodes.IndexOf(b) });
        dirtyNeighbors = true;
    }

    /// <summary>
    /// Calcule les connexions possibles entre les noeuds
    /// </summary>
    public void UpdateCache()
    {
        if (dirtyNeighbors)
        {
            dirtyNeighbors = false;

            foreach (var node in nodes)
            {
                node.neighbors.Clear();
            }
            foreach (var link in links)
            {
                var from = nodes[link.from];
                var to = nodes[link.to];
                from.neighbors.Add(to);
                to.neighbors.Add(from);
            }
        }
    }

    /// <summary>
    /// Dans la mail de point, recherche le chemin le plus court
    /// </summary>
    internal static List<PathNode> FindNodesPath(PathNode start, PathNode target)
    {
        List<PathNode> openSet = new List<PathNode>();
        HashSet<PathNode> closedSet = new HashSet<PathNode>();

        openSet.Add(start);

        while (openSet.Count > 0)
        {
            PathNode current = openSet[0];

            // Trouver le plus petit fCost
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < current.fCost ||
                   (openSet[i].fCost == current.fCost && openSet[i].hCost < current.hCost))
                {
                    current = openSet[i];
                }
            }

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == target)
                return RetracePath(start, target);

            foreach (PathNode neighbour in current.neighbors)
            {
                if (closedSet.Contains(neighbour))
                    continue;

                float newCost = current.gCost + Vector3.Distance(current.position, neighbour.position);

                if (newCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCost;
                    neighbour.hCost = Vector3.Distance(neighbour.position, target.position);
                    neighbour.parent = current;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return null; // aucun chemin trouvé
    }

    static List<PathNode> RetracePath(PathNode start, PathNode end)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode current = end;

        while (current != start)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Add(start);
        path.Reverse();

        return path;
    }

    /// <summary>
    /// Recherche le chemin le plus court dans le réseau de noeud
    /// </summary>
    /// <param name="start">Position de départ</param>
    /// <param name="end">Position d'arrivée</param>
    /// <returns></returns>
    public Queue<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        UpdateCache();

        Queue<Vector3> list = new();
        
        //1. Recherche le noeud de départ le plus proche ET accessible de la position de départ
        PathNode startNode = nodes.First();
        foreach(var node in nodes.Skip(1))
        {
            if (Vector3.Distance(start, this.transform.position + node.position) < Vector3.Distance(start, this.transform.position + startNode.position))
            {
                startNode = node;
            }
        }

        //2. Recherche le noeud de terminaisont le plus proche ET accessible de la position d'arrivée
        PathNode endNode = nodes.First();
        foreach (var node in nodes.Skip(1))
        {
            if (Vector3.Distance(end, this.transform.position + node.position) < Vector3.Distance(end, this.transform.position + endNode.position))
            {
                endNode = node;
            }
        }


        //3. Recherche le chemin le plus court entre les 2 noeuds
        var path = FindNodesPath(startNode, endNode);

        // Construit le chemin de positions
        foreach (var node in path)
            list.Enqueue(this.transform.position + node.position);

        return list;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
