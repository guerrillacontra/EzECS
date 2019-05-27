using UnityEngine;

namespace Ez.Scripts.Util
{
    
    /// <summary>
    /// A useful script to link a game object to an Entity so it is easier to find the root entity node in a complex
    /// hierarchy.
    /// </summary>
    public sealed class EzRelationship : MonoBehaviour
    {
        [SerializeField] private EzEntity _entity;
        
        public EzEntity Entity
        {
            get { return _entity; }
        }
    }
}