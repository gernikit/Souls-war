using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticleTypesForMob
{
    HealDone,
    BreakingHeart
}
public static class ParticleManager
{
    public static Dictionary<ParticleTypesForMob, GameObject> particlesForMobs; //for PERFOMANCE will make static???
    static ParticleManager()
    {
        particlesForMobs = new Dictionary<ParticleTypesForMob, GameObject>();
        foreach (ParticleTypesForMob el in System.Enum.GetValues(typeof(ParticleTypesForMob)))
        {
            particlesForMobs.Add(el, Resources.Load<GameObject>("Particles\\" + el.ToString()));
        }
    }
}
