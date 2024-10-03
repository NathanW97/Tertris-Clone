using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    public int rotationIndex {  get; private set; }

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    { 
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;

        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        { 
            cells[i]= (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        board.Clear(this);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(1);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }

        //Soft drop
        if (Input.GetKeyDown(KeyCode.S))
        { 
            Move(Vector2Int.down);
        }

        //Hard Drop
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            HardDrop();
        }


        board.Set(this);
    }


    // Hard Drop Function
    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        { 
            continue;
        }
    }

    private bool Move(Vector2Int translation)
    {
       Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        //test if this new position is valid
        bool vaild = board.IsValidPosition(this,newPosition);
        if (vaild)
        {
            position = newPosition;
        }

        return vaild;
    }

    private void Rotate(int direction)
    {

        rotationIndex = Wrap(rotationIndex + direction, 0, 4);

        for (int i = 0; i < cells.Length; i++)
        { 
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }
            cells[i] = new Vector3Int(x, y, 0);
        }

    }

    // defining the wrap function
    // E.g. Input = -2 
    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else 
        { 
            return min - (input - min) % (max - min);
        }
    }
}
