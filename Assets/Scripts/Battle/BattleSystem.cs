using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using static MonsterBase;


public enum BattleState { StartTurnBasedBattle, EventSelection, MoveSelection, RunningTurn, Busy, PartyScreenPopUp, TurnBasedBattleOver }
public enum BattleAction { Move, SwitchMonster, UseMonsterBall, Run }

public class BattleSystem : MonoBehaviour
{
    private bool UpKeyPressed => Input.GetKeyDown(upKey) || Input.GetKeyDown(upKeyAlt);
    private bool DownKeyPressed => Input.GetKeyDown(downKey) || Input.GetKeyDown(downKeyAlt);
    private bool LeftKeyPressed => Input.GetKeyDown(leftKey) || Input.GetKeyDown(leftKeyAlt);
    private bool RightKeyPressed => Input.GetKeyDown(rightKey) || Input.GetKeyDown(rightKeyAlt);
    private bool ConfirmKeyPressed => Input.GetKeyDown(confirmKey) || Input.GetKeyDown(confirmKeyAlt);
    private bool BackKeyPressed => Input.GetKeyDown(backKey) || Input.GetKeyDown(backKeyAlt);
    [Header("Battle System Controls")]
    private KeyCode upKey = KeyCode.UpArrow, upKeyAlt = KeyCode.W;
    private KeyCode downKey = KeyCode.DownArrow, downKeyAlt = KeyCode.S;
    private KeyCode leftKey = KeyCode.LeftArrow, leftKeyAlt = KeyCode.A;
    private KeyCode rightKey = KeyCode.RightArrow, rightKeyAlt = KeyCode.D;
    private KeyCode confirmKey = KeyCode.Z, confirmKeyAlt = KeyCode.KeypadEnter;
    private KeyCode backKey = KeyCode.X, backKeyAlt = KeyCode.Backspace;
    [Header("Resources")]
    [SerializeField] private BattleUnit heroMonsterUnit;
    [SerializeField] private BattleUnit enemyMonsterUnit;
    [SerializeField] private BattleDialogBox dialog;
    [SerializeField] private PartyScreen partyPopup;
    [SerializeField] private GameObject monsterballSprite;

    public event Action<bool> OnBattleOver;
    BattleState state;
    BattleState? prevState;

    int currEvent;
    int currMove;
    int currentMember;
    MonsterParty heroParty;
    MonsterParty villainParty;
    Monster crazyMonster;
    HeroControls hero;
    TrainerController villain;
    int escAttempts;

    bool isVillainBattle = false;



    public void StartTurnBasedBattle(MonsterParty heroParty, Monster crazyMonster)
    {
        this.heroParty = heroParty;
        this.crazyMonster = crazyMonster;
        hero = heroParty.GetComponent<HeroControls>();
        isVillainBattle = false;
        StartCoroutine(StartSetupBattle());
    }
    public void StartTurnBasedVillainBattle(MonsterParty heroParty, MonsterParty villainParty)
    {
        this.heroParty = heroParty;
        this.villainParty = villainParty;
        isVillainBattle = true;
        hero = heroParty.GetComponent<HeroControls>();
        villain = villainParty.GetComponent<TrainerController>();
        StartCoroutine(StartSetupBattle());
    }

    private void ClearAfterEffects()
    {
        heroMonsterUnit.ClearBattleHUD();
        enemyMonsterUnit.ClearBattleHUD();
        heroMonsterUnit.BattleHud.UpdateHP();
    }

    private IEnumerator CrazyMonsterApear()
    {
        heroMonsterUnit.Setup(heroParty.GetHealthyMonster());
        enemyMonsterUnit.Setup(crazyMonster);
        dialog.ImplementMoveNames(heroMonsterUnit.Monster.Moves);
        yield return dialog.WriteDialog($"A Wild {enemyMonsterUnit.Monster.Base.GetMonsterName()} appeared.");
    }

    private IEnumerator VillianMonsterApear()
    {
        heroMonsterUnit.gameObject.SetActive(false); // hide the player whilst enemy diaglog is occuring
        enemyMonsterUnit.gameObject.SetActive(true);
        Monster enemyMonster = villainParty.GetHealthyMonster();
        enemyMonsterUnit.Setup(enemyMonster);
        yield return dialog.WriteDialog($"{villain.Name} send out {enemyMonster.Base.GetMonsterName()} appeared.");
    }
    private IEnumerator HeroMonsterApear()
    {
        heroMonsterUnit.gameObject.SetActive(true);
        Monster playerMonster = heroParty.GetHealthyMonster();
        heroMonsterUnit.Setup(playerMonster);
        yield return dialog.WriteDialog($"Go {playerMonster.Base.GetMonsterName()}!");
        dialog.ImplementMoveNames(heroMonsterUnit.Monster.Moves);
    }

