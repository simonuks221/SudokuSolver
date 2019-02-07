using System;
using System.Collections.Generic;

public class SudokuNode
{
    public int nodeNumber = -1;

    public int positionX;
    public int positionY;

    public List<int> possibleNumbers;

	public SudokuNode(int _nodeNumber, int _positionX, int _positionY)
	{
        nodeNumber = _nodeNumber;

        positionX = _positionX;
        positionY = _positionY;

        possibleNumbers = new List<int>();
	}
}
