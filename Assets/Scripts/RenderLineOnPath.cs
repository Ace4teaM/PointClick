using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dessine une RenderLine en suivant le chemin d'un DoTween Path
/// </summary>
/// <remarks>
/// AffectRotationZ permet d'orienter l'objet en fonction de l'avancement de l'aniamtion
/// </remarks>
[RequireComponent(typeof(DOTweenPath))]
[RequireComponent(typeof(LineRenderer))]
public class RenderLineOnPath : MonoBehaviour
{
    private DOTweenPath dtPath;
    private Tween pathTween;
    private LineRenderer lr;
    private Vector3 lastPoint;
    public float distanceThreshold = 0.5f;
    private List<Vector3> pointsToShow;
    public bool AffectRotationZ = true;

    void Start()
    {
        pointsToShow = new List<Vector3>();

        dtPath = GetComponent<DOTweenPath>();
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;   // commence vide

        pathTween = dtPath.tween;

        // S'abonner
        pathTween.OnUpdate(OnPathUpdate);
    }

    void OnDestroy()
    {
        // Se désabonner et tuer le tween si l’objet est détruit
        if (pathTween != null && pathTween.IsActive())
        {
            pathTween.OnUpdate(null); // supprime le callback
        }
    }

    void OnPathUpdate()
    {
        Vector3 pos = dtPath.transform.position;
        var percent = pathTween.ElapsedDirectionalPercentage();

        if (pointsToShow.Count == 0)
        {
            lastPoint = pos;
            pointsToShow.Add(pos);
        }
        else
        {
            // ajoute un point au bout d'un certain seuil
            if (Vector3.Distance(lastPoint, pos) >= distanceThreshold)
            {
                // Affecte la rotation pour suivre le chemin
                if (AffectRotationZ)
                {
                    float angleZ = lastPoint.AngleTo(pos);
                    transform.rotation = Quaternion.Euler(0f, 0f, angleZ);
                }

                pointsToShow.Add(pos);

                lr.positionCount = pointsToShow.Count;

                for (int i = 0; i < pointsToShow.Count; i++)
                {
                    lr.SetPosition(i, pointsToShow[i]);
                }
                lastPoint = pos;
            }
            // ajuste le dernier point pour suivre la courbe au plus précis
            else
            {
                if(lr.positionCount > 0)
                    lr.SetPosition(pointsToShow.Count-1, pos);
            }
        }
    }
}
