using Unity.Burst;
using UnityEngine;

public enum EnemyType
{
    Eagle,
    Cannon,
    Goomba
}
[BurstCompile]
public struct EnemyData
{
    public EnemyType enemyType;
    public float range;
    public void DefineSpecifies(EnemyType enemyType,float range)
    {
        this.enemyType = enemyType;
        this.range = range;
    }
}
