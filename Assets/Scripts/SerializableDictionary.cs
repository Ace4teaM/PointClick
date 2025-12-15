using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Dictionnaire sérialisable avec ToString / Parse statique
/// </summary>
[Serializable]
public class SerializableDictionary : Dictionary<string, string>, ISerializationCallbackReceiver
{
    [HideInInspector, SerializeField] internal string raw;

    public SerializableDictionary()
    {
        raw = String.Empty;
    }

    public SerializableDictionary(string content)
    {
        raw = content;
        Unserialize();
    }

    internal void Unserialize()
    {
        Clear();
        if (string.IsNullOrEmpty(raw))
            return;
        foreach (var line in raw.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var item = line.Split('=', 2, StringSplitOptions.None);
            this.Add(item[0], item[1].Replace(@"\\", "\r\n"));
        }
    }

    /// <summary>
    /// Sérialise le dictionnaire
    /// </summary>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach(var item in this)
        {
            sb.Append(item.Key);
            sb.Append('=');
            sb.AppendLine(item.Value.Replace("\r\n", @"\\"));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Parse une string JSON et retourne une nouvelle instance
    /// </summary>
    public static SerializableDictionary Parse(string raw)
    {
        return new SerializableDictionary(raw);
    }

    public void OnBeforeSerialize()
    {
        raw = ToString();
    }

    public void OnAfterDeserialize()
    {
        Unserialize();
    }
}
