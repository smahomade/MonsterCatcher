using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern = 2;
    NPCState state;
    float idleTimer = 0f;
    int currentPattern = 0;
    Character character;

    private void Awake() {character = GetComponent<Character>();}
   
    private void Update()
    {
        if (state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }
        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.Walking;
        Vector3 oldPos = transform.position;
        yield return character.Move(movementPattern[currentPattern]);
        if (transform.position != oldPos)
            currentPattern = (currentPattern + 1) % movementPattern.Count;
        state = NPCState.Idle;
    }
    public void Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowardsPosition(initiator.position);
            StartCoroutine(DialogManager.Instance.DisplayDialog(dialog, () => {
                idleTimer = 0f;
                state = NPCState.Idle;
            }));
        }
        //StartCoroutine(character.Move(new Vector2(0, 2)));
    }
    public enum NPCState { Idle, Walking, Dialog }
}
