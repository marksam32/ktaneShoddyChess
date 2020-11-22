using UnityEngine;
using System.Collections;

public class ExampleModule : MonoBehaviour
{
    public KMSelectable[] buttons;

    int correctIndex;
    bool isActivated = false;
    private static double solvedelay = 0;

    void Start()
    {   
        Init();
        
        GetComponent<KMBombModule>().OnActivate += ActivateModule;
    }

    void Init()
    {
        correctIndex = Random.Range(0, 4);

        for(int i = 0; i < buttons.Length; i++)
        {
            string label = i == correctIndex ? "O" : "X";

            TextMesh buttonText = buttons[i].GetComponentInChildren<TextMesh>();
            buttonText.text = label;
            int j = i;
            buttons[i].OnInteract += delegate () { OnPress(j == correctIndex); return false; };
        }
    }

    void ActivateModule()
    {
        //StartCoroutine(Solve());
        isActivated = true;
    }

    void OnPress(bool correctButton)
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        GetComponent<KMSelectable>().AddInteractionPunch();

        if (!isActivated)
        {
            Debug.Log("Pressed button before module has been activated!");
            GetComponent<KMBombModule>().HandleStrike();
        }
        else
        {
            Debug.Log("Pressed " + correctButton + " button");
            if (correctButton)
            {
                GetComponent<KMBombModule>().HandlePass();
            }
            else
            {
                GetComponent<KMBombModule>().HandleStrike();
            }
        }
    }

    private IEnumerator Solve()
    {
        solvedelay += .2;
        yield return new WaitForSeconds(float.Parse(solvedelay.ToString()));
        GetComponent<KMBombModule>().HandlePass();
    }
}
