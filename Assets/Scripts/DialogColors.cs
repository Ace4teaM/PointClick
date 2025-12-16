using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/DialogColors")]
public class DialogColors : ScriptableObject
{
    [Serializable]
    public class ColorAttributes
    {
        public string name;
        public Color color;
        public Color outline;
    }
    public ColorAttributes[] TextColors = new []{
        new ColorAttributes
        {
            name = "default",
            color = Color.white,
            outline = Color.blue
        }
    };

    public ColorAttributes GetColor(string name)
    {
        var att = TextColors.FirstOrDefault(p => p.name == name);
        if (att != null)
        {
            return att;
        }
        return TextColors.FirstOrDefault(p=>p.name == "default");
    }
}
