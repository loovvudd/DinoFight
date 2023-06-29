using UnityEngine;
//Cristian Avendaño Guajardo
public class BatallaNavalScript : MonoBehaviour
{
    bool[,] batallaNaval = new bool[10, 8];
    float timer = 1.0f;

    void Start()
    {
        // Asignar 5 posiciones de barcos
        batallaNaval[8, 8] = true;
        batallaNaval[3, 1] = true;
        batallaNaval[2, 6] = true;
        batallaNaval[9, 4] = true;
        batallaNaval[5, 7] = true;
    }

        void Update()
        {
            timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    int x = Random.Range(0, 10);
                    int y = Random.Range(0, 8);

                        if (batallaNaval[x, y])
                        {
                            batallaNaval[x, y] = false;
                            Debug.Log($"Oh no, han destruido tu barco en la posición {x}, {y}");
                        }

                    timer = 1.0f;
                }
        }
}

