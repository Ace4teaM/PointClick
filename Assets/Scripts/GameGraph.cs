using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(GameGraph))]
public class GameGraphEditor : Editor
{
    bool hadTextFocusLastFrame = false;
    bool textModified = false;

    public override void OnInspectorGUI()
    {
        // Dessine l’inspecteur par défaut
        DrawDefaultInspector();

        GameGraph myComp = (GameGraph)target;

        EditorGUILayout.LabelField("Graph Index");
        GUI.SetNextControlName("Graph Index");
        var index = EditorGUILayout.IntSlider(myComp.graphIndex, 0, myComp.graphs.Count - 1);
        if (index != myComp.graphIndex)
        {
            myComp.graphIndex = index;

            var enumerator = myComp.graphs.GetEnumerator();
            enumerator.MoveNext();
            for (int i = 0; i < index; i++)
                enumerator.MoveNext();

            myComp.graphStep = 'A';
            myComp.graphText = enumerator.Current;
        }

        EditorGUILayout.LabelField("Graph Step");
        GUI.SetNextControlName("Graph Step");
        var step = EditorGUILayout.TextField(myComp.graphStep.ToString());
        if (step.Length > 0 && step != myComp.graphStep.ToString())
        {
            myComp.graphStep = char.ToUpper(step[0]);

            // Marque l'objet comme "dirty" pour que Unity sauvegarde la scène
            EditorUtility.SetDirty(myComp);

            // Force le redraw de l’inspector
            Repaint();
        }

        EditorGUILayout.LabelField("Graph Content");
        GUI.SetNextControlName("Graph Content");
        var text = EditorGUILayout.TextArea(myComp.graphText, GUILayout.Height(300));
        if (String.Compare(text, myComp.graphText, true) != 0)
        {
            myComp.graphs[myComp.graphIndex] = text;
            myComp.graphText = text;
            textModified = true;
        }
        // Perte du focus
        bool hasTextFocus = GUI.GetNameOfFocusedControl() == "Content";
        if (hadTextFocusLastFrame && !hasTextFocus && textModified)
        {
            textModified = false;
        }

        hadTextFocusLastFrame = hasTextFocus;

        if (myComp.graphs.Count > 0 && GUILayout.Button("Supprimer ce graph à la position " + myComp.graphIndex))
        {
            myComp.graphs.RemoveAt(myComp.graphIndex);
            if (myComp.graphs.Count == 0)
            {
                myComp.graphText = String.Empty;
                myComp.graphIndex = 0;
            }
            else
            {
                if (myComp.graphIndex >= myComp.graphs.Count)
                    myComp.graphIndex--;
                myComp.graphText = myComp.graphs[myComp.graphIndex];
            }

            // Marque l'objet comme "dirty" pour que Unity sauvegarde la scène
            EditorUtility.SetDirty(myComp);

            // Force le redraw de l’inspector
            Repaint();
        }

        if (myComp.graphs.Count > 0 && GUILayout.Button("Insérer un graph à la position " + myComp.graphIndex))
        {
            myComp.graphs.Insert(myComp.graphIndex, "graph TB\nA((S))-- > B[Action]\nB --> Z((F))");
            myComp.graphText = myComp.graphs[myComp.graphIndex];

            // Marque l'objet comme "dirty" pour que Unity sauvegarde la scène
            EditorUtility.SetDirty(myComp);

            // Force le redraw de l’inspector
            Repaint();
        }

        // Ajoute un bouton
        if (myComp.graphs.Count > 0 && GUILayout.Button("Insérer un graph à la position " + (myComp.graphIndex + 1)))
        {
            myComp.graphs.Insert(myComp.graphIndex + 1, "graph TB\nA((S))-- > B[Action]\nB --> Z((F))");
            myComp.graphIndex++;
            myComp.graphText = myComp.graphs[myComp.graphIndex];

            // Marque l'objet comme "dirty" pour que Unity sauvegarde la scène
            EditorUtility.SetDirty(myComp);

            // Force le redraw de l’inspector
            Repaint();
        }
        else if (myComp.graphs.Count == 0 && GUILayout.Button("Insérer un graph"))
        {
            myComp.graphs.Add("graph TB\nA((S))-- > B[Action]\nB --> Z((F))");
            myComp.graphIndex = 0;
            myComp.graphText = myComp.graphs[myComp.graphIndex];

            // Marque l'objet comme "dirty" pour que Unity sauvegarde la scène
            EditorUtility.SetDirty(myComp);

            // Force le redraw de l’inspector
            Repaint();
        }
    }
}
#endif

