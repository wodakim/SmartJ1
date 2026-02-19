using UnityEngine;

namespace EntropySyndicate.Utils
{
    public class PooledObject : MonoBehaviour
    {
        private DeterministicObjectPool _ownerPool;

        public void Bind(DeterministicObjectPool owner)
        {
            _ownerPool = owner;
        }

        public void Release()
        {
            if (_ownerPool == null)
            {
                gameObject.SetActive(false);
                return;
            }

            _ownerPool.Release(gameObject);
        }
    }
}
