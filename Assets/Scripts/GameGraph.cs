using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(GameGraph))]
public class GameGraphEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Dessine l’inspecteur par défaut
        DrawDefaultInspector();

        GameGraph myComp = (GameGraph)target;

        if (myComp.graphs.Count > 0 && GUILayout.Button("Supprimer ce graph à la position " + myComp.graphIndex))
        {
            myComp.graphs.RemoveAt(myComp.graphIndex);
            if (myComp.graphs.Count == 0)
                myComp.graphText = String.Empty;
            else
            {
                if (myComp.graphIndex > myComp.graphs.Count)
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

public class GraphTextAttribute : PropertyAttribute
{
    public int minLines;
    public int maxLines;

    public GraphTextAttribute(int minLines = 3, int maxLines = 3)
    {
        this.minLines = minLines;
        this.maxLines = maxLines;
    }
}

[CustomPropertyDrawer(typeof(GraphTextAttribute))]
public class GraphTextDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var comp = property.serializedObject.targetObject as GameGraph;

        if (comp.graphs.Count > 0)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();

            property.stringValue = EditorGUI.TextArea(position, property.stringValue);

            if (EditorGUI.EndChangeCheck())
            {
                // Accéder au serializedObject parent
                var serializedObj = property.serializedObject;

                // Trouver le SerializedProperty graphIndex
                var graphIndexProp = serializedObj.FindProperty("graphIndex");

                // Trouver le SerializedProperty graphs
                var graphsProp = serializedObj.FindProperty("graphs");

                if (graphsProp != null && graphIndexProp != null)
                {
                    graphsProp.GetArrayElementAtIndex(graphIndexProp.intValue).stringValue = property.stringValue;
                }

                // Appliquer les changements
                serializedObj.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var attr = (GraphTextAttribute)attribute;

        int lines = Mathf.Max(attr.minLines, attr.maxLines);
        return EditorGUIUtility.singleLineHeight * lines * 1.2f;
    }
}
public class GraphIndexAttribute : PropertyAttribute
{
    public int minLines;
    public int maxLines;

    public GraphIndexAttribute(int minLines = 3, int maxLines = 3)
    {
        this.minLines = minLines;
        this.maxLines = maxLines;
    }
}


[CustomPropertyDrawer(typeof(GraphIndexAttribute))]
public class GraphIndexDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var comp = property.serializedObject.targetObject as GameGraph;

        if (comp.graphs.Count > 0)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();

            property.intValue = EditorGUI.IntSlider(position, property.intValue, 0, comp.graphs.Count - 1);

            if (EditorGUI.EndChangeCheck() && property.intValue != comp.graphIndex)
            {
                // Accéder au serializedObject parent
                var serializedObj = property.serializedObject;

                // Trouver le SerializedProperty graphText
                var graphTextProp = serializedObj.FindProperty("graphText");

                // Trouver le SerializedProperty graphs
                var graphsProp = serializedObj.FindProperty("graphs");

                if (graphsProp != null && graphTextProp != null)
                {
                    graphTextProp.stringValue = graphsProp.GetArrayElementAtIndex(property.intValue).stringValue;
                }

                // Appliquer les changements
                serializedObj.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var attr = (GraphIndexAttribute)attribute;

        int lines = Mathf.Max(attr.minLines, attr.maxLines);
        return EditorGUIUtility.singleLineHeight * lines * 1.2f;
    }
}
#endif

public class GameGraph : MonoBehaviour
{
    public static GameGraph Instance;

    private void Awake()
    {
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
    [GraphIndex]
    public int graphIndex = 0;
    public char graphStep = 'A';
    [GraphText(3, 15)]
    public string graphText = string.Empty;

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
        return Regex.IsMatch(graphText, $@"^\s*[{step}].*$", RegexOptions.Multiline);
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
        var match = Regex.Match(graphText, $@"^\s*[{step}](:?\(\(S\)\))?\s*\-+\>\s*[A-z].*$", RegexOptions.Multiline);
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
        expression = new GraphExpression();
        var pattern = $@"^\s*[{step}]\s*\-+\>\s*\|\s*{action}\s+{actionName}\s*\|\s*(.*)$";
        var match = Regex.Match(graphText, pattern, RegexOptions.Multiline);
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
        var match = Regex.Match(line, pattern, RegexOptions.Multiline);
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
        var match = Regex.Match(line, pattern, RegexOptions.Multiline);
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
        var match = Regex.Match(line, pattern, RegexOptions.Multiline);
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
        dialog = String.Empty;

        if (char.IsLetter(graphText[expression.textStart]) && graphText[expression.textStart + 1] == '>')
        {
            var start = expression.textStart + 2;
            var length = expression.textEnd - start;
            dialog = graphText.Substring(start, length - 2); // - ']\r'
            return true;
        }

        return false;
    }
    /// <summary>
    /// Essayer de parser l'expression comme une animation
    /// </summary>
    /// <param name="expression">Expression donné</param>
    /// <param name="anim">Nom de l'animation</param>
    internal bool TryGetAnimation(GraphExpression expression, out string anim)
    {
        anim = String.Empty;

        if (char.IsLetter(graphText[expression.textStart]) && graphText[expression.textStart + 1] == '(')
        {
            var start = expression.textStart + 2;
            var length = expression.textEnd - start;
            anim = graphText.Substring(start, length - 2); // - ')\r'
            return true;
        }

        return false;
    }
    /// <summary>
    /// Essayer de parser l'expression comme une transition
    /// </summary>
    /// <param name="expression">Expression donné</param>
    /// <param name="dialog">Nom de la scène</param>
    internal bool TryGetTransition(GraphExpression expression, out string scene)
    {
        scene = String.Empty;

        var prefixe = "Transition: ";

        if (char.IsLetter(graphText[expression.textStart]) && graphText[expression.textStart + 1] == '[' && graphText.Substring(expression.textStart + 2).StartsWith(prefixe, true, System.Globalization.CultureInfo.InvariantCulture))
        {
            var start = expression.textStart + 2 + prefixe.Length;
            var length = expression.textEnd - start;
            scene = graphText.Substring(start, length - 2); // - ']\r'
            return true;
        }

        return false;
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
