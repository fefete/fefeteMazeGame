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
        public Cell()
        {
            eWalls = (CardinalDirs.Up | CardinalDirs.Down | CardinalDirs.Left | CardinalDirs.Right);
        }
    }

    public int iDimensions;
    public GameObject RightWall;
    public GameObject LeftWall;
    public GameObject UpWall;
    public GameObject DownWall;
    float fMaxX;
    float fMaxZ;
    float fMinX;
    float fMinZ;
    int Width;
    static int NumberOfDirections = 4;
    const float fMinWallWidth = 0.25f;
    List<GameObject> vCubesInMap = new List<GameObject>();
    List<Cell> vStack = new List<Cell>();
    List<Cell> vMapRepresentation = new List<Cell>();
    List<Cell> vData = new List<Cell>();
    CardinalDirs[] vDirections = { CardinalDirs.Up, CardinalDirs.Down, CardinalDirs.Left, CardinalDirs.Right };
    System.Random rng = new System.Random();
    List<Cell> vNeighBorsList = new List<Cell>();

    // Start is called before the first frame update
    void Start()
    {
        fMaxX = iDimensions;
        fMaxZ = iDimensions;
        fMinX = 0;
        fMinZ = 0;
        Width = (int)(fMaxX - fMinX);
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
        Cell oStartingCell = vMapRepresentation[Random.Range(0, vMapRepresentation.Count - 1)];
        oStartingCell.bVisited = true;
        RecursiveBackTracking(oStartingCell, (int)fMaxX, (int)fMaxZ, (int)fMinX, (int)fMinZ);
        CreateMazeFromData();
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
            bool bRight =   (c.eWalls & CardinalDirs.Right) != CardinalDirs.None;
            bool bLeft =    (c.eWalls & CardinalDirs.Left) != CardinalDirs.None;
            bool bUp =      (c.eWalls & CardinalDirs.Up) != CardinalDirs.None;
            bool bDown =    (c.eWalls & CardinalDirs.Down) != CardinalDirs.None;
            bool bNone =    (c.eWalls == CardinalDirs.None);

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
    }

    void AttachWallToGameObject(CardinalDirs _eDir, GameObject _oGObj, bool _bDeadEnd)

    {
        GameObject W = null;
        switch (_eDir)
        {
            case CardinalDirs.Up:
                W = Instantiate(UpWall, _oGObj.transform);
                W.GetComponent<Renderer>().material.color = Color.red;
                break;
            case CardinalDirs.Down:
                W = Instantiate(DownWall, _oGObj.transform);
                W.GetComponent<Renderer>().material.color = Color.red;

                break;
            case CardinalDirs.Left:
                W = Instantiate(LeftWall, _oGObj.transform);
                W.GetComponent<Renderer>().material.color = Color.red;

                break;
            case CardinalDirs.Right:
                W = Instantiate(RightWall, _oGObj.transform);
                W.GetComponent<Renderer>().material.color = Color.red;
                break;
        }

    }

    void RecursiveBackTracking(Cell _oStartingCell, int _iMaxX, int _iMaxY, int _iMinX, int _iMinY)
    {
        CardinalDirs[] vDirectionsToCheck = this.vDirections;
        Shuffle<CardinalDirs>(vDirections);
        _oStartingCell.bVisited = true;
        getNeighbors(_oStartingCell);
        List<Cell> vNeighBors = vNeighBorsList;
        bool bHasNeighBors = false;
        foreach (Cell v in vNeighBors)
        {
            if (v != null)
                bHasNeighBors = true;
        }
        Cell oNextCell = null;
        if (bHasNeighBors)
        {
            vStack.Add(_oStartingCell);
            int iCardinalIdx = 0;
            while (iCardinalIdx < NumberOfDirections)
            {
                CardinalDirs eDir = vDirectionsToCheck[iCardinalIdx];
                iCardinalIdx++;
                switch (eDir)
                {
                    //NEIGHBORS POSITIONS IN THE LIST ARE HARDCODED BECAUSE WE KNOW EXACTLY WHERE EXACTLY A CELL IS (Watch getNeighbors())
                    case CardinalDirs.Up:
                        if (vNeighBors[0] != null && vNeighBors[0].bVisited == false)
                        {
                            oNextCell = vNeighBors[0];
                            oNextCell.eWalls &= ~CardinalDirs.Down;
                            _oStartingCell.eWalls &= ~CardinalDirs.Up;
                            RecursiveBackTracking(oNextCell, (int)fMaxX, (int)fMaxZ, (int)fMinX, (int)fMinZ);
                        }
                        break;
                    case CardinalDirs.Down:
                        if (vNeighBors[1] != null && vNeighBors[1].bVisited == false)
                        {
                            oNextCell = vNeighBors[1];

                            oNextCell.eWalls &= ~CardinalDirs.Up;
                            _oStartingCell.eWalls &= ~CardinalDirs.Down;
                            RecursiveBackTracking(oNextCell, (int)fMaxX, (int)fMaxZ, (int)fMinX, (int)fMinZ);
                        }
                        break;
                    case CardinalDirs.Right:
                        if (vNeighBors[2] != null && vNeighBors[2].bVisited == false)
                        {
                            oNextCell = vNeighBors[2];
                            _oStartingCell.eWalls &= ~CardinalDirs.Right;
                            oNextCell.eWalls &= ~CardinalDirs.Left;
                            RecursiveBackTracking(oNextCell, (int)fMaxX, (int)fMaxZ, (int)fMinX, (int)fMinZ);
                        }
                        break;
                    case CardinalDirs.Left:
                        if (vNeighBors[3] != null && vNeighBors[3].bVisited == false)
                        {
                            oNextCell = vNeighBors[3];
                            _oStartingCell.eWalls &= ~CardinalDirs.Left;
                            oNextCell.eWalls &= ~CardinalDirs.Right;
                            RecursiveBackTracking(oNextCell, (int)fMaxX, (int)fMaxZ, (int)fMinX, (int)fMinZ);

                        }
                        break;
                }
            }

        }
        else
        {

            if (vStack.Count > 0)
            {
                oNextCell = Pop<Cell>(vStack);
            }
            if (oNextCell != null)
                RecursiveBackTracking(oNextCell, (int)fMaxX, (int)fMaxZ, (int)fMinX, (int)fMinZ);
        }

    }

    void Shuffle<T>(IList<T> _vListToShuffle)
    {
        int n = _vListToShuffle.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = _vListToShuffle[k];
            _vListToShuffle[k] = _vListToShuffle[n];
            _vListToShuffle[n] = value;
        }
    }

    void getNeighbors(Cell _oCell)
    {
        vNeighBorsList.Clear();
        Cell up = null;
        Cell down = null;
        Cell right = null;
        Cell left = null;
        if (_oCell.vPos.y + 1 < Width)
            up = GetCell(_oCell.vPos.x, _oCell.vPos.y + 1);
        if (_oCell.vPos.y - 1 >= 0)
            down = GetCell(_oCell.vPos.x, _oCell.vPos.y - 1);
        if (_oCell.vPos.x + 1 < Width)
            right = GetCell(_oCell.vPos.x + 1, _oCell.vPos.y);
        if (_oCell.vPos.x - 1 >= 0)
            left = GetCell(_oCell.vPos.x - 1, _oCell.vPos.y);

        vNeighBorsList.Add((up != null && up.bVisited == false) ? up : null);
        vNeighBorsList.Add(down != null && down.bVisited == false ? down : null);
        vNeighBorsList.Add(right != null && right.bVisited == false ? right : null);
        vNeighBorsList.Add(left != null && left.bVisited == false ? left : null);
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