    public IEnumerator StartSetupBattle()
    {
        ClearAfterEffects();
        if (!isVillainBattle) // Crazy Monster Battle Code
                              // Sends out Crazy Wild Monster and Hero Monster
            yield return StartCoroutine(CrazyMonsterApear());
        else // Villain Battle Code
        {   // Villain Sends out first Monster
            yield return StartCoroutine(VillianMonsterApear());
            // Hero Sends out first Monster
            yield return StartCoroutine(HeroMonsterApear());
        }
        escAttempts = 0;
        // initalising the party screen
        partyPopup.Init();
        // determines who goes first by calculating the speed of the monster
        EventSelection();
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialog.ActivateEventSelector(false);
        dialog.ActivateDialogText(false);
        dialog.ActivateMoveSelector(true);
    }
    void PartyScreenPopUp()
    {
        state = BattleState.PartyScreenPopUp;
        partyPopup.ImplementPartyData(heroParty.Monsters);
        partyPopup.gameObject.SetActive(true);
    }

    void EventSelection()
    {
        state = BattleState.EventSelection;
        StartCoroutine(dialog.WriteDialog("Choose an Action"));
        dialog.ActivateEventSelector(true);
    }

    void TurnBasedBattleOver(bool won)
    {
        state = BattleState.TurnBasedBattleOver;
        heroParty.Monsters.ForEach(p => p.WhenTurnBasedBattleOver());
        OnBattleOver(won);
    }

    bool WhoGoesFirst()
    {
        // Check who goes first
        int herosTurnPrio = heroMonsterUnit.Monster.CurrentMove.Base.GetMovePriority();
        int villainTurnPrio = enemyMonsterUnit.Monster.CurrentMove.Base.GetMovePriority();

        // Check who goes first
        bool heroIsFirst = true;
        if (villainTurnPrio > herosTurnPrio)
            heroIsFirst = false;
        else if (villainTurnPrio == herosTurnPrio)
            heroIsFirst = heroMonsterUnit.Monster.GetSpeed() >= enemyMonsterUnit.Monster.GetSpeed();

        return heroIsFirst;
    }

    IEnumerator HerosTurn(BattleAction heroAction)
    {
        switch (heroAction)
        {
            case BattleAction.SwitchMonster:
                var selectedMonster = heroParty.Monsters[currentMember];
                state = BattleState.Busy;
                yield return SwitchMonster(selectedMonster);
                break;

            case BattleAction.UseMonsterBall:
                dialog.ActivateEventSelector(false);
                yield return ThrowMonsterBall();
                break;
            case BattleAction.Run:
                yield return AttemptToRun();
                break;
        }

        // Enemy Turn
        Move enemyMove = enemyMonsterUnit.Monster.GetRandomMove();

        yield return DuringMove(enemyMonsterUnit, heroMonsterUnit, enemyMove);
        yield return FleeAfterTurnEnds(enemyMonsterUnit);
        if (state == BattleState.TurnBasedBattleOver) yield break;
    }

