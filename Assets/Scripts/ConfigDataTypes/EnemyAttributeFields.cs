using System.Collections.Generic;

public class EnemyData : Savable {
    public List<EnemyAttributes> allEnemies;

    public EnemyData() {
        allEnemies = new List<EnemyAttributes>();
    }

    public override string ToString() {
        var output = "{";

        foreach (var enemy in allEnemies) {
            output += enemy + " ";
        }

        output += "}";
        return output;
    }
}

[System.Serializable]
public class EnemyAttributes {
    public string name;
    public float moveSpeed;
    public float targetDistance;
    public int health;
    public float attackSpeed;
    public bool isRanged;
    public int minXThiefSpawn;
    public int maxXThiefSpawn;
    public int minZThiefSpawn;
    public int maxZThiefSpawn;
    public int maxThiefSpawnAttempt;
    public float windupSpeed;
    public float windupTime;
    public float dashSpeed;
    public float dashTime;
    public float swingTime;
    public float projectileSpeed;
    public float projectileLifetime;
    public float arbalestRotationSpeed;


    public override string ToString() {
        var output = "Enemy " + name + " has " + health + " health, a moveSpeed of " + moveSpeed +
                     ", an attack speed of " + attackSpeed +
                     ", is ranged bool: " + isRanged +
                     ", and a target distance of " + targetDistance + " units.";
        return output;
    }
}

public class EnemyWaveData : Savable
{
    public List<SpawnableEnemies> nightlyEnemies;
    public List<SpawnAttributes> enemySpawnData;

    public List<float> nightlyPropMelee;

    [System.Serializable]
    public struct SpawnAttributes
    {
        public string path;
        public int weight;
        public int cost;
        public bool isMelee;
    }
}

[System.Serializable]
public class SpawnableEnemies
{
    public List<int> indices;

    public int this[int key] 
    {
        get {
            return indices[key];
        }

        private set {

        }
    }
}