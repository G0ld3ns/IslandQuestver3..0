using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    [Header("Enemy prefabs")]
    public GameObject enemy1Prefab;
    public GameObject enemy2Prefab;
    public GameObject enemy3Prefab;

    public GameObject CreateRandomEnemy(Vector3 position, Transform parent)
    {
        int roll = Random.Range(0, 3);
        GameObject prefab = enemy1Prefab;

        if (roll == 1) prefab = enemy2Prefab;
        else if (roll == 2) prefab = enemy3Prefab;

        Quaternion rot = Quaternion.Euler(0, 180, 0);

        return Instantiate(prefab, position, rot, parent);
    }
}
