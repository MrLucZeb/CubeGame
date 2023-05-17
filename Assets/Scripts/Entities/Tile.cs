using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum State { IDLE, HOVER, MOVEOPTION, HOVER_MOVEOPTION };

    public Vector2Int gridPosition;
    State state;
    MoveAnimation scaleAnimation;
    Coroutine animationCoroutine = null;
    Material material;
    bool uneven = false;
    float target;
    float start;
    float baseX;

    bool isVoid;

    public bool isHovered;
    public bool isMoveOption;
    //public bool isHovered;
    //public bool isMoveOption;

    [SerializeField] Color IDLE_COLOR;
    [SerializeField] Color HOVER_COLOR;
    [SerializeField] Color MOVEOPTION_COLOR;
    [SerializeField] Color HOVER_MOVEOPTION_COLOR;
    [SerializeField] Color IDLE_UNEVEN;

    [SerializeField] float IDLE_SCALE = 1;
    [SerializeField] float HOVER_SCALE = 1;
    [SerializeField] float MOVEOPTION_SCALE = 1;
    [SerializeField] float HOVER_MOVEOPTION_SCALE = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (!isVoid)
        {
            baseX = transform.GetChild(0).localScale.x;
            target = baseX;
            scaleAnimation = GetComponent<MoveAnimation>();
            material = transform.GetComponentInChildren<Renderer>().material;
            UpdateState();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Init(Vector2Int position, bool isVoid)
    {
        gridPosition = position;
        this.isVoid = isVoid;
        uneven = position.x % 2 != 0 ^ position.y % 2 != 0;
        if (uneven)
        {
            IDLE_COLOR = IDLE_UNEVEN;

        }
    }

    public void SetState(State state)
    {
        this.state = state;

        switch (state)
        {
            case State.IDLE:

                break;
            case State.MOVEOPTION:
                SetScale(baseX + .1f);
                break;
        }
    }

    public void UpdateState()
    {
        if (isVoid) return;

        if (isHovered && isMoveOption)
        {
            SetColor(HOVER_MOVEOPTION_COLOR);
            SetScale(HOVER_MOVEOPTION_SCALE);
        } 
        else if (isHovered)
        {
            SetColor(HOVER_COLOR);
            SetScale(HOVER_SCALE);
        } 
        else if (isMoveOption)
        {
            SetColor(MOVEOPTION_COLOR);
            SetScale(MOVEOPTION_SCALE);
        } 
        else
        {
            SetColor(IDLE_COLOR);
            SetScale(IDLE_SCALE);
        }
    }

    // Determene wether tile is being hovered over by the mouse
    public void SetIsHovered(bool isHovered)
    {
        this.isHovered = isHovered;
        UpdateState();
    }

    // Set the target scale of tile
    private void SetScale(float scaleTarget)
    {
        transform.GetChild(0).localScale = new Vector3(scaleTarget, scaleTarget, 1);

        //if (animationCoroutine != null)
        //    StopCoroutine(animationCoroutine);

        //animationCoroutine = StartCoroutine(Scale(scaleTarget));
    }

    // Set the target COLOR of tile
    private void SetColor(Color color)
    {
        if (material != null)
            material.color = color;
    }

    IEnumerator Scale(float scaleTarget)
    {
        Vector3 a = transform.GetChild(0).localScale;
        Vector3 b = new Vector3(scaleTarget, scaleTarget, 1);

        scaleAnimation.Play();

        while (scaleAnimation.IsPlaying())
        {
            transform.GetChild(0).localScale = Vector3.LerpUnclamped(a, b, scaleAnimation.value);
            yield return null;
        }

        scaleAnimation.Reset();

        yield break;
    }
}
