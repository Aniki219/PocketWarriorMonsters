using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ScriptHelper
{
    /* Parses a Script into a series of messages
     * messages may not exceed a certain width (not sure how to check this)
     * */
    public static List<string> Parse(string script, int maxWidth)
    {
        TextGenerator generator = new TextGenerator();
        TextGenerationSettings settings = new TextGenerationSettings();

        settings.font = Resources.Load<Font>("Fonts/NintendoFont");

        //We return a list of strings, each representing a single line of text.
        List<string> messages = new List<string>();

        //First we split the input text into an array of words
        string[] words = script.Split(' ');

        //trueString is the text that compirses the entire line
        //cleanString removes any <tags> and is used for calculating line width
        string trueString = "";
        string cleanString = "";
        foreach (string word in words)
        {
            //First we check if adding the new word to the cleanString will exceed
            //the maximum text width.
            //We need to ignore any <tags> since their length should not effect the cutoff
            string checkString = cleanString + (word.Contains('<') ? "" : word);
            if (generator.GetPreferredWidth(checkString, settings) >= maxWidth)
            {
                //If we exceed the length of a single line, we add a breakpoint to the
                //return string, and start a new line by clearing the cleanString
                trueString += "<br>";
                cleanString = "";
            }
            //Then we add the new word to the trueString
            trueString += word + (word.Contains('<') ? "" : " ");
            //..and if it isn't a tag we also add it to the cleanString
            if (!word.Contains('<'))
            {
                cleanString += word + " ";
            } else
            {
                //If it was a tag and specifically the <br> tag, then we're starting a new line
                //and thus need to reset the cleanString for length calculations
                if (word.Contains("<br>"))
                { 
                    cleanString = "";
                }
            }
        }
        //At this point we have the trueString with all of the <br>'s set
        //We now use the <br>'s as a delimiter to split trueString into an array of lines
        string[] delim = new string[] { "<br>" };
        string[] lines = trueString.Split(delim, StringSplitOptions.None);

        //We add each individual line as an entry to the return list
        foreach (string line in lines)
        {
            //We also need to add the newline character back to the end of each line.
            messages.Add(line + "<br>");
        }

        //Debug
        //foreach (string m in messages)
        //{
        //    Debug.Log(m);
        //}

        //We return all of the lines in a List
        return new List<string>(messages);
    }
}

