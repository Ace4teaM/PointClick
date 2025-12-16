using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(InitStates))]
public class InitStatesEditor : Editor
{
    void OnEnable()
    {
        InitStates myComp = (InitStates)target;

        if (myComp.states.Count > 0)
        {
            var enumerator = myComp.states.GetEnumerator();
            enumerator.MoveNext();
            for (int i = 0; i < myComp.index; i++)
                enumerator.MoveNext();

            myComp.title = enumerator.Current.Key;
            myComp.text = enumerator.Current.Value;
        }
    }

    bool hadTextFocusLastFrame = false;
    bool textModified = false;

    bool hadTitleFocusLastFrame = false;
    bool titleModified = false;
    string prevTitle = String.Empty;

    public override void OnInspectorGUI()
    {
        // Dessine l’inspecteur par défaut
        DrawDefaultInspector();

        InitStates myComp = (InitStates)target;

        if (myComp.states.Count > 0)
        {
            EditorGUILayout.LabelField("Index");
            GUI.SetNextControlName("Index");
            var index = EditorGUILayout.IntSlider(myComp.index, 0, myComp.states.Count-1);
            if(index != myComp.index)
            {
                myComp.index = index;

                var enumerator = myComp.states.GetEnumerator();
                enumerator.MoveNext();
                for (int i = 0; i < index; i++)
                    enumerator.MoveNext();

                myComp.title = enumerator.Current.Key;
                myComp.text = enumerator.Current.Value;

                myComp.RestoreState(myComp.text);
            }

            EditorGUILayout.LabelField("Title");
            GUI.SetNextControlName("Title");
            var title = EditorGUILayout.TextField(myComp.title);
            if (String.Compare(title, myComp.title) != 0)
            {
                if (titleModified == false)
                    prevTitle = myComp.title;
                myComp.title = title;
                titleModified = true;
            }
            // Perte du focus
            bool hasTitleFocus = GUI.GetNameOfFocusedControl() == "Title";
            if (hadTitleFocusLastFrame && !hasTitleFocus && titleModified)
            {
                if(myComp.states.ContainsKey(title))
                {
                    EditorUtility.DisplayDialog(
                            "Saisie",
                            $"La clé '{title}' est déjà utilisée dans la liste des états",
                            "OK"
                        );
                }
                else
                {
                    if (myComp.states.TryGetValue(prevTitle, out var content))
                    {
                        myComp.states.Remove(prevTitle);
                        myComp.states.Add(title, content);
                        myComp.GetEnumeratorFromTitle(title, out myComp.index);
                    }
                    titleModified = false;

                    Repaint();
                }
            }

            hadTitleFocusLastFrame = hasTitleFocus;

            EditorGUILayout.LabelField("Content");
            GUI.SetNextControlName("Content");
            var text = EditorGUILayout.TextArea(myComp.text, GUILayout.Height(300));
            if (String.Compare(text, myComp.text,true) != 0)
            {
                myComp.states[myComp.title] = text;
                myComp.text = text;
                textModified = true;
            }
            // Perte du focus
            bool hasTextFocus = GUI.GetNameOfFocusedControl() == "Content";
            if (hadTextFocusLastFrame && !hasTextFocus && textModified)
            {
                myComp.RestoreState(myComp.text);
                textModified = false;
            }

            hadTextFocusLastFrame = hasTextFocus;
        }

        if (GUILayout.Button("Nouveau"))
        {
            var name = "Nouveau";
            int i = 1;
            while(myComp.states.ContainsKey(name+i))
            {
                i++;
            }

            var key = name + i;
            var content = myComp.CaptureState();

            myComp.states.Add(key, content);
            myComp.title = key;
            myComp.text = content;
            myComp.GetEnumeratorFromTitle(key, out myComp.index);

            Repaint();
        }

        if (myComp.states.Count > 0 && GUILayout.Button("Recharger"))
        {
            myComp.RestoreState(myComp.text);
        }

        if (myComp.states.Count > 0 && GUILayout.Button("Enregistrer"))
        {
            if(myComp.states.Count == 0)
            {
                var key = "Initial";
                var content = myComp.CaptureState();

                myComp.states.Add(key, content);
                myComp.title = key;
                myComp.text = content;
            }
            else
            {
                myComp.text = myComp.CaptureState();
                myComp.states[myComp.title] = myComp.text;
            }
        }
        if (myComp.states.Count > 0 && GUILayout.Button("Supprimer"))
        {
            myComp.states.Remove(myComp.title);

            if (myComp.states.Count > 0)
            {
                myComp.index = 0;
                myComp.title = myComp.states.First().Key;
                myComp.text = myComp.states.First().Value;
            }

            Repaint();
        }


        if (myComp.states.Count > 0 && GUILayout.Button("Tout effacer"))
        {
            if (EditorUtility.DisplayDialog(
                    "Confirmation",
                    "Est-vous sûr de vouloir supprimer tous les états ?",
                    "Oui",
                    "Annuler"
                ))
            {
                myComp.states.Clear();
            }
        }
    }
}

#endif

public class InitStates : MonoBehaviour
{
    public enum SceneType
    {
        GameScene,
        UIScene
    }
    public SceneType sceneType;

    [Serializable]
    public class Element
    {
        public GameObject gameObject;
        public RestorableStates[] states;
    }

    /// <summary>
    /// Liste des éléments à serialiser
    /// </summary>
    [Tooltip("Indique les éléments et leurs états qui seront sauvegardés")]
    public Element[] elements;

    /// <summary>
    /// Etat par défaut
    /// </summary>
    [HideInInspector, SerializeField]
    public string defaultStates = String.Empty;

    /// <summary>
    /// Etats spécifiques
    /// </summary>
    [HideInInspector, SerializeField]
    internal SerializableDictionary states = new SerializableDictionary();

    /*
     Variables de présentation dans l'inspecteur
     */
    internal int index = 0;
    internal string title = string.Empty;
    internal string text = string.Empty;

    /// <summary>
    /// Obtient l'index et énumérateur du dictionnaire en fonct de la clé
    /// </summary>
    /// <param name="key"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    internal Dictionary<string, string>.Enumerator GetEnumeratorFromTitle(string key, out int index)
    {
        index = -1;
        var enumerator = states.GetEnumerator();
        do
        {
            if (enumerator.MoveNext() == false)
            {
                index=-1;
                break;
            }
            index++;
        } while (String.Compare(key, enumerator.Current.Key) != 0);

        return enumerator;
    }

    /// <summary>
    /// Capture tous les états des éléments et retourne une chaine serialisé
    /// </summary>
    /// <returns></returns>
    internal string CaptureState()
    {
        SerializableDictionary dic = new SerializableDictionary();
        foreach(var e in elements)
        {
            foreach(var s in e.states)
                dic[e.gameObject.name + "." + s.ID] = s.CaptureState(e.gameObject);
        }

        return dic.ToString();
    }

    /// <summary>
    /// Restore tous les états des éléments à partir
    /// </summary>
    internal void RestoreState(string text)
    {
        SerializableDictionary dic = new SerializableDictionary(text);
        foreach (var e in elements)
        {
            foreach (var s in e.states)
            {
                if(dic.TryGetValue(e.gameObject.name + "." + s.ID, out var val))
                {
                    s.RestoreState(e.gameObject, val);
                }
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Stoke le nom de l'objet
        switch (sceneType)
        {
            case SceneType.GameScene:
                GameData.CurrentSceneGame = gameObject.scene.name;
                break;
            case SceneType.UIScene:
                GameData.CurrentSceneUI = gameObject.scene.name;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
