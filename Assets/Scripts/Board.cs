using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.Rendering.DebugUI.Table;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public GameManager gameManager;
    public Piece activePiece { get; private set; }
    public NextPiece nextactivePiece { get; private set; }
    public TetrominoData[] tetrominos;
    public Vector3Int spawnPosition;
    public Vector3Int nextspawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public int nextPieceID = 0;
    public int score = 0;
    public int hiScore = 0;

    //Hi and current scoring text
    //public Text scoreText;
    //public Text hiScoreText;
    //[SerializeField] private TextMeshProUGUI scoreText;
    //[SerializeField] private TextMeshProUGUI hiScoreText;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-5, -10);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();
        nextactivePiece = GetComponentInChildren<NextPiece>();
        //gameManager = GetComponentInChildren<GameManager>();

        for (int i = 0; i < tetrominos.Length; i++) {  
            tetrominos[i].Initialize();
        }
        // Setup the next piece ready to be used on the board
        nextPieceID = Random.Range(0, tetrominos.Length);
        TetrominoData data = tetrominos[nextPieceID];
        nextactivePiece.Initialize(this, nextspawnPosition, data);
    }

    private void Start()
    {

        SetNext(nextactivePiece);

        // Clear and set score on screen
        //score = 0;
        //scoreText.text = score.ToString();
        //hiScoreText.text = "Hi Score: " + hiScore.ToString();

        SpawnPiece();
    }

    public void SpawnPiece()
    {
        // Clear Next Piece
        ClearNext(nextactivePiece);

       // Set Next Piece to be current Piece
        TetrominoData data = tetrominos[nextPieceID];
        activePiece.Initialize(this, spawnPosition, data);

        // Check if spawn location is valid
        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }

        // select next Piece and save ID
        nextPieceID = Random.Range(0, tetrominos.Length);
        data = tetrominos[nextPieceID];
        nextactivePiece.Initialize(this, nextspawnPosition, data);

        // set the next Piece to be visible
        SetNext(nextactivePiece);
    }

    private void GameOver()
    {
        this.tilemap.ClearAllTiles();
        //... game over screen and restart
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void SetNext(NextPiece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public void ClearNext(NextPiece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;    

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // check if piece location is occupied 
            if (tilemap.HasTile(tilePosition)) 
            {
                return false;
            }

            // check if out-of-bounds
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }
        }
        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        int scoreCount = 0;

        // Step through each row until board is cleared
        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                scoreCount++;
            } 
            else 
            {
                row++;   
            }
        }
        // send lines cleared to game manager for scoring
        gameManager.CalclineScore(scoreCount);
        //score += gameManager.CalclineScore(scoreCount);
        //scoreText.text = score.ToString();
        //if (score > hiScore)
        //{
        //    hiScore = score;
        //    hiScoreText.text = "Hi Score: " + hiScore.ToString();
        //}
    }

    private bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        { 
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }
        return true;
    }

    private void LineClear(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }
            row++;
        }
    }
}
