
using System.Collections.Generic;

public class EnemyData : Savable
{
    public List<EnemyAttributes> allEnemies;

    public EnemyData()
    {
        allEnemies = new List<EnemyAttributes>();
    }

    public override string ToString()
    {
        string output = "{";
        for (int a = 0; a < allEnemies.Count; ++a)
        {
            output += allEnemies[a].ToString() + " ";
        }
        output += "}";
        return output;
    }
}

[System.Serializable]
public class EnemyAttributes
{
    public string name;
    public float moveSpeed;
    public float targetDistance;
    public int health;
    public float attackSpeed;


    public override string ToString()
    {
        string output = "Enemy " + name + " has " + health + " health, a moveSpeed of " + moveSpeed +
            ", an attack speed of " + attackSpeed + 
            ", and a target distance of " + targetDistance + " units.";
        return output;
    }
}