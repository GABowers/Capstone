using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class LifeTest : MonoBehaviour
{
    CA myCA;
    Texture2D tex;
    Sprite board;
    SpriteRenderer sr;

    private void Awake()
    {
        myCA = new CA(50, 50, 2, NType.Moore);


        for (int i = 0; i <= 8; ++i)
        {
            // 0s never become 1s
            myCA.SetStateInfo(0, 1, i, 0);
            // 1s always become 0s
            myCA.SetStateInfo(1, 0, i, 1);
        }
        // only 3 neighbors can turn a 0 into 1
        myCA.SetStateInfo(0, 1, 3, 1);
        // 2 or 3 neighbors stop 1s from being 0s
        // 6 empty = 2 live,  5 empty = 3 live
        myCA.SetStateInfo(1, 0, 6, 0);
        myCA.SetStateInfo(1, 0, 5, 0);

        for (int i = 0; i < 50; ++i)
        {
            for (int j = 0; j < 50; ++j)
            {
                if (Random.Range(0, 10) == 0)
                    myCA.SetCellState(i, j, 1);
            }
        }
    }

    private void Start()
    {
        tex = new Texture2D(500, 500);
        board = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(500, 500)), Vector2.zero, 100);
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = board;
        UpdateBoard();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            myCA.OneIteration();
            UpdateBoard();
        }
    }

    private void UpdateBoard()
    {
        Color tileColor;
        for (int i = 0; i < 50; ++i)
        {
            for (int j = 0; j < 50; ++j)
            {
                if (myCA.GetCellState(i, j) == 0)
                    tileColor = Color.black;
                else
                    tileColor = Color.yellow;
                for (int a = i * 10; a < i * 10 + 10; ++a)
                    for (int b = j * 10; b < j * 10 + 10; ++b)
                        tex.SetPixel(a, b, tileColor);
            }
        }
        tex.Apply();
    }

}*/