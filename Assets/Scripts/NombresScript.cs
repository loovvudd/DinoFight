using UnityEngine;
//Cristian Avendaño Guajardo
public class NombresScript : MonoBehaviour
{
    public string[] nombres = new string[10] { "Diego", "Cristian", "Ivan", "Max", "Vanesa", "Luciel", "Ian", "Francisco", "Francisca", "Natalia" };

    void Start()
    {
                for (int i = 0; i < 1000; i++)
                {
                    string nombre = nombres[Random.Range(0, nombres.Length)];

                            if (nombre == "Francisca")
                            {
                                Debug.Log("¡He encontrado el nombre que buscaba!");
                                break;
                            }
          
                }
    }
}

