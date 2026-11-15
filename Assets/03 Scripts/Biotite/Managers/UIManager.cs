using UnityEngine;

public class UIManager : MonoBehaviour, IManager
{

    [field: SerializeField] public GameObject Blocker { get; private set; }
    [field: SerializeField] public ItemToolTip ItemToolTip { get; private set; }
    [field:SerializeField] public MouseObject MouseObject { get; private set; }

    public void Init()
    {
      
    }

    public void Release()
    {
 
    }


}
