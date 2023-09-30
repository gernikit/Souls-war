using System;
using UnityEngine;

public class HealingAura : MonoBehaviour
{
    public event Action<Collider2D> AuraTriggerEnter;
    public event Action<Collider2D> AuraTriggerExit;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        AuraTriggerEnter?.Invoke(collider);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        AuraTriggerExit?.Invoke(collider);
    }
}
