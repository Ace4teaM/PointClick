using UnityEngine;

/// <summary>
/// Cette classe réagit au changement d'événement du type d'action
/// pour mettre à jour l'animation d'un bouton
/// </summary>
public class ButtonAnimatorChangeEvent : MonoBehaviour
{
    private Animator animator;
    public ActionType type;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // S'abonner à l'event global
        GameData.OnActionChanged += OnActionChanged;
    }

    private void OnDisable()
    {
        // Se désabonner pour éviter les fuites
        GameData.OnActionChanged -= OnActionChanged;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator.SetBool("Actived", GameData.action == type);
    }

    // Fonction appelée quand l'état global change
    private void OnActionChanged(ActionType value)
    {
        animator.SetBool("Actived", value == type);
    }
}