public class GameGraph : MonoBehaviour
{
    public class InitialStates
    {
        public string GameScene;
        public string UIScene;
    }

    /// <summary>
    /// Contient le texte du flow graph mermaid qui détermine les séquences du gameplay
    /// </summary>
    /// <example>
    /// graph TB
    /// A((S)) --> C{Actions}
    /// C-- >| Inspect PS4 | D > Fred: 'Je peux sentir la marque de ce carton dans mon crane']
    /// C-- >| Inspect NES | E > Fred: 'Non ! pas les tortues... pas les tortues !']
    /// C-- >| Inspect Power Glove| H>Fred: 'Cette merde ne servira plus jamais']
    /// C-- >| Inspect Boule de cristal| F>Fred: 'Ha... la belle époque...']
    /// C-- >| Inspect Publicité | G > Fred: 'Une publicité ? Qu'est ce que ça fait là ?] --> Z((F))
    /// </example>
    [HideInInspector]
    public List<string> graphs = new List<string>();

    [SerializeField, HideInInspector]
    internal int graphIndex = 0;
    [SerializeField, HideInInspector]
    internal char graphStep = 'A';
    [SerializeField, HideInInspector]
    internal string graphText = string.Empty;

    internal struct GraphExpression
    {
        public int textStart;
        public int textEnd;
    }

