using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ArtifactTileButton : MonoBehaviour
{
    private const int UI_OFFSET = 37;

    public ArtifactTileButtonAnimator buttonAnimator;
    public RectTransform imageRectTransform;
    [SerializeField] private UIArtifact buttonManager;

    [FormerlySerializedAs("emptySprite")]
    [SerializeField] private Sprite emptySpriteDefault;
    public Sprite islandSpriteDefault; // public for editor tools
    public Sprite completedSprite; // public for editor tools
    private Sprite completedSpriteDefault;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite blankSprite;
    [SerializeField] private List<ArtifactTBPlugin> plugins;

    // public static bool canComplete = false;
    public bool isComplete = false;
    // public bool isInMove = false;
    public int islandId = -1;
    public int x;
    public int y;

    public bool shouldFlicker = false;

    protected Sprite islandSprite;
    protected Sprite emptySprite;

    public STile MyStile {
        get
        {
            if (SGrid.Current == null)
            {
                return null;
            }
            return SGrid.Current.GetStile(islandId);
        }
    }
    public bool TileIsActive { 
        get
        {
            if (MyStile != null)
            {
                return MyStile.isTileActive;
            }
            return false;
        } 
    }
    public ArtifactTileButton LinkButton { get; private set; }

    protected void Awake() 
    {
        Init();
        RestoreDefaultIslandSprite();
        RestoreDefaultEmptySprite();
    }

    protected void OnDisable()
    {
        if (MyStile != null && MyStile.isTileActive)
        {
            SetSpriteToIslandOrEmpty();
        }
    }

    protected virtual void Start()
    {
        UpdateTileActive();
    }

    public void UpdateTileActive()
    {
        SetSpriteToIslandOrEmpty();

        //Fix for Conveyor Bug in Factory
        if (MyStile.GetMovingDirection() == Vector2.zero && UIArtifact.GetInstance().MoveQueueEmpty())
        {
            SetPosition(MyStile.x, MyStile.y, false);
        }
    }

    public virtual void SetPosition(int x, int y, bool animateChange=false)
    {
        //Debug.Log($"Changed button pos from ({this.x}, {this.y}) to ({x}, {y})");
        if (animateChange && TileIsActive)
        {
            // The "Travel" direction
            Vector2 dif = new Vector2(x - this.x, y - this.y).normalized;
            buttonAnimator.AnimatePositionFrom(-dif * 2);
        }

        this.x = x;
        this.y = y;

        SetAnchoredPos(x, y);

        plugins.ForEach(plugin =>
        {
            plugin.OnPosChanged();
        });

        SetSpriteToIslandOrEmpty();
    }

    public void SetSpriteToIslandOrEmpty()
    {
        if (TileIsActive)
        {
            buttonAnimator.sliderImage.sprite = isComplete ? completedSprite : islandSprite;
        }
        else
        {
            SetSpriteToEmpty();
        }
    }

    public void SetSpriteToEmpty()
    {
        buttonAnimator.sliderImage.sprite = emptySprite;
    }

    public void SetSpriteToHover()
    {
        buttonAnimator.sliderImage.sprite = hoverSprite;
    }

    public void RestoreDefaultIslandSprite()
    {
        islandSprite = islandSpriteDefault;
    }

    public void RestoreDefaultCompletedSprite()
    {
        completedSprite = completedSpriteDefault;
    }

    public void RestoreDefaultEmptySprite()
    {
        emptySprite = emptySpriteDefault;
    }

    public void SetIslandSprite(Sprite s)
    {
        islandSprite = s;
    }

    public void SetCompletedSprite(Sprite s)
    {
        completedSprite = s;
    }

    public void SetEmptySprite(Sprite s)
    {
        emptySprite = s;
    }

    public void AfterStileMoveDragged(object sender, SGridAnimator.OnTileMoveArgs e)
    {
        if (e.stile.islandId == islandId)
        {
            SetPushedDown(false);
        }
    }

    public void SelectButton()
    {
        buttonManager.SelectButton(this);
    }

    //public void UpdatePushedDown()
    //{
    //    buttonAnimator.UpdatePushedDown();
    //}

    public void SetHighlighted(bool v)
    {
        buttonAnimator.SetHighlighted(v);
    }

    public void SetPushedDown(bool v)
    {
        buttonAnimator.SetPushedDown(v);
    }

    public void SetLightning(bool v)
    {
        buttonAnimator.SetLightning(v);
    }

    public void SetSelected(bool v)
    {
        buttonAnimator.SetSelected(v);
    }

    public void SetIsInMove(bool v)
    {
        buttonAnimator.SetIsForcedDown(v && TileIsActive);
    }

    public void SetComplete(bool value)
    {
        if (!TileIsActive)
            return;

        isComplete = value;
        SetSpriteToIslandOrEmpty();
    }

    public void SetShouldFlicker(bool shouldFlicker)
    {
        this.shouldFlicker = shouldFlicker;
    }

    public void Flicker(int numFlickers) 
    {
        shouldFlicker = false;
        StartCoroutine(NewButtonFlicker(numFlickers));
    }

    public void FlickerImmediate(int numFlickers)
    {
        shouldFlicker = false;
        StartCoroutine(NewButtonFlicker(numFlickers, true));
    }

    protected virtual void SetAnchoredPos(int x, int y)
    {
        Vector3 pos = new Vector3((x % SGrid.Current.Height) - 1, y - 1) * UI_OFFSET; //C: i refuse to make everything into MT buttons
        GetComponent<RectTransform>().anchoredPosition = pos;
    }

    private IEnumerator NewButtonFlicker(int numFlickers, bool startOnBlank=false) {
        if (!startOnBlank)
        {
            SetSpriteToIslandOrEmpty();
            yield return new WaitForSeconds(.25f);
        }
        
        for (int i = 0; i < numFlickers; i++) 
        {
            buttonAnimator.sliderImage.sprite = blankSprite;
            yield return new WaitForSeconds(.25f);
            SetSpriteToIslandOrEmpty();
            yield return new WaitForSeconds(.25f);
        }
    }

    private void Init()
    {
        LinkButton = null;
        completedSpriteDefault = completedSprite;
        foreach (ArtifactTileButton b in buttonManager.buttons)
        {
            if (MyStile != null && MyStile.linkTile != null && MyStile.linkTile == b.MyStile)
            {
                LinkButton = b;
                b.LinkButton = this;
            }
        }
    }
}