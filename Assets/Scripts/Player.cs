using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{

    /*
     * 
     *          Othello
     *  Grado en Tecnologıas Interactivas
     * 
     *  Hecho por: Andrey Kuzmin
     *  git: https://github.com/andrey342/Othello-AK.git
     *  
     *  NO quiero participar en el torneo de Othello.
     *  
     *  Esta tarea no cuenta con la parte avanzada para llegar a la maxima nota.
     *  
     */

    public Tile[] board = new Tile[Constants.NumTiles];
    public Node parent;
    public List<Node> childList = new List<Node>();
    public int type;//Constants.MIN o Constants.MAX
    public double utility;
    public double alfa;
    public double beta;
    public int nextPosition;

    public Node(Tile[] tiles)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            this.board[i] = new Tile();
            this.board[i].value = tiles[i].value;
        }

    }

}

public class Player : MonoBehaviour
{
    public int turn;
    private BoardManager boardManager;

    void Start()
    {
        boardManager = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
    }

    /*
     * Entrada: Dado un tablero
     * Salida: Posición donde mueve  
     */
    public int SelectTile(Tile[] board)
    {
        //Generamos el nodo raíz del árbol (MAX)
        Node root = new Node(board);
        root.type = Constants.MAX;

        //Generamos primer nivel de nodos hijos
        List<int> selectableTiles = boardManager.FindSelectableTiles(board, turn);

        // nvl 1 --------> MIN
        foreach (int s in selectableTiles)  // por todas las posiciones posibles que se puede mover se crea un hijo
        {
            //Creo un nuevo nodo hijo con el tablero padre
            Node n = new Node(root.board);
            //Lo añadimos a la lista de nodos hijo
            root.childList.Add(n);
            //Enlazo con su padre
            n.parent = root;
            //En nivel 1, los hijos son MIN
            n.type = Constants.MIN;
            //Aplico un movimiento, generando un nuevo tablero con ese movimiento
            boardManager.Move(n.board, s, turn);
            //si queremos imprimir el nodo generado (tablero hijo)
            //boardManager.PrintBoard(n.board);

            //Genero la nueva lista de movientos del nodo hijo
            List<int> selectableTiles2 = boardManager.FindSelectableTiles(n.board, -turn);

            //NVL 2 ------> MAX
            foreach (int s2 in selectableTiles2) // lo mismo que arriba pero esta vez la raiz es el hijo
            {
                //Creo un nuevo nodo MAX con el tablero anterior MIN como padre
                Node n2 = new Node(n.board);
                n.childList.Add(n2);
                // enla<o con el nodo anterior del nivel 1
                n2.parent = n;
                // en el nivel 2 los hijos son MAX
                n2.type = Constants.MAX;
                boardManager.Move(n2.board, s2, -turn); // turn es blancas y -turn es cuando mueve las negras
                //calcululo la utilidad de posiciones que renta más de las blancas
                obtenerUtilidad(n2, s2, turn);
                // volcamos el valor de los nodos MAX de este nivel
                volcarNodo(n2);
            }
            // volcamos los nodos MIN de este nivel
            volcarNodo(n);
        }

        //Selecciono un movimiento aleatorio. Esto habrá que modificarlo para elegir el mejor movimiento según MINIMAX
        int movimiento = Random.Range(0, selectableTiles.Count);

        return selectableTiles[root.nextPosition];

    }

    public void volcarNodo(Node nodo) // le pasamos el nodo y pilla el mayor y menor de los hijos
    {
        // si el nodo el MAX
        if (nodo.parent.type == Constants.MAX)
        {
            // obtenemos el nodo mayor
            if (nodo.parent.utility < nodo.utility)
            {
                // lo volcamos al parent
                nodo.parent.utility = nodo.utility;
                nodo.parent.nextPosition = nodo.nextPosition;
            }
        }
        // si el nodo en MIN
        else if (nodo.parent.type == Constants.MIN)
        {
            // obtenemos el nodo menor
            if (nodo.parent.utility > nodo.utility)
            {
                // lo volcamos al parent
                nodo.parent.utility = nodo.utility;
                nodo.parent.nextPosition = nodo.nextPosition;
            }
        }
        
    }

    public void obtenerUtilidad(Node o, int s, int turn) // utilidad , a que posicion moverme renta más
    {
        // obtenemos una lista con las posiciones del oponente que cambiarían de color
        List<int> listaPosicionesOponente = boardManager.FindSwappablePieces(o.board, s, turn);

        // contador de posiciones del oponente
        int cont = 0;
        foreach (int posicion in listaPosicionesOponente)
        {   
            // contador de posiciones del opnoennte
            cont = cont + 1;
        }
        o.utility = cont; // utilidad de las fichas que en ese turno van a cambiar de color.
    }
}


// ------------------------------------------------------