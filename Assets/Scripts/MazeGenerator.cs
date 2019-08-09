using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    enum CardinalDirs : uint
    {
        None = 0x0000,
        m_oUp = 0x0001,
        m_oDown = 0x0010,
        m_oLeft = 0x0100,
        m_oRight = 0x1000
    }

    class Cell
    {
        public bool m_bVisited { get; set; }
        public CardinalDirs m_eWalls { get; set; }
        public Vector2Int m_vPos { get; set; }
        public bool m_bDeadEnd;
        public List<Cell> m_vNeigbors;
        public Cell()
        {
            m_eWalls = (CardinalDirs.m_oUp | CardinalDirs.m_oDown | CardinalDirs.m_oLeft | CardinalDirs.m_oRight);
            m_vNeigbors = new List<Cell>(4);
            m_vNeigbors.Clear();
        }
    }

    public int m_iDimensions;
    public GameObject m_oRightWall;
    public GameObject m_oLeftWall;
    public GameObject m_oUpWall;
    public GameObject m_oDownWall;
    public GameObject m_oFloor;
    public GameObject m_oPlayer;
    public UnityEngine.AI.NavMeshSurface m_oSurface; 
    float m_fMaxX;
    float m_fMaxZ;
    float m_fMinX;
    float m_fMinZ;
    int m_Width;
    static int m_iNumberOfDirections = 4;
    const float m_fMinWallm_Width = 0.25f;
    List<Cell> m_vStack;
    List<Cell> m_vMapRepresentation = new List<Cell>();
    CardinalDirs[] m_vDirections = { CardinalDirs.m_oUp, CardinalDirs.m_oDown, CardinalDirs.m_oLeft, CardinalDirs.m_oRight };
    System.Random m_oRng = new System.Random();
    Cell m_oUp = null;
    Cell m_oDown = null;
    Cell m_oRight = null;
    Cell m_oLeft = null;
    Cell m_oNextCell = null;
    private void Awake()
    {
        m_fMaxX = m_iDimensions;
        m_fMaxZ = m_iDimensions;
        m_fMinX = 0;
        m_fMinZ = 0;
        m_Width = (int)(m_fMaxX - m_fMinX);
        m_vStack = new List<Cell>(m_Width * m_Width);
        for (int x = (int)m_fMinX; x < (int)m_fMaxX; x++)
        {
            for (int z = (int)m_fMinZ; z < (int)m_fMaxZ; z++)
            {
                // if((x != m_fMaxX && x != m_fMinX) && (z != m_fMinZ && z != m_fMaxZ))
                {
                    Cell obj = new Cell();
                    obj.m_vPos = new Vector2Int(x, z);
                    m_vMapRepresentation.Add(obj);
                }
            }
        }
    }

    // Start is called before the first frame m_oUpdate
    void Start()
    {
        Cell oStartingCell = m_vMapRepresentation[Random.Range(0, m_vMapRepresentation.Count - 1)];
        oStartingCell.m_bVisited = true;
        RecursiveBackTracking(oStartingCell);
        CreateMazeFromData();
        m_oPlayer.transform.position = new Vector3(oStartingCell.m_vPos.x, 1, oStartingCell.m_vPos.y);
        m_oSurface.BuildNavMesh();
    }


    // m_oUpdate is called once per frame
    void m_oUpdate()
    {

    }

    void CreateMazeFromData()
    {
        GameObject oEmpty = new GameObject();
        foreach (Cell c in m_vMapRepresentation)
        {
            GameObject oGameObject = Instantiate(oEmpty, new Vector3(c.m_vPos.x, 1, c.m_vPos.y), Quaternion.identity, null);
            bool bm_oRight = (c.m_eWalls & CardinalDirs.m_oRight) != CardinalDirs.None;
            bool bm_oLeft = (c.m_eWalls & CardinalDirs.m_oLeft) != CardinalDirs.None;
            bool bm_oUp = (c.m_eWalls & CardinalDirs.m_oUp) != CardinalDirs.None;
            bool bm_oDown = (c.m_eWalls & CardinalDirs.m_oDown) != CardinalDirs.None;
            bool bNone = (c.m_eWalls == CardinalDirs.None);

            if (bm_oRight)
            {
                AttachWallToGameObject(CardinalDirs.m_oRight, oGameObject, c.m_bDeadEnd);
            }
            if (bm_oLeft)
            {
                AttachWallToGameObject(CardinalDirs.m_oLeft, oGameObject, c.m_bDeadEnd);
            }
            if (bm_oUp)
            {
                AttachWallToGameObject(CardinalDirs.m_oUp, oGameObject, c.m_bDeadEnd);
            }
            if (bm_oDown)
            {
                AttachWallToGameObject(CardinalDirs.m_oDown, oGameObject, c.m_bDeadEnd);
            }
        }
        m_oFloor.transform.position = new Vector3(m_Width * 0.5f, 0.1f, m_Width * 0.5f);
        m_oFloor.transform.localScale = new Vector3(m_Width+5, 1f, m_Width+5);
    }

    void AttachWallToGameObject(CardinalDirs _eDir, GameObject _oGObj, bool _m_bDeadEnd)
    {
        GameObject W = null;
        switch (_eDir)
        {
            case CardinalDirs.m_oUp:
                W = Instantiate(m_oUpWall, _oGObj.transform);
                W.GetComponent<Renderer>().material.color = Color.red;
                break;
            case CardinalDirs.m_oDown:
                W = Instantiate(m_oDownWall, _oGObj.transform);
                W.GetComponent<Renderer>().material.color = Color.red;

                break;
            case CardinalDirs.m_oLeft:
                W = Instantiate(m_oLeftWall, _oGObj.transform);
                W.GetComponent<Renderer>().material.color = Color.red;

                break;
            case CardinalDirs.m_oRight:
                W = Instantiate(m_oRightWall, _oGObj.transform);
                W.GetComponent<Renderer>().material.color = Color.red;
                break;
        }

    }
    void RecursiveBackTracking(Cell _oStartingCell)
    {
        CardinalDirs[] m_vDirectionsToCheck = this.m_vDirections;
        Shuffle<CardinalDirs>(m_vDirections);
        _oStartingCell.m_bVisited = true;
        if (_oStartingCell.m_vNeigbors.Count <= 0)
            getNeighbors(_oStartingCell);
        bool bHasNotCheckedNeighbors = false;
        for (uint i = 0; i < _oStartingCell.m_vNeigbors.Count; i++)
        {
            if (_oStartingCell.m_vNeigbors[(int)i] != null)
                bHasNotCheckedNeighbors = true;
        }
        m_oNextCell = null;
        if (bHasNotCheckedNeighbors)
        {
            m_vStack.Add(_oStartingCell);
            int iCardinalIdx = 0;
            while (iCardinalIdx < m_iNumberOfDirections)
            {
                switch (m_vDirectionsToCheck[iCardinalIdx])
                {
                    //NEIGHBORS POSITIONS IN THE LIST ARE HARDCODED BECAUSE WE KNOW EXACTLY WHERE EXACTLY A CELL IS (Watch getNeighbors())
                    case CardinalDirs.m_oUp:
                        if (_oStartingCell.m_vNeigbors[0] != null && _oStartingCell.m_vNeigbors[0].m_bVisited == false)
                        {
                            m_oNextCell = _oStartingCell.m_vNeigbors[0];
                            m_oNextCell.m_eWalls &= ~CardinalDirs.m_oDown;
                            _oStartingCell.m_eWalls &= ~CardinalDirs.m_oUp;
                            RecursiveBackTracking(m_oNextCell);
                        }
                        break;
                    case CardinalDirs.m_oDown:
                        if (_oStartingCell.m_vNeigbors[1] != null && _oStartingCell.m_vNeigbors[1].m_bVisited == false)
                        {
                            m_oNextCell = _oStartingCell.m_vNeigbors[1];

                            m_oNextCell.m_eWalls &= ~CardinalDirs.m_oUp;
                            _oStartingCell.m_eWalls &= ~CardinalDirs.m_oDown;
                            RecursiveBackTracking(m_oNextCell);
                        }
                        break;
                    case CardinalDirs.m_oRight:
                        if (_oStartingCell.m_vNeigbors[2] != null && _oStartingCell.m_vNeigbors[2].m_bVisited == false)
                        {
                            m_oNextCell = _oStartingCell.m_vNeigbors[2];
                            _oStartingCell.m_eWalls &= ~CardinalDirs.m_oRight;
                            m_oNextCell.m_eWalls &= ~CardinalDirs.m_oLeft;
                            RecursiveBackTracking(m_oNextCell);
                        }
                        break;
                    case CardinalDirs.m_oLeft:
                        if (_oStartingCell.m_vNeigbors[3] != null && _oStartingCell.m_vNeigbors[3].m_bVisited == false)
                        {
                            m_oNextCell = _oStartingCell.m_vNeigbors[3];
                            _oStartingCell.m_eWalls &= ~CardinalDirs.m_oLeft;
                            m_oNextCell.m_eWalls &= ~CardinalDirs.m_oRight;
                            RecursiveBackTracking(m_oNextCell);

                        }
                        break;
                }
                iCardinalIdx++;
            }
        }
        else
        {

            if (m_vStack.Count > 0)
            {
                m_oNextCell = Pop<Cell>(m_vStack);
            }
            if (m_oNextCell != null)
                RecursiveBackTracking(m_oNextCell);
        }
    }

    void Shuffle<T>(IList<T> _vListToShuffle)
    {
        int n = _vListToShuffle.Count;
        int k = 0;
        while (n > 1)
        {
            n--;
            k = m_oRng.Next(n + 1);
            T value = _vListToShuffle[k];
            _vListToShuffle[k] = _vListToShuffle[n];
            _vListToShuffle[n] = value;
        }
    }

    void getNeighbors(Cell _oCell)
    {
        m_oUp = null;
        m_oDown = null;
        m_oRight = null;
        m_oLeft = null;
        if (_oCell.m_vPos.y + 1 < m_Width)
            m_oUp = GetCell(_oCell.m_vPos.x, _oCell.m_vPos.y + 1);
        if (_oCell.m_vPos.y - 1 >= 0)
            m_oDown = GetCell(_oCell.m_vPos.x, _oCell.m_vPos.y - 1);
        if (_oCell.m_vPos.x + 1 < m_Width)
            m_oRight = GetCell(_oCell.m_vPos.x + 1, _oCell.m_vPos.y);
        if (_oCell.m_vPos.x - 1 >= 0)
            m_oLeft = GetCell(_oCell.m_vPos.x - 1, _oCell.m_vPos.y);

        _oCell.m_vNeigbors.Add((m_oUp != null && m_oUp.m_bVisited == false) ? m_oUp : null);
        _oCell.m_vNeigbors.Add(m_oDown != null && m_oDown.m_bVisited == false ? m_oDown : null);
        _oCell.m_vNeigbors.Add(m_oRight != null && m_oRight.m_bVisited == false ? m_oRight : null);
        _oCell.m_vNeigbors.Add(m_oLeft != null && m_oLeft.m_bVisited == false ? m_oLeft : null);
    }

    Cell GetCell(int row, int col)
    {
        int iRetCellIdx = row * m_Width + col;
        return m_vMapRepresentation[iRetCellIdx];
    }

    T Pop<T>(IList<T> _vListToPopElement)
    {
        T ElementPopped = _vListToPopElement[0];
        _vListToPopElement.RemoveAt(0);
        return ElementPopped;
    }
}
