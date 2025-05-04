using UnityEngine;

namespace Suits
{  
    
    public abstract class SuitAbility : MonoBehaviour
    {
        public float cooldownTime;
        public abstract void ExecuteAbility(GameObject character);
    }
}