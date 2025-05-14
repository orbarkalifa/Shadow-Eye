using UnityEngine;

namespace Suits
{  
    
    public abstract class SuitAbility : ScriptableObject
    {
        public float cooldownTime;
        public abstract void ExecuteAbility(GameObject character);
    }
}