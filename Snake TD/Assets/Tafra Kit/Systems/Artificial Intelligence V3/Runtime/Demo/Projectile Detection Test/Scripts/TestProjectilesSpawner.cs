using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.AI3.Demo
{
    public class TestProjectilesSpawner : FixedUpdateReceiver
    {
        [SerializeField] private FixedUpdateReceiver projectile;
        [SerializeField] private float spawnInterval = 0.25f;
        [SerializeField] private Transform target;

        [SerializeField] private bool orbit;
        [SerializeField] private float orbitDistance = 8;
        [SerializeField] private float orbitSpeed = 10;

        private float lastSpawnTime;
        private float curDegreeAngle;
        private List<FixedUpdateReceiver> projectiles = new List<FixedUpdateReceiver>();

        public override void FixedTick()
        {
            Vector3 targetPosition = target.position;
            targetPosition.y = transform.position.y;

            transform.LookAt(targetPosition);

            if (Time.time > lastSpawnTime + spawnInterval)
            {
                FixedUpdateReceiver spawnedProjecitle = Instantiate(projectile, transform.position, transform.rotation);
                spawnedProjecitle.gameObject.SetActive(true);
                projectiles.Add(spawnedProjecitle);
                lastSpawnTime = Time.time;
            }

            if (orbit)
            {
                curDegreeAngle += orbitSpeed * Time.deltaTime;

                float radians = Mathf.Deg2Rad * curDegreeAngle;

                transform.position = new Vector3(Mathf.Sin(radians) * orbitDistance, transform.position.y, Mathf.Cos(radians) * orbitDistance);
            }

            for (int i = 0; i < projectiles.Count; i++)
            {
                if (projectiles[i] != null)
                {
                    projectiles[i].FixedTick();
                }
                else
                {
                    projectiles.RemoveAt(i);
                    i--;
                }
            }
        }
        void Update()
        {
        }
    }
}