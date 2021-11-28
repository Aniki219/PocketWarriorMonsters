using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class BattleMessageController : MonoBehaviour
{
    private SpriteRenderer nextIcon;
    private Transform textObject;
    private TextAnimatorPlayer player;
    private TextAnimator textAnimator;
    private Animator anim;
    [SerializeField] BattleController battleController;

    public int maxWidth = 450;
    public int numberOfLines = 1;

    //Typewriter speeds
    float fastSpeed = 4;
    float regularSpeed = 1;

    void Awake()
    {
        nextIcon = transform.Find("nextIcon").GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        textObject = transform.Find("Text");
        textObject.gameObject.SetActive(true);
        player = textObject.GetComponent<TextAnimatorPlayer>();
        textAnimator = textObject.GetComponent<TextAnimator>();
        textAnimator.onEvent += OnEvent;
    }

    private void OnDestroy()
    {
        textAnimator.onEvent -= OnEvent;
    }

    private void OnEvent(string message)
    {
        switch (message)
        {
            case "zoom":
                int i = BattleController.currentTargetIndex;
                Debug.Log(i);
                FieldSlotController target;
                if (i < 3) {
                    target = battleController.allyFieldSlots[i];
                } else
                {
                    target = battleController.enemyFieldSlots[i-3];
                }
                BattleController.cam.SetTarget(target.transform.position);
                break;
            case "unzoom":
                BattleController.cam.Reset();
                break;
            default:
                throw new System.Exception("No handler for event " + message);
        }
    }


    private void Update()
    {
        //We can speedup the typewriter animation by holding "Confirm"
        player.SetTypewriterSpeed(InputManager.getKey("Confirm") ? fastSpeed : regularSpeed);
    }

    /* This function must call the animator trigger "Start" and "End" to make the 
     * message box appear and disappear.
     * Things to consider, what if we have a message nested in the middle of another.
     * I dont think this should happen, but I could see it mid battle..
     * */
    public async Task performScript(string script)
    {
        //Tells the message box to FadeIn
        anim.SetTrigger("Start");

        //We need to wait for the message box to appear.
        //Just using raw seconds for now, could be programmatic about this in the future
        await Task.Delay(250);

        //The script comes as a list of strings. Each string should correspond to
        //one line of text.
        List<string> battleMessageScript = ScriptHelper.Parse(script, maxWidth);

        //Since the BattleMessage contains two lines of text at maximum
        //we increment by 2 lines at a time.
        for (int i = 0; i < battleMessageScript.Count; i+=numberOfLines)
        {
            string finaltext = battleMessageScript[i];
            if (finaltext.Equals("<br>"))
            {
                //Here we want to go to the next line and skip this
                //blank line. Since we increment by 2 we can first decrement by 1
                //to offset this
                i -= numberOfLines - 1;
                continue;
            }

            //There may not be a second line so we must check first
            //It doesn't matter if line 2 is just <br>. In fact we must display it
            //or it may grab another line that shouldn't be here yet
            for (int j = 1; j < numberOfLines; j++)
            {
                if (battleMessageScript.Count > i + j)
                {
                    finaltext += battleMessageScript[i + j];
                }
            }
            player.ShowText(finaltext);

            //Wait for message to finish writing
            await WaitFor.Event(player.onTextShowed);

            //if (battleMessageScript.Count >= i + 2)
            //{
                nextIcon.enabled = true;
            //}

            //After showing these two lines we display the nextIcon and wait for the
            //player to press 'A'.
            await WaitFor.ButtonPress("Confirm");

            nextIcon.enabled = false;

            //Adding this little delay at the end helps with the speedup control
            //Tapping "Confirm" shouldn't cause the text to write quickly like holding does
            await Task.Delay(100);
        }
        //Tells the message box to FadeOut shortly after removing text
        player.textAnimator.SetText("", true);
        await Task.Delay(100);
        anim.SetTrigger("End");
        await Task.Delay(200);
    }
}
