using DG.Tweening;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    void Start()
    {
        var go = gameObject;
        var x = go.transform.DOMoveX(go.transform.position.x + 1, 1).SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);
    }

    public Tweener StartRunning()
    {
        var go = gameObject;
        return go.transform.DOMoveX(go.transform.position.x + 1, 1).SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);
    }
}