using Core.Extensions;
using System.Collections;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class TestStageSection : StageSection
    {
        [SerializeField] EnemyUnit unitToSpawn;
        protected override IEnumerator StartSection(float startingTime)
        {
            yield return new WaitForSeconds(3f);
            DungeonUnit iteration;
            for (int ii = 0; ii < 4; ii++)
            {
                for (int i = 0; i < 9; i++)
                {
                    Vector2 random = (Vector2)transform.position + Random.insideUnitCircle * 1.5f + new Vector2(2f.RandomPositiveNegativeRange(), 0f);
                    if (SpawnUnit(unitToSpawn, random, out iteration))
                    {
                        if (iteration is EnemyUnit enemy)
                        {
                            StagePath path = new(random, new Vector2[6]
                            {
                            new(2f,-3f),
                            new(-2f, -6f),
                            new(2,-9f),
                            new(-2f,-12f),
                            new(0,-15f),
                            new(6, -25f)
                            });
                            enemy.SetStagePath(path);
                        }
                    }
                    yield return new WaitForSeconds(0.35f);
                }
                yield return new WaitForSeconds(2f);
            }
        }
        private void Start()
        {
            if (gameObject.activeInHierarchy)
            {
                ActivateSection(0f);
            }
        }
    }
}
