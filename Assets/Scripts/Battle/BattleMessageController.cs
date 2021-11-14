using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class BattleMessageController : MonoBehaviour
{
    private SpriteRenderer nextIcon;
    private TextAnimatorPlayer player;
    private Animator anim;
    void Start()
    {
        nextIcon = transform.Find("nextIcon").GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        player = GetComponentInChildren<TextAnimatorPlayer>();
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
        List<string> battleMessageScript = ScriptHelper.Parse(script, 500);

        //Since the BattleMessage contains two lines of text at maximum
        //we increment by 2 lines at a time.
        for (int i = 0; i < battleMessageScript.Count; i+=2)
        {
            string line1 = battleMessageScript[i];
            if (line1.Equals("<br>"))
            {
                //Here we want to go to the next line and skip this
                //blank line. Since we increment by 2 we can first decrement by 1
                //to offset this
                i--;
                continue;
            }

            //There may not be a second line so we must check first
            //It doesn't matter if line 2 is just <br>. In fact we must display it
            //or it may grab another line that shouldn't be here yet
            string line2 = "";
            if (battleMessageScript.Count > i+1)
            {
                line2 = battleMessageScript[i + 1];
            }
            player.ShowText(line1 + line2);

            //TODO: wait for message to finish writing...
            await WaitForEvent.getTask(player.onTextShowed);

            //if (battleMessageScript.Count >= i + 2)
            //{
                nextIcon.enabled = true;
            //}

            //After showing these two lines we display the nextIcon and wait for the
            //player to press 'A'.
            await WaitForButtonPress.getTask("Confirm");

            nextIcon.enabled = false;
        }
        //Tells the message box to FadeOut shortly after removing text
        player.textAnimator.SetText("", true);
        await Task.Delay(100);
        anim.SetTrigger("End");
        await Task.Delay(200);
    }
}
