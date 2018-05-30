/*
ATENÇÃO:
	Conectar 
		- Chave tipo push button (tecla contato momentâneo) entre o pino 1 (+3,3Vcc) e o 16 (GPIO 23) 
		- LED com resistor em série (560ohms ~ 8K2) entre o pino 12(GPIO 18) e o pino 14 (GND)

REV 0 - 27/05/2018
by Marcelo Campos -  Garoa Hacker Clube

*/
using System;
using System.Diagnostics;


namespace Raspi_io
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			int numeroCiclos = 3;

			Console.WriteLine ("Iniciando...");

			// 
			GPIO.PinMode(18, GPIO.Direction.Output);
			GPIO.PinMode(23, GPIO.Direction.Input);


			// LED 
			for (int i = 0; i < numeroCiclos; i++)
			{
				GPIO.Write(18, GPIO.Value.High);
				System.Threading.Thread.Sleep(300);

				GPIO.Write(18, GPIO.Value.Low);
				System.Threading.Thread.Sleep(300);

				Console.Write( (numeroCiclos - i).ToString() + " ");
				Debug.Print(".");
			}

			GPIO.Write(18, GPIO.Value.High);
			Console.WriteLine (" Pressione a Tecla no GPIO 23 para sair");

			while (GPIO.Read(23) == GPIO.Value.Low) 	// ativo ao +3,3 pois RasPi tem PULL DOWN
			{
				System.Threading.Thread.Sleep(5);
			}

			Console.WriteLine ("Pressionada Tecla");


			// importante
			GPIO.ClosePin(18);
			GPIO.ClosePin(23);

			Console.WriteLine("Saindo ...");
		}


	}
}
