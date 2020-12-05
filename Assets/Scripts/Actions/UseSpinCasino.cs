using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UseSpinCasino : AdditiveAction
{
    public Animation[] rolls;

    public override void Use()
    {
        StartCoroutine(SpinAutomat());
    }

    public IEnumerator SpinAutomat()
    {
        for (int i = 0; i < rolls.Length; i++)
        {
            rolls[i].Stop();
            int value = UnityEngine.Random.Range(0, 9);
            Debug.Log(String.Format("{0}-позиция {1}-дроп", i, value));
            yield return new WaitForSeconds(2f);
        }
        yield return new WaitForSeconds(5f);
        for (int i = 0; i < rolls.Length; i++)
        {
            rolls[i].Play();
        }
    }
}
