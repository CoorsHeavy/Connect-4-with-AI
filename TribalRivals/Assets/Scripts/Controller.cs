using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private int matchesWon = 0;
    public Transform[] spawn;
    public GameObject[] floors;
    public GameObject redItem;
    public GameObject blueItem;
    public GameObject redPiece;
    public GameObject bluePiece;
    public GameObject redWeight;
    public GameObject blueWeight;
    public GameObject frame;
    public GameObject mad;
    public GameObject medium;
    public GameObject happy;
    public Canvas canvas;
    public Text message;
    public Text type;
    public Text log;
    public Text score;
    public AudioClip drum;
    public AudioClip panic;
    public AudioClip cheer;
    public class MyEqualityComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            if (x.Length != y.Length)
            {
                return false;
            }
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(int[] obj)
        {
            int result = 17;
            for (int i = 0; i < obj.Length; i++)
            {
                unchecked
                {
                    result = result * 23 + obj[i];
                }
            }
            return result;
        }
    }
    public Dictionary<int[], GameObject> gameMap = new Dictionary<int[], GameObject>(new MyEqualityComparer());
    HashSet<int[,]> yourLessons = new HashSet<int[,]>();
    HashSet<int[,]> aiLessons = new HashSet<int[,]>();
    List<int[,]> past = new List<int[,]>();
    Stack<int[,]> stack = new Stack<int[,]>();
    enum states : int { gameStart = 1, gameDone = 2 };
    private bool inputReady = true;
    private bool draw = false;
    private int[,] gamePlace = new int[7, 6];
    private Vector2[,] positions = new Vector2[7, 6];
    private GameObject[,] placements = new GameObject[7, 6];
    private int victory = 0;
    private int turn = 1;
    private int special = 0;
    private List<string> Eventlog = new List<string>();
    private string guiText = "";
    public int maxLines = 4;
    private bool aiUsed = false;
    private bool aiShifted = false;
    private bool playerUsed = false;
    private bool playerShifted = false;
    Queue<int[]> queue = new Queue<int[]>();
    enum ani : int { ready = 1, taken = 2 };
    int animateState = (int) ani.ready;
    public void Animate()
    {
        if (queue.Count == 0) return;
        if(animateState == (int) ani.ready)
        {
            int[] current = queue.Dequeue();
            int piece = current[0];
            int column = current[1];
            int row = current[2];
            animateState = (int) ani.taken;
            
            if (piece == 3)
            {
                Sprite temp = grab(0, row).GetComponent<SpriteRenderer>().sprite;
                grab(0, row).GetComponent<SpriteRenderer>().sprite = grab(1, row).GetComponent<SpriteRenderer>().sprite;
                grab(1, row).GetComponent<SpriteRenderer>().sprite = grab(2, row).GetComponent<SpriteRenderer>().sprite;
                grab(2, row).GetComponent<SpriteRenderer>().sprite = grab(3, row).GetComponent<SpriteRenderer>().sprite;
                grab(3, row).GetComponent<SpriteRenderer>().sprite = grab(4, row).GetComponent<SpriteRenderer>().sprite;
                grab(4, row).GetComponent<SpriteRenderer>().sprite = grab(5, row).GetComponent<SpriteRenderer>().sprite;
                grab(5, row).GetComponent<SpriteRenderer>().sprite = grab(6, row).GetComponent<SpriteRenderer>().sprite;
                grab(6, row).GetComponent<SpriteRenderer>().sprite = temp;
            }
            if (piece == 2)
            {
                floors[column].SetActive(false);
                IEnumerator p = removeItems(column);
                StartCoroutine(p);
            }
            if (piece == 1)
            {
                gameMap[new int[] { column, row }] = Instantiate(redItem, spawn[column].position, Quaternion.identity);
            }
            if (piece == -1)
            {
                gameMap[new int[] { column, row }] = Instantiate(blueItem, spawn[column].position, Quaternion.identity);
            }
            if (piece == -2)
            {
                floors[column].SetActive(false);
                IEnumerator p = removeItems(column);
                StartCoroutine(p);
            }
            if (piece == -3)
            {
                Sprite temp = grab(0, row).GetComponent<SpriteRenderer>().sprite;
                grab(0, row).GetComponent<SpriteRenderer>().sprite = grab(1, row).GetComponent<SpriteRenderer>().sprite;
                grab(1, row).GetComponent<SpriteRenderer>().sprite = grab(2, row).GetComponent<SpriteRenderer>().sprite;
                grab(2, row).GetComponent<SpriteRenderer>().sprite = grab(3, row).GetComponent<SpriteRenderer>().sprite;
                grab(3, row).GetComponent<SpriteRenderer>().sprite = grab(4, row).GetComponent<SpriteRenderer>().sprite;
                grab(4, row).GetComponent<SpriteRenderer>().sprite = grab(5, row).GetComponent<SpriteRenderer>().sprite;
                grab(5, row).GetComponent<SpriteRenderer>().sprite = grab(6, row).GetComponent<SpriteRenderer>().sprite;
                grab(6, row).GetComponent<SpriteRenderer>().sprite = temp;

            }
            StartCoroutine("readyTime");
        }
    }
    GameObject grab(int column, int row)
    {
        return gameMap[new int[] { column, row }];
    }
    IEnumerator removeItems(int column)
    {
        yield return new WaitForSeconds(1.7f);
        for (int i = 0; i < 6; i++)
        {
            if (gameMap.ContainsKey(new int[] { column, i}))
            {
                Destroy(gameMap[new int[] { column, i }]);
                gameMap.Remove(new int[] { column, i });
            }
        }
    }
    IEnumerator readyTime()
    {
        yield return new WaitForSeconds(2);
        animateState = (int) ani.ready;
        for (int i = 0; i < 7; i++)
        {
            floors[i].SetActive(true);
        }
    }
    public void Start()
    {
        happy.SetActive(false);
        medium.SetActive(true);
        mad.SetActive(false);
        GetComponent<AudioSource>().clip = drum;
        GetComponent<AudioSource>().loop = true;
        GetComponent<AudioSource>().Play();
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                positions[j, i] = new Vector2(j + 0.5f, i + 0.5f);
                //Instantiate(redPiece, new Vector2(j + 0.5f, i + 0.5f), Quaternion.identity);
            }
        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (KeyValuePair<int[], GameObject> kvp in gameMap)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                Debug.Log(kvp.Key[0] + " _ " + kvp.Key[1]);
            }
        }
        if (draw) { message.text = "Draw"; }
        if (turn == 1)
        {
            message.text = "Your Turn";
        }
        if (turn == -1)
        {
            message.text = "Their Turn";
        }
        if (victory == 1)
        {
            message.text = "You win!";
        }
        if (victory == -1)
        {
            message.text = "You Lose";
        }
        Animate();
        type.text = new string[] { "Normal\nTap the number key of the column you want to drop a piece in", "Remove\nTap the number key of the column you want to remove pieces from", "Row Shift\nTap the number key of the row you want to shift to the left." }[special];
        log.text = guiText;
        if (animateState == (int)ani.taken) return;
        for (int i = 7; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                GameObject.Destroy(placements[j, i]);
                if (gamePlace[j, i] == 2)
                {
                    placements[j, i] = Instantiate(redWeight, positions[j, i], Quaternion.identity);
                }
                if (gamePlace[j, i] == 1)
                {
                    placements[j, i] = Instantiate(redPiece, positions[j, i], Quaternion.identity);
                }
                if (gamePlace[j, i] == -1)
                {
                    placements[j, i] = Instantiate(bluePiece, positions[j, i], Quaternion.identity);
                }
                if (gamePlace[j, i] == 2)
                {
                    placements[j, i] = Instantiate(blueWeight, positions[j, i], Quaternion.identity);
                }
                //
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            special = 0;
            type.text = "Normal";
            AddEvent("Piece type changed to Normal");
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            special = 1;
            type.text = "Remover";
            AddEvent("Piece type changed to Remover");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            special = 2;
            type.text = "Row Shift";
            AddEvent("Piece type changed to Shift");
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && special == 1)
        {
            keyAction(2, 0); return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && special == 1)
        {
            keyAction(2, 1); return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && special == 1)
        {
            keyAction(2, 2); return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && special == 1)
        {
            keyAction(2, 3); return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && special == 1)
        {
            keyAction(2, 4); return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6) && special == 1)
        {
            keyAction(2, 5); return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7) && special == 1)
        {
            keyAction(2, 6); return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && special == 2)
        {
            keyAction(3, 0); return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && special == 2)
        {
            keyAction(3, 1); return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && special == 2)
        {
            keyAction(3, 2); return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && special == 2)
        {
            keyAction(3, 3); return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && special == 2)
        {
            keyAction(3, 4); return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6) && special == 2)
        {
            keyAction(3, 5); return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            keyAction(1, 0); return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            keyAction(1, 1); return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            keyAction(1, 2); return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            keyAction(1, 3); return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            keyAction(1, 4); return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            keyAction(1, 5); return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7) && special == 0)
        {
            keyAction(1, 6); return;
        }

    }
    public void victoryProtocol()
    {
        draw = isDraw();
        victory = getWinner(gamePlace);
        if (isDraw())
        {
            inputReady = false;
            StartCoroutine("tieAnimation");
            StartCoroutine("resetMatch");
        }
        if(victory == 1)
        {
            inputReady = false;
            if(stack.Count != 0)
                aiLessons.Add(stack.Pop());
            if (stack.Count != 0)
                aiLessons.Add(stack.Pop());
            stack.Clear();
            matchesWon++;
            score.text = ("" + matchesWon + "");
            StartCoroutine("winAnimation");
            StartCoroutine("resetMatch");
        }
        if (victory == -1)
        {
            inputReady = false;
            
            StartCoroutine("loseAnimation");
            StartCoroutine("resetMatch");
        }
    }
    public IEnumerator winAnimation()
    {
        happy.SetActive(true);
        medium.SetActive(false);
        mad.SetActive(false);
        //GetComponent<AudioSource>().Stop();
        //GetComponent<AudioSource>().loop = true;
        //GetComponent<AudioSource>().PlayOneShot(cheer, 1);
        yield return new WaitForSeconds(5);
        happy.SetActive(false);
        medium.SetActive(true);
        mad.SetActive(false);

    }
    public IEnumerator tieAnimation()
    {
        happy.SetActive(false);
        medium.SetActive(true);
        mad.SetActive(false);
        yield return new WaitForSeconds(5);
        happy.SetActive(false);
        medium.SetActive(true);
        mad.SetActive(false);
    }
    public IEnumerator loseAnimation()
    {
        happy.SetActive(false);
        medium.SetActive(false);
        mad.SetActive(true);
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().loop = true;
        GetComponent<AudioSource>().PlayOneShot(panic, 0.5f);
        yield return new WaitForSeconds(7);
        AddEvent("You lost");
        SceneManager.LoadScene(0);
    }
    public IEnumerator resetMatch()
    {
        yield return new WaitForSeconds(5);
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                try
                {
                    gamePlace[j, i] = 0;   
                    GameObject.Destroy(grab(j, i));
                    GameObject.Destroy(placements[j, i]);
                }catch
                {

                }
            }
        }
        gameMap.Clear();
        victory = 0;
        turn = 1;
        aiUsed = false;
        aiShifted = false;
        playerUsed = false;
        playerShifted = false;
        inputReady = true;
        AddEvent("You won");
        AddEvent("Board Cleared");
    }
    public void keyAction(int piece, int column)
    {
        if (piece == 1)
        {
            if (isColumnOpen(column) && inputReady && !draw)
            {
                action(1, column);
                AddEvent("You dropped on column " + (column + 1));
                StartCoroutine("runAI");
            }
            else
            {
                AddEvent("Invalid Drop. Column Might Be Full.");
            }
        }
        if (piece == 2)
        {
            if (playerUsed)
            {
                AddEvent("You can only remove a column once."); return;
            }
            if (inputReady && !draw && gamePlace[column, 0] != 0 && playerUsed == false)
            {
                action(2, column);
                AddEvent("You removed column " + (column + 1));
                playerUsed = true;
                StartCoroutine("runAI");
            }
            else
            {
                AddEvent("Invalid Remove Move. Column Might Be Empty.");
            }
        }
        if (piece == 3)
        {
            if (playerShifted)
            {
                AddEvent("You can only shift a row once."); return;
            }
            if (!(column <= 6 && column >= 0))
            {
                AddEvent("Invalid Shift Move. There are only six rows.");
                return;
            }
            bool moveable = true;
            for (int k = 0; k < 7; k++)
            {
                if (gamePlace[k, column] == 0) moveable = false;
            }
            if (inputReady && !draw && moveable && playerShifted == false)
            {
                action(3, column);
                AddEvent("You shifted row " + (column + 1));
                playerShifted = true;
                StartCoroutine("runAI");
            }
            else
            {
                AddEvent("Invalid Shift Move. Row must be full first.");
            }
        }

    }
    public void action(int piece, int column)
    {
        if (piece == 3)
        {
            queue.Enqueue(new int[] { 3, 0, column });
            leftRotatebyOne(gamePlace, column);
        }
        if (piece == 2)
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (j == column) {
                        gamePlace[j, i] = 0;
                    }
                }
            }
            queue.Enqueue(new int[] { 2, column, 0 });
        }
        if (piece == 1)
        {
            for (int j = 0; j < 6; j++)
            {
                if (gamePlace[column, j] == 0)
                {
                    queue.Enqueue(new int[] { 1, column, j });
                    gamePlace[column, j] = 1;
                    break;
                }
            }
        }
        if (piece == -1)
        {
            for (int j = 0; j < 6; j++)
            {
                if (gamePlace[column, j] == 0)
                {
                    queue.Enqueue(new int[] {-1, column, j });
                    gamePlace[column, j] = -1;
                    break;
                }
            }
        }
        if (piece == -2)
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (j == column) gamePlace[j, i] = 0;
                }
            }
            queue.Enqueue(new int[] { -2, column, 0 });
        }
        if(piece == -3)
        {
            queue.Enqueue(new int[] { 3, 0, column });
            leftRotatebyOne(gamePlace, column);
        }
        
        stack.Push(gamePlace);
        victoryProtocol();
    }
    public bool isDraw()
    {
        bool result = true;
        for (int i = 0; i < 7; i++)
        {
            if (gamePlace[i, 5] == 0)
            {
                result = false;
                break;
            }
        }
        return result;
    }
    public void AddEvent(string eventString)
    {
        Eventlog.Add(eventString);

        if (Eventlog.Count >= maxLines)
            Eventlog.RemoveAt(0);

        guiText = "";

        foreach (string logEvent in Eventlog)
        {
            guiText += logEvent;
            guiText += "\n";
        }
    }
    public bool isColumnOpen(int column)
    {
        if (gamePlace[column, 5] != 0)
        {
            return false;
        }
        return true;
    }
    public int getWinner(int[,] matrix)
    {
        if (matrix[0, 0] == -1 && matrix[1, 0] == -1 && matrix[2, 0] == -1 && matrix[3, 0] == -1) { return -1; }
        if (matrix[0, 0] == 1 && matrix[1, 0] == 1 && matrix[2, 0] == 1 && matrix[3, 0] == 1) { return 1; }
        if (matrix[1, 0] == -1 && matrix[2, 0] == -1 && matrix[3, 0] == -1 && matrix[4, 0] == -1) { return -1; }
        if (matrix[1, 0] == 1 && matrix[2, 0] == 1 && matrix[3, 0] == 1 && matrix[4, 0] == 1) { return 1; }
        if (matrix[2, 0] == -1 && matrix[3, 0] == -1 && matrix[4, 0] == -1 && matrix[5, 0] == -1) { return -1; }
        if (matrix[2, 0] == 1 && matrix[3, 0] == 1 && matrix[4, 0] == 1 && matrix[5, 0] == 1) { return 1; }
        if (matrix[3, 0] == -1 && matrix[4, 0] == -1 && matrix[5, 0] == -1 && matrix[6, 0] == -1) { return -1; }
        if (matrix[3, 0] == 1 && matrix[4, 0] == 1 && matrix[5, 0] == 1 && matrix[6, 0] == 1) { return 1; }
        if (matrix[0, 1] == -1 && matrix[1, 1] == -1 && matrix[2, 1] == -1 && matrix[3, 1] == -1) { return -1; }
        if (matrix[0, 1] == 1 && matrix[1, 1] == 1 && matrix[2, 1] == 1 && matrix[3, 1] == 1) { return 1; }
        if (matrix[1, 1] == -1 && matrix[2, 1] == -1 && matrix[3, 1] == -1 && matrix[4, 1] == -1) { return -1; }
        if (matrix[1, 1] == 1 && matrix[2, 1] == 1 && matrix[3, 1] == 1 && matrix[4, 1] == 1) { return 1; }
        if (matrix[2, 1] == -1 && matrix[3, 1] == -1 && matrix[4, 1] == -1 && matrix[5, 1] == -1) { return -1; }
        if (matrix[2, 1] == 1 && matrix[3, 1] == 1 && matrix[4, 1] == 1 && matrix[5, 1] == 1) { return 1; }
        if (matrix[3, 1] == -1 && matrix[4, 1] == -1 && matrix[5, 1] == -1 && matrix[6, 1] == -1) { return -1; }
        if (matrix[3, 1] == 1 && matrix[4, 1] == 1 && matrix[5, 1] == 1 && matrix[6, 1] == 1) { return 1; }
        if (matrix[0, 2] == -1 && matrix[1, 2] == -1 && matrix[2, 2] == -1 && matrix[3, 2] == -1) { return -1; }
        if (matrix[0, 2] == 1 && matrix[1, 2] == 1 && matrix[2, 2] == 1 && matrix[3, 2] == 1) { return 1; }
        if (matrix[1, 2] == -1 && matrix[2, 2] == -1 && matrix[3, 2] == -1 && matrix[4, 2] == -1) { return -1; }
        if (matrix[1, 2] == 1 && matrix[2, 2] == 1 && matrix[3, 2] == 1 && matrix[4, 2] == 1) { return 1; }
        if (matrix[2, 2] == -1 && matrix[3, 2] == -1 && matrix[4, 2] == -1 && matrix[5, 2] == -1) { return -1; }
        if (matrix[2, 2] == 1 && matrix[3, 2] == 1 && matrix[4, 2] == 1 && matrix[5, 2] == 1) { return 1; }
        if (matrix[3, 2] == -1 && matrix[4, 2] == -1 && matrix[5, 2] == -1 && matrix[6, 2] == -1) { return -1; }
        if (matrix[3, 2] == 1 && matrix[4, 2] == 1 && matrix[5, 2] == 1 && matrix[6, 2] == 1) { return 1; }
        if (matrix[0, 3] == -1 && matrix[1, 3] == -1 && matrix[2, 3] == -1 && matrix[3, 3] == -1) { return -1; }
        if (matrix[0, 3] == 1 && matrix[1, 3] == 1 && matrix[2, 3] == 1 && matrix[3, 3] == 1) { return 1; }
        if (matrix[1, 3] == -1 && matrix[2, 3] == -1 && matrix[3, 3] == -1 && matrix[4, 3] == -1) { return -1; }
        if (matrix[1, 3] == 1 && matrix[2, 3] == 1 && matrix[3, 3] == 1 && matrix[4, 3] == 1) { return 1; }
        if (matrix[2, 3] == -1 && matrix[3, 3] == -1 && matrix[4, 3] == -1 && matrix[5, 3] == -1) { return -1; }
        if (matrix[2, 3] == 1 && matrix[3, 3] == 1 && matrix[4, 3] == 1 && matrix[5, 3] == 1) { return 1; }
        if (matrix[3, 3] == -1 && matrix[4, 3] == -1 && matrix[5, 3] == -1 && matrix[6, 3] == -1) { return -1; }
        if (matrix[3, 3] == 1 && matrix[4, 3] == 1 && matrix[5, 3] == 1 && matrix[6, 3] == 1) { return 1; }
        if (matrix[0, 4] == -1 && matrix[1, 4] == -1 && matrix[2, 4] == -1 && matrix[3, 4] == -1) { return -1; }
        if (matrix[0, 4] == 1 && matrix[1, 4] == 1 && matrix[2, 4] == 1 && matrix[3, 4] == 1) { return 1; }
        if (matrix[1, 4] == -1 && matrix[2, 4] == -1 && matrix[3, 4] == -1 && matrix[4, 4] == -1) { return -1; }
        if (matrix[1, 4] == 1 && matrix[2, 4] == 1 && matrix[3, 4] == 1 && matrix[4, 4] == 1) { return 1; }
        if (matrix[2, 4] == -1 && matrix[3, 4] == -1 && matrix[4, 4] == -1 && matrix[5, 4] == -1) { return -1; }
        if (matrix[2, 4] == 1 && matrix[3, 4] == 1 && matrix[4, 4] == 1 && matrix[5, 4] == 1) { return 1; }
        if (matrix[3, 4] == -1 && matrix[4, 4] == -1 && matrix[5, 4] == -1 && matrix[6, 4] == -1) { return -1; }
        if (matrix[3, 4] == 1 && matrix[4, 4] == 1 && matrix[5, 4] == 1 && matrix[6, 4] == 1) { return 1; }
        if (matrix[0, 5] == -1 && matrix[1, 5] == -1 && matrix[2, 5] == -1 && matrix[3, 5] == -1) { return -1; }
        if (matrix[0, 5] == 1 && matrix[1, 5] == 1 && matrix[2, 5] == 1 && matrix[3, 5] == 1) { return 1; }
        if (matrix[1, 5] == -1 && matrix[2, 5] == -1 && matrix[3, 5] == -1 && matrix[4, 5] == -1) { return -1; }
        if (matrix[1, 5] == 1 && matrix[2, 5] == 1 && matrix[3, 5] == 1 && matrix[4, 5] == 1) { return 1; }
        if (matrix[2, 5] == -1 && matrix[3, 5] == -1 && matrix[4, 5] == -1 && matrix[5, 5] == -1) { return -1; }
        if (matrix[2, 5] == 1 && matrix[3, 5] == 1 && matrix[4, 5] == 1 && matrix[5, 5] == 1) { return 1; }
        if (matrix[3, 5] == -1 && matrix[4, 5] == -1 && matrix[5, 5] == -1 && matrix[6, 5] == -1) { return -1; }
        if (matrix[3, 5] == 1 && matrix[4, 5] == 1 && matrix[5, 5] == 1 && matrix[6, 5] == 1) { return 1; }
        if (matrix[0, 0] == -1 && matrix[0, 1] == -1 && matrix[0, 2] == -1 && matrix[0, 3] == -1) { return -1; }
        if (matrix[0, 0] == 1 && matrix[0, 1] == 1 && matrix[0, 2] == 1 && matrix[0, 3] == 1) { return 1; }
        if (matrix[0, 1] == -1 && matrix[0, 2] == -1 && matrix[0, 3] == -1 && matrix[0, 4] == -1) { return -1; }
        if (matrix[0, 1] == 1 && matrix[0, 2] == 1 && matrix[0, 3] == 1 && matrix[0, 4] == 1) { return 1; }
        if (matrix[0, 2] == -1 && matrix[0, 3] == -1 && matrix[0, 4] == -1 && matrix[0, 5] == -1) { return -1; }
        if (matrix[0, 2] == 1 && matrix[0, 3] == 1 && matrix[0, 4] == 1 && matrix[0, 5] == 1) { return 1; }
        if (matrix[1, 0] == -1 && matrix[1, 1] == -1 && matrix[1, 2] == -1 && matrix[1, 3] == -1) { return -1; }
        if (matrix[1, 0] == 1 && matrix[1, 1] == 1 && matrix[1, 2] == 1 && matrix[1, 3] == 1) { return 1; }
        if (matrix[1, 1] == -1 && matrix[1, 2] == -1 && matrix[1, 3] == -1 && matrix[1, 4] == -1) { return -1; }
        if (matrix[1, 1] == 1 && matrix[1, 2] == 1 && matrix[1, 3] == 1 && matrix[1, 4] == 1) { return 1; }
        if (matrix[1, 2] == -1 && matrix[1, 3] == -1 && matrix[1, 4] == -1 && matrix[1, 5] == -1) { return -1; }
        if (matrix[1, 2] == 1 && matrix[1, 3] == 1 && matrix[1, 4] == 1 && matrix[1, 5] == 1) { return 1; }
        if (matrix[2, 0] == -1 && matrix[2, 1] == -1 && matrix[2, 2] == -1 && matrix[2, 3] == -1) { return -1; }
        if (matrix[2, 0] == 1 && matrix[2, 1] == 1 && matrix[2, 2] == 1 && matrix[2, 3] == 1) { return 1; }
        if (matrix[2, 1] == -1 && matrix[2, 2] == -1 && matrix[2, 3] == -1 && matrix[2, 4] == -1) { return -1; }
        if (matrix[2, 1] == 1 && matrix[2, 2] == 1 && matrix[2, 3] == 1 && matrix[2, 4] == 1) { return 1; }
        if (matrix[2, 2] == -1 && matrix[2, 3] == -1 && matrix[2, 4] == -1 && matrix[2, 5] == -1) { return -1; }
        if (matrix[2, 2] == 1 && matrix[2, 3] == 1 && matrix[2, 4] == 1 && matrix[2, 5] == 1) { return 1; }
        if (matrix[3, 0] == -1 && matrix[3, 1] == -1 && matrix[3, 2] == -1 && matrix[3, 3] == -1) { return -1; }
        if (matrix[3, 0] == 1 && matrix[3, 1] == 1 && matrix[3, 2] == 1 && matrix[3, 3] == 1) { return 1; }
        if (matrix[3, 1] == -1 && matrix[3, 2] == -1 && matrix[3, 3] == -1 && matrix[3, 4] == -1) { return -1; }
        if (matrix[3, 1] == 1 && matrix[3, 2] == 1 && matrix[3, 3] == 1 && matrix[3, 4] == 1) { return 1; }
        if (matrix[3, 2] == -1 && matrix[3, 3] == -1 && matrix[3, 4] == -1 && matrix[3, 5] == -1) { return -1; }
        if (matrix[3, 2] == 1 && matrix[3, 3] == 1 && matrix[3, 4] == 1 && matrix[3, 5] == 1) { return 1; }
        if (matrix[4, 0] == -1 && matrix[4, 1] == -1 && matrix[4, 2] == -1 && matrix[4, 3] == -1) { return -1; }
        if (matrix[4, 0] == 1 && matrix[4, 1] == 1 && matrix[4, 2] == 1 && matrix[4, 3] == 1) { return 1; }
        if (matrix[4, 1] == -1 && matrix[4, 2] == -1 && matrix[4, 3] == -1 && matrix[4, 4] == -1) { return -1; }
        if (matrix[4, 1] == 1 && matrix[4, 2] == 1 && matrix[4, 3] == 1 && matrix[4, 4] == 1) { return 1; }
        if (matrix[4, 2] == -1 && matrix[4, 3] == -1 && matrix[4, 4] == -1 && matrix[4, 5] == -1) { return -1; }
        if (matrix[4, 2] == 1 && matrix[4, 3] == 1 && matrix[4, 4] == 1 && matrix[4, 5] == 1) { return 1; }
        if (matrix[5, 0] == -1 && matrix[5, 1] == -1 && matrix[5, 2] == -1 && matrix[5, 3] == -1) { return -1; }
        if (matrix[5, 0] == 1 && matrix[5, 1] == 1 && matrix[5, 2] == 1 && matrix[5, 3] == 1) { return 1; }
        if (matrix[5, 1] == -1 && matrix[5, 2] == -1 && matrix[5, 3] == -1 && matrix[5, 4] == -1) { return -1; }
        if (matrix[5, 1] == 1 && matrix[5, 2] == 1 && matrix[5, 3] == 1 && matrix[5, 4] == 1) { return 1; }
        if (matrix[5, 2] == -1 && matrix[5, 3] == -1 && matrix[5, 4] == -1 && matrix[5, 5] == -1) { return -1; }
        if (matrix[5, 2] == 1 && matrix[5, 3] == 1 && matrix[5, 4] == 1 && matrix[5, 5] == 1) { return 1; }
        if (matrix[6, 0] == -1 && matrix[6, 1] == -1 && matrix[6, 2] == -1 && matrix[6, 3] == -1) { return -1; }
        if (matrix[6, 0] == 1 && matrix[6, 1] == 1 && matrix[6, 2] == 1 && matrix[6, 3] == 1) { return 1; }
        if (matrix[6, 1] == -1 && matrix[6, 2] == -1 && matrix[6, 3] == -1 && matrix[6, 4] == -1) { return -1; }
        if (matrix[6, 1] == 1 && matrix[6, 2] == 1 && matrix[6, 3] == 1 && matrix[6, 4] == 1) { return 1; }
        if (matrix[6, 2] == -1 && matrix[6, 3] == -1 && matrix[6, 4] == -1 && matrix[6, 5] == -1) { return -1; }
        if (matrix[6, 2] == 1 && matrix[6, 3] == 1 && matrix[6, 4] == 1 && matrix[6, 5] == 1) { return 1; }
        if (matrix[0, 3] == -1 && matrix[1, 2] == -1 && matrix[2, 1] == -1 && matrix[3, 0] == -1) { return -1; }
        if (matrix[0, 3] == 1 && matrix[1, 2] == 1 && matrix[2, 1] == 1 && matrix[3, 0] == 1) { return 1; }
        if (matrix[3, 3] == -1 && matrix[2, 2] == -1 && matrix[1, 1] == -1 && matrix[0, 0] == -1) { return -1; }
        if (matrix[3, 3] == 1 && matrix[2, 2] == 1 && matrix[1, 1] == 1 && matrix[0, 0] == 1) { return 1; }
        if (matrix[0, 4] == -1 && matrix[1, 3] == -1 && matrix[2, 2] == -1 && matrix[3, 1] == -1) { return -1; }
        if (matrix[0, 4] == 1 && matrix[1, 3] == 1 && matrix[2, 2] == 1 && matrix[3, 1] == 1) { return 1; }
        if (matrix[3, 4] == -1 && matrix[2, 3] == -1 && matrix[1, 2] == -1 && matrix[0, 1] == -1) { return -1; }
        if (matrix[3, 4] == 1 && matrix[2, 3] == 1 && matrix[1, 2] == 1 && matrix[0, 1] == 1) { return 1; }
        if (matrix[0, 5] == -1 && matrix[1, 4] == -1 && matrix[2, 3] == -1 && matrix[3, 2] == -1) { return -1; }
        if (matrix[0, 5] == 1 && matrix[1, 4] == 1 && matrix[2, 3] == 1 && matrix[3, 2] == 1) { return 1; }
        if (matrix[3, 5] == -1 && matrix[2, 4] == -1 && matrix[1, 3] == -1 && matrix[0, 2] == -1) { return -1; }
        if (matrix[3, 5] == 1 && matrix[2, 4] == 1 && matrix[1, 3] == 1 && matrix[0, 2] == 1) { return 1; }
        if (matrix[1, 3] == -1 && matrix[2, 2] == -1 && matrix[3, 1] == -1 && matrix[4, 0] == -1) { return -1; }
        if (matrix[1, 3] == 1 && matrix[2, 2] == 1 && matrix[3, 1] == 1 && matrix[4, 0] == 1) { return 1; }
        if (matrix[4, 3] == -1 && matrix[3, 2] == -1 && matrix[2, 1] == -1 && matrix[1, 0] == -1) { return -1; }
        if (matrix[4, 3] == 1 && matrix[3, 2] == 1 && matrix[2, 1] == 1 && matrix[1, 0] == 1) { return 1; }
        if (matrix[1, 4] == -1 && matrix[2, 3] == -1 && matrix[3, 2] == -1 && matrix[4, 1] == -1) { return -1; }
        if (matrix[1, 4] == 1 && matrix[2, 3] == 1 && matrix[3, 2] == 1 && matrix[4, 1] == 1) { return 1; }
        if (matrix[4, 4] == -1 && matrix[3, 3] == -1 && matrix[2, 2] == -1 && matrix[1, 1] == -1) { return -1; }
        if (matrix[4, 4] == 1 && matrix[3, 3] == 1 && matrix[2, 2] == 1 && matrix[1, 1] == 1) { return 1; }
        if (matrix[1, 5] == -1 && matrix[2, 4] == -1 && matrix[3, 3] == -1 && matrix[4, 2] == -1) { return -1; }
        if (matrix[1, 5] == 1 && matrix[2, 4] == 1 && matrix[3, 3] == 1 && matrix[4, 2] == 1) { return 1; }
        if (matrix[4, 5] == -1 && matrix[3, 4] == -1 && matrix[2, 3] == -1 && matrix[1, 2] == -1) { return -1; }
        if (matrix[4, 5] == 1 && matrix[3, 4] == 1 && matrix[2, 3] == 1 && matrix[1, 2] == 1) { return 1; }
        if (matrix[2, 3] == -1 && matrix[3, 2] == -1 && matrix[4, 1] == -1 && matrix[5, 0] == -1) { return -1; }
        if (matrix[2, 3] == 1 && matrix[3, 2] == 1 && matrix[4, 1] == 1 && matrix[5, 0] == 1) { return 1; }
        if (matrix[5, 3] == -1 && matrix[4, 2] == -1 && matrix[3, 1] == -1 && matrix[2, 0] == -1) { return -1; }
        if (matrix[5, 3] == 1 && matrix[4, 2] == 1 && matrix[3, 1] == 1 && matrix[2, 0] == 1) { return 1; }
        if (matrix[2, 4] == -1 && matrix[3, 3] == -1 && matrix[4, 2] == -1 && matrix[5, 1] == -1) { return -1; }
        if (matrix[2, 4] == 1 && matrix[3, 3] == 1 && matrix[4, 2] == 1 && matrix[5, 1] == 1) { return 1; }
        if (matrix[5, 4] == -1 && matrix[4, 3] == -1 && matrix[3, 2] == -1 && matrix[2, 1] == -1) { return -1; }
        if (matrix[5, 4] == 1 && matrix[4, 3] == 1 && matrix[3, 2] == 1 && matrix[2, 1] == 1) { return 1; }
        if (matrix[2, 5] == -1 && matrix[3, 4] == -1 && matrix[4, 3] == -1 && matrix[5, 2] == -1) { return -1; }
        if (matrix[2, 5] == 1 && matrix[3, 4] == 1 && matrix[4, 3] == 1 && matrix[5, 2] == 1) { return 1; }
        if (matrix[5, 5] == -1 && matrix[4, 4] == -1 && matrix[3, 3] == -1 && matrix[2, 2] == -1) { return -1; }
        if (matrix[5, 5] == 1 && matrix[4, 4] == 1 && matrix[3, 3] == 1 && matrix[2, 2] == 1) { return 1; }
        if (matrix[3, 3] == -1 && matrix[4, 2] == -1 && matrix[5, 1] == -1 && matrix[6, 0] == -1) { return -1; }
        if (matrix[3, 3] == 1 && matrix[4, 2] == 1 && matrix[5, 1] == 1 && matrix[6, 0] == 1) { return 1; }
        if (matrix[6, 3] == -1 && matrix[5, 2] == -1 && matrix[4, 1] == -1 && matrix[3, 0] == -1) { return -1; }
        if (matrix[6, 3] == 1 && matrix[5, 2] == 1 && matrix[4, 1] == 1 && matrix[3, 0] == 1) { return 1; }
        if (matrix[3, 4] == -1 && matrix[4, 3] == -1 && matrix[5, 2] == -1 && matrix[6, 1] == -1) { return -1; }
        if (matrix[3, 4] == 1 && matrix[4, 3] == 1 && matrix[5, 2] == 1 && matrix[6, 1] == 1) { return 1; }
        if (matrix[6, 4] == -1 && matrix[5, 3] == -1 && matrix[4, 2] == -1 && matrix[3, 1] == -1) { return -1; }
        if (matrix[6, 4] == 1 && matrix[5, 3] == 1 && matrix[4, 2] == 1 && matrix[3, 1] == 1) { return 1; }
        if (matrix[3, 5] == -1 && matrix[4, 4] == -1 && matrix[5, 3] == -1 && matrix[6, 2] == -1) { return -1; }
        if (matrix[3, 5] == 1 && matrix[4, 4] == 1 && matrix[5, 3] == 1 && matrix[6, 2] == 1) { return 1; }
        if (matrix[6, 5] == -1 && matrix[5, 4] == -1 && matrix[4, 3] == -1 && matrix[3, 2] == -1) { return -1; }
        if (matrix[6, 5] == 1 && matrix[5, 4] == 1 && matrix[4, 3] == 1 && matrix[3, 2] == 1) { return 1; }
        return 0;
    }
    public void leftRotatebyOne(int[,] arr, int y)
    {
        int i, temp;
        temp = arr[0, y];
        for (i = 0; i < 7 - 1; i++)
            arr[i, y] = arr[i + 1, y];
        arr[i, y] = temp;
    }
    public IEnumerator runAI()
    {

        inputReady = false;
        turn = -1;
        yield return new WaitForSeconds(4);
        if (isDraw() || getWinner(gamePlace) == -1 || getWinner(gamePlace) == 1)
        {

        }
        else
        {
            int[] move = doSomeWork(gamePlace, 4, aiUsed, aiShifted);
            if (move[0] == -1)
            {
                AddEvent("They dropped on column " + (move[1] + 1));
            }
            if (move[0] == -2)
            {
                aiUsed = true;
                AddEvent("They removed column " + (move[1] + 1));
            }
            if (move[0] == -3)
            {
                aiShifted = true;
                AddEvent("They shifted row " + (move[1] + 1));
            }
            action(move[0], move[1]);
            inputReady = true;
            turn = 1;
        }
    }
    public int[] doSomeWork(int[,] board, int depth, bool used, bool shifted)
    {
        List<int[]> move = new List<int[]> { new int[]{ 0, -1 } };
        float max = Mathf.NegativeInfinity;
        for (int i = 0; i < 7; i++)
        {
            float result;
            int col = getOpenning(board, i);
            if (col != -1)
            {
                if(getWinner(generateResult(board, i, col, -1)) == -1)
                {
                    return new int[] { -1, i };
                }
                result = howGoodIsThisMove(false, generateResult(board, i, col, -1), depth - 1, i, col, used, shifted);

                if (result > max)
                {
                    max = result;
                    move = new List<int[]> { new int[] { -1, i } };
                }
                if (result == max)
                {
                    move.Add(new int[] { -1, i });
                }
                Debug.Log(i + ", " + (-1) + ", " + result);
            }
            if (!used && (col > 0 || col == -1))
            {
                result = howGoodIsThisMove(false, generateResult(board, i, col, -2), depth - 1, i, col, true, shifted);
                if (result > max)
                {
                    max = result;
                    move = new List<int[]> { new int[] { -2, i } };
                }
                if (result == max)
                {
                    move.Add(new int[] { -2, i });
                }
                Debug.Log(i + ", " + (-2) + ", " + result);
            }

            if ( !shifted && (i < 6))
            {
                bool moveable = true;
                for (int k = 0; k < 7; k++)
                {
                    if (board[k, i] == 0) moveable = false;
                }
                if (moveable) {
                    result = howGoodIsThisMove(false, generateResult(board, i, col, -3), depth - 1, i, col, used, true) - 2;
                    if (result > max)
                    {
                        max = result;
                        move = new List<int[]> { new int[] { -3, i } };
                    }
                    if (result == max)
                    {
                        move.Add(new int[] { -3, i });
                    }
                    Debug.Log(i + ", " + (-3) + ", " + result);
                }
            }
        }
        List<int[]> slides = new List<int[] > { };
        foreach(int[] mov in move)
        {
            if (mov[0] == -1) slides.Add(mov);
        }
        if(slides.Count != 0)
        {
            int x = Random.Range(0, slides.Count);
            return slides[x];
        }
        int r = Random.Range(0, move.Count);
        return move[r];
    }
    public float howGoodIsThisMove(bool turn, int[,] board, int depth, int lastmoveX, int lastMoveY, bool used, bool shifted)
    {
        if (getWinner(board) == 1 || aiLessons.Contains(board))
        {
            return Mathf.NegativeInfinity;
        }else if(getWinner(board) == -1 || yourLessons.Contains(board))
        {
            return Mathf.Infinity;
        }
        else if (isDraw(board))
        {
            return 0;
        }
        else if (depth < 0)
        {
            return evalboard(board);
        }
        else if (turn)
        {
            //return max
            float max = Mathf.NegativeInfinity;
            for (int i = 0; i < 7; i++)
            {
                float result;
                int col = getOpenning(board, i);
                if (col != -1)
                {
                    result = howGoodIsThisMove(!turn, generateResult(board, i, col, -1), depth - 1, i, col, used, shifted);
                    if (result > max)
                    {
                        max = result;
                    }
                }
                if (col > 0 && !used)
                {
                    result = howGoodIsThisMove(!turn, generateResult(board, i, col, -2), depth - 1, i, col, true, shifted);
                    if (result > max)
                    {
                        max = result;
                    }
                }
                if (i < 6 && !shifted)
                {
                    bool moveable = true;
                    for (int k = 0; k < 7; k++)
                    {
                        if (board[k, i] == 0) moveable = false;
                    }
                    if (moveable)
                    {
                        result = howGoodIsThisMove(false, generateResult(board, i, col, -3), depth - 1, i, col, used, true) - 2;
                        if (result > max)
                        {
                            max = result;
                        }
                    }
                }
            }
            return max;
        }
        else
        {
            float min = Mathf.Infinity;
            for (int i = 0; i < 7; i++)
            {
                float result;
                int col = getOpenning(board, i);
                if (col != -1)
                {
                    result = howGoodIsThisMove(!turn, generateResult(board, i, col, 1), depth - 1, i, col, used, shifted);
                    if (result < min)
                    {
                        min = result;
                    }
                }
                if (col > 0 && !used) {
                    result = howGoodIsThisMove(!turn, generateResult(board, i, col, 2), depth - 1, i, col, true, shifted);
                    if (result < min)
                    {
                        min = result;
                    }
                }
                if (i < 6 && !shifted)
                {
                    bool moveable = true;
                    for (int k = 0; k < 7; k++)
                    {
                        if (board[k, i] == 0) moveable = false;
                    }
                    if (moveable)
                    {
                        result = howGoodIsThisMove(false, generateResult(board, i, col, 3), depth - 1, i, col, used, true) - 2;
                        if (result < min)
                        {
                            min = result;
                        }
                    }
                }
            }
            return min;
        }

    }
    public int[,] generateResult(int[,] board, int x, int y, int value)
    {   
        //copy
        int[,] newBoard = new int[7, 6];
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                newBoard[i, j] = board[i, j];
            }
        }
        //put piece
        if (value == 1 || value == -1)
        {
            newBoard[x, y] = value;
        }
        //clear row
        else if(value == -2 || value == 2)
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    newBoard[x, j] = 0;
                }
            }
        }
        else if (value == -3 || value == 3)
        {
            leftRotatebyOne(newBoard, x);
        }
        return newBoard;
    }
    public float evalboard(int[,] board)
    {
        float ret = 0;
        for(int i = 0; i < 7; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                ret += -1 * board[i, j] * computeValueForLocation(board, i, j) ^ 2;
            }
        }
        return ret;
    }
    public int getOpenning(int[,] board, int row)
    {
        int ret = -1;
        for (int j = 0; j < 6; j++)
        {
            if (board[row, j] == 0)
            {
                ret = j;
                break;
            }
        }
        return ret;
    }
    public int computeValueForLocation(int[,] board, int col, int row)
    {
        int value = 0;
        value += lookInDirection(board, col, row, -1, -1);
        value += lookInDirection(board, col, row, -1, 0);
        value += lookInDirection(board, col, row, -1, 1);
        value += lookInDirection(board, col, row, 1, -1);
        value += lookInDirection(board, col, row, 1, 0);
        value += lookInDirection(board, col, row, 1, 1);
        value += lookInDirection(board, col, row, 0, 1);
        value += lookInDirection(board, col, row, 0, -1);
        return value;
    }
    public int lookInDirection(int[,] board, int x, int y, int directionVert, int directionHor)
    {
        int ret = 0;
        if (x + directionHor < 0 || x + directionHor > 6 || y + directionVert < 0 || y + directionVert > 5)
        {
            return 0;
        }
        int first = board[x + directionHor, y + directionVert];
        int vertOffset = directionVert;
        int horOffset = directionHor;
        while (board[x + horOffset, y + vertOffset] != 0 && board[x + horOffset, y + vertOffset] == first)
        {
            ret += board[x + horOffset, y + vertOffset];
            vertOffset += directionVert;
            horOffset += directionHor;
            if (x + horOffset < 0 || x + horOffset > 6 || y + vertOffset < 0 || y + vertOffset > 5)
            {
                break;
            }
        }
        return ret;
    }
    public bool isDraw(int[,] board)
    {
        bool result = true;
        for (int i = 0; i < 7; i++)
        {
            if (board[i, 5] == 0)
            {
                result = false;
                break;
            }
        }
        return result;
    }
}