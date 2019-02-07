using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuSolver
{
    public partial class Form1 : Form
    {
        SudokuManager sudokuManager;
        TextBox[,] textBoxes;

        //Hardest sudoku loaded

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) //Setup text boxes on start
        {
            sudokuManager = new SudokuManager();
            textBoxes = new TextBox[9, 9] {
            {Grid00, Grid10, Grid20, Grid30, Grid40, Grid50, Grid60, Grid70, Grid80}, 
            {Grid01, Grid11, Grid21, Grid31, Grid41, Grid51, Grid61, Grid71, Grid81},
            {Grid02, Grid12, Grid22, Grid32, Grid42,Grid52,Grid62,Grid72,Grid82},
            {Grid03, Grid13, Grid23,Grid33,Grid43,Grid53,Grid63,Grid73,Grid83},
            {Grid04, Grid14, Grid24,Grid34,Grid44,Grid54,Grid64,Grid74,Grid84},
            {Grid05, Grid15, Grid25,Grid35,Grid45,Grid55,Grid65,Grid75,Grid85},
            {Grid06, Grid16, Grid26,Grid36,Grid46,Grid56,Grid66,Grid76,Grid86},
            {Grid07, Grid17, Grid27,Grid37,Grid47,Grid57,Grid67,Grid77,Grid87},
            {Grid08, Grid18, Grid28,Grid38,Grid48,Grid58,Grid68,Grid78,Grid88}
            };
        }

        private void StartSolvingButtonClicked(object sender, EventArgs e) //Start solving button pressed
        {
            bool noError = sudokuManager.InitializeGrid(textBoxes, StatusText, TimeText);
            if (noError) //If everything is correct display the grid
            {
                DisplayGrid();
            }
        }

        void DisplayGrid()
        {
            for(int x = 0; x < 9; x++)
            {
                for(int y = 0; y < 9; y++)
                {
                    if(sudokuManager.nodeArray[x, y].nodeNumber == 0)
                    {
                        textBoxes[x, y].Text = "";
                    }else
                    {
                        textBoxes[x, y].Text = sudokuManager.nodeArray[x, y].nodeNumber.ToString();
                    }
                }
            }
        }

        private void ClearButtonClick(object sender, EventArgs e)
        {
            ClearGrid();
        }

        void ClearGrid()
        {
            foreach (TextBox t in textBoxes)
            {
                t.BackColor = System.Drawing.Color.White;
                t.Text = "";
            }
        }
    }
}