    public Move EnemyMoveCounteringType()
    {
        MonsterType enemyType = enemyMonsterUnit.Monster.Base.GetMonsterType();
        MonsterType heroType = heroMonsterUnit.Monster.Base.GetMonsterType();
        Move enemyCounterAttack = enemyMonsterUnit.Monster.Moves[1];
        Move enemyRandomAttack = enemyMonsterUnit.Monster.GetRandomMove();


        //Enemy Is Fire Monster
        if (enemyType == MonsterType.Fire)
        {
            // Player is Plant or Wind
            if (heroType == MonsterType.Plant || heroType == MonsterType.Wind)
                return enemyCounterAttack;
            else
                return enemyRandomAttack;
        }

        //Enemy Is Plant Monster
        else if (enemyType == MonsterType.Plant)
        {
            // Player is Water or Electric
            if (heroType == MonsterType.Water || heroType == MonsterType.Electric)
                return enemyCounterAttack;
            else
                return enemyRandomAttack;
        }

        //Enemy Is Earth Monster
        else if (enemyType == MonsterType.Earth)
        {
            // Player is Fire or Electric
            if (heroType == MonsterType.Fire || heroType == MonsterType.Electric)
                return enemyCounterAttack;
            else
                return enemyRandomAttack;
        }

        //Enemy Is Wind Monster
        else if (enemyType == MonsterType.Wind)
        {
            // Player is Water or Electric
            if (heroType == MonsterType.Plant || heroType == MonsterType.Earth)
                return enemyCounterAttack;
            else
                return enemyRandomAttack;
        }

        //Enemy Is Water Monster
        else if (enemyType == MonsterType.Water)
        {
            // Player is Fire or Earth
            if (heroType == MonsterType.Fire || heroType == MonsterType.Earth)
                return enemyCounterAttack;
            else
                return enemyRandomAttack;
        }

        //Enemy Is Electric
        else if (enemyType == MonsterType.Electric)
        {
            // Player is Water or Wind
            if (heroType == MonsterType.Water || heroType == MonsterType.Wind)
                return enemyCounterAttack;
            else
                return enemyRandomAttack;
        }

        //Enemy Is Light Monster
        else if (enemyType == MonsterType.Light)
        {
            // Player is Dark
            if (heroType == MonsterType.Dark)
                return enemyCounterAttack;
            else
                return enemyRandomAttack;
        }

        //Enemy Is Dark Monster
        else if (enemyType == MonsterType.Dark)
        {
            // Player is Light
            if (heroType == MonsterType.Light)
                return enemyCounterAttack;
            else
                return enemyRandomAttack;
        }
        else
            return enemyRandomAttack;
    }



    IEnumerator DuringTurn(BattleAction heroAction)
    {


        state = BattleState.RunningTurn;
        if (heroAction == BattleAction.Move)
        {
            heroMonsterUnit.Monster.CurrentMove = heroMonsterUnit.Monster.Moves[currMove];

            if (isVillainBattle)
                enemyMonsterUnit.Monster.CurrentMove = enemyMonsterUnit.Monster.GetRandomMove();
            else if (enemyMonsterUnit.Monster.Lvl >= 5)
            {
                enemyMonsterUnit.Monster.CurrentMove = EnemyMoveCounteringType();
            }
            else
                
                enemyMonsterUnit.Monster.CurrentMove = enemyMonsterUnit.Monster.GetRandomMove();


            BattleUnit firstUnit = (WhoGoesFirst()) ? heroMonsterUnit : enemyMonsterUnit;
            BattleUnit secondUnit = (WhoGoesFirst()) ? enemyMonsterUnit : heroMonsterUnit;


            Monster secondMonster = secondUnit.Monster;

            // First Turn Dependent upon Speed
            yield return DuringMove(firstUnit, secondUnit, firstUnit.Monster.CurrentMove);
            yield return FleeAfterTurnEnds(firstUnit);
            if (state == BattleState.TurnBasedBattleOver) yield break;

            if (secondMonster.HP > 0)
            {
                // Second Turn Dependent upon Speed
                yield return DuringMove(secondUnit, firstUnit, secondUnit.Monster.CurrentMove);
                yield return FleeAfterTurnEnds(secondUnit);
                if (state == BattleState.TurnBasedBattleOver) yield break;
            }
        }
        else
            yield return StartCoroutine(HerosTurn(heroAction));

        if (state != BattleState.TurnBasedBattleOver)
            EventSelection();
    }

    IEnumerator StatusEffectsOn(BattleUnit sourceUnit)
    {
        bool canRunMove = sourceUnit.Monster.OnBeforeMove();
        if (!canRunMove)
        {
            yield return CheckAndShowStatusChanging(sourceUnit.Monster);
            yield return sourceUnit.BattleHud.UpdateHP();
            yield break;
        }
    }

    IEnumerator DuringMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        yield return StartCoroutine(StatusEffectsOn(sourceUnit));

        yield return CheckAndShowStatusChanging(sourceUnit.Monster);
        move.PP--;
        yield return dialog.WriteDialog($"{sourceUnit.Monster.Base.GetMonsterName()} used {move.Base.GetMoveName()}");

