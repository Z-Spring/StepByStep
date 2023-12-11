using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pool
{
    public class FloorPool : MonoBehaviour
    {
        public static FloorPool Instance { get; private set; }

        [SerializeField] int initFloorNum;
        [SerializeField] GameObject floorPrefab;

        readonly Queue<GameObject> floorPool = new();

        private void Awake()
        {
            Instance = this;
            InitPool();
        }

        void InitPool()
        {
            SpawnFloor(initFloorNum);
        }

        void SpawnFloor(int num)
        {
            for (int i = 0; i < num; i++)
            {
                GameObject go = Instantiate(floorPrefab,transform);
                go.SetActive(false);
                floorPool.Enqueue(go);
            }
        }

        public void ReturnFloor(GameObject floor)
        {
            floorPool.Enqueue(floor);
            floor.SetActive(false);
        }

        public GameObject GetFloorFromPool()
        {
            if (floorPool.Count == 0)
            {
                SpawnFloor(1);
            }

            GameObject go = floorPool.Dequeue();
            go.SetActive(true);
            return go;
        }
    }
}