    /// <summary>
    /// Retourne true si cette étape en possède une prochaine dans le graph
    /// </summary>
    internal bool HasNextStep(char step)
    {
        return Regex.IsMatch(graphText, $@"^\s*[{step}].*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
    }
    /// <summary>
    /// Recherche l'étape correspondante dans le texte du graph
    /// </summary>
    /// <param name="step">Etape à rechercher</param>
    /// <param name="expression">expression trouvée</param>
    /// <remarks>Retourne la première expression trouvée</remarks>
    /// <remarks>L'expression doit être automatique elle n'est pas suivi d'une --> |action|</remarks>
    internal bool TryFindImmediateStep(char step, out GraphExpression expression)
    {
        expression = new GraphExpression();
        var match = Regex.Match(graphText, $@"^\s*[{step}](:?\(\(S\)\))?\s*\-+\>\s*[A-z].*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        if (match.Success)
        {
            expression.textStart = match.Index;
            expression.textEnd = match.Index + match.Length;
            return true;
        }
        return false;
    }
    /// <summary>
    /// Recherche l'étape correspondante à l'action donnée en argument
    /// </summary>
    /// <param name="step">Etape à rechercher</param>
    /// <param name="action">Type de l'action à rechercher</param>
    /// <param name="actionName">Nom de l'action à rechercher</param>
    /// <param name="expression">Expression trouvée</param>
    internal bool TryFindAction(char step, ActionType action, string actionName, out GraphExpression expression)
    {
        string _action = EnumExtensions.GetDescription(action);
        expression = new GraphExpression();
        var pattern = $@"^\s*[{step}]\s*\-+\>\s*\|\s*{_action}\s+{actionName}\s*\|\s*(.*)$";
        var match = Regex.Match(graphText, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        if (match.Success)
        {
            expression.textStart = match.Groups[1].Index;
            expression.textEnd = match.Groups[1].Index + match.Groups[1].Length;
            return true;
        }
        return false;
    }
    /// <summary>
    /// Recherche l'étape correspondante à l'action donnée en argument
    /// </summary>
    /// <param name="step">Etape à rechercher</param>
    /// <param name="inventoryItem">Nom de l'item</param>
    /// <param name="objectName">Nom de l'objet à rechercher</param>
    /// <param name="expression">Expression trouvée</param>
    internal bool TryFindUseAction(char step, string inventoryItem, string objectName, out GraphExpression expression)
    {
        var action = EnumExtensions.GetDescription(ActionType.Activate);
        expression = new GraphExpression();
        var pattern = $@"^\s*[{step}]\s*\-+\>\s*\|\s*{action}\s+{inventoryItem}\s+sur\s+{objectName}\s*\|\s*(.*)$";
        var match = Regex.Match(graphText, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        if (match.Success)
        {
            expression.textStart = match.Groups[1].Index;
            expression.textEnd = match.Groups[1].Index + match.Groups[1].Length;
            return true;
        }
        return false;
    }
    /// <summary>
    /// Recherche l'étape cible de l'expression donnée
    /// </summary>
    /// <param name="expression">Expression obtenue par une fonction Try...</param>
    /// <param name="step">Code de l'étape trouvé</param>
    /// <param name="nextExpression">Expression trouvée</param>
    internal bool TryGetNextStep(GraphExpression expression, out char step, out GraphExpression nextExpression)
    {
        var line = graphText.Substring(expression.textStart, expression.textEnd - expression.textStart);

        var pattern = $@"^\s*[A-z](:?\(\(S\)\))?\s*\-+\>\s*(:?\|.*\|)?\s*([A-z])(.*)$";
        var match = Regex.Match(line, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        if (match.Success)
        {
            var start = expression.textStart + match.Groups[3].Index;
            var length = match.Groups[4].Length + 1;

            step = match.Groups[3].Value[0];
            nextExpression.textStart = start;
            nextExpression.textEnd = start + length;
            return true;
        }

        step = char.MinValue;
        nextExpression.textStart = 0;
        nextExpression.textEnd = 0;
        return false;
    }
    /// <summary>
    /// Essayer de parser l'expression comme un changement d'état
    /// </summary>
    internal bool TryGetState(GraphExpression expression, out string objectName, out string stateName, out object stateValue)
    {
        var line = graphText.Substring(expression.textStart, expression.textEnd - expression.textStart).Trim();

        var pattern = $@"^\s*[A-z]\[([A-z]+).([A-z]+)=(.*)\]$";
        var match = Regex.Match(line, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        if (match.Success)
        {
            objectName = match.Groups[1].Value;
            stateName = match.Groups[2].Value;
            stateValue = match.Groups[3].Value;
            // essai de parser la valeur pour déterminer son type
            if (bool.TryParse(match.Groups[3].Value, out var boolValue))
                stateValue = boolValue;
            else if (float.TryParse(match.Groups[3].Value, out var floatValue))
                stateValue = floatValue;
            else if (int.TryParse(match.Groups[3].Value, out var intValue))
                stateValue = intValue;
            return true;
        }

        objectName = null;
        stateName = null;
        stateValue = null;
        return false;
    }
    /// <summary>
    /// Essayer de parser l'expression comme un choix
    /// </summary>
    /// <param name="expression">Expression donné</param>
    /// <remarks>Un choix indique qu'il faut passer immédiatement à la prochaine étape</remarks>
    internal bool TryGetChoice(GraphExpression expression)
    {
        if (char.IsLetter(graphText[expression.textStart]))
        {
            var text = graphText.Substring(expression.textStart + 1);
            return text.StartsWith("{Actions}", true, System.Globalization.CultureInfo.InvariantCulture);
        }

        return false;
    }
    /// <summary>
    /// Essayer de parser l'expression comme une animation
    /// </summary>
    /// <param name="expression">Expression donné</param>
    internal bool TryGetWaitAnimation(GraphExpression expression, out char nextStep, out double duration)
    {
        var line = graphText.Substring(expression.textStart, expression.textEnd - expression.textStart);

        var pattern = $@"^\s*[A-z]\s*\-+\>\s*\|Wait\s*(\d)sec\|\s*([A-z])(.*)$";
        var match = Regex.Match(line, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        if (match.Success)
        {
            duration = double.Parse(match.Groups[1].Value);
            nextStep = match.Groups[2].Value[0];
            return true;
        }

        nextStep = char.MinValue;
        duration = double.MinValue;
        return false;
    }
    /// <summary>
    /// Essayer de parser l'expression comme un dialogue
    /// </summary>
    /// <param name="expression">Expression donné</param>
    /// <param name="dialog">Texte du dialogue</param>
    internal bool TryGetDialog(GraphExpression expression, out string dialog)
    {
        var line = graphText.Substring(expression.textStart, expression.textEnd - expression.textStart).Trim();

        var pattern = $@"^\s*[A-z]>(.*)]$";
        var match = Regex.Match(line, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        if (match.Success)
        {
            dialog = match.Groups[1].Value;
            return true;
        }

        dialog = String.Empty;
        return false;
    }
    /// <summary>
    /// Essayer de parser l'expression comme une animation
    /// </summary>
    /// <param name="expression">Expression donné</param>
    /// <param name="anim">Nom de l'animation</param>
    internal bool TryGetAnimation(GraphExpression expression, out string anim)
    {
        var line = graphText.Substring(expression.textStart, expression.textEnd - expression.textStart).Trim();

        var pattern = $@"^\s*[A-z]\((.*)\)$";
        var match = Regex.Match(line, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        if (match.Success)
        {
            anim = match.Groups[1].Value;
            return true;
        }

        anim = String.Empty;
        return false;
    }
    /// <summary>
    /// Essayer de parser l'expression comme une transition
    /// </summary>
    /// <param name="expression">Expression donné</param>
    /// <param name="dialog">Nom de la scène</param>
    internal bool TryGetTransition(GraphExpression expression, out string scene, out string initialStates)
    {
        var line = graphText.Substring(expression.textStart, expression.textEnd - expression.textStart).Trim();

        var pattern = $@"^[A-z]\[\s*Transition\s*:\s*(?:([^']+)|([^']+)\'([^']+)\')\]$";
        var match = Regex.Match(line, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        if (match.Success)
        {
            if (String.IsNullOrEmpty(match.Groups[1].Value))
            {
                scene = match.Groups[2].Value.Trim();
                initialStates = match.Groups[3].Value.Trim();
            }
            else
            {
                scene = match.Groups[1].Value.Trim();
                initialStates = null;
            }
            return true;
        }

        scene = String.Empty;
        initialStates = String.Empty;
        return false;
    }

    /// <summary>
    /// Essayer de parser l'expression comme : obtenir un item
    /// </summary>
    /// <param name="expression">Expression donné</param>
    /// <param name="item">Nom de l'item</param>
    internal bool TryGetItem(GraphExpression expression, out string item)
    {
        var line = graphText.Substring(expression.textStart, expression.textEnd - expression.textStart).Trim();

        var pattern = $@"^[A-z]\[\[\s*([^!]+)\]\]$";
        var match = Regex.Match(line, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        if (match.Success)
        {
            item = match.Groups[1].Value.Trim();
            return true;
        }

        item = String.Empty;
        return false;
    }

    /// <summary>
    /// Essayer de parser l'expression comme : perdre un item
    /// </summary>
    /// <param name="expression">Expression donné</param>
    /// <param name="item">Nom de l'item</param>
    internal bool TryLoseItem(GraphExpression expression, out string item)
    {
        var line = graphText.Substring(expression.textStart, expression.textEnd - expression.textStart).Trim();

        var pattern = $@"^[A-z]\[\[\s*\!\s*(.+)\]\]$";
        var match = Regex.Match(line, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        if (match.Success)
        {
            item = match.Groups[1].Value.Trim();
            return true;
        }

        item = String.Empty;
        return false;
    }

    protected virtual void Awake()
    {
        graphText = graphs[graphIndex];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
}

public class GlobalGameGraph : GameGraph
{
    public static GameGraph Instance;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
