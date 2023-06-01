
using Microsoft.Maui.Graphics;
using System.Reflection;

namespace sudoku;

public partial class PlayPage : ContentPage
{
    int[,] board = new int[9, 9]; //Inititalize empty board [r,c]
    int[,] solvingBoard = new int[9, 9];
    //int[,] solvedBoard;
    int solvedCounts = 0;
    int difficulty;

    Stack<Label> userInputs = new();

    List<List<int>> coords = new(); //Initilize empty list of coordinates

    Label lastSelected = null;
    Label undoLabel = null;


    public PlayPage(int d)
	{
        //WriteBoard();
        difficulty = d;
        InitializeComponent();
        GenUniqueBoard(difficulty);
        DrawBoard();
    }

    void DrawBoard()
    {
        int top;
        int bottom;
        int left;
        int right;

        int thick = 3;

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                top = 1; bottom = 1; left = 1; right = 1;

                

                Label lbl = new Label();
                TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, e) =>
                {

                    lbl.BackgroundColor = Colors.SteelBlue;
                    if (lastSelected != null)
                    {
                        lastSelected.BackgroundColor = Colors.LightSteelBlue;
                    }
                    lastSelected = lbl;
                };



                lbl.GestureRecognizers.Add(tapGestureRecognizer);

                MainBoard.Children.Add(lbl);
                Grid.SetRow(lbl, i);
                Grid.SetColumn(lbl, j);

                if (i % 3 == 0) { top = thick; }
                if (j % 3 == 0) { left = thick; }
                if (i == 8) { bottom = thick; }
                if (j == 8) { right = thick; }

                lbl.Margin = new Thickness(left, top, right, bottom);

