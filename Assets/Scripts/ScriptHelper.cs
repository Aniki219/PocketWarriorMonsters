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

        List<string> messages = new List<string>();

        int startIndex = 0;
        int line = 0;
        string message = "";
        for (int i = 0; i < script.Length; i++)
        {
            while (startIndex < script.Length && script[startIndex] == ' ')
            {
                startIndex++;
            }

            string substring = script.Substring(startIndex, i - startIndex);
            if (generator.GetPreferredWidth(substring, settings) >= maxWidth)
            {
                string stopCharacters = ".?!" + ((line == 0) ? " " : "");

                int r = i;
                while (r > startIndex && !stopCharacters.Contains(script[r]))
                {
                    r--;
                }
                if (r != startIndex)
                {
                    message += substring.Substring(0, r - startIndex) + script[r] + "<br>";
                }
                else
                {
                    message += substring.Substring(0, i - startIndex);
                }
                startIndex = r+1;

                if (line == 1)
                {
                    messages.Add(message);
                    message = "";
                }
                line++;
                line %= 2;
                
            }
            else
            {
                if (i == script.Length - 1)
                {
                    message += script.Substring(startIndex, i - startIndex);
                    messages.Add(message);
                }
            }
        }

        foreach (string m in messages)
        {
            Debug.Log(m);
        }

        return messages;
    }
}

