
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Generator : MonoBehaviour
{

    [SerializeField] private GameObject cell;
    [SerializeField] private int height, width, bombsCount;
    [SerializeField] private GameObject[][] cellMatrix;

    [SerializeField] private AudioSource clearSound;
    [SerializeField] private AudioSource explosionSound;

    public static Generator instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            instance = this;
        }
    }


    public void setWidth(int width) { this.width = width; }

    public void setHeight(int height) { this.height = height; }

    public void setBombsCount(int bombsNumber) { bombsCount = bombsNumber; }


    public int Validate()
    {
        int errorCode = 0;
        if (width <= 1 || width > 17) errorCode += 4;
        if (height <= 1 || height > 10)errorCode += 2;
        if (bombsCount <= 0 || bombsCount >= width * height) errorCode += 1;

        return errorCode;
    }

    public void Generate()
    {
        cellMatrix = new GameObject[width][];
        for (int i = 0; i < cellMatrix.Length; i++)
        {
            cellMatrix[i] = new GameObject[height];
        } 
        for (int j = 0; j < height; j++)
            for (int i = 0; i < width; i++) {

                cellMatrix[i][j] = Instantiate(cell, new Vector3(i, j, 0), Quaternion.identity);
                cellMatrix[i][j].GetComponent<Cell>().setX(i);
                cellMatrix[i][j].GetComponent<Cell>().setY(j);

            }

        Camera.main.transform.position = new Vector3((float)(width / 2f -0.5f), (float) (height / 2f -0.5f), -10);

        for (int i = 0;i < bombsCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            if (!cellMatrix[x][y].GetComponent<Cell>().hasBomb())
                cellMatrix[x][y].GetComponent<Cell>().setBomb(true);
            else
                i--;
            //cellMatrix[Random.Range(0, width)][Random.Range(0, height)].GetComponent<SpriteRenderer>().material.color = Color.red;
        }

    }

    public int GetBombsAround(int x, int y)
    {
        int cont = 0;
        //ARRIBA IZQUIERDA
        if (x > 0 && y < height - 1 && cellMatrix[x - 1][y + 1].GetComponent<Cell>().hasBomb())
            cont++;
        //ARRIBA CENTRO
        if (y < height - 1 && cellMatrix[x][y + 1].GetComponent<Cell>().hasBomb())
            cont++;
        //ARRIBA DERECHA
        if (x < width - 1 && y < height - 1 && cellMatrix[x + 1][y + 1].GetComponent<Cell>().hasBomb())
            cont++;
        //CENTRO IZQUIERDA
        if (x > 0 && cellMatrix[x - 1][y].GetComponent<Cell>().hasBomb())
            cont++;
        //CENTRO DERECHA
        if (x < width - 1 && cellMatrix[x + 1][y].GetComponent<Cell>().hasBomb())
            cont++;
        //ABAJO IZQUIERDA
        if (x > 0 && y > 0 && cellMatrix[x - 1][y - 1].GetComponent<Cell>().hasBomb())
            cont++;
        //ABAJO CENTRO
        if (y > 0 && cellMatrix[x][y - 1].GetComponent<Cell>().hasBomb())
            cont++;
        //ABAJO DERECHA
        if (x < width - 1 && y > 0 && cellMatrix[x + 1][y - 1].GetComponent<Cell>().hasBomb())
            cont++;

        return cont;
    }

    public int GetUndiscoveredAdjacents(int x, int y)
    {
        int cont = 0;
        //ARRIBA IZQUIERDA
        if (x > 0 && y < height - 1 && !cellMatrix[x - 1][y + 1].GetComponent<Cell>().isSeen())
            cont++;
        //ARRIBA CENTRO
        if (y < height - 1 && !cellMatrix[x][y + 1].GetComponent<Cell>().isSeen())
            cont++;
        //ARRIBA DERECHA
        if (x < width - 1 && y < height - 1 && !cellMatrix[x + 1][y + 1].GetComponent<Cell>().isSeen())
            cont++;
        //CENTRO IZQUIERDA
        if (x > 0 && !cellMatrix[x - 1][y].GetComponent<Cell>().isSeen())
            cont++;
        //CENTRO DERECHA
        if (x < width - 1 && !cellMatrix[x + 1][y].GetComponent<Cell>().isSeen())
            cont++;
        //ABAJO IZQUIERDA
        if (x > 0 && y > 0 && !cellMatrix[x - 1][y - 1].GetComponent<Cell>().isSeen())
            cont++;
        //ABAJO CENTRO
        if (y > 0 && !cellMatrix[x][y - 1].GetComponent<Cell>().isSeen())
            cont++;
        //ABAJO DERECHA
        if (x < width - 1 && y > 0 && !cellMatrix[x + 1][y - 1].GetComponent<Cell>().isSeen())
            cont++;

        return cont;
    }

    public int GetFlaggedAdjacentTo(int x, int y)
    {
        int cont = 0;
        //ARRIBA IZQUIERDA
        if (x > 0 && y < height - 1 && cellMatrix[x - 1][y + 1].GetComponent<Cell>().isFlagged())
            cont++;
        //ARRIBA CENTRO
        if (y < height - 1 && cellMatrix[x][y + 1].GetComponent<Cell>().isFlagged())
            cont++;
        //ARRIBA DERECHA
        if (x < width - 1 && y < height - 1 && cellMatrix[x + 1][y + 1].GetComponent<Cell>().isFlagged())
            cont++;
        //CENTRO IZQUIERDA
        if (x > 0 && cellMatrix[x - 1][y].GetComponent<Cell>().isFlagged())
            cont++;
        //CENTRO DERECHA
        if (x < width - 1 && cellMatrix[x + 1][y].GetComponent<Cell>().isFlagged())
            cont++;
        //ABAJO IZQUIERDA
        if (x > 0 && y > 0 && cellMatrix[x - 1][y - 1].GetComponent<Cell>().isFlagged())
            cont++;
        //ABAJO CENTRO
        if (y > 0 && cellMatrix[x][y - 1].GetComponent<Cell>().isFlagged())
            cont++;
        //ABAJO DERECHA
        if (x < width - 1 && y > 0 && cellMatrix[x + 1][y - 1].GetComponent<Cell>().isFlagged())
            cont++;

        return cont;
    }

    internal void DrawBombsAdjacentTo(int x, int y)
    {
        //ARRIBA IZQUIERDA
        if (x > 0 && y < height - 1 && !cellMatrix[x - 1][y + 1].GetComponent<Cell>().isSeen()
                                    && !cellMatrix[x - 1][y + 1].GetComponent<Cell>().isFlagged())
            cellMatrix[x - 1][y + 1].GetComponent<Cell>().DrawBomb();
        //ARRIBA CENTRO
        if (y < height - 1 && !cellMatrix[x][y + 1].GetComponent<Cell>().isSeen()
                           && !cellMatrix[x][y + 1].GetComponent<Cell>().isFlagged())
            cellMatrix[x][y + 1].GetComponent<Cell>().DrawBomb();
        //ARRIBA DERECHA
        if (x < width - 1 && y < height - 1 && !cellMatrix[x + 1][y + 1].GetComponent<Cell>().isSeen()
                                            && !cellMatrix[x + 1][y + 1].GetComponent<Cell>().isFlagged())
            cellMatrix[x + 1][y + 1].GetComponent<Cell>().DrawBomb();
        //CENTRO IZQUIERDA
        if (x > 0 && !cellMatrix[x - 1][y].GetComponent<Cell>().isSeen()
                  && !cellMatrix[x - 1][y].GetComponent<Cell>().isFlagged())
            cellMatrix[x - 1][y].GetComponent<Cell>().DrawBomb();
        //CENTRO DERECHA
        if (x < width - 1 && !cellMatrix[x + 1][y].GetComponent<Cell>().isSeen()
                          && !cellMatrix[x + 1][y].GetComponent<Cell>().isFlagged())
            cellMatrix[x + 1][y].GetComponent<Cell>().DrawBomb();
        //ABAJO IZQUIERDA
        if (x > 0 && y > 0 && !cellMatrix[x - 1][y - 1].GetComponent<Cell>().isSeen()
                           && !cellMatrix[x - 1][y - 1].GetComponent<Cell>().isFlagged())
            cellMatrix[x - 1][y - 1].GetComponent<Cell>().DrawBomb();
        //ABAJO CENTRO
        if (y > 0 && !cellMatrix[x][y - 1].GetComponent<Cell>().isSeen()
                  && !cellMatrix[x][y - 1].GetComponent<Cell>().isFlagged())
            cellMatrix[x][y - 1].GetComponent<Cell>().DrawBomb();
        //ABAJO DERECHA
        if (x < width - 1 && y > 0 && !cellMatrix[x + 1][y - 1].GetComponent<Cell>().isSeen()
                                   && !cellMatrix[x + 1][y - 1].GetComponent<Cell>().isFlagged())
            cellMatrix[x + 1][y - 1].GetComponent<Cell>().DrawBomb();
    }
    internal void FlagCellsAdjacentTo(int x, int y)
    {
        //ARRIBA IZQUIERDA
        if (x > 0 && y < height - 1 && !cellMatrix[x - 1][y + 1].GetComponent<Cell>().isSeen()
                                    && !cellMatrix[x - 1][y + 1].GetComponent<Cell>().isFlagged())
            cellMatrix[x - 1][y + 1].GetComponent<Cell>().FlagCell();
        //ARRIBA CENTRO
        if (y < height - 1 && !cellMatrix[x][y + 1].GetComponent<Cell>().isSeen()
                           && !cellMatrix[x][y + 1].GetComponent<Cell>().isFlagged())
            cellMatrix[x][y + 1].GetComponent<Cell>().FlagCell();
        //ARRIBA DERECHA
        if (x < width - 1 && y < height - 1 && !cellMatrix[x + 1][y + 1].GetComponent<Cell>().isSeen()
                                            && !cellMatrix[x + 1][y + 1].GetComponent<Cell>().isFlagged())
            cellMatrix[x + 1][y + 1].GetComponent<Cell>().FlagCell();
        //CENTRO IZQUIERDA
        if (x > 0 && !cellMatrix[x - 1][y].GetComponent<Cell>().isSeen()
                  && !cellMatrix[x - 1][y].GetComponent<Cell>().isFlagged())
            cellMatrix[x - 1][y].GetComponent<Cell>().FlagCell();
        //CENTRO DERECHA
        if (x < width - 1 && !cellMatrix[x + 1][y].GetComponent<Cell>().isSeen()
                          && !cellMatrix[x + 1][y].GetComponent<Cell>().isFlagged())
            cellMatrix[x + 1][y].GetComponent<Cell>().FlagCell();
        //ABAJO IZQUIERDA
        if (x > 0 && y > 0 && !cellMatrix[x - 1][y - 1].GetComponent<Cell>().isSeen()
                           && !cellMatrix[x - 1][y - 1].GetComponent<Cell>().isFlagged())
            cellMatrix[x - 1][y - 1].GetComponent<Cell>().FlagCell();
        //ABAJO CENTRO
        if (y > 0 && !cellMatrix[x][y - 1].GetComponent<Cell>().isSeen()
                  && !cellMatrix[x][y - 1].GetComponent<Cell>().isFlagged())
            cellMatrix[x][y - 1].GetComponent<Cell>().FlagCell();
        //ABAJO DERECHA
        if (x < width - 1 && y > 0 && !cellMatrix[x + 1][y - 1].GetComponent<Cell>().isSeen()
                                   && !cellMatrix[x + 1][y - 1].GetComponent<Cell>().isFlagged())
            cellMatrix[x + 1][y - 1].GetComponent<Cell>().FlagCell();
    }

    public int GetPossibleAdjacents(int x, int y)
    {
        int cont = 0;
        //ARRIBA IZQUIERDA
        if (x > 0 && y < height - 1)
            cont++;
        //ARRIBA CENTRO
        if (y < height - 1)
            cont++;
        //ARRIBA DERECHA
        if (x < width - 1 && y < height - 1)
            cont++;
        //CENTRO IZQUIERDA
        if (x > 0)
            cont++;
        //CENTRO DERECHA
        if (x < width - 1)
            cont++;
        //ABAJO IZQUIERDA
        if (x > 0 && y > 0)
            cont++;
        //ABAJO CENTRO
        if (y > 0)
            cont++;
        //ABAJO DERECHA
        if (x < width - 1 && y > 0)
            cont++;

        return cont;
    }


    public void CheckPiecesAround(int x, int y)
    {
        //ARRIBA IZQUIERDA
        if (x > 0 && y < height - 1)
            cellMatrix[x - 1][y + 1].GetComponent<Cell>().DrawBomb();
        //ARRIBA CENTRO
        if (y < height - 1)
            cellMatrix[x][y + 1].GetComponent<Cell>().DrawBomb();
        //ARRIBA DERECHA
        if (x < width - 1 && y < height - 1)
            cellMatrix[x + 1][y + 1].GetComponent<Cell>().DrawBomb();
        //CENTRO IZQUIERDA
        if (x > 0)
            cellMatrix[x - 1][y].GetComponent<Cell>().DrawBomb();
        //CENTRO DERECHA
        if (x < width - 1)
            cellMatrix[x + 1][y].GetComponent<Cell>().DrawBomb();
        //ABAJO IZQUIERDA
        if (x > 0 && y > 0)
            cellMatrix[x - 1][y - 1].GetComponent<Cell>().DrawBomb();
        //ABAJO CENTRO
        if (y > 0)
            cellMatrix[x][y - 1].GetComponent<Cell>().DrawBomb();
        //ABAJO DERECHA
        if (x < width - 1 && y > 0)
            cellMatrix[x + 1][y - 1].GetComponent<Cell>().DrawBomb();
    }

    public void DestroyCellMatrix()
    {
        for (int i=0; i<cellMatrix.Length; i++)
        {
            for (int j=0; j<cellMatrix[i].Length; j++)
            {
                Destroy(cellMatrix[i][j]);
            }
        }
    }
    
    public void CheckBoardComplete()
    {
        foreach (GameObject[] i in cellMatrix)
        {
            foreach(GameObject j in i)
            {
                Cell cell = j.GetComponent<Cell>();
                if (!cell.isSeen() && !cell.hasBomb()) return;
                if (!cell.isFlagged() && cell.hasBomb()) return;
            }
        }
        StartCoroutine(WinAndWaitToWinScreen());
        GameManager.instance.gameOver = true;
    }

    public List<Cell> GetBombedCells()
    {
        List<Cell> array = new();
        foreach (GameObject[] i in cellMatrix)
        {
            foreach (GameObject j in i)
            {
                Cell cell = j.GetComponent<Cell>();
                if (cell.hasBomb()) array.Add(cell);
            }
        }
        return array;
    }

    public GameObject[][] ReturnMatrix()
    {
        return cellMatrix;
    }

    internal void Reset()
    {
        width = height = bombsCount = 0;
    }

    private IEnumerator WinAndWaitToWinScreen()
    {
        //CHANGE COLOR TO GREEN TO ALL BOMBED CELLS
        foreach (Cell c in GetBombedCells())
        {
            c.GetComponent<SpriteRenderer>().material.color = Color.green;
        }

        //CLEARED STAGE SOUND
        clearSound.Play();

        yield return new WaitForSecondsRealtime(2F);
        GameManager.instance.GameOverWin();
    }
}
