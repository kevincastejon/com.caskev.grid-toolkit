using UnityEngine;

public class DebugToDelete : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Listing all components attached to this GameObject:" + gameObject.name);
        Component[] comps = GetComponents<Component>();
        foreach (Component comp in comps)
        {
            Debug.Log(comp.GetType().Name);
        }
    }
}
