using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdditiveAction : Action
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerInput>().btnAdditiveAction = Use;
        other.GetComponent<PlayerInput>().btnAdditive.interactable = true;
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<PlayerInput>().btnAdditiveAction = null;
        other.GetComponent<PlayerInput>().btnAdditive.interactable = false;
    }

    public virtual void Use() { }
}
