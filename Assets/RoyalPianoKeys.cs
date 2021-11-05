using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class RoyalPianoKeys : MonoBehaviour
{
    public new KMAudio audio;
    public KMBombInfo bomb;
    public KMBombModule module;

    public KMSelectable[] pianoKeys;
    public KMSelectable[] foods;
    public SpriteRenderer[] selectedFoods;
    public Sprite[] availableFoods;
    public AudioClip[] pianoSounds;

    string[] melodyName = new string[]{"Godzilla's Theme", "A Town with an Ocean's View", "Take Me Home, Country Roads", "Shiny Smily Story", "Pirates of the Carribean", "Let it Be", "Für Elise", "Kiseki Knot" };
    int[][] melody = new int[][] { new int[] { 3, 2, 0, 3, 2, 0, 3, 2, 0, 10, 0, 2, 3, 2, 0, 5, 3, 2, 5, 3, 2, 5, 3, 2, 0, 2, 3, 5, 3, 2 }, new int[] { 2, 10, 2, 9, 2, 7, 5, 3, 5, 10, 0, 3, 7, 10, 9, 5, 2, 0, 2 }, new int[] { 10, 0, 2, 0, 10, 0, 2, 0, 10, 2, 5, 7, 7, 2, 5, 2, 0, 10, 0, 2, 2, 0, 10, 10, 0, 10 }, new int[] { 6, 8, 10, 11, 6, 1, 11, 10, 11, 1, 3, 11, 6, 1, 11, 10, 11, 1, 3 }, new int[] { 0, 3, 5, 5, 5, 7, 8, 8, 8, 10, 7, 7, 5, 3, 3, 5, 0, 3, 5, 5, 5, 7, 8, 8, 8, 10, 7, 7, 5, 3, 3, 5 }, new int[] { 7, 5, 3, 7, 10, 0, 10, 7, 5, 3, 0, 10, 7, 10, 7, 7, 8, 7, 7, 5, 7, 5, 5, 3 }, new int[] { 7, 6, 7, 6, 7, 2, 5, 3, 0, 3, 7, 0, 2, 7, 0, 2, 3, 7, 7, 6, 7, 6, 7, 2, 5, 3, 0 }, new int[] { 1, 1, 1, 1, 11, 2, 1, 11, 9, 9, 9, 9, 9, 8, 9, 11, 6, 8, 9, 4, 4, 6, 8, 9, 1, 1, 11, 11, 9, 9, 11, 11, 9 } }; 
    int[][] melodySequence = new int[][] { new int[] { 15, 2, 0, 15, 2, 0, 15, 2, 0, 10, 0, 2, 15, 2, 0, 17, 15, 2, 17, 15, 2, 17, 15, 2, 0, 2, 15, 17, 15, 2 }, new int[] { 14, 34, 14, 33, 14, 31, 29, 27, 29, 22, 12, 27, 31, 34, 33, 29, 14, 12, 14 }, new int[] { 22, 12, 14, 12, 22, 12, 14, 12, 22, 14, 29, 31, 31, 14, 29, 14, 12, 22, 12, 14, 14, 12, 22, 22, 12, 22 }, new int[] { 18, 20, 22, 23, 18, 13, 23, 22, 23, 13, 27, 23, 18, 13, 23, 22, 23, 13, 27 }, new int[] { 0, 15, 17, 17, 17, 19, 20, 20, 20, 22, 19, 19, 17, 15, 15, 17, 0, 15, 17, 17, 17, 19, 20, 20, 20, 22, 19, 19, 17, 15, 15, 17 }, new int[] { 19, 17, 15, 19, 22, 12, 22, 19, 17, 15, 0, 10, 19, 10, 19, 19, 20, 19, 19, 17, 19, 17, 17, 15 }, new int[] { 31, 30, 31, 30, 31, 14, 29, 27, 12, 15, 19, 12, 14, 19, 12, 14, 27, 19, 31, 30, 31, 30, 31, 14, 29, 27, 12 }, new int[] { 13, 13, 13, 13, 23, 14, 13, 23, 21, 21, 21, 21, 21, 20, 21, 23, 18, 20, 21, 16, 16, 18, 20, 21, 1, 13, 23, 23, 21, 21, 23, 23, 21 } };
    int[][] melodyBreak = new int[][] { new int[] { 5, 14, 20, 29 }, new int[] { 3, 8, 17, 18 }, new int[] { 5, 11, 19, 25 }, new int[] { 6, 10, 14, 18 }, new int[] { 9, 15, 25, 31 }, new int[] { 5, 12, 19, 23 }, new int[] { 8, 12, 16, 26 }, new int[] { 6, 15, 23, 32 } };

    private int[] foodSwitch = new int[3];
    private int[] takoyaki = new int[3];
    private int mood;
    private static int moduleIdCounter = 1;
    private int moduleId;
    private int chosenMelody;
    private int note = 0;
    private bool moduleActivated, moduleSolved; 

    void Awake()
    {
    	moduleId = moduleIdCounter++;
        for (int i = 0; i < foods.Length; i++)
        {
            int k = i;
            foods[k].OnInteract += () => { foodChanger(k); return false; };
        }
        for (int i = 0; i < pianoKeys.Length; i++)
        {
            int j = i;
            pianoKeys[j].OnInteract += () => { keyHandler(j); return false; };
        }
    }

    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            foodSwitch[i] = Random.Range(0, 6);
            Debug.LogFormat("[Royal Piano Keys #{0}] For food #{1} you should only switch {2} times.", moduleId, i + 1, foodSwitch[i]);
            foodChanger(i);
        }
        if (foodSwitch[0] == 0 && foodSwitch[1] == 0 && foodSwitch[2] == 0)
        {
            moodChecker();
        }
        mood = Enumerable.Sum(foodSwitch) / 4;
        moduleActivated = true;
    }

    void keyHandler(int k)
    {
        pianoKeys[k].gameObject.GetComponent<Animator>().SetTrigger("PressTrigger");
        bool input = true;
        for (int i = 0; i < 3; i++)
        {
           if (foodSwitch[i] != 0)
            {
                input = false;
            }
        }
        if (input == false)
        {
            audio.PlaySoundAtTransform(pianoSounds[k].name, transform);
            module.HandleStrike();
            Debug.LogFormat("[Royal Piano Keys #{0}] Strike! The princess is not satisfied with the food given yet.", moduleId);
        }
        else
        {
            if (!moduleSolved)
            {
                if (k == melody[chosenMelody][note])
                {
                    audio.PlaySoundAtTransform(pianoSounds[melodySequence[chosenMelody][note]].name, transform);
                    note++;
                    if (note >= melodyBreak[chosenMelody][mood] + 1)
                    {
                        audio.PlaySoundAtTransform("nanora", transform);
                        module.HandlePass();
                        Debug.LogFormat("[Royal Piano Keys #{0}] The princess is satisfied. Module solved nanora.", moduleId);
                        moduleSolved = true;
                    }
                }
                else
                {
                    audio.PlaySoundAtTransform(pianoSounds[k].name, transform);
                    module.HandleStrike();
                    Debug.LogFormat("[Royal Piano Keys #{0}] Strike! A wrong key was pressed.", moduleId);
                }
            }
            else
            {
                audio.PlaySoundAtTransform(pianoSounds[k].name, transform);
                module.HandleStrike();
                Debug.LogFormat("[Royal Piano Keys #{0}] This piano is only for entertainment purposes for the princess, it is not for you to play whenever you feel like. Strike.", moduleId);
            }
        }
    }

    void foodChanger(int k)
    {
        if (moduleActivated == false)
        {
            if (foodSwitch[k] == 0)
            {
                int i = Random.Range(20, 25);
                selectedFoods[k].sprite = availableFoods[i];
                takoyaki[k] = i - 19;
                Debug.LogFormat("[Royal Piano Keys #{0}] There are {1} \"takoyakis\" in food #{2}.", moduleId, takoyaki[k], k + 1);
            }
            else
            {
                int i = Random.Range(0, 20);
                selectedFoods[k].sprite = availableFoods[i];
            }
        }
        else
        {
            foods[k].AddInteractionPunch();
            if (foodSwitch[k] == 0)
            {
                module.HandleStrike();
                Debug.LogFormat("[Royal Piano Keys #{0}] Strike! This food is already one of those \"takoyakis\".", moduleId);
            }
            else
            {
                foodSwitch[k]--;
                audio.PlaySoundAtTransform("food change", transform);
                if (foodSwitch[k] == 0)
                {
                    int i = Random.Range(20, 25);
                    selectedFoods[k].sprite = availableFoods[i];
                    takoyaki[k] = i - 19;
                    Debug.LogFormat("[Royal Piano Keys #{0}] There are {1} \"takoyakis\" in food #{2}.", moduleId, takoyaki[k], k + 1);
                    moodChecker();
                }
                else
                {
                    int i = Random.Range(0, 20);
                    selectedFoods[k].sprite = availableFoods[i];
                }
            }
        }
    }

    void moodChecker()
    {
        bool input = true;
        for (int i = 0; i < 3; i++)
        {
            if (foodSwitch[i] != 0)
            {
                input = false;
            }
        }
        if (input == true)
        {
            Debug.LogFormat("[Royal Piano Keys #{0}] The food is now ready!", moduleId);
            string moodName = "";
            switch (mood)
            {
                case 0:
                    moodName = "calm";
                    break;
                case 1:
                    moodName = "pouting";
                    break;
                case 2:
                    moodName = "annoyed";
                    break;
                case 3:
                    moodName = "mad";
                    break;
            }
            Debug.LogFormat("[Royal Piano Keys #{0}] The princess' current mood is {1}, thus you should play {2} measure(s) of the melody.", moduleId, moodName, mood + 1);
            melodySelect();
        }
    }
    
    void melodySelect()
    {
        for (int i = 0; i < 8; i++)
        {
            switch (i)
            {
                case 0:
                    if ((takoyaki[0] > takoyaki[1]) && (takoyaki[0] > takoyaki[2])) { chosenMelody = 0; i = 8; } break;
                case 1:
                    if (Enumerable.Sum(takoyaki) >= 6 && Enumerable.Sum(takoyaki) <= 9) { chosenMelody = 1; i = 8; } break;
                case 2:
                    if (takoyaki[1] < 3) { chosenMelody = 2; i = 8; } break;
                case 3:
                    if (takoyaki[1] + takoyaki[2] > 8) { chosenMelody = 3; i = 8; } break; 
                case 4:
                    if ((takoyaki[0] == takoyaki[1]) && (takoyaki[0] == takoyaki[2]) && (takoyaki[1] == takoyaki[2])) { chosenMelody = 4; i = 8; } break; 
                case 5:
                    if (takoyaki[2] == 2 || takoyaki[2] == 3 || takoyaki[2] == 5) { chosenMelody = 5; i = 8; } break; 
                case 6:
                    if ((takoyaki[0] != 4) && (takoyaki[1] != 4) && (takoyaki[2] != 4)) { chosenMelody = 6; i = 8; } break; 
                case 7:
                    chosenMelody = 7; i = 8; break;
            }
        }
        Debug.LogFormat("[Royal Piano Keys #{0}] You should play {1}.", moduleId, melodyName[chosenMelody]);
    }

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} food <pos> [Presses the food in the specified position] | !{0} press <key> [Presses the specified key] | Valid food positions are 1-3 from left to right | Key presses are chainable, for ex: !{0} press Ab Db E";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*food\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify the position of the food you wish to press!";
            }
            else if (parameters.Length == 2)
            {
                int temp = -1;
                if (!int.TryParse(parameters[1], out temp))
                {
                    yield return "sendtochaterror!f The specified position '" + parameters[1] + "' is invalid!";
                    yield break;
                }
                if (temp < 1 || temp > 3)
                {
                    yield return "sendtochaterror The specified position '" + parameters[1] + "' is out of range 1-3!";
                    yield break;
                }
                foods[temp - 1].OnInteract();
            }
            else if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
        }
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify at least one key to press!";
            }
            else
            {
                List<KMSelectable> presses = new List<KMSelectable>();
                for (int i = 1; i < parameters.Length; i++)
                {
                    switch (parameters[i].ToUpper())
                    {
                        case "A":
                            presses.Add(pianoKeys[0]);
                            break;
                        case "A#":
                        case "Bb":
                            presses.Add(pianoKeys[1]);
                            break;
                        case "B":
                            presses.Add(pianoKeys[2]);
                            break;
                        case "C":
                            presses.Add(pianoKeys[3]);
                            break;
                        case "C#":
                        case "Db":
                            presses.Add(pianoKeys[4]);
                            break;
                        case "D":
                            presses.Add(pianoKeys[5]);
                            break;
                        case "D#":
                        case "Eb":
                            presses.Add(pianoKeys[6]);
                            break;
                        case "E":
                            presses.Add(pianoKeys[7]);
                            break;
                        case "F":
                            presses.Add(pianoKeys[8]);
                            break;
                        case "F#":
                        case "Gb":
                            presses.Add(pianoKeys[9]);
                            break;
                        case "G":
                            presses.Add(pianoKeys[10]);
                            break;
                        case "G#":
                        case "Ab":
                            presses.Add(pianoKeys[11]);
                            break;
                        default:
                            yield return "sendtochaterror!f The specified key '" + parameters[i] + "' is invalid!";
                            yield break;
                    }
                }
                foreach (KMSelectable btn in presses)
                {
                    btn.OnInteract();
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        for (int i = 0; i < 3; i++)
        {
            while (foodSwitch[i] != 0)
            {
                foods[i].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
        }
        int start = note;
        for (int i = start; i < melodyBreak[chosenMelody][mood] + 1; i++)
        {
            pianoKeys[melody[chosenMelody][i]].OnInteract();
            yield return new WaitForSeconds(0.2f);
        }
    }
}