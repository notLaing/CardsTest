using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sits on Blank Card prefab
/// Handles displaying the card and moving it around
/// </summary>

public class CardDisplay : MonoBehaviour
{
    public enum StateAnimation
    {
        Idle,
        Draw,
        Spread,
        Play,
        Discard,
        Discarded
    }

    public CardScriptableObj cardScriptObj;
    Vector3 dragOffset, handPositionVec;
    Camera cam;
    StateAnimation animState;
    public float drawAnimTime = .3f;
    public float discardPlayAnimTime = .6f;
    public float discardEndAnimTime = 1.5f;
    float spreadAnimTime = .3f;
    public float playStillAnimTime = 2f;
    float animTime = 0;
    float spreadRotMultiplier = -10f;
    float spreadHeightMultiplier = -20f;
    float spreadWidthMultiplier = 200f;
    float xCurveRate = 300;//2.5f;
    float yCurveRate = 3.7f;
    float endTransitionCutoff = 2.1f;

    public Text textName;
    public Text textCost;
    public Text textType;
    public Text textDescription;
    public Image icon;
    public Canvas canvas;
    Color enoughColor = Color.white;
    Color notEnoughColor = Color.red;
    public GameObject glow;
    float playHeight = 150f;
    float playWidth = 400;
    float playSize = .7f;
    float hoverRaiseAmount = 100;
    public int indexInHand;
    public int indexOverall;
    public bool inHand = false;
    bool beingDragged = false;

    public void CreateCardDisplay(CardScriptableObj c)
    {
        cardScriptObj = c;
        textName.text = cardScriptObj.name;
        textCost.text = cardScriptObj.GetCardCost().ToString();
        textCost.color = enoughColor;
        textType.text = cardScriptObj.type;
        textDescription.text = cardScriptObj.GetDescription();
        icon.sprite = GameManager.Instance.imageList[cardScriptObj.image_id];
        glow.SetActive(true);

        transform.eulerAngles = Vector3.zero;
    }

    void Awake()
    {
        cam = Camera.main;
        animState = StateAnimation.Idle;
    }

    void Update()
    {
        switch (animState)
        {
            case StateAnimation.Idle:
                animTime = 0;
                break;
            case StateAnimation.Draw:
                AnimateDraw();
                if(ResetAnimTime(drawAnimTime)) animState = StateAnimation.Spread;
                break;
            case StateAnimation.Spread:
                AnimateSpread();
                if(ResetAnimTime(spreadAnimTime)) animState = StateAnimation.Idle;
                break;
            case StateAnimation.Play:
                AnimatePlay();
                if(ResetAnimTime(playStillAnimTime + discardPlayAnimTime)) animState = StateAnimation.Discarded;
                break;
            case StateAnimation.Discard:
                AnimateDiscard();
                if(ResetAnimTime(discardEndAnimTime)) GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerTurnPrep);
                break;
            case StateAnimation.Discarded:
                FinishDiscard();
                break;
        }

