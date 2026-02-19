using System.Collections.Generic;
using UnityEngine;

namespace EntropySyndicate.Utils
{
    public class DeterministicObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int initialSize = 32;

        private readonly Queue<GameObject> _available = new Queue<GameObject>(128);
        private Transform _root;

        public void Configure(GameObject poolPrefab, int preload)
        {
            prefab = poolPrefab;
            initialSize = preload;
        }

        private void Awake()
        {
            _root = new GameObject(name + "_PoolRoot").transform;
            _root.SetParent(transform);
            Warm();
        }

        private void Warm()
        {
            if (prefab == null)
            {
                return;
            }

            for (int i = 0; i < initialSize; i++)
            {
                GameObject spawned = Instantiate(prefab, _root);
                spawned.SetActive(false);
                _available.Enqueue(spawned);
            }
        }

        public GameObject Get(Vector3 position)
        {
            GameObject instance = _available.Count > 0 ? _available.Dequeue() : Instantiate(prefab, _root);
            instance.transform.position = position;

            PooledObject pooledObject = instance.GetComponent<PooledObject>();
            if (pooledObject == null)
            {
                pooledObject = instance.AddComponent<PooledObject>();
            }

            pooledObject.Bind(this);
            instance.SetActive(true);
            return instance;
        }

        public void Release(GameObject instance)
        {
            if (instance == null)
            {
                return;
            }

            instance.SetActive(false);
            instance.transform.SetParent(_root, false);
            _available.Enqueue(instance);
        }
    }
}
