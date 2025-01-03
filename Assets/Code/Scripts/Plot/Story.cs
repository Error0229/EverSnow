using UnityEngine;
public class Story : MonoBehaviour
{
    [SerializeField]
    private string state;
    public string State { get => state; set => state = value; }
}
