using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float movementSpd;

    public bool IsMoving { get; set; }

    //public float OffsetY { get; private set; } = 0f;

    CharacterAnimator animator;
    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
        SetPositionAndSnapToTile(transform.position);
    }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) /*+ 0.5f + OffsetY*/;
        transform.position = pos;
    }

    public IEnumerator Move(Vector2 moveVector, Action OnMoveOver = null)
    {
        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;

        if (!CheckIfPathIsClear(targetPos))
            yield break;

        IsMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, movementSpd * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        IsMoving = false;

        OnMoveOver?.Invoke();
    }
    public void LookTowardsPosition(Vector3 targetPos)
    {
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xdiff == 0 || ydiff == 0)
        {
            animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        }
        else
            Debug.LogError("Error in Look Towards: You can't ask the character to look diagonally");
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    private bool CheckIfPathIsClear(Vector3 characterPos)
    {
        Vector3 diff = characterPos - transform.position, dir = diff.normalized;

        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude-1f, GameLayers.n.ObjectLayer | GameLayers.n.InteractableLayer | GameLayers.n.HeroLayer) == true)
            return false;

        return true;
    }

    public CharacterAnimator Animator
    {
        get => animator;
    }
}
