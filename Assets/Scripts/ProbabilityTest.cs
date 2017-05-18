using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProbabilityTest : MonoBehaviour
{

    CA myCA;
    Texture2D tex;
    Sprite board;
    SpriteRenderer sr;

    private void Awake()
    {
        /*myCA = new CA(50, 50, 3, NType.VonNeumann);

        for (int startState = 0; startState < 3; ++startState)
        {
            for (int endState = 0; endState < 3; ++endState)
            {
                if (endState == startState)
                    continue;
                for (int n = 0; n <= 4; ++n)
                    myCA.SetStateInfo(startState, endState, n, (float)n / 4.0f);
            }
        }*/


        for (int i = 0; i < 50; ++i)
        {
            for (int j = 0; j < 50; ++j)
            {
                myCA.SetCellState(i, j, Random.Range(0, 3));
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
                switch (myCA.GetCellState(i, j))
                {
                    case 0:
                        tileColor = Color.black;
                        break;
                    case 1:
                        tileColor = Color.green;
                        break;
                    case 2:
                        tileColor = Color.red;
                        break;
                    default:
                        tileColor = Color.blue;
                        break;
                }

                for (int a = i * 10; a < i * 10 + 10; ++a)
                    for (int b = j * 10; b < j * 10 + 10; ++b)
                        tex.SetPixel(a, b, tileColor);
            }
        }
        tex.Apply();
    }
}