        if (DidAttackHit(move, sourceUnit.Monster, targetUnit.Monster))
        {
            switch (move.Base.GetCategory())
            {
                case MoveCategory.Status:
                    yield return EffectsDuringMove(move.Base.GetEffects(), sourceUnit.Monster, targetUnit.Monster, move.Base.GetTarget());
                    break;

                default:
                    DamageDetails damageDetails = targetUnit.Monster.RecieveDamageFromMonster(move, sourceUnit.Monster);
                    yield return targetUnit.BattleHud.UpdateHP();
                    yield return ShowDamageCalculations(damageDetails);
                    break;
            }

            if (move.Base.GetSecondaryEffect() != null && move.Base.GetSecondaryEffect().Count > 0 && targetUnit.Monster.HP > 0)
            {
                foreach (SecondaryEffect secondary in move.Base.GetSecondaryEffect())
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd < secondary.GetChance())
                        yield return EffectsDuringMove(secondary, sourceUnit.Monster, targetUnit.Monster, secondary.GetTarget());
                }
            }

            if (targetUnit.Monster.HP <= 0)
                yield return HandleFaintedMonster(targetUnit);
        }
        else
            yield return dialog.WriteDialog($"{sourceUnit.Monster.Base.GetMonsterName()}'s Attack Missed");
    }
    IEnumerator EffectsDuringMove(MoveEffects effects, Monster srcMonster, Monster targetMonster, MoveTarget moveTarget)
    {
        // Stat Boosting
        if (effects.GetBoosts() != null)
        {
            switch (moveTarget)
            {
                case MoveTarget.Self:
                    srcMonster.RegisterTheIncreasedStats(effects.GetBoosts());
                    break;
                default:
                    targetMonster.RegisterTheIncreasedStats(effects.GetBoosts());
                    break;
            }
        }
        // Status Condition
        if (effects.GetStatus() != ConditionID.none)
            targetMonster.ImplementStatus(effects.GetStatus());

        // Volatile Status Condition
        if (effects.GetVolatileStatus() != ConditionID.none)
            targetMonster.SetVolatileStatus(effects.GetVolatileStatus());

        yield return CheckAndShowStatusChanging(srcMonster);
        yield return CheckAndShowStatusChanging(targetMonster);
    }
    IEnumerator FleeAfterTurnEnds(BattleUnit srcMonster)
    {
        if (state == BattleState.TurnBasedBattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        // Psn and Burn will damage target
        srcMonster.Monster.AfterTheTurnEnds();
        yield return CheckAndShowStatusChanging(srcMonster.Monster);
        yield return srcMonster.BattleHud.UpdateHP();
        if (srcMonster.Monster.HP <= 0)
        {
            yield return HandleFaintedMonster(srcMonster);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }
    bool DidAttackHit(Move moves, Monster srcMonster, Monster targetMonster)
    {
        if (moves.Base.GetGuaranteeHit())
            return true;

        int precision = srcMonster.StatBoosts[Stat.Accuracy], dodge = targetMonster.StatBoosts[Stat.Evasion];
        float[] raiseValues = new float[] { 1, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };
        float movePrecision = moves.Base.GetAccuracy();

        switch (precision)
        {
            case > 0:
                movePrecision *= raiseValues[precision];
                movePrecision /= raiseValues[dodge];
                break;
            case <= 0:
                movePrecision /= raiseValues[-precision];
                movePrecision *= raiseValues[-dodge];
                break;
        }
        return UnityEngine.Random.Range(1, 101) <= movePrecision;
    }

    IEnumerator CheckAndShowStatusChanging(Monster monster)
    {
        while (monster.StatusChanges.Count > 0)
        {
            String message = monster.StatusChanges.Dequeue();
            yield return dialog.WriteDialog(message);
        }
    }

    IEnumerator XpGain(BattleUnit faintedMonster)
    {
        int xpYield = faintedMonster.Monster.Base.GetExpYield(), monsterLvl = faintedMonster.Monster.Lvl;
        float villainBonus = (isVillainBattle) ? 1.5f : 1f;
        int xpObtained = Mathf.FloorToInt((xpYield * monsterLvl * villainBonus) / 7);
        heroMonsterUnit.Monster.Xp += xpObtained;
        yield return dialog.WriteDialog($"{heroMonsterUnit.Monster.Base.GetMonsterName()} gained {xpObtained} exp");
        yield return heroMonsterUnit.BattleHud.ImplementXpTransitionSmoothly();
    }

    IEnumerator CheckLvlUp()
    {
        while (heroMonsterUnit.Monster.CheckForLevelUp())
        {
            heroMonsterUnit.BattleHud.ImplementLvl();
            yield return dialog.WriteDialog($"{heroMonsterUnit.Monster.Base.GetMonsterName()} grew to level {heroMonsterUnit.Monster.Lvl}");
            // Try to learn a new Move
            LearnableMove newMove = heroMonsterUnit.Monster.GetLearnableMoveAtCurrLevel();
            if (newMove != null)
            {
                if (heroMonsterUnit.Monster.Moves.Count < MonsterBase.MaxNumOfMoves)
                {
                    heroMonsterUnit.Monster.LearnMove(newMove);
                    yield return dialog.WriteDialog($"{heroMonsterUnit.Monster.Base.GetMonsterName()} learned {newMove.GetBase().GetMoveName()}");
                    dialog.ImplementMoveNames(heroMonsterUnit.Monster.Moves);
                }
            }
            yield return heroMonsterUnit.BattleHud.ImplementXpTransitionSmoothly(true);
        }
    }

    IEnumerator HandleFaintedMonster(BattleUnit faintedMonster)
    {
        yield return dialog.WriteDialog($"{faintedMonster.Monster.Base.GetMonsterName()} Fainted");
        yield return new WaitForSeconds(2f);

        if (!faintedMonster.IsHero)
        {
            // experience gained
            yield return StartCoroutine(XpGain(faintedMonster));

            // check to see to level
            yield return StartCoroutine(CheckLvlUp());
            yield return new WaitForSeconds(1f);
        }
        CheckTurnBasedBattleOver(faintedMonster);
    }

    void CheckTurnBasedBattleOver(BattleUnit faintedMonster)
    {
        int arrayNumberParty = 0;
        Monster nextMonster = null;
        try { nextMonster = faintedMonster.IsHero ? heroParty.GetHealthyMonster() : villainParty.GetHealthyMonster(); }
        catch (NullReferenceException) { }

        if (faintedMonster.IsHero && !isVillainBattle)
        {
            if (nextMonster != null) PartyScreenPopUp();    // This conditon checks if there is any Monsters with health
            else
            {
                TurnBasedBattleOver(false); // This ends the battle and records a loss
                Life.Instance.lifeCount -= nextMonster != null ? 0 : 1;
                for (int i = 0; i < heroParty.Monsters.Count; ++i)
                {
                    var prevMonster = heroParty.GetFaintedMonster(arrayNumberParty);
                    prevMonster.ResetHealthWhenLifeLost();
                    ++arrayNumberParty;
                }
            }

        }
        else if (faintedMonster.IsHero && isVillainBattle)
        {
            if (nextMonster != null) PartyScreenPopUp();    // This conditon checks if there is any Monsters with health
            else
            {
                TurnBasedBattleOver(false);
                for (int i = 0; i < heroParty.Monsters.Count; ++i)
                {
                    var prevMonster = heroParty.GetFaintedMonster(arrayNumberParty);
                    prevMonster.CapHPAtTenPercent();
                    ++arrayNumberParty;
                }
            }
        }
        else
        {
            if (isVillainBattle && nextMonster != null)
                StartCoroutine(SendNextVillainMonster());
            else
            {
                TurnBasedBattleOver(isVillainBattle || nextMonster == null);
                Currency.Instance.coin += !isVillainBattle ? 5 : nextMonster != null ? 0 : 150;
            }
        }

    }



    IEnumerator ShowDamageCalculations(DamageDetails damageDetails)
    {

        if (damageDetails.Critical > 1f)
            yield return dialog.WriteDialog("A Critical Hit!");

        switch (damageDetails.TypeEffectiveness)
        {
            case > 1f:
                yield return dialog.WriteDialog("Super Effective!");
                break;
            case < 1f:
                yield return dialog.WriteDialog("Not Very Effective!");
                break;
        }
    }

    public void HandleUpdate()
    {
        switch (state)
        {
            case BattleState.EventSelection:
                HandleEventSelection();
                break;

            case BattleState.MoveSelection:
                HandleMoveSelection();
                break;

            case BattleState.PartyScreenPopUp:
                HandlePartySelection();
                break;
        }
    }

    void HandleEventSelection()
    {
        if (RightKeyPressed)
            ++currEvent;
        else if (LeftKeyPressed)
            --currEvent;
        else if (DownKeyPressed)
            currEvent += 2;
        else if (UpKeyPressed)
            currEvent -= 2;

        currEvent = Mathf.Clamp(currEvent, 0, 3);
        dialog.UpdateEventSelection(currEvent);
        if (ConfirmKeyPressed)
        {
            switch (currEvent)
            {
                case 0:
                    MoveSelection(); // Attack
                    break;

                case 1:
                    StartCoroutine(DuringTurn(BattleAction.UseMonsterBall)); // Use MonsterBall
                    break;

                case 2:
                    prevState = state;
                    PartyScreenPopUp(); // Monster Party Screen
                    break;

                case 3:
                    StartCoroutine(DuringTurn(BattleAction.Run)); // Run
                    break;
            }
        }
    }

    void HandleMoveSelection()
    {
        if (RightKeyPressed)
            ++currMove;
        else if (LeftKeyPressed)
            --currMove;
        else if (DownKeyPressed)
            currMove += 2;
        else if (UpKeyPressed)
            currMove = -2;

        currMove = Mathf.Clamp(currMove, 0, heroMonsterUnit.Monster.Moves.Count - 1);

        dialog.UpdateMoveSelection(currMove, heroMonsterUnit.Monster.Moves[currMove]);

        if (ConfirmKeyPressed)
        {
            if (heroMonsterUnit.Monster.Moves[currMove].PP == 0)
                return;
            dialog.ActivateMoveSelector(false);
            dialog.ActivateDialogText(true);
            StartCoroutine(DuringTurn(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialog.ActivateMoveSelector(false);
            dialog.ActivateDialogText(true);
            EventSelection();
        }
    }

    void SwitchingMonstersDuringBattle()
    {
        Monster selectedMember = heroParty.Monsters[currentMember];
        if (selectedMember.HP <= 0)
        {
            partyPopup.ImplementMsgText("Cannot Play Fainted Monster");
            return;
        }
        if (selectedMember == heroMonsterUnit.Monster)
        {
            partyPopup.ImplementMsgText("That Monster is Already in Play");
            return;
        }
        if (BackKeyPressed)
        {
            return;
        }

        partyPopup.gameObject.SetActive(false);

        switch (prevState)
        {
            case BattleState.EventSelection:
                prevState = null;
                StartCoroutine(DuringTurn(BattleAction.SwitchMonster));
                break;

            default:
                state = BattleState.Busy;
                StartCoroutine(SwitchMonster(selectedMember));
                break;
        }
    }

    void HandlePartySelection()
    {
        if (RightKeyPressed)
            ++currentMember;
        else if (LeftKeyPressed)
            --currentMember;
        else if (DownKeyPressed)
            currentMember += 2;
        else if (UpKeyPressed)
            currentMember = -2;

        currentMember = Mathf.Clamp(currentMember, 0, heroParty.Monsters.Count - 1);
        partyPopup.UpdatePartyMonsterSelection(currentMember);

        if (ConfirmKeyPressed)
        {
            SwitchingMonstersDuringBattle();
        }
        else if (BackKeyPressed)
        {
            partyPopup.gameObject.SetActive(false);
            EventSelection();
        }
    }

    IEnumerator SwitchMonster(Monster newMonster)
    {
        if (heroMonsterUnit.Monster.MaxHP > 0)
        {
            yield return dialog.WriteDialog($"Come Back {heroMonsterUnit.Monster.Base.GetMonsterName()}");
            yield return new WaitForSeconds(2f);
        }
        heroMonsterUnit.Setup(newMonster);
        dialog.ImplementMoveNames(newMonster.Moves);
        yield return dialog.WriteDialog($"Go {newMonster.Base.GetMonsterName()}!");

        state = BattleState.RunningTurn;
    }

    IEnumerator SendNextVillainMonster()
    {
        state = BattleState.Busy;

        var nextMonster = villainParty.GetHealthyMonster();
        enemyMonsterUnit.Setup(nextMonster);
        yield return dialog.WriteDialog($"{villain.Name} send out {nextMonster.Base.GetMonsterName()}!");

        state = BattleState.RunningTurn;
    }

    IEnumerator CantStealVillainMonster()
    {
        yield return dialog.WriteDialog($"You can't steal the trainers pokemon!");
        state = BattleState.RunningTurn;
        yield break;
    }

    IEnumerator MonsterIsCaught(SpriteRenderer monsterballSprite)
    {
        // Monster is caught
        yield return dialog.WriteDialog($"{enemyMonsterUnit.Monster.Base.GetMonsterName()} was caught");
        yield return monsterballSprite.DOFade(0, 1.5f).WaitForCompletion();

        heroParty.AddMonster(enemyMonsterUnit.Monster);
        yield return dialog.WriteDialog($"{enemyMonsterUnit.Monster.Base.GetMonsterName()} has been added to your party");

        Destroy(monsterballSprite);
        TurnBasedBattleOver(true);
    }

    IEnumerator MonsterBrokeOut(int monsterballShakeTime, SpriteRenderer monsterballSprite)
    {
        // Monster broke out
        yield return new WaitForSeconds(1f);
        monsterballSprite.DOFade(0, 0.2f);
        yield return enemyMonsterUnit.PlayBreakOutAnimation();

        if (monsterballShakeTime < 2)
            yield return dialog.WriteDialog($"{enemyMonsterUnit.Monster.Base.GetMonsterName()} broke free");
        else
            yield return dialog.WriteDialog($"Almost caught it");

        Destroy(monsterballSprite);
        state = BattleState.RunningTurn;
    }


    IEnumerator ThrowMonsterBall()
    {
        state = BattleState.Busy;
        if (isVillainBattle)
        {
            yield return StartCoroutine(CantStealVillainMonster());
            yield break;
        }

        yield return dialog.WriteDialog($"{hero.Name} used Monsterball!");

        GameObject monsterballObj = Instantiate(this.monsterballSprite, heroMonsterUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        SpriteRenderer monsterballSprite = monsterballObj.GetComponent<SpriteRenderer>();

        // Animations
        yield return monsterballSprite.transform.DOJump(enemyMonsterUnit.transform.position + new Vector3(0, 2), 2f, 1, 1f).WaitForCompletion();
        yield return enemyMonsterUnit.AnimationUponCaptureWithMonsterBall();
        yield return monsterballSprite.transform.DOMoveY(enemyMonsterUnit.transform.position.y - 1.3f, 0.5f).WaitForCompletion();

        int monsterballShakeTimes = TryToCatchMonster(enemyMonsterUnit.Monster);

        for (int i = 0; i < Mathf.Min(monsterballShakeTimes, 3); ++i)
        {
            yield return new WaitForSeconds(0.5f);
            yield return monsterballSprite.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if (monsterballShakeTimes == 4)
            yield return StartCoroutine(MonsterIsCaught(monsterballSprite));
        else
            yield return StartCoroutine(MonsterBrokeOut(monsterballShakeTimes, monsterballSprite));
    }

    int TryToCatchMonster(Monster monster)
    {
        float n = (3 * monster.MaxHP - 2 * monster.HP) * monster.Base.CatchChance() * ConditionsDB.GetStatusBonus(monster.Status) / (3 * monster.MaxHP);

        if (n >= 255)
            return 4;

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / n));

        int monsterballShakeTimes = 0;
        while (monsterballShakeTimes < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;

            ++monsterballShakeTimes;
        }

        return monsterballShakeTimes;
    }

    IEnumerator CantRunFromVillain()
    {
        yield return dialog.WriteDialog($"You can't run from trainer battles!");
        state = BattleState.RunningTurn;
    }

    IEnumerator AttemptToRun()
    {
        state = BattleState.Busy;

        if (isVillainBattle)
            yield return StartCoroutine(CantRunFromVillain());

        ++escAttempts;

        int heroMonsterSpeed = heroMonsterUnit.Monster.GetSpeed();
        int enemyMonsterSpeed = enemyMonsterUnit.Monster.GetSpeed();

        if (enemyMonsterSpeed < heroMonsterSpeed)
        {
            yield return dialog.WriteDialog($"Ran away safely!");
            TurnBasedBattleOver(true);
        }
        else
        {
            float f = (heroMonsterSpeed * 128) / enemyMonsterSpeed + 30 * escAttempts;
            f %= 256;
            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialog.WriteDialog($"Ran away safely!");
                TurnBasedBattleOver(true);
            }
            else
            {
                yield return dialog.WriteDialog($"Can't escape!");
                state = BattleState.RunningTurn;
            }
        }
    }
}
