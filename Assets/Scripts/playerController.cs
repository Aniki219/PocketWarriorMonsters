using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    Animator anim;
    SpriteRenderer sprite;
    public Vector2 velocity;
    public Vector2 lastVelocity;

    public float lastDirection;
    overworldPokemonController[] pokemon;

    public enum State
    {
        Idle,
        Turning,
        Moving,
    }

    public State state;

    // Start is called before the first frame update
    void Start()
    {
        state = State.Idle;
        velocity = Vector2.zero;
        anim = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        pokemon = GameObject.Find("Followers").GetComponentsInChildren<overworldPokemonController>();
        for (int i = 0; i < pokemon.Length; i++)
        {
            pokemon[i].targetPosition = transform.position;
            pokemon[i].pokename = PlayerPokemon.instance
                .pokemon[i].name.ToString().ToLower();
            pokemon[i].setSprite();
        }
    }

    // Update is called once per frame
    void Update()
    {
        sprite.sortingOrder = 4+(int)Mathf.Ceil(-transform.position.y + 110);
        for (int i = 0; i < pokemon.Length; i++)
        {
            overworldPokemonController p = pokemon[i];
            p.GetComponent<SpriteRenderer>().sortingOrder = (int)Mathf.Ceil(-p.transform.position.y + 110) + 3-i;
        }
        switch(state)
        {
            case State.Idle:
                pollInput();
                startMove();
                break;
            case State.Moving:
                break;
            default:
                break;
        }
    }

    //If the player is able to input a direction
    //this method should be called to set velocity
    public void pollInput()
    {
        Vector2 inputDir = InputManager.getAxisState("DPad");

        if (inputDir.Equals(Vector2.zero))
        {
            velocity = Vector2.zero;
        } else if (inputDir.x != lastVelocity.x)
        {
            velocity = new Vector2(inputDir.x, 0);
        } else if (inputDir.y != lastVelocity.y)
        {
            velocity = new Vector2(0, inputDir.y);
        } else
        {
            if (inputDir.x != 0)
            {
                velocity = new Vector2(inputDir.x, 0);
            } else
            {
                velocity = new Vector2(0, inputDir.y);
            }
        }
        lastVelocity = velocity;
    }

    private void startMove()
    {
        bool isWalking = !velocity.Equals(Vector2.zero);
        anim.SetBool("isWalking", isWalking);
        if (isWalking)
        {
            if (state == State.Idle) changeState(State.Turning);

            float angle = Mathf.Atan2(velocity.y, velocity.x);
            float direction = ((angle / Mathf.PI / 2.0f + 2.0f) % 1.0f) * 1.3f;

            anim.SetFloat("Direction", direction);
            
            StartCoroutine(moveCoroutine(lastDirection != direction));
            lastDirection = direction;
        }
    }

    private IEnumerator moveCoroutine(bool turn = true)
    { 
        float timeBetweenTiles = 0.3f;
        float turnTime = 0.1f;
        float startTime = Time.time;
        float elapsedTime = 0;

        Vector2 startPosition = transform.position;
        Vector2 destination = startPosition + velocity;

        if (state != State.Moving)
        {
            while (turn && elapsedTime < turnTime)
            {
                elapsedTime = Time.time - startTime;
                yield return new WaitForEndOfFrame();
            }
            pollInput();
            changeState(State.Moving);
        }

        if (turn)
        {
            anim.SetTrigger("StaggerWalk");
        }

        if (velocity.Equals(Vector2.zero))
        {
            changeState(State.Idle);
            yield break;
        } else
        {
            updatePositionHistory();
        }

        startTime = Time.time;
        elapsedTime = 0;
        while (elapsedTime < timeBetweenTiles) {
            elapsedTime = Time.time - startTime;
            transform.position = Vector2.Lerp(startPosition, destination, 
                elapsedTime / timeBetweenTiles);
            yield return new WaitForEndOfFrame();
        }

        pollInput();
        if (velocity.Equals(Vector2.zero))
        {
            changeState(State.Idle);
            sprite.GetComponent<spriteController>().flipCycleOffset();
        } else
        {
            startMove();
        }
        yield return 0;
    }

    public void changeState(State newState)
    {
        state = newState;
    }

    void updatePositionHistory()
    {
        for (int i = pokemon.Length - 1; i > 0; i--)
        {
            pokemon[i].targetPosition = pokemon[i-1].targetPosition;
        }
        pokemon[0].targetPosition = transform.position;
        for (int i = 0; i < pokemon.Length; i++)
        {
            pokemon[i].move();
        }
    }
}

