using UnityEngine;

public class GetAnim : MonoBehaviour
{
    // Método para obtener el animator del sprite de un enemigo y así dárselo a otro hijo

    public Animator GetAnimator()
    {
        return transform.GetComponentInChildren<Animator>();
    }
    
}
