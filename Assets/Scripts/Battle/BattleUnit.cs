using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isHero;
    [SerializeField] BattleHud battleHud;

    public Monster Monster { get; set; }
    public BattleHud BattleHud{ get { return battleHud; } }
    public bool IsHero { get { return isHero; } }

    Image image;
    Vector3 orgPos;
    Color orgColour = new Color32(255, 255, 255, 255);
    private void Awake()
    {
        image = GetComponent<Image>();
        orgPos = image.transform.localPosition;
        image.color = orgColour;
    }

    public void ClearBattleHUD()
    {
        battleHud.gameObject.SetActive(false);
    }
    public void Setup(Monster monster)
    {

        Monster = monster;
        image.sprite = isHero ? Monster.Base.GetBackSprite() : Monster.Base.GetFrontSprite();

        battleHud.gameObject.SetActive(true);
        battleHud.ImplementData(monster);
        transform.localScale = new Vector3(1, 1, 1);
        image.color = orgColour;

        AnimationOnArrival();
    }
    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1, 0.5f));
        sequence.Join(transform.DOLocalMoveY(orgPos.y, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }
    public IEnumerator AnimationUponCaptureWithMonsterBall()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0, 0.5f));
        sequence.Join(transform.DOLocalMoveY(orgPos.y + 50f, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }


    public void AnimationOnArrival()
    {
        image.transform.localPosition = isHero ? new Vector3(-500f, orgPos.y) : new Vector3(500f, orgPos.y);
        image.transform.DOLocalMoveX(orgPos.x, 1f);
    }


}
