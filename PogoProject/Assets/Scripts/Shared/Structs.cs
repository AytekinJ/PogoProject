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
    public int hp;
    public int level;
    public float range;
    public void DefineSpecifies(EnemyType enemyType, int hp, int level,float range)
    {
        this.hp = hp;
        this.level = level;
        this.enemyType = enemyType;
        this.range = range;
    }
}
