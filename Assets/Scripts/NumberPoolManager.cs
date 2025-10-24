using UnityEngine;
using System.Collections.Generic;

public class NumberPoolManager : MonoBehaviour
{
    [Header("Settings")]
    public GameObject numberPrefab;
    public int poolSize = 10;
    public float spacing = 1.5f;
    public Vector3 poolStartPosition = new Vector3(-5.55f, - 1.87f, 0);

    private List<GameObject> activeNumbers = new List<GameObject>();

    public void SpawnNumbers(List<int> numbers)
{
    
    foreach (GameObject num in activeNumbers)
    {
        Destroy(num);
    }
    activeNumbers.Clear();

  
    List<int> allDigits = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    
   
    Shuffle(allDigits);

    // Spawn exactly 10 numbers (0-9, each appearing once)
    for (int i = 0; i < 10; i++)
    {
        Vector3 pos = poolStartPosition + Vector3.right * (i * spacing);
        GameObject numberObj = Instantiate(numberPrefab, pos, Quaternion.identity, transform);

        DraggableNumber draggable = numberObj.transform.GetChild(0).GetComponent<DraggableNumber>();
        if (draggable != null)
        {
            draggable.SetNumber(allDigits[i]);
        }

        activeNumbers.Add(numberObj);
    }
}


    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}