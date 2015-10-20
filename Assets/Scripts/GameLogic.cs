using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class GameLogic : MonoBehaviour
{
    enum MoveDirection
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }

    #region UI

    private GridLayoutGroup _grid;
    private Text _lblBestScore;
    private Text _lblNowScore;
    private Text _lblUndoDesc;

    private Button _btnUndo;
    private Button _btnRestart;
    private Button _btnNewGame;
    private Button _btnContinueGame;
    private Button _btnQuitGame;

    private GameObject _preBtn;
    private GameObject _goMenu;

    #endregion

    private const int MATRIX_NUM = 4;
    private const int FILL_VALUE = 2;

    public static GameLogic Instance { get; private set; }

    private System.Random _random = new System.Random();
    private int _maxMemoryCount = 5;
    private float _timeDistance = 0.25f;

    private int _countAlreadyUndoMemory = 0;
    private int _step = 0;
    private int _scoreBest = 0;
    private int _scoreNow = 0;

    private float _timeTemp = 0.25f;
    private bool _isCanContinue = false;
    private bool _isMoved = false;
    private bool _isDraging;

    private Dictionary<Tuple<int, int>, int> _dic = new Dictionary<Tuple<int, int>, int>();
    private Dictionary<Tuple<int, int>, Text> _dicText = new Dictionary<Tuple<int, int>, Text>();
    private Dictionary<Tuple<int, int>, Image> _dicImage = new Dictionary<Tuple<int, int>, Image>();
    private List<Dictionary<Tuple<int, int>, int>> _listMemory = new List<Dictionary<Tuple<int, int>, int>>();

    void Awake()
    {
        Instance = this;

        #region UI

        _preBtn = Resources.Load("Prefab/pre_btn") as GameObject;
        _grid = transform.Find("Grid").GetComponent<GridLayoutGroup>();
        _lblBestScore = transform.Find("LblScoreBest").GetComponent<Text>();
        _lblNowScore = transform.Find("LblScoreNow").GetComponent<Text>();
        _lblUndoDesc = transform.Find("BtnUndo/Text").GetComponent<Text>();

        _btnUndo = transform.Find("BtnUndo").GetComponent<Button>();
        _btnRestart = transform.Find("BtnRestart").GetComponent<Button>();

        _btnNewGame = transform.Find("Menu/BtnContinue").GetComponent<Button>();
        _btnContinueGame = transform.Find("Menu/BtnContinue").GetComponent<Button>();
        _btnQuitGame = transform.Find("Menu/BtnQuit").GetComponent<Button>();
        _goMenu = transform.Find("Menu").gameObject;

        #endregion

        EventTriggerListener.SetListener(_grid.gameObject, EventTriggerType.Drag, OnDrag);
        EventTriggerListener.SetListener(_btnRestart.gameObject, EventTriggerType.PointerClick, OnClickBtnRestart);
        EventTriggerListener.SetListener(_btnUndo.gameObject, EventTriggerType.PointerClick, OnClickBtnUndo);
        EventTriggerListener.SetListener(_btnNewGame.gameObject, EventTriggerType.PointerClick, OnClickBtnRestart);
        EventTriggerListener.SetListener(_btnContinueGame.gameObject, EventTriggerType.PointerClick, OnClickBtnContinue);
        EventTriggerListener.SetListener(_btnQuitGame.gameObject, EventTriggerType.PointerClick, OnClickBtnQuit);

        #region Init

        for (int i = _grid.transform.childCount; i < MATRIX_NUM * MATRIX_NUM; i++)
        {
            var tr = Instantiate(_preBtn).transform;
            tr.parent = _grid.transform;
            tr.localPosition = Vector2.zero;
            tr.localScale = Vector3.one;
        }

        int row = 0;
        int col = 0;
        for (int i = 0; i < _grid.transform.childCount; i++)
        {
            row = i / 4;
            col = i % 4;

            _dicText[new Tuple<int, int>(row, col)] = _grid.transform.GetChild(i).Find("Text").GetComponent<Text>();
            _dicImage[new Tuple<int, int>(row, col)] = _grid.transform.GetChild(i).GetComponent<Image>();
        }

        #endregion

        ReStart();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            _goMenu.gameObject.SetActive(true);

        if (!_isDraging)
            _timeTemp -= Time.deltaTime;

        if (_scoreNow > _scoreBest)
        {
            _scoreBest = _scoreNow;
            _lblBestScore.text = _scoreBest.ToString();

            PlayerPrefs.SetInt("BestScore", _scoreBest);
        }
    }
    void Destroy()
    {

    }

    private void ReStart()
    {
        _goMenu.SetActive(false);

        _dic.Clear();
        _listMemory.Clear();
        _step = 0;
        _countAlreadyUndoMemory = 0;

        for (int row = 0; row < MATRIX_NUM; row++)
        {
            for (int col = 0; col < MATRIX_NUM; col++)
            {
                _dic.Add(new Tuple<int, int>(row, col), 0);
            }
        }

        _dic[new Tuple<int, int>(_random.Next(0, MATRIX_NUM), _random.Next(0, MATRIX_NUM))] = FILL_VALUE;

        _scoreBest = PlayerPrefs.GetInt("BestScore");
        _scoreNow = 0;

        ShowContent();
    }
    private bool CheckCanContinue()
    {
        return CheckHasHole() || CheckHasNearSameValue();
    }
    private bool CheckHasHole()
    {
        bool flag = false;
        foreach (var tuple in _dic)
        {
            if (tuple.Value == 0)
            {
                flag = true;
                break;
            }
        }
        return flag;
    }
    private bool CheckHasNearSameValue()
    {
        bool flag = false;
        Tuple<int, int> tuple1 = null;
        Tuple<int, int> tuple2 = null;
        Tuple<int, int> tuple3 = null;

        for (int row = 0; row < MATRIX_NUM; row++)
        {
            for (int col = 0; col < MATRIX_NUM; col++)
            {
                tuple1 = new Tuple<int, int>(row, col);
                tuple2 = new Tuple<int, int>(row, col + 1);
                tuple3 = new Tuple<int, int>(row + 1, col);

                if (_dic.ContainsKey(tuple2) && _dic[tuple1] == _dic[tuple2])
                {
                    flag = true;
                    break;
                }
                if (_dic.ContainsKey(tuple3) && _dic[tuple1] == _dic[tuple3])
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
                break;
        }

        return flag;
    }

    private bool Calc(MoveDirection key)
    {
        _isMoved = false;

        #region Calc

        List<int> list = new List<int>();

        if (key == MoveDirection.None)
            return true; //输入非法, 不做处理
        else
            AddNewMemory();

        if (key == MoveDirection.Up)
        {
            for (int col = 0; col < MATRIX_NUM; col++)
            {
                list = _dic.Where(item => item.Key.Item2 == col).OrderBy(item => item.Key.Item1).Select(item => item.Value).ToList();
                list = CalcSingle(list);

                for (int index = 0; index < MATRIX_NUM; index++)
                {
                    _dic[new Tuple<int, int>(index, col)] = list[index];
                }
            }
        }
        else if (key == MoveDirection.Down)
        {
            for (int col = 0; col < MATRIX_NUM; col++)
            {
                list = _dic.Where(item => item.Key.Item2 == col).OrderByDescending(item => item.Key.Item1).Select(item => item.Value).ToList();
                list = CalcSingle(list);
                list.Reverse();

                for (int index = 0; index < MATRIX_NUM; index++)
                {
                    _dic[new Tuple<int, int>(index, col)] = list[index];
                }
            }
        }
        else if (key == MoveDirection.Left)
        {
            for (int row = 0; row < MATRIX_NUM; row++)
            {
                list = _dic.Where(item => item.Key.Item1 == row).OrderBy(item => item.Key.Item2).Select(item => item.Value).ToList();
                list = CalcSingle(list);

                for (int index = 0; index < MATRIX_NUM; index++)
                {
                    _dic[new Tuple<int, int>(row, index)] = list[index];
                }
            }
        }
        else if (key == MoveDirection.Right)
        {
            for (int row = 0; row < MATRIX_NUM; row++)
            {
                list = _dic.Where(item => item.Key.Item1 == row).OrderByDescending(item => item.Key.Item2).Select(item => item.Value).ToList();
                list = CalcSingle(list);
                list.Reverse();

                for (int index = 0; index < MATRIX_NUM; index++)
                {
                    _dic[new Tuple<int, int>(row, index)] = list[index];
                }
            }
        }
        else
        {
            return true; //输入非法, 不做处理
        }

        #endregion

        if (!_isMoved) //没有产生移动
            return CheckCanContinue();

        if (CheckHasHole())
        {
            var listEmpty = _dic.Where(item => item.Value == 0).Select(item => item.Key).ToList();
            var tuple = listEmpty[_random.Next(listEmpty.Count)];
            _dic[tuple] = FILL_VALUE;
            _step++;

            ShowContent();

            return CheckCanContinue();
        }
        else
        {
            return CheckHasNearSameValue();
        }
    }
    private List<int> CalcSingle(List<int> list)
    {
        int index = 0;
        list = RemoveEmpty(list);
        while (index < MATRIX_NUM - 1)
        {
            if (list[index] > 0 && list[index + 1] == list[index])
            {
                list[index] *= 2;
                list[index + 1] = 0;
                list = RemoveEmpty(list);

                _isMoved = true;
            }
            index++;
        }
        list = RemoveEmpty(list);

        return list;
    }
    private List<int> RemoveEmpty(List<int> list)
    {
        List<int> listNew = new List<int>();
        foreach (var item in list)
        {
            if (item > 0)
                listNew.Add(item);
        }
        for (int i = listNew.Count; i < list.Count; i++)
        {
            listNew.Add(0);

            if (list[listNew.Count - 1] != 0)
                _isMoved = true;
        }

        return listNew;
    }

    private void ShowContent()
    {
        for (int row = 0; row < MATRIX_NUM; row++)
        {
            for (int col = 0; col < MATRIX_NUM; col++)
            {
                var key = new Tuple<int, int>(row, col);
                var value = _dic[key];

                _dicText[key].text = value > 0 ? value.ToString() : "";
                _dicImage[key].color = GetBGColorForValue(value);
            }
        }

        _scoreNow = _dic.Values.Sum();
        _lblNowScore.text = _scoreNow.ToString();
        _lblBestScore.text = _scoreBest.ToString();
        _lblUndoDesc.text = "回退(" + (_maxMemoryCount - _countAlreadyUndoMemory).ToString() + ")";
    }
    private Color GetBGColorForValue(int value)
    {
        if (value == 0)
        {
            return new Color(255, 255, 255);
        }
        if (value < 16)
        {
            return new Color(184, 184, 184);
        }
        else if (value < 128)
        {
            return new Color(0, 245, 0);
        }
        else if (value < 1024)
        {
            return new Color(0, 122, 245);
        }
        else if (value < 4096)
        {
            return new Color(143, 0, 214);
        }
        else //if (value <= 65536)
        {
            return new Color(255, 177, 20);
        }
    }
    private void AddNewMemory()
    {
        if (_listMemory.Count >= _maxMemoryCount)
        {
            _listMemory.Remove(_listMemory.First());
        }

        var dic = new Dictionary<Tuple<int, int>, int>();
        foreach (var pair in _dic)
        {
            dic.Add(new Tuple<int, int>(pair.Key.Item1, pair.Key.Item2), pair.Value);
        }
        _listMemory.Add(dic);
    }
    private Dictionary<Tuple<int, int>, int> UndoMemory()
    {
        Dictionary<Tuple<int, int>, int> dic = null;
        if (_listMemory.Count > 0)
        {
            dic = _listMemory.Last();
            _listMemory.Remove(dic);
        }
        return dic;
    }

    private void OnDrag(PointerEventData eventData)
    {
        if (_timeTemp < 0)
        {
            _isDraging = true;
            _timeTemp = _timeDistance;

            var delta = eventData.delta;

            if (delta.x > 10)
                _isCanContinue = Calc(MoveDirection.Right);
            else if (delta.x < -10)
                _isCanContinue = Calc(MoveDirection.Left);
            else if (delta.y > 10)
                _isCanContinue = Calc(MoveDirection.Up);
            else if (delta.y < -10)
                _isCanContinue = Calc(MoveDirection.Down);

            _isDraging = false;

            if (!_isCanContinue)
            {
                ReStart();
            }
        }
    }
    private void OnClickBtnUndo(PointerEventData eventData)
    {
        if (_countAlreadyUndoMemory >= _maxMemoryCount)
            return;

        var dicTemp = UndoMemory();
        if (dicTemp == null)
            return;

        _dic = dicTemp;
        _isCanContinue = true;
        _step--;
        _countAlreadyUndoMemory++;

        ShowContent();
    }
    private void OnClickBtnRestart(PointerEventData eventData)
    {
        ReStart();
    }
    private void OnClickBtnContinue(PointerEventData eventData)
    {
        _goMenu.SetActive(false);
    }
    private void OnClickBtnQuit(PointerEventData eventData)
    {
        Application.Quit();
    }
}

