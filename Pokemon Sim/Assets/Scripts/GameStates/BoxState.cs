using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class BoxState : State<GameController>
{
    [SerializeField] BoxUI boxUI;
    [SerializeField] PointerSelector pointer;

    int selection;
    public static BoxState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public override void EnterState(GameController owner)
    {
        StartCoroutine(FadeIntoBox());
    }

    public override void Execute()
    {
        boxUI.HandleUpdate();
    }

    public override void ExitState()
    {
        StartCoroutine(FadeOutOfBox());
    }

    public IEnumerator FadeIntoBox()
    {
        yield return Fader.instance.FadeIn(0.4f);
        boxUI.gameObject.SetActive(true);
        yield return Fader.instance.FadeOut(0.4f);
        boxUI.UpdateHoverSelection(0,0);
    }

    public IEnumerator FadeOutOfBox()
    {
        yield return Fader.instance.FadeIn(0.4f);
        boxUI.gameObject.SetActive(false);
        yield return Fader.instance.FadeOut(0.4f);
    }

}
