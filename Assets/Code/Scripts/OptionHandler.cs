using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class OptionHandler : MonoBehaviour
{
    [SerializeField]
    private Button button;
    [SerializeField]
    private TextMeshProUGUI optionText;

    private readonly Color defaultColor = new Color(50 / 255f, 52 / 255f, 108 / 255f); // Color(50, 52, 108); // hsl(238, 37, 31)
    private readonly Color highlightColor = Color.white;
    public Selectable AnySelectable;
    private PropertyInfo _selectableStateInfo = null;

    private void Awake()
    {
        _selectableStateInfo = typeof(Selectable).GetProperty("currentSelectionState", BindingFlags.NonPublic | BindingFlags.Instance);
    }


    public void Update()
    {
        var state = (int)_selectableStateInfo.GetValue(AnySelectable);
        if (state == 0)
        {
            optionText.color = defaultColor;
        }
        else
        {
            optionText.color = highlightColor;
        }
    }

    public void Init(PlotDialogOption option)
    {
        optionText.text = option.Text;
        optionText.color = defaultColor;
        button.onClick.AddListener(() =>
        {
            StoryUI.Instance.OnOptionClick(option.Text);
            optionText.color = highlightColor;
        });
    }
}
