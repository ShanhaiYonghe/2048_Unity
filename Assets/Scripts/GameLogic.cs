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

    private GameObject _preBtn;
    private GridLayoutGroup _grid;

    private float _timeDistance = 0.25f;
    private float _timeTemp = 0.25f;
    private bool _isDraging;


    private const int MATRIX_NUM = 4;
    private const int FILL_VALUE = 2;

    private static System.Random _random = new System.Random();
    private static int _step = 0;
    private static bool _isMoved = false;
    private static bool _isCanContinue = false;

    private static Dictionary<Tuple<int, int>, int> _dic = new Dictionary<Tuple<int, int>, int>();
    private static Dictionary<Tuple<int, int>, Text> _dicText = new Dictionary<Tuple<int, int>, Text>();

    void Awake()
    {
        _preBtn = Resources.Load("Prefab/pre_btn") as GameObject;
        _grid = transform.Find("Grid").GetComponent<GridLayoutGroup>();

        EventTriggerListener.SetListener(_grid.gameObject, EventTriggerType.Drag, OnDrag);
    }
    void Start()
    {
        Init();
        InitUI();
    }
    void Update()
    {
        if (!_isDraging)
            _timeTemp -= Time.deltaTime;
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
                Init();
            }
        }
    }

    private void Init()
    {
        _dic.Clear();
        _step = 0;

        for (int row = 0; row < MATRIX_NUM; row++)
        {
            for (int col = 0; col < MATRIX_NUM; col++)
            {
                _dic.Add(new Tuple<int, int>(row, col), 0);
            }
        }

        _dic[new Tuple<int, int>(_random.Next(0, MATRIX_NUM), _random.Next(0, MATRIX_NUM))] = FILL_VALUE;
    }
    private void InitUI()
    {
        _dicText.Clear();
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
        }
        Print();
    }
    private static bool CheckCanContinue()
    {
        return CheckHasHole() || CheckHasNearSameValue();
    }
    private static bool CheckHasHole()
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
    private static bool CheckHasNearSameValue()
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

    private static bool Calc(MoveDirection key)
    {
        _isMoved = false;

        #region Calc

        List<int> list = new List<int>();
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
            Print();

            return CheckCanContinue();
        }
        else
        {
            return CheckHasNearSameValue();
        }
    }
    private static List<int> CalcSingle(List<int> list)
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
    private static List<int> RemoveEmpty(List<int> list)
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
    private static void Print()
    {
        for (int row = 0; row < MATRIX_NUM; row++)
        {
            for (int col = 0; col < MATRIX_NUM; col++)
            {
                var key = new Tuple<int, int>(row, col);
                var value = _dic[key];
                _dicText[key].text = value > 0 ? value.ToString() : "";
            }
        }
    }
}

