using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class OptionHandler : MonoBehaviour
{
    [SerializeField]
    private Button button;

    public void Init(PlotDialogOption option)
    {
        button.GetComponentInChildren<TextMeshProUGUI>().text = option.Text;
        button.onClick.AddListener(() => StoryUI.Instance.OnOptionClick(option.Text));
    }
}
