using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CircleTransformAnimation : MonoBehaviour
{
    private Image image;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
        image.material = new Material(image.material); // crée une instance unique
        image.material.SetFloat("_HoleRadius", 0.6f);
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneTransition.fadeInTimer > 0f)
        {
            image.material.SetFloat("_HoleRadius", 0.6f * (1f-SceneTransition.fadeInTimer));
        }
        if (SceneTransition.fadeOutTimer > 0f)
        {
            image.material.SetFloat("_HoleRadius", 0.6f * SceneTransition.fadeOutTimer);
        }
    }
}
