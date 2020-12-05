using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryShop : MonoBehaviour {

    [SerializeField] private GameObject shopping;
    [SerializeField] private RectTransform overlap;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform grid;
    [SerializeField] private Button sortButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private InventoryCells[] cells;
    [SerializeField] private GameObject parentDrop;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button applyButton;
    [SerializeField] private Slider sliderDrop;
    [SerializeField] private Text textDrop;
    [SerializeField] private Text textTitle;
    [SerializeField] private Text textDropInfo;
    [SerializeField] private Image avatarDrop;
    [SerializeField] private Text priceDrop;
    [SerializeField] private GameObject shopInfo;
    [SerializeField] private Color money = Color.green;
    [SerializeField] private Color moneyLimit = Color.red;
    [Header("Массив магазинов:")]
    [SerializeField] private StoreData[] stores;
    [HideInInspector] [SerializeField] private int width = 5;
    [HideInInspector] [SerializeField] private int height = 7;
    private InventoryIcon[] icons;
    private Inventory inventory;
    private StoreData currentStore;
    private InventoryIcon icon, dropIcon;
    private InventoryComponent[] items;
    private List<InventoryIcon> pIcon;
    private List<InventoryCells> targetCell, lastCell;
    private PointerEventData pointerData = new PointerEventData(EventSystem.current);
    private List<RaycastResult> resultsData = new List<RaycastResult>();
    private int layerMask, buy, sell;
    private static InventoryShop _internal;
    private Vector3 offset;
    private Vector2 lastPosition;
    private static bool _Active;
    private InventoryCells[,] field;
    private Sprite avatarDropDef;
    private StoreItem storeItem;
    public bool isLock { get; set; }
    public static bool isShop { get; set; }
    private string moneyHex, moneyLimitHex;

    public static bool isActive
    {
        get { return _Active; }
    }

#if UNITY_EDITOR
    public RectTransform Grid { get { return grid; } }
    public InventoryCells[] Cells { get { return cells; } set { cells = value; } }
    public int Width { set { width = value; } }
    public int Height { set { height = value; } }
#endif

    public void Setup(InventoryIcon[] _icons, Inventory _inventory, InventoryComponent[] _items, int _layerMask)
    {
        moneyHex = ColorUtility.ToHtmlStringRGBA(money);
        moneyLimitHex = ColorUtility.ToHtmlStringRGBA(moneyLimit);
        icons = _icons;
        inventory = _inventory;
        items = _items;
        layerMask = _layerMask;
        _internal = this;
        isShop = false;
        exitButton.onClick.AddListener(() => { Hide(); });
        sortButton.onClick.AddListener(() => { Sort(); });
        cancelButton.onClick.AddListener(() => { ShopCancel(); });
        applyButton.onClick.AddListener(() => { ShopApply(); });
        sliderDrop.wholeNumbers = true;
        sliderDrop.onValueChanged.AddListener(delegate { DropSlider(sliderDrop); });
        pIcon = new List<InventoryIcon>();
        avatarDropDef = avatarDrop.sprite;
        field = new InventoryCells[width, height];

        int j = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[j].item = string.Empty;
                field[x, y] = cells[j];
                field[x, y].isInventory = false;
                j++;
            }
        }

        Hide();
        ShopCancel();
    }

    void ShopCancel()
    {
        applyButton.interactable = false;
        cancelButton.interactable = false;
        sliderDrop.gameObject.SetActive(false);
        textDrop.gameObject.SetActive(false);
        textDropInfo.gameObject.SetActive(true);
        priceDrop.text = "-- // --";
        textTitle.text = "-- // --";
        avatarDrop.sprite = avatarDropDef;
        isShop = false;
    }

    void ShopApply()
    {
        if (dropIcon.isInventory)
        {
            Inventory.UseItem(dropIcon.item, (int)sliderDrop.value);
            Inventory.AdjustMoney(sell);
            AddItem_internal(dropIcon.item, (int)sliderDrop.value);
        }
        else
        {
            Inventory.AddItem(dropIcon.item, (int)sliderDrop.value);
            Inventory.AdjustMoney(-buy);
            UseItem_internal(dropIcon.item, (int)sliderDrop.value);
        }

        ShopCancel();
    }

    void ShopStart(InventoryIcon ico)
    {
        if (ico == null)
        {
            ShopCancel();
            return;
        }

        dropIcon = ico;
        applyButton.interactable = true;
        cancelButton.interactable = true;
        sliderDrop.gameObject.SetActive(true);
        textDrop.gameObject.SetActive(true);
        textDropInfo.gameObject.SetActive(false);
        sliderDrop.minValue = 1;
        sliderDrop.maxValue = ico.counter;
        sliderDrop.value = 1;
        textDrop.text = "Количество: 1";
        isShop = true;
        sliderDrop.onValueChanged.Invoke(0);
    }

    void DropSlider(Slider slider)
    {
        textDrop.text = "Количество: " + sliderDrop.value;

        if (dropIcon.isInventory)
        {
            sell = storeItem.sell * (int)sliderDrop.value;
            priceDrop.text = "Цена продажи:\n<b><color=#" + moneyHex + ">" + sell + "</color></b>";
        }
        else
        {
            buy = storeItem.buy * (int)sliderDrop.value;
            applyButton.interactable = (buy > Inventory.Money) ? false : true;
            priceDrop.text = "Цена покупки:\n<b><color=#" + ((buy > Inventory.Money) ? moneyLimitHex : moneyHex) + ">" + buy + "</color></b>";
        }
    }

    void ShowInfo(StoreItem si, InventoryIcon ico)
    {
        avatarDrop.sprite = ico.iconImage.sprite;
        priceDrop.text = "Продажа:\n<b>" + si.sell + "</b>\nПокупка:\n<b>" + si.buy + "</b>";
        textTitle.text = si.name;
        dropIcon = ico;
        shopInfo.SetActive(true);
    }

    public void Open(string storeName)
    {
        foreach (InventoryIcon i in pIcon)
        {
            Destroy(i.gameObject);
        }

        ClearField();
        pIcon.Clear();

        for (int i = 0; i < stores.Length; i++)
        {
            if (stores[i].name.CompareTo(storeName) == 0)
            {
                currentStore = stores[i];
                break;
            }
        }

        for (int i = 0; i < currentStore.items.Length; i++)
        {
            if (currentStore.items[i].count > 0)
            {
                AddItem_internal(currentStore.items[i].name, currentStore.items[i].count);
            }
        }

        Show();
        inventory.Show();
        shopInfo.SetActive(false);
    }

    void Sort()
    {
        ClearField();

        for (int i = 0; i < pIcon.Count; i++)
        {
            icon = pIcon[i];
            targetCell = GetCells(icon.size);
            AddCurrentIcon(true);
        }
    }

    void ClearField()
    {
        foreach (InventoryCells cell in cells)
        {
            cell.item = string.Empty;
            cell.isLocked = false;
        }
    }

    public void Show()
    {
        _Active = true;
        shopping.SetActive(true);
        parentDrop.SetActive(true);
    }

    public void Hide()
    {
        _Active = false;
        shopping.SetActive(false);
        parentDrop.SetActive(false);
        ShopCancel();
        isLock = false;
        inventory.isLock = false;
    }

    public void MyLateUpdate()
    {
        if (_Active)
        {
            if (Input.GetMouseButtonDown(1))
            {
                ShopStart(GetIcon(Input.mousePosition));
            }

            Control();
        } 
    }

    void ResetCurrent()
    {
        Destroy(icon.gameObject);
    }

    InventoryIcon GetIcon(Vector3 position)
    {
        pointerData.position = position;
        EventSystem.current.RaycastAll(pointerData, resultsData);

        if (resultsData.Count > 0)
        {
            InventoryIcon i = resultsData[0].gameObject.GetComponent<InventoryIcon>();
            return (i != null) ? i : null;
        }

        return null;
    }

    void Control() // управление
    {
        if (icon == null || isLock) return;

        icon.rectTransform.position = offset + Input.mousePosition;

        if (Input.GetMouseButtonUp(0))
        {
            if (!icon.isInside && IsOverlap(icon.element))
            {
                ResetCurrent();
                return;
            }
            else if (icon.isInside && IsOverlap(icon.element))
            {
                ResetDrag();
                return;
            }

            AddCurrentIcon(false);
            resultsData.Clear();
        }
    }

    public static void Focus(InventoryIcon curIcon)
    {
        _internal.Focus_internal(curIcon);
    }

    public void Focus_internal(InventoryIcon curIcon)
    {
        isLock = false;
        inventory.isLock = true;
        ItemInfo(curIcon);
    }

    public static void ItemInfo(InventoryIcon curIcon)
    {
        _internal.ItemInfo_internal(curIcon);
    }

    public void ItemInfo_internal(InventoryIcon curIcon)
    {
        if (!isActive) return;

        for (int i = 0; i < currentStore.items.Length; i++)
        {
            if (currentStore.items[i].name.CompareTo(curIcon.item) == 0)
            {
                storeItem = currentStore.items[i];
                break;
            }
        }

        ShowInfo(storeItem, curIcon);
    }

    public static void BeginDrag(InventoryIcon curIcon) // функция для иконок инвентаря
    {
        _internal.BeginDrag_internal(curIcon);
    }

    public void BeginDrag_internal(InventoryIcon curIcon)
    {
        offset = curIcon.rectTransform.position - Input.mousePosition;
        lastPosition = curIcon.rectTransform.position;
        icon = curIcon;
        icon.iconImage.raycastTarget = false;
        SetUnlock(curIcon.item);
        icon.rectTransform.SetParent(overlap);
    }

    void SetUnlock(string item)
    {
        lastCell = new List<InventoryCells>();
        foreach (InventoryCells cell in cells)
        {
            if (cell.item.CompareTo(item) == 0)
            {
                lastCell.Add(cell);
                cell.item = string.Empty;
                cell.isLocked = false;
            }
        }
    }

    void ResetDrag()
    {
        foreach (InventoryCells cell in lastCell)
        {
            cell.item = icon.item;
            cell.isLocked = true;
        }

        icon.iconImage.raycastTarget = true;
        icon.rectTransform.SetParent(content);
        icon.rectTransform.position = lastPosition;
        icon = null;
    }

    void AddItem_internal(string item, int count)
    {
        foreach (InventoryComponent i in items)
        {
            if (i.item.CompareTo(item) == 0)
            {
                AddQuickly(i, count);
                return;
            }
        }

        Debug.Log(this + " --> item: [ " + item + " ] | Предмет(ы) добавить невозможно! Запрашиваемого предмета не существует!");
    }

    void UseItem_internal(string value, int count)
    {
        int j = 0;
        foreach (InventoryIcon i in pIcon)
        {
            if (i.item.CompareTo(value) == 0 && i.counter > 1)
            {
                i.counter -= count;
                if (i.counter <= 0)
                {
                    SetUnlock(i.item);
                    pIcon.RemoveAt(j);
                    Destroy(i.gameObject);
                    return;
                }
                i.iconCountText.text = i.counter.ToString();
                return;
            }
            else if (i.item.CompareTo(value) == 0 && i.counter == 1)
            {
                SetUnlock(i.item);
                pIcon.RemoveAt(j);
                Destroy(i.gameObject);
                return;
            }

            j++;
        }

        Debug.Log(this + " --> item: [ " + value + " ] | Предмет(ы) использовать невозможно! Запрашиваемого предмета не существует!");
    }

    void AddQuickly(InventoryComponent val, int count) // быстрое добавление
    {
        if (IsSimilar(val, count))
        {
            return;
        }

        targetCell = GetCells(val.size);

        if (targetCell.Count > 0)
        {
            icon = SetIcon(val.size, val.item, content, count, false);
            if (!icon) return;
            AddCurrentIcon(false);
        }
        else
        {
            Debug.Log(this + " --> item: [ " + val.item + " ] count: [ " + count + " ] | Предмет(ы) добавить невозможно! В инвентаре нет места!");
        }
    }

    Sprite GetSprite(string item)
    {
        foreach (InventoryComponent i in items)
        {
            if (i.item.CompareTo(item) == 0) return i.icon;
        }

        return null;
    }

    InventoryIcon SetIcon(InventoryEnum size, string item, RectTransform targetRect, int count, bool isInventory) // создание и настройка новой иконки
    {
        InventoryIcon target = null;

        foreach (InventoryIcon i in icons)
        {
            if (i.size == size)
            {
                target = i;
                break;
            }
        }

        if (target)
        {
            InventoryIcon clone = Instantiate(target) as InventoryIcon;
            clone.iconImage.sprite = GetSprite(item);
            clone.iconCountObject.SetActive(false);
            clone.gameObject.name = item;
            clone.item = item;
            clone.isInside = false;
            clone.isInventory = isInventory;
            clone.counter = count;
            clone.iconCountText.text = count.ToString();
            clone.iconImage.raycastTarget = false;
            clone.rectTransform.SetParent(targetRect);
            clone.rectTransform.localScale = Vector3.one;
            clone.rectTransform.position = Input.mousePosition;
            clone.gameObject.SetActive(true);
            return clone;
        }

        return null;
    }

    void AddCurrentIcon(bool isSort) // добавление иконки
    {
        Vector3 p1 = targetCell[0].rectTransform.position;
        Vector3 p2 = targetCell[targetCell.Count - 1].rectTransform.position;
        Vector3 pos = (p1 - p2) / 2;
        icon.transform.position = p2 + pos;

        foreach (InventoryCells cell in targetCell)
        {
            cell.item = icon.item;
            cell.isLocked = true;
        }

        if (!icon.isInside && !isSort) pIcon.Add(icon);
        icon.rectTransform.SetParent(content);
        icon.isInside = true;
        icon.iconImage.raycastTarget = true;
        icon.iconCountObject.SetActive(true);
        icon = null;
    }

    bool IsOverlap(RectTransform[] rectTransform) // проверка на перекрытие
    {
        targetCell = new List<InventoryCells>();
        foreach (RectTransform tr in rectTransform)
        {
            InventoryCells t = RaycastUI(tr.position);
            if (t == null || t.isLocked || t.isInventory != icon.isInventory) return true;
            targetCell.Add(t);
        }

        if (targetCell.Count == 0) return true;

        return false;
    }

    bool IsInside(RectTransform[] rectTransform) // проверка, объект внутри поля или нет
    {
        Vector3[] worldCorners = new Vector3[4];
        content.GetWorldCorners(worldCorners);
        foreach (RectTransform tr in icon.element)
        {
            if (!PointInside(tr.position, worldCorners)) return false;
        }

        return true;
    }

    bool PointInside(Vector3 position, Vector3[] worldCorners)
    {
        if (position.x > worldCorners[0].x && position.x < worldCorners[2].x
            && position.y > worldCorners[0].y && position.y < worldCorners[2].y)
        {
            return true;
        }

        return false;
    }

    InventoryCells RaycastUI(Vector3 position) // рейкаст по UI клеткам инвентаря
    {
        pointerData.position = position;
        EventSystem.current.RaycastAll(pointerData, resultsData);

        if (resultsData.Count > 0 && resultsData[0].gameObject.layer == layerMask)
        {
            return resultsData[0].gameObject.GetComponent<InventoryCells>();
        }

        return null;
    }

    bool IsSimilar(InventoryComponent val, int count) // поиск похожей иконки
    {
        for (int i = 0; i < pIcon.Count; i++)
        {
            if (pIcon[i].item.CompareTo(val.item) == 0)
            {
                if ((pIcon[i].counter + count) > val.limit)
                {
                    Debug.Log(this + " --> item: [ " + val.item + " ] count: [ " + count + " ] | Предмет(ы) добавить невозможно! Превышен лимит для данного типа.");
                    return true;
                }
                pIcon[i].counter += count;
                pIcon[i].iconCountText.text = pIcon[i].counter.ToString();
                return true;
            }
        }

        return false;
    }

    List<InventoryCells> GetCells(InventoryEnum value)
    {
        // внимание, если вы соберете новую иконку со своими размерами
        // ее необходимо добавить в InventoryEnum
        // и соответственно интегрировать в общую логику ниже

        int xx = 0, yy = 0;
        // определение области поиска, обрезка, чтобы не выйти за рамки двухмерного массива
        switch (value)
        {
            case InventoryEnum.size1x3:
                yy = 2;
                break;
            case InventoryEnum.size1x2:
                yy = 1;
                break;
            case InventoryEnum.size2x1:
                xx = 1;
                break;
            case InventoryEnum.size3x1:
                xx = 2;
                break;
            case InventoryEnum.size2x2:
                xx = 1;
                yy = 1;
                break;
            case InventoryEnum.size3x2:
                xx = 2;
                yy = 1;
                break;
            case InventoryEnum.size2x3:
                xx = 1;
                yy = 2;
                break;
        }

        targetCell = new List<InventoryCells>();
        for (int y = 0; y < height - yy; y++)
        {
            for (int x = 0; x < width - xx; x++)
            {
                switch (value) // ищем место для новой иконки
                {
                    case InventoryEnum.size1x1:
                        if (!field[x, y].isLocked)
                        {
                            targetCell.Add(field[x, y]);
                            return targetCell;
                        }
                        break;
                    case InventoryEnum.size1x3:
                        if (!field[x, y].isLocked && !field[x, y + 1].isLocked && !field[x, y + 2].isLocked)
                        {
                            targetCell.Add(field[x, y]);
                            targetCell.Add(field[x, y + 1]);
                            targetCell.Add(field[x, y + 2]);
                            return targetCell;
                        }
                        break;
                    case InventoryEnum.size1x2:
                        if (!field[x, y].isLocked && !field[x, y + 1].isLocked)
                        {
                            targetCell.Add(field[x, y]);
                            targetCell.Add(field[x, y + 1]);
                            return targetCell;
                        }
                        break;
                    case InventoryEnum.size2x1:
                        if (!field[x, y].isLocked && !field[x + 1, y].isLocked)
                        {
                            targetCell.Add(field[x, y]);
                            targetCell.Add(field[x + 1, y]);
                            return targetCell;
                        }
                        break;
                    case InventoryEnum.size3x1:
                        if (!field[x, y].isLocked && !field[x + 1, y].isLocked && !field[x + 2, y].isLocked)
                        {
                            targetCell.Add(field[x, y]);
                            targetCell.Add(field[x + 1, y]);
                            targetCell.Add(field[x + 2, y]);
                            return targetCell;
                        }
                        break;
                    case InventoryEnum.size2x2:
                        if (!field[x, y].isLocked && !field[x + 1, y].isLocked && !field[x, y + 1].isLocked && !field[x + 1, y + 1].isLocked)
                        {
                            targetCell.Add(field[x, y]);
                            targetCell.Add(field[x + 1, y]);
                            targetCell.Add(field[x, y + 1]);
                            targetCell.Add(field[x + 1, y + 1]);
                            return targetCell;
                        }
                        break;
                    case InventoryEnum.size3x2:
                        if (!field[x, y].isLocked && !field[x + 1, y].isLocked && !field[x, y + 1].isLocked && !field[x + 1, y + 1].isLocked && !field[x + 2, y].isLocked && !field[x + 2, y + 1].isLocked)
                        {
                            targetCell.Add(field[x, y]);
                            targetCell.Add(field[x + 1, y]);
                            targetCell.Add(field[x + 2, y]);
                            targetCell.Add(field[x, y + 1]);
                            targetCell.Add(field[x + 1, y + 1]);
                            targetCell.Add(field[x + 2, y + 1]);
                            return targetCell;
                        }
                        break;
                    case InventoryEnum.size2x3:
                        if (!field[x, y].isLocked && !field[x + 1, y].isLocked && !field[x, y + 1].isLocked && !field[x + 1, y + 1].isLocked && !field[x, y + 2].isLocked && !field[x + 1, y + 2].isLocked)
                        {
                            targetCell.Add(field[x, y]);
                            targetCell.Add(field[x + 1, y]);
                            targetCell.Add(field[x, y + 1]);
                            targetCell.Add(field[x + 1, y + 1]);
                            targetCell.Add(field[x, y + 2]);
                            targetCell.Add(field[x + 1, y + 2]);
                            return targetCell;
                        }
                        break;
                }
            }
        }

        return targetCell;
    }
}
