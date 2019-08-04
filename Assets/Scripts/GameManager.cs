using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;

public class GameManager : MonoBehaviour
{
    enum CardinalDirs
    {
        Up = 0x1000,
        Down = 0x0100,
        Left = 0x0010,
        Right = 0x0001
    }

    class Cell
    {
        public bool bVisited { get; set; }
        public CardinalDirs eDir { get; set; }
        public CardinalDirs ePreviousCellDir { get; set; }
        public Vector2Int vPos { get; set; }
        public GameObject oRepresentativeObject;
    }

    public GameObject Floor;
    public GameObject Cube;
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
    CardinalDirs[] vDirections = { CardinalDirs.Up, CardinalDirs.Down, CardinalDirs.Left, CardinalDirs.Right };

    // Start is called before the first frame update
    void Start()
    {
        Vector3 vSc = Floor.transform.localScale;
        fMaxX = vSc.x;
        fMaxZ = vSc.z;
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
        print(oStartingCell.vPos);
        oStartingCell.bVisited = true;
        GameObject oGameObject = GameObject.Instantiate(Cube, new Vector3(oStartingCell.vPos.x, 1, oStartingCell.vPos.y), Quaternion.identity, null);
        oGameObject.SetActive(true);
        oGameObject.name = oStartingCell.vPos.ToString();
        oStartingCell.oRepresentativeObject = oGameObject;
        RecursiveBackTracking(oStartingCell, (int)fMaxX, (int)fMaxZ, (int)fMinX, (int)fMinZ);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RecursiveBackTracking(Cell _oStartingCell, int _iMaxX, int _iMaxY, int _iMinX, int _iMinY)
    {
        CardinalDirs[] vDirectionsToCheck = this.vDirections;
        Shuffle<CardinalDirs>(vDirections);
        _oStartingCell.bVisited = true;
        List<Cell> vNeighBors = getNeighbors(_oStartingCell);
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
                oNextCell = null;
                CardinalDirs eDir = vDirectionsToCheck[iCardinalIdx];
                iCardinalIdx++;
                switch (eDir)
                {
                    //NEIGHBORS POSITIONS IN THE LIST ARE HARDCODED BECAUSE WE KNOW EXACTLY WHERE EXACTLY A CELL IS (Watch getNeighbors())
                    case CardinalDirs.Up:
                        Cell oUpCell = vNeighBors[0];
                        if (oUpCell != null && oUpCell.bVisited == false)
                        {
                            GameObject oGameObject = GameObject.Instantiate(Cube, new Vector3(oUpCell.vPos.x, 1, oUpCell.vPos.y), Quaternion.identity, null);
                            oGameObject.name = oUpCell.vPos.ToString();

                            Transform oChildTransform = _oStartingCell.oRepresentativeObject.transform.Find("Up");
                            if (oChildTransform)
                                GameObject.Destroy(oChildTransform.gameObject);

                            oChildTransform = oGameObject.transform.Find("Down");
                            if (oChildTransform)
                                GameObject.Destroy(oChildTransform.gameObject);

                            oGameObject.SetActive(true);

                            vCubesInMap.Add(oGameObject);
                            oNextCell = oUpCell;
                            oNextCell.oRepresentativeObject = oGameObject;
                            RecursiveBackTracking(oNextCell, (int)fMaxX, (int)fMaxZ, (int)fMinX, (int)fMinZ);
                        }
                        break;
                    case CardinalDirs.Down:
                        Cell oDownCell = vNeighBors[1];
                        if (oDownCell != null && oDownCell.bVisited == false)
                        {
                            GameObject oGameObject = GameObject.Instantiate(Cube, new Vector3(oDownCell.vPos.x, 1, oDownCell.vPos.y), Quaternion.identity, null);
                            oGameObject.name = oDownCell.vPos.ToString();

                            Transform oChildTransform = _oStartingCell.oRepresentativeObject.transform.Find("Down");
                            if (oChildTransform)
                                GameObject.Destroy(oChildTransform.gameObject);

                            oChildTransform = oGameObject.transform.Find("Up");
                            if (oChildTransform)
                                GameObject.Destroy(oChildTransform.gameObject);

                            oGameObject.SetActive(true);

                            vCubesInMap.Add(oGameObject);


                            //print(oDownCell.vPos);
                            oNextCell = oDownCell;
                            oNextCell.oRepresentativeObject = oGameObject;
                            RecursiveBackTracking(oNextCell, (int)fMaxX, (int)fMaxZ, (int)fMinX, (int)fMinZ);
                        }
                        break;
                    case CardinalDirs.Right:
                        Cell oRightCell = vNeighBors[2];
                        if (oRightCell != null && oRightCell.bVisited == false)
                        {
                            GameObject oGameObject = GameObject.Instantiate(Cube, new Vector3(oRightCell.vPos.x, 1, oRightCell.vPos.y), Quaternion.identity, null);
                            oGameObject.name = oRightCell.vPos.ToString();

                            Transform oChildTransform = _oStartingCell.oRepresentativeObject.transform.Find("Right");
                            if (oChildTransform)
                                GameObject.Destroy(oChildTransform.gameObject);
                            oChildTransform = oGameObject.transform.Find("Left");
                            if (oChildTransform)
                                GameObject.Destroy(oChildTransform.gameObject);

                            oGameObject.SetActive(true);

                            vCubesInMap.Add(oGameObject);

                            //print(oRightCell.vPos);
                            oNextCell = oRightCell;
                            oNextCell.oRepresentativeObject = oGameObject;
                            RecursiveBackTracking(oNextCell, (int)fMaxX, (int)fMaxZ, (int)fMinX, (int)fMinZ);
                        }
                        break;
                    case CardinalDirs.Left:
                        Cell oLeftCell = vNeighBors[3];
                        if (oLeftCell != null && oLeftCell.bVisited == false)
                        {
                            GameObject oGameObject = GameObject.Instantiate(Cube, new Vector3(oLeftCell.vPos.x, 1, oLeftCell.vPos.y), Quaternion.identity, null);
                            oGameObject.name = oLeftCell.vPos.ToString();

                            Transform oChildTransform = _oStartingCell.oRepresentativeObject.transform.Find("Left");
                            if (oChildTransform)
                                GameObject.Destroy(oChildTransform.gameObject);
                            oChildTransform = oGameObject.transform.Find("Right");
                            if (oChildTransform)
                                GameObject.Destroy(oChildTransform.gameObject);

                            oGameObject.SetActive(true);
                            vCubesInMap.Add(oGameObject);

                            //print(oLeftCell.vPos);
                            oNextCell = oLeftCell;
                            oNextCell.oRepresentativeObject = oGameObject;
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
        System.Random rng = new System.Random();
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = _vListToShuffle[k];
            _vListToShuffle[k] = _vListToShuffle[n];
            _vListToShuffle[n] = value;
        }
    }

    List<Cell> getNeighbors(Cell _oCell)
    {
        Cell up = null;
        Cell down = null;
        Cell right = null;
        Cell left = null;
        if (_oCell.vPos.y + 1 < Width)
            up = GetCell(_oCell.vPos.x, _oCell.vPos.y + 1);
        if (_oCell.vPos.y - 1 > 0)
            down = GetCell(_oCell.vPos.x, _oCell.vPos.y - 1);
        if (_oCell.vPos.x + 1 < Width)
            right = GetCell(_oCell.vPos.x + 1, _oCell.vPos.y);
        if (_oCell.vPos.x - 1 > 0)
            left = GetCell(_oCell.vPos.x - 1, _oCell.vPos.y);

        List<Cell> vNeighBorsList = new List<Cell>();
        vNeighBorsList.Add((up != null && up.bVisited == false) ? up : null);
        vNeighBorsList.Add(down != null && down.bVisited == false ? down : null);
        vNeighBorsList.Add(right != null && right.bVisited == false ? right : null);
        vNeighBorsList.Add(left != null && left.bVisited == false ? left : null);
        return vNeighBorsList;
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
