using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Déplace un point à l'écran en fonction de la destination donné
/// </summary>
[ExecuteAlways]// pour afficher ldans l'éditeur
public class MoverAnimatorTester : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public Transform walkingPoint;
    public MoverAnimator.Direction direction;
    public Vector3 directionVector;

    public bool reverseSpriteRenderer = true;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(walkingPoint.position, directionVector);
    }


    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(mousePos);

        // obtient la direction la plus proche du vecteur de destination
        directionVector = (targetPos - walkingPoint.position).normalized;
        direction = MoverAnimator.GetClosestDirection(directionVector);

        // Animation
        if (animator)
        {
            animator.SetInteger("Direction", (int)direction);
            animator.SetBool("IsMoving", true);
            switch (direction)
            {
                case MoverAnimator.Direction.N:
                    animator.SetFloat("DirX", 0.0f);
                    animator.SetFloat("DirY", -1.0f);
                    break;
                case MoverAnimator.Direction.S:
                    animator.SetFloat("DirX", 0.0f);
                    animator.SetFloat("DirY", 1.0f);
                    break;
                case MoverAnimator.Direction.E:
                    animator.SetFloat("DirX", -1.0f);
                    animator.SetFloat("DirY", 0.0f);
                    break;
                case MoverAnimator.Direction.W:
                    animator.SetFloat("DirX", -1.0f);
                    animator.SetFloat("DirY", 0.0f);
                    break;
                case MoverAnimator.Direction.NE:
                    animator.SetFloat("DirX", 1.0f);
                    animator.SetFloat("DirY", -1.0f);
                    break;
                case MoverAnimator.Direction.NW:
                    animator.SetFloat("DirX", -1.0f);
                    animator.SetFloat("DirY", -1.0f);
                    break;
                case MoverAnimator.Direction.SE:
                    animator.SetFloat("DirX", 1.0f);
                    animator.SetFloat("DirY", 1.0f);
                    break;
                case MoverAnimator.Direction.SW:
                    animator.SetFloat("DirX", -1.0f);
                    animator.SetFloat("DirY", 1.0f);
                    break;
            }
        }

        // Animation
        if (reverseSpriteRenderer && spriteRenderer)
        {
            spriteRenderer.flipX = direction == MoverAnimator.Direction.W || direction == MoverAnimator.Direction.N || direction == MoverAnimator.Direction.NW || direction == MoverAnimator.Direction.SW;
        }
    }

    // Initialisation
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    private void OnValidate()
    {
        if (walkingPoint == null)
            walkingPoint = this.transform;
    }
}