        animTime += Time.deltaTime;
    }

    public void SetAnimState(StateAnimation a)
    {
        animTime = 0;
        animState = a;
    }

    bool ResetAnimTime(float maxTime)
    {
        if(animTime > maxTime)
        {
            ForceReset();
            return true;
        }
        return false;
    }

    public void ForceReset()
    {
        animState = StateAnimation.Idle;
        animTime = 0;
    }

    void AnimateDraw()
    {
        //transform.localPosition = AnimMath.Ease(transform.localPosition, GameManager.Instance.cardHandLocation.localPosition, .001f);
        transform.localScale = AnimMath.Ease(transform.localScale, GameManager.Instance.cardHandLocation.localScale, .001f);
        AnimateSpread();
    }

    void AnimateSpread()
    {
        // find the spread amount/position/rotation for the card
        float range = (float)(GameManager.Instance.handCards.Count - 1);
        float spread = (range < 1) ? 0 : AnimMath.Map(indexInHand, 0, range, (range / -2), (range / 2));

        Vector3 place = new Vector3(spread * spreadWidthMultiplier, Mathf.Abs(spread) * spreadHeightMultiplier, 0);
        transform.localPosition = AnimMath.Ease(transform.localPosition, GameManager.Instance.cardHandLocation.localPosition + place, .001f);
        handPositionVec = transform.localPosition;

        Quaternion targetRot = GameManager.Instance.cardHandLocation.localRotation;
        targetRot.eulerAngles += new Vector3(0, 0, spread * spreadRotMultiplier);
        transform.localRotation = AnimMath.Ease(transform.localRotation, targetRot, .001f);
    }

    void AnimatePlay()
    {
        // hold in center. Currently accounting for a card GameObject with a centered pivot point
        if(animTime < playStillAnimTime)
        {
            transform.localPosition = AnimMath.Ease(transform.localPosition, Vector3.zero, .001f);
            transform.localRotation = AnimMath.Ease(transform.localRotation, Quaternion.identity, .001f);
            transform.localScale = AnimMath.Ease(transform.localScale, Vector3.one * playSize, .001f);
        }
        // send to discard
        else
        {
            transform.localPosition = AnimMath.Ease(transform.localPosition, GameManager.Instance.cardDiscardLocation.localPosition, .001f);
            transform.localScale = AnimMath.Ease(transform.localScale, GameManager.Instance.cardDiscardLocation.localScale, .001f);
        }
    }

    void FinishDiscard()
    {
        // move off the screen
        transform.localPosition += new Vector3(0, -3000, 0);
        GameManager.Instance.pileManager.playingCard = false;
        animState = StateAnimation.Idle;
    }

    void AnimateDiscard()
    {
        // discard from hand at the end of the turn
        transform.localScale = AnimMath.Ease(transform.localScale, GameManager.Instance.cardDiscardLocation.localScale, .001f);

        // alter the speed
        float cutoffValue = (animTime / discardEndAnimTime) * yCurveRate;
        if (cutoffValue < endTransitionCutoff)
        {
            float xTarget = AnimMath.Ease(transform.localPosition.x, handPositionVec.x + xCurveRate, .2f);
            float yTarget = AnimMath.Lerp(GameManager.Instance.cardDiscardLocation.localPosition.y, 0, Mathf.Clamp(Mathf.Sin(cutoffValue), 0, 1), false);

            transform.localPosition = new Vector3(xTarget, yTarget, 0);
        }
        else
        {
            transform.localPosition = AnimMath.Ease(transform.localPosition, GameManager.Instance.cardDiscardLocation.localPosition, .001f);
        }
    }

    public void ChangeColor(bool b)
    {
        textCost.color = b ? enoughColor : notEnoughColor;
        glow.SetActive(b);
    }

    void OnMouseOver()
    {
        if (!beingDragged && (animState == StateAnimation.Idle)) transform.localPosition = AnimMath.Ease(transform.localPosition, handPositionVec + new Vector3(0, hoverRaiseAmount, 0), .001f);
        canvas.overrideSorting = true;
    }

    void OnMouseExit()
    {
        if(animState == StateAnimation.Idle) transform.localPosition = handPositionVec;
        canvas.overrideSorting = false;
    }

    void OnMouseDown()
    {
        dragOffset = transform.position - GetMousePos();
        beingDragged = true;
    }

    void OnMouseDrag()
    {
        transform.position = GetMousePos() + dragOffset;
    }

    void OnMouseUp()
    {
        beingDragged = false;
        if((Mathf.Abs(transform.localPosition.y) < playHeight) && (Mathf.Abs(transform.localPosition.x) < playWidth))
        {
            // play card
            GameManager.Instance.pileManager.PlayCard(indexInHand, indexOverall);
        }
    }

    Vector3 GetMousePos()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }
}
