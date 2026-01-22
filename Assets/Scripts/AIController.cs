using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AIController : MonoBehaviour
{
    // Declaración de variables (necesitarás algunas más)
    public float turnTime = 0.5f;
    private int moves = 0;

    [SerializeField] TextMeshProUGUI AIDebugText;

    bool firstPlay = true;
    Cell[,] cellMatrix;


    public IEnumerator Play()
    {
        firstPlay = true;
        GameObject[][] matrix = Generator.instance.ReturnMatrix();
        cellMatrix = new Cell[matrix.Length, matrix[0].Length];
        for (int i = 0; i < matrix.Length; i++)
        {
            for (int j = 0; j < matrix[i].Length; j++)
            {
                cellMatrix[i, j] = matrix[i][j].GetComponent<Cell>();
            }
        }


        yield return new WaitForSeconds(1f);

        while (!GameManager.instance.gameOver)
        {
            yield return new WaitForSecondsRealtime(turnTime);
            yield return LogicPlay();
            //bool actionDone = LogicPlay();
            //if (!actionDone)
            //{
            //    // Si no hay lógica aplicable, jugar aleatoriamente
            //    RandomPlay();
            //}
            AIDebugText.text = "Move: " + moves++;
            yield return null;
        }
    }


    // Lógica general del bot

    IEnumerator LogicPlay()
    {
        if (!firstPlay)
        {
            bool actioned = false;

            foreach (Cell cell in cellMatrix)
            {
                StartCoroutine(Debug(cell));
                yield return new WaitForSecondsRealtime(0.02F);
                bool cellResolved = false;
                int bombs = -1;
                bool securedBombs = false;
                bool securedSafe = false;

                //IF CELL IS DISCOVERED, GET ADJACENT CELLS.
                if (cell.isSeen())
                {
                    int adjacentUndiscoveredCells = Generator.instance.GetUndiscoveredAdjacents(cell.getX(), cell.getY());
                    int adjacentFlaggedCells = Generator.instance.GetFlaggedAdjacentTo(cell.getX(), cell.getY());
                    //IF ALL ADJACENT SAFES ARE DISCOVERED AND/OR ALL BOMBS ARE FLAGGED, CELL IS RESOLVED
                    if (adjacentUndiscoveredCells == adjacentFlaggedCells)
                        cellResolved = true;
                    else
                    {
                        bombs = Generator.instance.GetBombsAround(cell.getX(), cell.getY());
                    }
                    //IF CELL IS NOT RESOLVED, PROCESS. OTHERWISE, SKIP CELL.
                    if (!cellResolved)
                    {
                        //IF CELL HAS BOMBS BY IT, TRY TO CALCULATE IF REMAINING CELLS ARE SAFE TO DISCOVER
                        //IF NUMBER OF BOMBS ADJACENT = NUMBER OF FLAGS ADJACENT = ALL UNDISCOVERED UNMARKED CELLS ARE SAFE TO PRESS

                        if (bombs == adjacentFlaggedCells)
                        {
                            securedSafe = true;
                        }

                        if (securedSafe)
                        {
                            Generator.instance.DrawBombsAdjacentTo(cell.getX(), cell.getY());
                            actioned = true; break;
                            //return true;
                        }
                        //ELSE, CHECK IF BOMBS NUMBER - FLAGGED CELLS - DISCOVERED CELLS = NUMBER OF
                        //UNDISCOVERED/UNFLAGGED CELLS AROUND = ALL AVAILABLE CELLS LEFT ARE BOMBS
                        else
                        {
                            if ((adjacentUndiscoveredCells - adjacentFlaggedCells) == (bombs - adjacentFlaggedCells))
                            {
                                //REMAINING UNDISCOVERED CELLS ARE BOMBS
                                securedBombs = true;
                            }
                        }

                        //IF ALL UNDISCOVERED ADJACENTS HAVE BOMBS, MARK ALL.
                        if (securedBombs)
                        {
                            Generator.instance.FlagCellsAdjacentTo(cell.getX(), cell.getY());
                            actioned |= true; break;
                            //return true;
                        }
                    }
                }

            }

            if (!actioned) RandomPlay();
            //IF NONE MEET SPECTATIONS, RESORT TO LOGICAL RANDOM PLAY

            // Buscamos todas las casilla comprobadas con bombas alrededor (check == true)

            // Para cada casilla comprobada   

            // Regla 1: todas ocultas son minas: click_derecho (Flag) 


            // Regla 2: todas ocultas son seguras: clic_izquierdo (Flag)
        }
        else RandomPlay();
        //return false;
    }

    void RandomPlay()
    {
        if (firstPlay)
        {
            bool played = false;
            while (!played)
            {
                Cell c = cellMatrix[Random.Range(0, cellMatrix.GetLength(0)), Random.Range(0, cellMatrix.GetLength(0))];
                if (!c.isSeen())
                {
                    played = true;
                    c.DrawBomb();
                }
            }
            firstPlay = false;
            return;
        }

        //TRY RATIONAL CELL AWAY FROM THE ONES EXPOSED TO BOMBS
        List<Cell> safeRandoms = new();
        for (int i = 0; i < cellMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < cellMatrix.GetLength(1); j++)
            {
                Cell c = cellMatrix[i, j];
                if (!c.isSeen() && Generator.instance.GetUndiscoveredAdjacents(c.getX(), c.getY()) == Generator.instance.GetPossibleAdjacents(c.getX(), c.getY()))
                {
                    safeRandoms.Add(c);
                }
            }
        }


        //IF SAFE PLAY FAILED
        if (safeRandoms.Count == 0)
        {
            //TRY ANY CELL
            bool played = false;
            while (!played)
            {
                Cell c = cellMatrix[Random.Range(0, cellMatrix.GetLength(0)), Random.Range(0, cellMatrix.GetLength(0))];
                if (!c.isSeen())
                {

                    played = true;
                    c.DrawBomb();
                }
            }
        }
        //ELSE: TRY ANY FROM THE GENERATED LIST OF RELATIVELY SAFE CELLS
        else
        {
            int c = Random.Range(0, safeRandoms.Count);
            safeRandoms[c].DrawBomb();
        }
        // De todas las celdas que no se han chequeado, click_izquierdo en una de forma aleatoria;
    }

    IEnumerator Debug(Cell c)
    {
        Color col = c.GetComponent<SpriteRenderer>().color;
        c.GetComponent<SpriteRenderer>().color = Color.blue;
        yield return new WaitForSecondsRealtime(0.010F);
        c.GetComponent<SpriteRenderer>().color = col;
    }
}

