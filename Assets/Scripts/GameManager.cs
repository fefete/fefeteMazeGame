using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;

public class GameManager : MonoBehaviour
{
    enum CardinalDirs : uint
    {
        None = 0x0000,
        Up = 0x0001,
        Down = 0x0010,
        Left = 0x0100,
        Right = 0x1000
    }

    class Cell
    {
        public bool bVisited { get; set; }
        public CardinalDirs eWalls { get; set; }
        public Vector2Int vPos { get; set; }
        public bool bDeadEnd;
        public List<Cell> vNeigbors;
        public Cell()
        {
            eWalls = (CardinalDirs.Up | CardinalDirs.Down | CardinalDirs.Left | CardinalDirs.Right);
            vNeigbors = new List<Cell>(4);
            vNeigbors.Clear();
        }
    }

    public int iDimensions;
    public GameObject oRightWall;
    public GameObject oLeftWall;
    public GameObject oUpWall;
    public GameObject oDownWall;
    public GameObject oFloor;
    public GameObject oPlayer;
    float fMaxX;
    float fMaxZ;
    float fMinX;
    float fMinZ;
    int Width;
    static int NumberOfDirections = 4;
    const float fMinWallWidth = 0.25f;
    List<Cell> vStack;
    List<Cell> vMapRepresentation = new List<Cell>();
    CardinalDirs[] vDirections = { CardinalDirs.Up, CardinalDirs.Down, CardinalDirs.Left, CardinalDirs.Right };
    System.Random rng = new System.Random();
    Cell up = null;
    Cell down = null;
    Cell right = null;
    Cell left = null;
    Cell oNextCell = null;
    private void Awake()
    {
        fMaxX = iDimensions;
        fMaxZ = iDimensions;
        fMinX = 0;
        fMinZ = 0;
        Width = (int)(fMaxX - fMinX);
        vStack = new List<Cell>(Width * Width);
        for (int x = (int)fMinX; x < (int)fMaxX; x++)
        {
            for (int z = (int)fMinZ; z < (int)fMaxZ; z++)
            {
                // if((x != fMaxX && x != fMinX) && (z != fMinZ && z != fMaxZ))
                {
                    Cell obj = new Cell();
                    obj.vPos = new Vector2Int(x, z);
                    vMapRepresentation.Add(obj);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Cell oStartingCell = vMapRepresentation[Random.Range(0, vMapRepresentation.Count - 1)];
        oStartingCell.bVisited = true;
        RecursiveBackTracking(oStartingCell);
        CreateMazeFromData();
        oPlayer.transform.position = new Vector3(oStartingCell.vPos.x, 1, oStartingCell.vPos.y);
    }


    // Update is called once per frame
    void Update()
    {

    }

    void CreateMazeFromData()
    {
        GameObject oEmpty = new GameObject();
        foreach (Cell c in vMapRepresentation)
        {
            GameObject oGameObject = Instantiate(oEmpty, new Vector3(c.vPos.x, 1, c.vPos.y), Quaternion.identity, null);
            bool bRight = (c.eWalls & CardinalDirs.Right) != CardinalDirs.None;
            bool bLeft = (c.eWalls & CardinalDirs.Left) != CardinalDirs.None;
            bool bUp = (c.eWalls & CardinalDirs.Up) != CardinalDirs.None;
            bool bDown = (c.eWalls & CardinalDirs.Down) != CardinalDirs.None;
            bool bNone = (c.eWalls == CardinalDirs.None);

            if (bRight)
            {
                AttachWallToGameObject(CardinalDirs.Right, oGameObject, c.bDeadEnd);
            }
            if (bLeft)
            {
                AttachWallToGameObject(CardinalDirs.Left, oGameObject, c.bDeadEnd);
            }
            if (bUp)
            {
                AttachWallToGameObject(CardinalDirs.Up, oGameObject, c.bDeadEnd);
            }
            if (bDown)
            {
                AttachWallToGameObject(CardinalDirs.Down, oGameObject, c.bDeadEnd);
            }
        }
        oFloor.transform.position = new Vector3(Width * 0.5f, 0, Width * 0.5f);
        oFloor.transform.localScale = new Vector3(Width, 1f, Width);
    }

    void AttachWallToGameObject(CardinalDirs _eDir, GameObject _oGObj, bool _bDeadEnd)
    {
        GameObject W = null;
        switch (_eDir)
        {
            case CardinalDirs.Up:
                W = Instantiate(oUpWall, _oGObj.transform);
                W.GetComponent<Renderer>().material.color = Color.red;
                break;
            case CardinalDirs.Down:
                W = Instantiate(oDownWall, _oGObj.transform);
                W.GetComponent<Renderer>().material.color = Color.red;

                break;
            case CardinalDirs.Left:
                W = Instantiate(oLeftWall, _oGObj.transform);
                W.GetComponent<Renderer>().material.color = Color.red;

                break;
            case CardinalDirs.Right:
                W = Instantiate(oRightWall, _oGObj.transform);
                W.GetComponent<Renderer>().material.color = Color.red;
                break;
        }

    }
    void RecursiveBackTracking(Cell _oStartingCell)
    {
        CardinalDirs[] vDirectionsToCheck = this.vDirections;
        Shuffle<CardinalDirs>(vDirections);
        _oStartingCell.bVisited = true;
        if (_oStartingCell.vNeigbors.Count <= 0)
            getNeighbors(_oStartingCell);
        bool bHasNotCheckedNeighbors = false;
        for (uint i = 0; i < _oStartingCell.vNeigbors.Count; i++)
        {
            if (_oStartingCell.vNeigbors[(int)i] != null)
                bHasNotCheckedNeighbors = true;
        }
        oNextCell = null;
        if (bHasNotCheckedNeighbors)
        {
            vStack.Add(_oStartingCell);
            int iCardinalIdx = 0;
            while (iCardinalIdx < NumberOfDirections)
            {
                switch (vDirectionsToCheck[iCardinalIdx])
                {
                    //NEIGHBORS POSITIONS IN THE LIST ARE HARDCODED BECAUSE WE KNOW EXACTLY WHERE EXACTLY A CELL IS (Watch getNeighbors())
                    case CardinalDirs.Up:
                        if (_oStartingCell.vNeigbors[0] != null && _oStartingCell.vNeigbors[0].bVisited == false)
                        {
                            oNextCell = _oStartingCell.vNeigbors[0];
                            oNextCell.eWalls &= ~CardinalDirs.Down;
                            _oStartingCell.eWalls &= ~CardinalDirs.Up;
                            RecursiveBackTracking(oNextCell);
                        }
                        break;
                    case CardinalDirs.Down:
                        if (_oStartingCell.vNeigbors[1] != null && _oStartingCell.vNeigbors[1].bVisited == false)
                        {
                            oNextCell = _oStartingCell.vNeigbors[1];

                            oNextCell.eWalls &= ~CardinalDirs.Up;
                            _oStartingCell.eWalls &= ~CardinalDirs.Down;
                            RecursiveBackTracking(oNextCell);
                        }
                        break;
                    case CardinalDirs.Right:
                        if (_oStartingCell.vNeigbors[2] != null && _oStartingCell.vNeigbors[2].bVisited == false)
                        {
                            oNextCell = _oStartingCell.vNeigbors[2];
                            _oStartingCell.eWalls &= ~CardinalDirs.Right;
                            oNextCell.eWalls &= ~CardinalDirs.Left;
                            RecursiveBackTracking(oNextCell);
                        }
                        break;
                    case CardinalDirs.Left:
                        if (_oStartingCell.vNeigbors[3] != null && _oStartingCell.vNeigbors[3].bVisited == false)
                        {
                            oNextCell = _oStartingCell.vNeigbors[3];
                            _oStartingCell.eWalls &= ~CardinalDirs.Left;
                            oNextCell.eWalls &= ~CardinalDirs.Right;
                            RecursiveBackTracking(oNextCell);

                        }
                        break;
                }
                iCardinalIdx++;
            }
        }
        else
        {

            if (vStack.Count > 0)
            {
                oNextCell = Pop<Cell>(vStack);
            }
            if (oNextCell != null)
                RecursiveBackTracking(oNextCell);
        }
    }

    void Shuffle<T>(IList<T> _vListToShuffle)
    {
        int n = _vListToShuffle.Count;
        int k = 0;
        while (n > 1)
        {
            n--;
            k = rng.Next(n + 1);
            T value = _vListToShuffle[k];
            _vListToShuffle[k] = _vListToShuffle[n];
            _vListToShuffle[n] = value;
        }
    }

    void getNeighbors(Cell _oCell)
    {
        up = null;
        down = null;
        right = null;
        left = null;
        if (_oCell.vPos.y + 1 < Width)
            up = GetCell(_oCell.vPos.x, _oCell.vPos.y + 1);
        if (_oCell.vPos.y - 1 >= 0)
            down = GetCell(_oCell.vPos.x, _oCell.vPos.y - 1);
        if (_oCell.vPos.x + 1 < Width)
            right = GetCell(_oCell.vPos.x + 1, _oCell.vPos.y);
        if (_oCell.vPos.x - 1 >= 0)
            left = GetCell(_oCell.vPos.x - 1, _oCell.vPos.y);

        _oCell.vNeigbors.Add((up != null && up.bVisited == false) ? up : null);
        _oCell.vNeigbors.Add(down != null && down.bVisited == false ? down : null);
        _oCell.vNeigbors.Add(right != null && right.bVisited == false ? right : null);
        _oCell.vNeigbors.Add(left != null && left.bVisited == false ? left : null);
    }

    Cell GetCell(int row, int col)
    {
        int iRetCellIdx = row * Width + col;
        return vMapRepresentation[iRetCellIdx];
    }

    T Pop<T>(IList<T> _vListToPopElement)
    {
        T ElementPopped = _vListToPopElement[0];
        _vListToPopElement.RemoveAt(0);
        return ElementPopped;
    }
}
