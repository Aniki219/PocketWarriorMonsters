using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class overworldPokemonController : MonoBehaviour
{
    SpriteRenderer sprite;
    public string pokename = "pokeball";
    Sprite[] frames;
    public int facing = 0;
    int step = 0;
    int walkCycle = 2;

    public Vector2 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        setSprite();
        StartCoroutine(doLeftRight());
    }

    // Update is called once per frame
    void Update()
    {
        int index = facing*walkCycle + step;
        sprite.sprite = frames[index];
    }

    public void setSprite()
    {
        frames = Resources.LoadAll<Sprite>("Sprites/Pokemon/Overworld/" + pokename);
        if (frames.Length < 4)
        {
            Resources.LoadAll<Sprite>("Sprites/Pokemon/Overworld/pokeball");
        }
        walkCycle = frames.Length / 4;

        Debug.Log(frames[0].bounds.extents);
    }

    IEnumerator doLeftRight()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.5f);
            step++;
            step %= walkCycle;
        }
    }

    public void move()
    {
        StartCoroutine(moveCoroutine());
    }

    IEnumerator moveCoroutine()
    {
        float timeBetweenTiles = 0.3f;
        float startTime = Time.time;
        float elapsedTime = 0;

        Vector2 startPosition = transform.position;
        Vector2 destination = targetPosition;
        Vector2 direction = targetPosition - startPosition;

        if (direction.x != 0)
        {
            facing = direction.x > 0 ? 3 : 2;
        } else if (direction.y != 0)
        {
            facing = direction.y > 0 ? 1 : 0;
        }

        while (elapsedTime < timeBetweenTiles)
        {
            elapsedTime = Time.time - startTime;
            transform.position = Vector2.Lerp(startPosition, destination,
                elapsedTime / timeBetweenTiles);
            yield return new WaitForEndOfFrame();
        }
        yield return 0;
    }
}
