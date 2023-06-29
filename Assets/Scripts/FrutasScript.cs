using UnityEngine;
//Cristian Avendaño Guajardo
public class FrutasScript : MonoBehaviour
{
            void Start()
            {
                string[] frutas = { "Melon", "Uva", "Maracuya", "Platano", "Manzana" };

                        for (int i = 0; i < frutas.Length; i++)
                        {
                            Debug.Log(frutas[i]);

                                if (i == 3)
                                {
                                    Debug.Log(frutas[3]);
                                }
                        }
            }
}