using UnityEngine;
// Clase auxiliar de listas, modificada para aceptar Transform
public class TransformList 
{
    // Clase privada para los nodos
    private class Node
    {
        public Transform dato;   // información del nodo (podría ser de cualquier tipo)
        public Node sig;   // referencia al siguiente
    }

    Node pri;  // referencia al primer nodo de la lista

    public TransformList()
    {  // constructora de la clase
        pri = null;   //  ||
    }


    // añadir nodo al final de la lista
    public void InsertInEnd(Transform e)
    {
        // distinguimos dos casos
        // lista vacia
        if (pri == null)
        {
            pri = new Node(); // creamos nodo en pri
            pri.dato = e;
            pri.sig = null;
        }
        else
        { // lista no vacia
            Node aux = pri;   // recorremos la lista hasta el ultimo nodo
            while (aux.sig != null)
            {
                aux = aux.sig;
            }
            // aux apunta al último nodo
            aux.sig = new Node(); // creamos el nuevo a continuación
            aux = aux.sig;         // avanzamos aux al nuevo nodo
            aux.dato = e;          // ponemos info 
            aux.sig = null;        // siguiente a null 
        }
    }

    public int Lenght()
    {
        // devuelve el número de elementos de la lista
        int cont = 0;
        Node aux = pri;
        if (pri != null){
            cont++;
            while (aux.sig != null)
            {
                aux = aux.sig;
                cont++;
            }
        }
        return cont;
    }


    // auxiliar privada para buscar un nodo con un elto
    // devuelve referencia al nodo donde está el elto
    // devuelve null si no está ese elto
    private Node SearchNode(Transform e)
    {
        Node aux = pri; // referencia al primero
        while (aux != null && aux.dato != e){  // búsqueda de nodo con elto e
            aux = aux.sig;
        }
        // termina con aux==null (elto no encontrado)
        // o bien con aux apuntando al primer nodo con elto e
        return aux;
    }

    private bool SearchTransform(Transform e)
    {
        Node aux = SearchNode(e);
        return (aux != null);
    }

    public int CheckNullNodes()
    {
        Node aux = pri;
        while (aux != null){
            if (aux.dato == null)
                DeleteElement(aux.dato);
            aux = aux.sig;
        }
         
        return Lenght();
    }

    public bool DeleteElement(Transform e)
    {
        bool found = SearchTransform(e);
        if (found)
        {
            Node aux = pri;
            if (pri.dato == e)
            {
                pri = pri.sig;
                return found;
            }
            while (aux.sig != null && aux.sig.dato != e){
                aux = aux.sig;
            }
            if (aux.sig != null){
                aux.sig = aux.sig.sig;
            }
            else{
                if (aux == pri)
                    pri = null;
                else
                    aux = null;
            }
        }
        return found;
    }
}
