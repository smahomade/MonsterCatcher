using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HeroControls : MonoBehaviour
{
    [SerializeField] string _name;

    private Vector2 movementControlsInput;
    public event Action OnEncountered;
    public event Action<Collider2D> OnEnterVillainView;
    private Character character;

    private void Awake() {character = GetComponent<Character>();}

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            //horizonal movement
            movementControlsInput.x = Input.GetAxisRaw("Horizontal");
            // vertical movement
            movementControlsInput.y = Input.GetAxisRaw("Vertical");
           
           
            // removing diagonal movement
            if (movementControlsInput.x != 0)
                movementControlsInput.y = 0;

            if (movementControlsInput != Vector2.zero)
                StartCoroutine(character.Move(movementControlsInput,OnMoveOver));
        }

        character.HandleUpdate();
        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
    }

    public void StopMoving()
    {
        character.IsMoving = false;
        character.HandleUpdate();
    }
    
    public bool InContactWithFence;
    void Interact()
    {
        Vector3 heroFacingDirection = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        Vector3 interactPos = transform.position + heroFacingDirection;

        Collider2D heroColliding = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.n.InteractableLayer);
        if (heroColliding != null)
            heroColliding.GetComponent<Interactable>()?.Interact(transform); //the question mark is a null component, so even if it returns a null this code wont crash
    }

    private void OnMoveOver()
    {
        //CheckFence();
        RandomEncounter();
        CheckIfInVillainView();
    }

    private void RandomEncounter()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.n.WildGrassLayer) != null)
        {
            //Debug.Log("Test2");
           if (UnityEngine.Random.Range(1, 10) == 1)
            {
                character.Animator.IsMoving =  false;
                OnEncountered();
            }
        }
    }

    private void CheckIfInVillainView()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.n.FovLayer);
        if (collider != null)
        {
            character.Animator.IsMoving = false;
            OnEnterVillainView?.Invoke(collider);
        }
    }

    public string Name
    {
        get => _name;
    }
}