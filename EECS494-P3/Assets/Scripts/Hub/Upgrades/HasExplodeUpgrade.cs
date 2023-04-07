using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasExplodeUpgrade : MonoBehaviour
{
    [SerializeField] bool oneShotEnemies = false;
    public float explosiveRadius;
    private int numBombs = 4;
    private float bombFrequency;
    private Subscription<PlayerDodgeEvent> dodgeEvent;
    GameObject bomb;

    // Start is called before the first frame update
    void Start()
    {
        dodgeEvent = EventBus.Subscribe<PlayerDodgeEvent>(_OnDodge);

        // Set how often bombs are dropped along trail
        bombFrequency = IsPlayer.instance.gameObject.GetComponent<PlayerController>().dodgeRollDuration / numBombs;
        Debug.Log("Bomb frequency: " + bombFrequency);

        bomb = Resources.Load<GameObject>("Prefabs/Weapons/Bomb");
    }

    // Explode on dash finish
    private void _OnDodge(PlayerDodgeEvent e)
    {
        if (e.start)
        {
            /*
            // Perform hit
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosiveRadius);

            foreach (Collider hit in hitColliders)
            {
                HasEnemyHealth enemyHit;
                HasPedestalHealth pedestalHit;

                if (hit.TryGetComponent<HasEnemyHealth>(out enemyHit))
                {
                    // Kill the enemy
                    int damageAmount = oneShotEnemies ? -1000 : -1;
                    enemyHit.AlterHealth(damageAmount);
                }
                else if (hit.TryGetComponent<HasPedestalHealth>(out pedestalHit))
                {
                    pedestalHit.AlterHealth(1);
                }
            }

            // Show visual
            
            // IsSprite spriteScaler = spawnedVisual.GetComponentInChildren<IsSprite>();
            // spriteScaler.scale = explosiveRadius * 2;
            

            // Destroy(spawnedVisual, explosion_anim.);
            StartCoroutine(ExplosionAnimation());
            */

            StartCoroutine(DropBombs());
        }
    }

    IEnumerator ExplosionAnimation()
    {
        // might want to change this visual to be ever-present but just deactivated
        GameObject visual = Resources.Load<GameObject>("Prefabs/EnemyWeapons/ExplosionRadius");
        GameObject spawnedVisual = Instantiate(visual, transform.position, Quaternion.identity);
        Animator animator = spawnedVisual.GetComponentInChildren<Animator>();
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // get the current state info
        while (stateInfo.normalizedTime < 1f) // wait for the animation to finish
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0); // get the updated state info
            yield return null;
        }
        Destroy(spawnedVisual);
        
    }

    IEnumerator DropBombs()
    {
        // Drop last bomb at last location
        yield return new WaitForSeconds(bombFrequency);
        // Drop the specified number of bombs at a constant frequency
        for (int i = 0; i < numBombs; i++)
        {
            Instantiate(bomb, transform);
            yield return new WaitForSeconds(bombFrequency);
        }
    }

    protected void OnDestroy()
    {
        EventBus.Unsubscribe(dodgeEvent);
    }
}
