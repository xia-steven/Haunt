using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class IsSpacebarSprite : MonoBehaviour
{
    static float startTime = -1;

    [SerializeField] Sprite pressedSprite;
    [SerializeField] Sprite standardSprite;

    SpriteRenderer render;

    bool animate = true;

    Subscription<PlayerDodgeEvent> dodgeSub;

    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<SpriteRenderer>();

        if(startTime < 0)
        {
            startTime = Time.time;
        }

        dodgeSub = EventBus.Subscribe<PlayerDodgeEvent>(_OnPlayerDodge);


        StartCoroutine(animateSpace());
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(dodgeSub);
    }


    IEnumerator animateSpace()
    {
        // Make sure to start all spacebars at the same time
        yield return new WaitForSeconds(Time.time - startTime);

        while(animate)
        {
            render.sprite = standardSprite;

            yield return new WaitForSeconds(0.5f);

            render.sprite = pressedSprite;

            yield return new WaitForSeconds(0.5f);
        }
    }

    void _OnPlayerDodge(PlayerDodgeEvent pde)
    {
        // Destroy object on dodge
        Destroy(transform.parent.gameObject);
    }
}
