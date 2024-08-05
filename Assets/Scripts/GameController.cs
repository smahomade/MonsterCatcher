using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {  FreeRoam, Battle, Dialog, Cutscene }
public class GameController : MonoBehaviour
{
    [SerializeField] private HeroControls heroControls;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private GameObject TimerText;
    [SerializeField] private Currency currency;
    [SerializeField] private GameObject backGroundMusic;
    public GameState state;

    public static GameController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        ConditionsDB.Init();
    }
    private void Start()
    {
        heroControls.OnEncountered += StartWildMonsterBattle;
        battleSystem.OnBattleOver += EndTurnBasedBattle;
        currency.coin = 0;

        heroControls.OnEnterVillainView += (Collider2D trainerCollider) =>
        {
            TrainerController trainer = trainerCollider.GetComponentInParent<TrainerController>();
            if (trainer != null)
            {
                state = GameState.Cutscene;
                StartCoroutine(trainer.TriggerVillainBattle(heroControls));
            }
        };

        DialogManager.Instance.onDisplayDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManager.Instance.onCeaseDialog += () =>
        {
            if (state == GameState.Dialog)
            {
                state = GameState.FreeRoam;
            }
        };
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            heroControls.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }

    void StartWildMonsterBattle()
    {
        state = GameState.Battle;
        //LoadAnimation();
        TimerText.SetActive(false);
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        backGroundMusic.gameObject.SetActive(false);
        MonsterParty playerParty = heroControls.GetComponent<MonsterParty>();
        Monster wildMonster = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildCrazyMonster();

        Monster wildMonsterCopy = new Monster(wildMonster.Base, wildMonster.Lvl);

        battleSystem.StartTurnBasedBattle(playerParty,wildMonsterCopy);
    }

    TrainerController villain;
    public void StartVillainBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        backGroundMusic.gameObject.SetActive(false);
        this.villain = trainer;
        MonsterParty playerParty = heroControls.GetComponent<MonsterParty>();
        MonsterParty trainerParty = trainer.GetComponent<MonsterParty>();
        battleSystem.StartTurnBasedVillainBattle(playerParty, trainerParty);
    }

    void EndTurnBasedBattle(bool won)
    {
        if (villain != null && won == true)
        {
            villain.LostBattle();
            villain = null;
        }

        state = GameState.FreeRoam;
        TimerText.SetActive(true);
        backGroundMusic.gameObject.SetActive(true);
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }
}

