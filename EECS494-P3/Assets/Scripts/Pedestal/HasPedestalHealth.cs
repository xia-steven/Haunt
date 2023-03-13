using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IsPedestal))]
public class HasPedestalHealth : HasHealth
{

    [SerializeField] int PedestalMaxHealth = 5;
    private int currHealth;

    IsPedestal pedestal;

    private void Start()
    {
        pedestal = GetComponent<IsPedestal>();
        currHealth = PedestalMaxHealth;

    }

    // Update is called once per frame
    public override bool AlterHealth(int healthDelta)
    {
        if (currHealth == 0 && healthDelta < 0) return false;

        if (currHealth + healthDelta >= PedestalMaxHealth)
        {
            if(pedestal.IsDead)
            {
                pedestal.PedestalRepaired();
                currHealth = PedestalMaxHealth;
            }
            return false;
        }
        currHealth += healthDelta;

        if(currHealth <= 0)
        {
            if(!pedestal.IsDead)
            {
                pedestal.PedestalDied();
            }
            currHealth = 0;
        }

        pedestal.updateColor(currHealth, PedestalMaxHealth);

        return true;

    }

}
