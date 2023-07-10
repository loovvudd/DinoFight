using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraDeVida : MonoBehaviour
{
   private Slider slider;

   private void Start()
   {
		slider = GetComponent<Slider>();
   }

   public void CambiarVidaMaxima (float maxLives)
   {
		slider.maxValue = maxLives;
   }

   public void CambiarVidaActual(float currentLives)
   {
		slider.value = currentLives;
   }
   
   public void InicializarBarraDeVida(float currentLives)
   {	
		CambiarVidaMaxima(currentLives);
		CambiarVidaActual(currentLives);
		
   }
}