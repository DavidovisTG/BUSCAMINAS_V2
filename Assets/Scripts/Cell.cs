using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Cell : MonoBehaviour
{

    [SerializeField] private int x, y;
    [SerializeField] private bool bomb, seen, flagged;

    [SerializeField] private Animator animator;

    //INT SETTER: POSICION X/Y
    public void setX(int x) { this.x = x; }
    public void setY(int y) { this.y = y; }
    //INT GETTER: POSICION X/Y
    public int getX() { return x; }
    public int getY() { return y; }

    //BOOLEAN S/G: CELDA TIENE BOMBA
    public void setBomb(bool hasBomb) { bomb = hasBomb; }
    public bool hasBomb() { return bomb; }

    //BOOLEAN S/G: CELDA MARCADA CON BANDERA
    public void setFlag(bool flagged) { this.flagged = flagged; }
    public bool isFlagged() { return flagged; }

    //BOOLEAN S/G: CELDA YA DESCUBIERTA
    public bool isSeen() { return seen; }
    public void setSeen(bool seen) { this.seen = seen; }

    void Start()
    {
        //OCULTAR SPRITE DE BOMBA, DE BANDERA
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
    }

    //DESCUBRIR CELDA CUANDO CLIC IZQD.
    private void OnMouseDown()
    {
        //SI EL JUEGO YA ESTA TERMINADO
        if (GameManager.instance.gameOver) return;

        //CLIC IZQD.
        DrawBomb();
    }



    //FLAG BOMB WHEN RIGHT CLICKED
    private void OnMouseOver()
    {
        if (GameManager.instance.gameOver) return;
        if (isSeen()) return;

        //IF RIGHT CLICK
        if (Input.GetMouseButtonDown(1)) { FlagCell(); }
    }

    public void FlagCell()
    {
        //Added "flag" sprite
        setFlag(!isFlagged());
        transform.GetChild(2).gameObject.SetActive(isFlagged());

        if (!isFlagged()) return;
        Generator.instance.CheckBoardComplete();
    }

    public void DrawBomb()
    {
        if (!isSeen())
        {
            setSeen(true);
            if (hasBomb())
            {
                //End game
                GameManager.instance.gameOver = true;
                StartCoroutine(LoseAndWaitToLoseScreen());

            }
            else
            {
                //HIDE DIAMOND IN CASE ITS FLAGGED
                transform.GetChild(2).gameObject.SetActive(false);

                //Cambiar a color oscuro
                GetComponent<SpriteRenderer>().material.color = Color.grey;
                int bombs = Generator.instance.GetBombsAround(x, y);
                if (bombs == 0)
                    Generator.instance.CheckPiecesAround(x, y);
                else
                    transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = bombs.ToString();

                Generator.instance.CheckBoardComplete();
            }
        }
    }

    private IEnumerator LoseAndWaitToLoseScreen()
    {
        //COLOR OF PRESSED MINED CELL TO RED
        GetComponent<SpriteRenderer>().material.color = Color.red;

        //For each mined cell
        foreach (Cell c in Generator.instance.GetBombedCells())
        {
            //SHOW ITS BOMB
            c.transform.GetChild(1).gameObject.SetActive(true);

            //HIDE DIAMOND IN CASE IT WAS FLAGGED
            c.transform.GetChild(2).gameObject.SetActive(false);
        }

        //ANIMATE EXPLOSION OF PRESSED MINED CELL
        animator.SetTrigger("Explode");
        
        //EXPLOSION SOUND
        //explosionSound.Play();

        yield return new WaitForSecondsRealtime(2F);
        GameManager.instance.GameOverLose();
    }

}
