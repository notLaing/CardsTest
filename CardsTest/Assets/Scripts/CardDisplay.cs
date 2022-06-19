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
        Discard
    }

    Vector3 dragOffset, handPositionVec;
    Camera cam;
    StateAnimation animState;
    float drawAnimTime = .3f;
    float discardPlayAnimTime = .6f;
    float discardEndAnimTime = 1.5f;
    float spreadAnimTime = .1f;
    float playStillAnimTime = 2f;
    float animTime = 0;
    float spreadRotMultiplier = -10f;
    float spreadHeightMultiplier = -.3f;
    float spreadWidthMultiplier = 2f;
    float xCurveRate = 2.5f;
    float yCurveRate = 3.7f;
    float endTransitionCutoff = 2.1f;

    public TextMesh textName;
    public TextMesh textCost;
    public TextMesh textType;
    public TextMesh textDescription;
    public SpriteRenderer icon;
    float playHeight = -2.3f;
    float playSize = .7f;
    float hoverSize = .5f;
    public int indexInHand;
    public int indexOverall;
    public bool inHand = false;

    public void CreateCardDisplay(CardScriptableObj cardScriptObj)
    {
        textName.text = cardScriptObj.name;
        textCost.text = cardScriptObj.GetCardCost().ToString();
        textType.text = cardScriptObj.type;
        textDescription.text = cardScriptObj.GetDescription();
        icon.sprite = GameManager.Instance.imageList[cardScriptObj.image_id];

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
                ResetAnimTime(playStillAnimTime + discardPlayAnimTime);
                break;
            case StateAnimation.Discard:
                AnimateDiscard();
                if(ResetAnimTime(discardEndAnimTime)) GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerTurnPrep);
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
        transform.position = AnimMath.Ease(transform.position, GameManager.Instance.cardHandLocation.position, .001f);
        transform.localScale = AnimMath.Ease(transform.localScale, GameManager.Instance.cardHandLocation.localScale, .001f);
    }

    void AnimateSpread()
    {
        // find the spread amount/position/rotation for the card
        float range = (float)(GameManager.Instance.handCards.Count - 1);
        float spread = AnimMath.Map(indexInHand, 0, range, (range / -2), (range / 2));

        Vector3 place = new Vector3(spread * spreadWidthMultiplier, Mathf.Abs(spread) * spreadHeightMultiplier, 0);
        transform.position = AnimMath.Ease(transform.position, GameManager.Instance.cardHandLocation.position + place, .001f);
        handPositionVec = transform.position;

        Quaternion targetRot = GameManager.Instance.cardHandLocation.rotation;
        targetRot.eulerAngles += new Vector3(0, 0, spread * spreadRotMultiplier);
        transform.rotation = AnimMath.Ease(transform.rotation, targetRot, .001f);
    }

    void AnimatePlay()
    {
        // hold in center
        if(animTime < playStillAnimTime)
        {
            transform.position = AnimMath.Ease(transform.position, Vector3.zero, .001f);
            transform.rotation = AnimMath.Ease(transform.rotation, Quaternion.identity, .001f);
            transform.localScale = AnimMath.Ease(transform.localScale, Vector3.one * playSize, .001f);
        }
        // send to discard
        else
        {
            transform.position = AnimMath.Ease(transform.position, GameManager.Instance.cardDiscardLocation.position, .001f);
            transform.localScale = AnimMath.Ease(transform.localScale, GameManager.Instance.cardDiscardLocation.localScale, .001f);
        }
    }

    void AnimateDiscard()
    {
        // discard from hand at the end of the turn
        transform.localScale = AnimMath.Ease(transform.localScale, GameManager.Instance.cardDiscardLocation.localScale, .001f);

        // alter the speed
        float cutoffValue = (animTime / discardEndAnimTime) * yCurveRate;
        if (cutoffValue < endTransitionCutoff)
        {
            float xTarget = AnimMath.Ease(transform.position.x, handPositionVec.x + xCurveRate, .2f);
            float yTarget = AnimMath.Lerp(GameManager.Instance.cardDiscardLocation.position.y, 0, Mathf.Clamp(Mathf.Sin(cutoffValue), 0, 1), false);

            transform.position = new Vector3(xTarget, yTarget, 0);
        }
        else
        {
            transform.position = AnimMath.Ease(transform.position, GameManager.Instance.cardDiscardLocation.position, .001f);
        }
    }

    void OnMouseOver()
    {
        if(animState == StateAnimation.Idle) transform.localScale = AnimMath.Ease(transform.localScale, Vector3.one * hoverSize, .001f);
    }

    void OnMouseExit()
    {
        if(animState == StateAnimation.Idle) transform.localScale = GameManager.Instance.cardHandLocation.localScale;
    }

    void OnMouseDown()
    {
        dragOffset = transform.position - GetMousePos();
    }

    void OnMouseDrag()
    {
        transform.position = GetMousePos() + dragOffset;
    }

    void OnMouseUp()
    {
        if(transform.position.y > playHeight)
        {
            // play card
            GameManager.Instance.pileManager.PlayCard(indexInHand, indexOverall);
        }
    }

    Vector3 GetMousePos()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);// mousePos;
    }
}
