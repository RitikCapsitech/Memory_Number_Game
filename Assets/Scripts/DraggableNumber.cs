
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Rendering;
using GameWise.ShapeMatching;

[RequireComponent(typeof(Collider2D))]
public class DraggableNumber : MonoBehaviour
{
    [Header("References")]
    public TextMeshPro numberText;
    public SpriteRenderer spriteRenderer;
    [SerializeField]private SortingGroup sortingGroup;
    private int number;
    private Vector3 spawnStartPos;
    private Vector3 mousePos;
    private Vector3 offset;
    private int defSortingOrder;
    private void Awake()
    {
        if (numberText == null)
            numberText = GetComponentInChildren<TextMeshPro>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        spawnStartPos = transform.position;
        defSortingOrder = sortingGroup.sortingOrder;
    }

    public void SetNumber(int num)
    {
        number = num;
        numberText.text = num.ToString();
    }

    public int GetNumber() => number;

    private void OnMouseDown()
    {
        SoundManager.Instance.Tap();
        if (GameManager.IsGamePaused) return;
        sortingGroup.sortingOrder = 10;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - mousePos;
        GetComponent<Collider2D>().enabled = false;
        var c = spriteRenderer.color;
        c.a = 0.6f;
        spriteRenderer.color = c;
    }

    private void OnMouseDrag()
    {
        if (GameManager.IsGamePaused) return;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, 0);
    }

    private void OnMouseUp()
    {
        if (GameManager.IsGamePaused) return;
        var c = spriteRenderer.color;
        c.a = 1f;
        spriteRenderer.color = c;
        var myCollider = GetComponent<Collider2D>();
        myCollider.enabled = true;
        Vector2 dropPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hits = Physics2D.OverlapPointAll(dropPoint);
        foreach (var hit in hits)
        {
            var cell = hit.GetComponent<Cell>();
            if (cell != null && !cell.IsFilled)
            {
                if (cell.TryPlaceNumber(number)) break;
            }
        }
        transform.position = spawnStartPos;
        sortingGroup.sortingOrder = defSortingOrder;
    }

    private IEnumerator FadeAndDestroy(SpriteRenderer renderer)
    {
        float duration = 0.5f, elapsed = 0f;
        var c = renderer.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, elapsed / duration);
            renderer.color = c;
            yield return null;
        }
        Destroy(renderer.gameObject);
    }
}


