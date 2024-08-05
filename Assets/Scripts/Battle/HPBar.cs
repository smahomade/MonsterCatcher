using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject Hp;

    public void ImplementHP(float modifyHp)
    {
        Hp.transform.localScale = new Vector3(modifyHp, 1f);
    }

    public IEnumerator ImplementHPSmoothly(float newHp)
    {
       // IsUpdating = true;

        float curHp = Hp.transform.localScale.x, changeAmountHp = curHp - newHp;

        while (curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeAmountHp * Time.deltaTime;
            Hp.transform.localScale = new Vector3(curHp, 1f);
            yield return null;
        }
        Hp.transform.localScale = new Vector3(newHp, 1f);

        //IsUpdating = false;
    }
}
