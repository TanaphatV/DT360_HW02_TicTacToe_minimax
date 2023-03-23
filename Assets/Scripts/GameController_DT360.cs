using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController_DT360 : MonoBehaviour
{

    public int whosTurn;
    public int turnCount;
    public int[] markedSpaces;
    public Button[] tictactoeSpaces;

    public GameObject[] turnIcons;
    public Sprite[] playIcons;
    public Text WinnerText;
    public GameObject[] winningLines;
    public GameObject WinnerPannel;
    public int xPlayersScore;
    public int oPlayersScore;
    public Text xPlaterScoreText;
    public Text oPlayerScoreText;
    public Button xPlayerButton;
    public Button oPlayerButton;

    // NEW:
    public enum Strategy
    {
        None,
        FirstAvailable,
        Random,
        MiniMax
    }
    public Strategy ai_strategy;
    const int EMPTY_SPACE = -100;

    const int PLAYER = 1;
    const int AI = 2;

    // TODO: 
    // Change ai_strategy to Strategy.MiniMax on the line 92 in Start() 
    /***************************************/
    /*****  IMPLEMENT YOUR CODE HERE  ******/
    /***************************************/

    int CheckWinner()//return 1 if player win 2 if ai win
    {
        int s1 = markedSpaces[0] + markedSpaces[1] + markedSpaces[2];
        int s2 = markedSpaces[3] + markedSpaces[4] + markedSpaces[5];
        int s3 = markedSpaces[6] + markedSpaces[7] + markedSpaces[8];
        int s4 = markedSpaces[0] + markedSpaces[3] + markedSpaces[6];
        int s5 = markedSpaces[1] + markedSpaces[4] + markedSpaces[7];
        int s6 = markedSpaces[2] + markedSpaces[5] + markedSpaces[8];
        int s7 = markedSpaces[0] + markedSpaces[4] + markedSpaces[8];
        int s8 = markedSpaces[2] + markedSpaces[4] + markedSpaces[6];
        var Solutions = new int[] { s1, s2, s3, s4, s5, s6, s7, s8 };

        for (int i = 0; i < Solutions.Length; i++)
        {
            if (Solutions[i] == 3)//player
            {
                return PLAYER;
            }
            else if(Solutions[i] == 6)//ai
            {
                return AI;
            }
        }

        foreach(int temp in markedSpaces)
        {
            if (temp == EMPTY_SPACE)
                return EMPTY_SPACE;//not tie
        }

        return -1;//tie game
    }

  
    int MiniMax(bool max)
    {
        // Check for terminal states
        int result = CheckWinner();
        switch(result)
        {
            case AI:
                return 10;
            case PLAYER:
                return -10;
            case -1:
                return 0;
            case EMPTY_SPACE:
                break;
        }

        if (max)
        {
            int bestScore = int.MinValue;
            for (int i = 0; i < 9; i++)
            {
                if (markedSpaces[i] == EMPTY_SPACE)
                {
                    markedSpaces[i] = AI;
                    int score = MiniMax(false); // minimize player
                    markedSpaces[i] = EMPTY_SPACE;
                    bestScore = Mathf.Max(score, bestScore);
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int i = 0; i < 9; i++)
            {
                if (markedSpaces[i] == EMPTY_SPACE)
                {
                    markedSpaces[i] = PLAYER;
                    int score = MiniMax(true); // switch to maximizing AI
                    markedSpaces[i] = EMPTY_SPACE;
                    bestScore = Mathf.Min(score, bestScore);
                }
            }
            return bestScore;
        }
    }

    int pickMiniMax()
    {
        // Get the best move using the MiniMax algorithm
        int bestScore = -10000;
        int pick = -1;

        for (int i = 0; i < 9; i++)
        {
            if (markedSpaces[i] == EMPTY_SPACE)
            {
                markedSpaces[i] = AI; // simulate opponent's move
                int score = MiniMax(false); // min first for player move
                markedSpaces[i] = EMPTY_SPACE;

                if (score > bestScore)
                {
                    bestScore = score;
                    pick = i;
                }
            }
        }

        return pick;
    }
    /***************************************/
    /***** END OF YOUR IMPLEMENTATION ******/
    /***************************************/
    // TODO: END


    // NEW:
    // Simple strategy to pick the first available spot.
    int pickFirstAvailableSpot()
    {
        for(int i=0; i<9; i++)
        {
            if (markedSpaces[i] == EMPTY_SPACE)
            {
                return i;
            }
        }

        return -1;
    }

    // NEW:
    // Simple strategy to randomly pick the available spot.
    int pickRandomSpot()
    {
        int spot = -1;
        var emptySpots = new List<int>();

        // create a list of empty spots
        for (int i=0; i<9; i++)
        {
            if (markedSpaces[i] == EMPTY_SPACE)
            {
                emptySpots.Add(i);
            }
        }

        // randomly pick an empty spot
        int index = UnityEngine.Random.Range(0, emptySpots.Count);
        spot = emptySpots[index];
        return spot;
    }

    // Start is called before the first frame update
    void Start()
    {
        ai_strategy = Strategy.MiniMax;
        GameSetup();
    }

    void GameSetup() 
    {
        turnCount = 0;
        SetPlayerTurn(0);

        for (int i = 0; i < tictactoeSpaces.Length; i++) 
        {
            markedSpaces[i] = EMPTY_SPACE;

            tictactoeSpaces[i].interactable = true;
            tictactoeSpaces[i].GetComponent<Image>().sprite = null;
        }
    }

    public void TicTacToeButton(int whichnumber) 
    {
        xPlayerButton.interactable = false;
        oPlayerButton.interactable = false;
        tictactoeSpaces[whichnumber].image.sprite = playIcons[whosTurn];
        tictactoeSpaces[whichnumber].interactable = false;

        markedSpaces[whichnumber] = whosTurn + 1;   // assign X = 1 & O = 2 for calculation of the winner in Winnercheck()
        turnCount++;

        if (turnCount >= 4) {
            bool iswinner =  Winnercheck();

            if (iswinner)
                return;

            if (turnCount == 9 && iswinner==false) {
                winnerDisplay(-1, whosTurn);  // tie
            }
        }
    
        SetPlayerTurn(whosTurn == 0 ? 1 : 0);
   
        // NEW
        // Player 1 picks the spot according to its strategy
        if (turnCount < 9 && whosTurn == 1)
        {
            int spot = -1;
            switch (ai_strategy)
            {
                case Strategy.None:             // manual play
                    break;

                case Strategy.FirstAvailable:   // pick first available spot
                    spot = pickFirstAvailableSpot();
                    break;

                case Strategy.Random:           // pick random spot
                    spot = pickRandomSpot();
                    break;

                case Strategy.MiniMax:          // compute from MiniMax algorithm
                    spot = pickMiniMax();
                    break;
            }
            TicTacToeButton(spot);
        }
    }

    bool Winnercheck() 
    {
        int s1 = markedSpaces[0] + markedSpaces[1] + markedSpaces[2];
        int s2 = markedSpaces[3] + markedSpaces[4] + markedSpaces[5];
        int s3 = markedSpaces[6] + markedSpaces[7] + markedSpaces[8];
        int s4 = markedSpaces[0] + markedSpaces[3] + markedSpaces[6];
        int s5 = markedSpaces[1] + markedSpaces[4] + markedSpaces[7];
        int s6 = markedSpaces[2] + markedSpaces[5] + markedSpaces[8];
        int s7 = markedSpaces[0] + markedSpaces[4] + markedSpaces[8];
        int s8 = markedSpaces[2] + markedSpaces[4] + markedSpaces[6];
        var Solutions = new int[] { s1, s2, s3, s4, s5, s6, s7, s8 };
        for (int i = 0; i < Solutions.Length; i++)
        {
            if (Solutions[i] == 3 *(whosTurn + 1))
            {
                winnerDisplay(i, whosTurn);
                return true;
            }
            
        }
        return false;
    }


    void winnerDisplay(int index, int who) 
    {
        if (index < 0)
        {
            WinnerText.text = "It's a draw!!";
            WinnerText.fontSize = 185;
        }
        else
        {
            if (who == 0)
            {
                WinnerText.text = "Player X Wins!";
                WinnerText.fontSize = 150;
                xPlayersScore++;
                xPlaterScoreText.text = xPlayersScore.ToString();
            }
            else
            {
                WinnerText.text = "Player O Wins!";
                WinnerText.fontSize = 150;
                oPlayersScore++;
                oPlayerScoreText.text = oPlayersScore.ToString();
            }

            winningLines[index].SetActive(true);
        }

        WinnerPannel.SetActive(true);
    }

    public void ReStart() 
    {
        GameSetup();

        xPlayerButton.interactable = true;
        oPlayerButton.interactable = true;
        WinnerPannel.SetActive(false);
        for (int i = 0; i < winningLines.Length; i++)
        {
            winningLines[i].SetActive(false);
        }
    }

    public void SetPlayerTurn(int player)
    {
        whosTurn = player;

        switch (player)
        {
            case 0:
                turnIcons[0].SetActive(true);
                turnIcons[1].SetActive(false);
                break;
            case 1:
                turnIcons[0].SetActive(false);
                turnIcons[1].SetActive(true);
                break;
        }
    }
}
