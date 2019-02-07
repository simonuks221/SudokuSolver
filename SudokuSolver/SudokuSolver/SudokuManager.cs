using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

public class SudokuManager
{
    public SudokuNode[,] nodeArray;

    Label statusText;
    Label timeText;

    Stopwatch stopwatch; //Solve time stopwatch

    public SudokuManager()
	{
        nodeArray = new SudokuNode[9, 9];
        stopwatch = new Stopwatch(); 
	}

    public bool InitializeGrid(TextBox[,] textBoxes, Label statusLabel, Label timeLabel) //Return true if everything is ok, false if error
    {
        stopwatch.Restart();
        statusText = statusLabel;
        timeText = timeLabel;

        statusText.Text = "Initializing";

        for(int x = 0; x < 9; x++) //Setup the 2d grid from text boxes nad check for errors
        {
            for(int y = 0; y < 9; y++)
            {
                int u = 0;
                if(Int32.TryParse(textBoxes[x, y].Text, out u))
                {
                    if(u < 0 || u > 9) //Found an invalid number
                    {
                        textBoxes[x, y].BackColor = System.Drawing.Color.Red;
                        statusText.Text = "Error";
                        stopwatch.Stop();
                        return false;
                    }
                    else
                    {
                        nodeArray[x, y] = new SudokuNode(u, x, y);
                    }
                }
                else
                {
                    nodeArray[x, y] = new SudokuNode(0, x, y);
                }
            }
        }

        StartSolving();
        return true;
    }

    void PopulatePossibleNumbers()
    {
        for(int x = 0; x < 9; x++) //Clear all grid possible numbers
        {
            for(int y = 0; y < 9; y++)
            {
                nodeArray[x, y].possibleNumbers.Clear();
            }
        }

        for(int y = 0; y < 9; y++) //Go thru all y lines
        {
            List<int> neededNumbers = new List<int>();

            for(int n = 1; n < 10; n++)  //Search for missing numbers
            {
                bool found = false;
                for(int x = 0; x < 9; x++)
                {
                    if(nodeArray[x, y].nodeNumber == n)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    neededNumbers.Add(n);
                }
            }

            foreach(int n in neededNumbers) //Find available spaces for missing numbers
            {
                for(int x = 0; x < 9; x++)
                {
                    if(nodeArray[x, y].nodeNumber == 0) //Found empty space in a horizontal
                    {
                        if (CanSetHere(x, y, n, nodeArray))
                        {
                            nodeArray[x, y].possibleNumbers.Add(n);
                        }
                    }
                }
            }
        }
    }

    bool FindOnlyPossibleNumbers()
    {
        bool changed = false;

        for (int x = 0; x < 9; x++) //Go thru 2d array and search for nodes with only 1 possible number and fill it up
        {
            for (int y = 0; y < 9; y++)
            {
                if (nodeArray[x, y].nodeNumber == 0)
                {
                    if (nodeArray[x, y].possibleNumbers.Count == 1)
                    {
                        nodeArray[x, y].nodeNumber = nodeArray[x, y].possibleNumbers[0];
                        nodeArray[x, y].possibleNumbers.Clear();

                        changed = true;
                    }
                }
            }
        }
        return changed;
    }

    bool SudokuSolved(SudokuNode[,] _nodeArray) //Check if sudoku is solved
    {
        foreach (SudokuNode n in _nodeArray)
        {
            if(n.nodeNumber == 0)
            {
                return false; //Not solved
            }
        }
        return true;
    }

    bool CanSetHere(int x, int y, int n, SudokuNode[,] _nodeArray) //Can set the number in the location
    {
        for (int y2 = 0; y2 < 9; y2++) //Search if a number is already vertical
        {
            if (_nodeArray[x, y2].nodeNumber == n)
            {
                return false;
            }
        }

        for (int x2 = 0; x2 < 9; x2++) //Search if a number is already horizontal
        {
            if (_nodeArray[x2, y].nodeNumber == n)
            {
                return false;
            }
        }

        int gridSquareX = (int)Math.Floor((float)x / 3); //Get 3x3 grid coord x and y
        int gridSquareY = (int)Math.Floor((float)y / 3);

        for (int x2 = gridSquareX * 3; x2 < gridSquareX * 3 + 3; x2++) //Check the 3x3 grid for the number
        {
            for (int y2 = gridSquareY * 3; y2 < gridSquareY * 3 + 3; y2++)
            {
                if (x2 != x && y2 != y)
                {
                    if (_nodeArray[x2, y2].nodeNumber == n)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    void StartSolving() //Start solving sudoku
    {
        bool solved = false;

        statusText.Text = "Solving";

        while (!solved) //Go until none was changed
        {
            PopulatePossibleNumbers();
            solved = !FindOnlyPossibleNumbers();
        }

        if (SudokuSolved(nodeArray)) //Check if sudoku is already solved, then finish
        {
            stopwatch.Stop();
            timeText.Text = stopwatch.ElapsedMilliseconds.ToString() + " ms";
            statusText.Text = "Solved";
        }
        else //Not solved, start brute forcing
        {
            Console.Out.WriteLine("brute foricng");
            statusText.Text = "Brute forcing";

            if (StartBruteForcing()) //brute forcing helped, finish
            {
                stopwatch.Stop();
                timeText.Text = stopwatch.ElapsedMilliseconds.ToString() + " ms";
                statusText.Text = "Solved";
            }
            else //Brute forcing didnt help
            {
                statusText.Text = "Error";
                stopwatch.Stop();
            }
        }
    }

    public bool StartBruteForcing()
    {
        SudokuNode[,] bruteForceArray = new SudokuNode[9,9];
        bruteForceArray = nodeArray;

        List<SudokuNode> emptyNodes = new List<SudokuNode>();
        
        for(int y = 0; y < 9; y++) //Get all empty nodes that need to be publicated
        {
            for(int x = 0; x < 9; x++)
            {
                if(bruteForceArray[x, y].nodeNumber == 0)
                {
                    emptyNodes.Add(bruteForceArray[x, y]);
                }
            }
        }

        bool end = false;
        int lastCahngedIndex = -1;

        while (!end) //Bruteforcing loop
        {
            if (lastCahngedIndex == emptyNodes.Count - 1)
            {
                end = true;
            }
            else
            {
                lastCahngedIndex++;
                bool foundANumber = false;
                for (int i = emptyNodes[lastCahngedIndex].nodeNumber + 1; i < 10; i++) //Increment the thing by minimum amount
                {
                    if(i != 0)
                    { 
                        if (CanSetHere(emptyNodes[lastCahngedIndex].positionX, emptyNodes[lastCahngedIndex].positionY, i, bruteForceArray))
                        {
                            emptyNodes[lastCahngedIndex].nodeNumber = i;
                            foundANumber = true;
                            break;
                        }
                    }
                }
                if (!foundANumber) //Didnt find a number, backtrack and change others
                {
                    emptyNodes[lastCahngedIndex].nodeNumber = 0;
                    lastCahngedIndex -= 2;
                }
            }
        }

        if (SudokuSolved(bruteForceArray)) //If solved
        {
            nodeArray = bruteForceArray;
            return true;
        }
        else //Not solved
        {
            return false;
        }
    }
}