                if (board[i, j] != 0)
                {
                    lbl.Text = board[i, j].ToString();
                    lbl.TextColor = Colors.DarkSlateGrey;
                }
            }
        }
    }

    static void WriteBoard(int[,] localBoard)
    {
        for (int i = 0; i < 9; i++) //ROW
        {
            for (int j = 0; j < 9; j++) //COL
            {
                System.Diagnostics.Debug.Write(localBoard[i, j]);
                if (j % 3 == 2) { System.Diagnostics.Debug.Write("|"); }
            }

            System.Diagnostics.Debug.WriteLine("");
            if (i % 3 == 2) { System.Diagnostics.Debug.WriteLine("------------"); }
            /*else { System.Diagnostics.Debug.WriteLine(""); }*/
            //System.Diagnostics.Debug.WriteLine("");
        }
    }

    public void SolveBoard()
    {
        //System.Diagnostics.Debug.WriteLine("Solving Board...");
        int guess;
        Random rnd = new Random();

        if (solvedCounts > 1)
        {
            return;
        }

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (solvingBoard[i, j] == 0)
                {
                    guess = rnd.Next(1, 10);
                    for (int g = 0; g < 9; g++)
                    {
                        if (ValidInput(guess, i, j))
                        {
                            solvingBoard[i, j] = guess;
                            SolveBoard();
                            if (IsFull())
                            {
                                solvedCounts++;
                                //return;
                            }
                            solvingBoard[i, j] = 0;
                        }
                        guess = (guess % 9) + 1;
                    }
                    return;
                }
            }
        }
    }

    void GenUniqueBoard(int difficulty)
    {
        MakeShuffledCoords();
        GenRandomBoard();
        WriteBoard(board);

        int removedCount = 0;
        int maxNumOfClues = 0;

        for (int i = coords.Count - 1; i >= 0; i--) //Iterate through all coords
        {
            //First determine coord to remove
            int r = coords[i][0];
            int c = coords[i][1];

            int lastValue = board[r, c];
            board[r, c] = 0;
            removedCount++;

            solvingBoard = board;
            solvedCounts = 0;
            SolveBoard();

            if (solvedCounts > 1) //If multiple solutions exist undo
            {
                removedCount--;
                board[r, c] = lastValue;
            }

            //Min 17 clues to be valid
            if(difficulty == 1) { maxNumOfClues = 45 ; }
            else if (difficulty == 2) { maxNumOfClues = 35; }
            else if (difficulty == 3) { maxNumOfClues = 20; }

            if (removedCount >= (81 - maxNumOfClues)) { break; }
        }

        //Test uniqueness using a fast backtracking solver. My solver is - in theory - able to count all solutions, but for testing uniqueness, it will stop immediately when it finds more than one solution.

        //If the current board has still just one solution, goto step 3) and repeat.

        //If the current board has more than one solution, undo the last removal (step 3), and continue step 3 with the next position from the list

        //Stop when you have tested all 81 position

    }

    public void GenRandomBoard()
    {
        //System.Diagnostics.Debug.WriteLine("Solving Board...");
        int guess;
        Random rnd = new Random();

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (board[i, j] == 0)
                {
                    guess = rnd.Next(1, 10);
                    for (int g = 0; g < 9; g++)
                    {
                        if (ValidInput(guess, i, j))
                        {
                            board[i, j] = guess;
                            GenRandomBoard();
                            if (IsFull())
                            {
                                return;
                            }
                            board[i, j] = 0;
                        }
                        guess = (guess % 9) + 1;
                    }
                    return;
                }
            }
        }
    } //Generates random fully solved board from blank board

    bool IsFull() //Checks if board is full
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (board[i, j] == 0) { return false; }
            }
        }
        return true;
    }

    bool ValidInput(int value, int r, int c) //Checks if the input is valid based on sudoku rules
    {
        if (value < 1 || value > 9) { return false; }

        int x;
        int y;

        for (int i = 0; i < 9; i++) //Checks row 
        {
            if (board[i, c] == value && i != r) { return false; } //checks row
            else if (board[r, i] == value && i != c) { return false; }//checks col

            x = Convert.ToInt32(Math.Floor(Convert.ToDouble(r / 3) * 3) + Math.Floor(Convert.ToDouble(i / 3)));
            y = Convert.ToInt32(Math.Floor(Convert.ToDouble(c / 3) * 3) + (i % 3));

            if (board[x, y] == value && x != r && y != c) { return false; } //checks box

        }

        return true;
    }

    void MakeShuffledCoords()
    {
        Random rnd = new Random();

        //Make a list of all 81 cell positions and shuffle it randomly.
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                coords.Add(new List<int> { i, j });
            }
        }

        //Shuffles List
        int n = coords.Count;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            List<int> temp = coords[k];
            coords[k] = coords[n];
            coords[n] = temp;
        }
    }

    /*BUTTON FUNCTIONS*/
    public void OnUndoBtnClicked(object sender, System.EventArgs e)
    {
        StartTimer();
        if(userInputs.Count > 0)
        {
            undoLabel = userInputs.Pop();
            undoLabel.Text = "";
            board[Grid.GetRow(undoLabel), Grid.GetColumn(undoLabel)] = 0;
        }
    }

    public void OnNumBtnClicked(object sender, System.EventArgs e)
    {
        Button btn = (Button)sender;
        int val = System.Convert.ToInt16(btn.Text);

        if (lastSelected.TextColor != Colors.DarkSlateGrey)
        {
            if (!ValidInput(val, Grid.GetRow(lastSelected), Grid.GetColumn(lastSelected)))
            {
                lastSelected.TextColor = Colors.Crimson;
            }
            else
            {
                lastSelected.TextColor = Colors.Black;
                board[Grid.GetRow(lastSelected), Grid.GetColumn(lastSelected)] = val;
            }

            if(userInputs.Count == 0)
            {
                userInputs.Push(lastSelected);
            }
            else if(userInputs.Peek() != lastSelected )
            {
                userInputs.Push(lastSelected);
            }

            lastSelected.Text = val.ToString();


            if (IsFull())
            {
                OnWin();
            }
        }
    }

    void OnPlayAgainBtnClicked(object sender, System.EventArgs e)
    {
        
        foreach(View view in MainBoard)
        {
            if(view.GetType() == typeof(Label) && Grid.GetRow(view) < 9 && Grid.GetColumn(view) < 9)
            {
                board[Grid.GetRow(view), Grid.GetColumn(view)] = 0;
                ((Label)view).Text = "";
            }
        }

        GenUniqueBoard(difficulty);
        DrawBoard();
        PlayAgainBtn.IsVisible = false;
        PlayAgainBtn.IsEnabled = false;
        UndoBtn.IsVisible = true;
        UndoBtn.IsEnabled = true;
    }

    void OnWin()
    {
        UndoBtn.IsVisible = false;
        UndoBtn.IsEnabled = false;
        PlayAgainBtn.IsVisible = true;
        PlayAgainBtn.IsEnabled = true;
    }

    private void StartTimer()
    {
        /*DateTime startTime = DateTime.Now;
        while (true) {
            var elapsedTime = (DateTime.Now - startTime).TotalMilliseconds/1000;
            Timer.Text = $"{elapsedTime}";
        }*/
    }
}