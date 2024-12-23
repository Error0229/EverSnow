using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class OptionHandler : MonoBehaviour
{
    [SerializeField]
    private Button button;
    [SerializeField]
    private TextMeshProUGUI optionText;

    public void Init(PlotDialogOption option)
    {
        optionText.text = option.Text;
        button.onClick.AddListener(() => StoryUI.Instance.OnOptionClick(option.Text));
    }
}
