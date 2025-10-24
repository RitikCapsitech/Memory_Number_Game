using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject cellPrefab;
    public int gridSize = 2;
    public float cellSpacing = 2f;
    public float showDuration = 3f;

    [Header("References")]
    public NumberPoolManager numberPoolManager;

    private List<Cell> cells = new List<Cell>();
    private List<int> gridNumbers = new List<int>();

    

    public void GenerateGrid()
    {
        
        StopAllCoroutines();

    
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        cells.Clear();
        gridNumbers.Clear();

        float offset = (gridSize - 1) * cellSpacing / 2f;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector2 pos = new Vector2(x * cellSpacing - offset, y * cellSpacing - offset);
                GameObject cellObj = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                cellObj.name = $"Cell_{x}_{y}";

               
                BoxCollider2D collider = cellObj.GetComponent<BoxCollider2D>();
                if (collider == null)
                {
                    collider = cellObj.AddComponent<BoxCollider2D>();
                    collider.isTrigger = false; 
                }

                
                SpriteRenderer sr = cellObj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    collider.size = sr.size;
                }
                else
                {
                    collider.size = Vector2.one; // fallback
                }

                Cell cell = cellObj.GetComponent<Cell>();
                if (cell != null)
                {
                    int randomNumber = Random.Range(1, 10);
                    cell.SetTargetNumber(randomNumber);
                    cells.Add(cell);
                    gridNumbers.Add(randomNumber);
                }
            }
        }
    }

    IEnumerator ShowNumbersSequence()
    {
    
        foreach (Cell cell in cells)
        {
            cell.ShowNumber();
        }

        yield return new WaitForSeconds(showDuration);

       
        foreach (Cell cell in cells)
        {
            cell.HideNumber();
        }

      
        if (numberPoolManager != null)
        {
            numberPoolManager.SpawnNumbers(gridNumbers);
        }
    }

   
    public void StartShowNumbersSequence()
    {
        StartCoroutine(ShowNumbersSequence());
    }

    public void IncreaseGridSize()
    {
        gridSize++;
        GenerateGrid();
        StartCoroutine(ShowNumbersSequence());
    }
}
