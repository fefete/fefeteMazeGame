using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;

public class GameManager : MonoBehaviour
{
    enum CardinalDirs
    {
        Up =    0x1000,
        Down =  0x0100,
        Left =  0x0010,
        Right = 0x0001
    }

    struct Cell
    {
        public bool bVisited { get; set;}
        public CardinalDirs eDir { get; set;} 
        public CardinalDirs ePreviousCellDir { get; set;} 
        public Vector2Int vPos{ get; set;}
    }

    public GameObject StartingPoint;
    public GameObject Floor;
    public GameObject Cube;
    float fMaxX;
    float fMaxZ;
    float fMinX;
    float fMinZ;
    static int NumberOfDirections = 4;
    const float fMinWallWidth = 0.25f;
    List<GameObject> vCubesInMap = new List<GameObject>();
    List<Cell> vStack = new List<Cell>();
    List<Cell> vMapRepresentation = new List<Cell>();
    CardinalDirs[] vDirections = { CardinalDirs.Up, CardinalDirs.Down, CardinalDirs.Left, CardinalDirs.Right };

    // Start is called before the first frame update
    void Start()
    {
        Vector3 sc = Floor.transform.localScale;
        fMaxX = sc.x;
        fMaxZ = sc.z;
        fMinX = -sc.x;
        fMinZ = -sc.z;
        for (int x = (int)fMaxX; x >= (int)fMinX; x--)
        {
            for (int z = (int)fMaxZ; z >= (int)fMinZ; z--)
            {
                if((x != fMaxX && x != fMinX) && (z != fMinZ && z != fMaxZ))
                    {
                        Cell obj = new Cell();
                        obj.vPos = new Vector2(x, z);
                        vMapRepresentation.Add(obj);
                        
                    }
                /*GameObject oGameObject = GameObject.Instantiate(Cube, new Vector3(x, 1, z), Quaternion.identity, null);
                oGameObject.SetActive(true);
                if((x != fMaxX && x != fMinX) && (z != fMinZ && z != fMaxZ))
                    vCubesInMap.Add(oGameObject);*/
            }
        }
        Cell oStartingCell = vMapRepresentation[Random.Range(0, vMapRepresentation.Count- 1)];

    }

    // Update is called once per frame
    void Update()
    {

    }

    void RecursiveBackTracking(Cell _oStartingCell, int _iMaxX, int _iMaxY)
    {
        _oStartingCell.bVisited = true;
        CardinalDirs[] vDirectionsToCheck = this.vDirections;
        Shuffle<CardinalDirs>(vDirectionsToCheck);
        if (_oStartingCell.bVisited)
        {
            
        }
        _oStartingCell.bVisited = true;
        vStack.Add(_oStartingCell);
        List<Cell> vNeighBors = getNeighbors(_oStartingCell);
        int iCardinalIdx = 0;
        while(iCardinalIdx++ < NumberOfDirections)
        {
            CardinalDirs eDir = vDirectionsToCheck[iCardinalIdx];
            switch (eDir)
            {
                case CardinalDirs.Up: 

                    break;
                case CardinalDirs.Down: 

                    break;
                case CardinalDirs.Right: 

                    break;
                case CardinalDirs.Left: 

                    break;
            }
        }


        
    }

    void Shuffle<T>(IList<T> _vListToShuffle)
    {
        int iCount = _vListToShuffle.Count;
        int iLastItem = iCount - 1;
        for(int i = 0; i < iLastItem; i++)
        {
            int iRandomIndex = UnityEngine.Random.Range(i, iCount);
            T tmp = _vListToShuffle[iRandomIndex];
            _vListToShuffle[i] = _vListToShuffle[iRandomIndex];
            _vListToShuffle[iRandomIndex] = tmp;
        }
    }

    List<Cell> getNeighbors(Cell _oCell)
    {

        Cell up = GetCell(_oCell.vPos.x, _oCell.vPos.y + 1);
        Cell down = GetCell(_oCell.vPos.x, _oCell.vPos.y - 1);
        Cell right = GetCell(_oCell.vPos.x + 1, _oCell.vPos.y);
        Cell left = GetCell(_oCell.vPos.x - 1, _oCell.vPos.y);

        List<Cell> vNeighBorsList = new List<Cell>();
        vNeighBorsList.Add(up);
        vNeighBorsList.Add(down);
        vNeighBorsList.Add(left);
        vNeighBorsList.Add(right);
        return vNeighBorsList;
    }

    Cell GetCell(int _iX, int _iY)
    {
        int iRetCellIdx = _iY * vMapRepresentation.Count + x;
        return vMapRepresentation[iRetCellIdx];
    }
}
