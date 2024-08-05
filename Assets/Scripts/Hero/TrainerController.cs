using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable
{
    [SerializeField] string _name;
   // [SerializeField] Sprite sprite;
    [SerializeField] Dialog villainDialogWhenChallenged;
    [SerializeField] Dialog villainDialogWhenBattleComplete;
    [SerializeField] GameObject fov;
    [SerializeField] GameObject exclamationMark;
    [SerializeField] GameObject monsterballSign;

    bool turnbasedBattleLost = false;

    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.DefaultDirection);
    }

    private void Update()
    {
        character.HandleUpdate();
    }

    public void Interact(Transform initiator)
    {
        character.LookTowardsPosition(initiator.position);
        if (!turnbasedBattleLost)
        {
            StartCoroutine(DialogManager.Instance.DisplayDialog(villainDialogWhenChallenged, () =>
            {
                GameController.Instance.StartVillainBattle(this);
            }));
        }
        else
            StartCoroutine(DialogManager.Instance.DisplayDialog(villainDialogWhenBattleComplete));
    }

    public IEnumerator TriggerVillainBattle(HeroControls player)
    {
        // Show Exclamation
        monsterballSign.SetActive(false);
        exclamationMark.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        exclamationMark.SetActive(false);

        // Walk towards the player
        Vector3 diff = player.transform.position - transform.position;
        Vector3 moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return character.Move(moveVec);

        // Show dialog
       StartCoroutine(DialogManager.Instance.DisplayDialog(villainDialogWhenChallenged, () =>
        {
            GameController.Instance.StartVillainBattle(this);
        }));
    }

    public void LostBattle()
    {
        turnbasedBattleLost = true;
        fov.gameObject.SetActive(false);
    }

    public void SetFovRotation(FacingDirection dir)
    {

        float angle = dir == FacingDirection.Right ? 90f : (
            dir == FacingDirection.Up ? 180f : (
            dir == FacingDirection.Left ? 270f : 0));


        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public string Name
    {
        get => name;
    }
}
