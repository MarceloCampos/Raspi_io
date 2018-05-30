/*
 * Copyright (c) 2016 Akinwale Ariwodola <akinwale@gmail.com>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy,
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */


using System;
using System.IO;
using System.Threading;
using System.Diagnostics;


namespace Raspi_io
{

	public static class GPIO
	{
		public enum Direction
		{
			Input = 0,
			Output = 1
		};

		public enum Value
		{
			Low = 0,
			High = 1
		};

		private const string GPIOPath = "/sys/class/gpio";

		private const string GPIODirection = "direction";

		private const string GPIOExport = "export";

		private const string GPIOUnexport = "unexport";

		private const string GPIOValue = "value";

		public static void PinMode(int pin, Direction direction)
		{
			ClosePin(pin);

			string pinPath = Path.Combine(GPIOPath, string.Format("gpio{0}", pin));
			if (!Directory.Exists(pinPath))
			{
				try
				{
					using (StreamWriter writer = new StreamWriter(new FileStream(
						Path.Combine(GPIOPath, GPIOExport), FileMode.Open, FileAccess.Write, FileShare.ReadWrite)))
					{
						writer.Write(pin);
					}
				}
				catch (IOException ex)
				{
					Debug.Print("Unable to export the pin.", ex);
				}
			}

			do
			{
				// Wait until the pin has been initialised properly before setting the direction
				Thread.Sleep(500);
			} while (!Directory.Exists(pinPath));

			try
			{
				using (StreamWriter writer = new StreamWriter(new FileStream(
					Path.Combine(pinPath, GPIODirection), FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite)))
				{
					writer.Write(direction == Direction.Input ? "in" : "out");
				}
			}
			catch (IOException ex)
			{
				Debug.Print("Unable to set the pin direction.", ex);
			}
		}

		public static void ClosePin(int pin)
		{
			string pinPath = Path.Combine(GPIOPath, string.Format("gpio{0}", pin));
			if (Directory.Exists(pinPath))
			{
				try
				{
					using (StreamWriter writer = new StreamWriter(new FileStream(
						Path.Combine(GPIOPath, GPIOUnexport), FileMode.Open, FileAccess.Write, FileShare.ReadWrite)))
					{
						writer.Write(pin);
					}
				}
				catch (IOException ex)
				{
					Debug.Print("Unable to close the pin.", ex);
				}
			}
		}

		public static void Write(int pin, Value value)
		{
			try
			{
				string pinPath = Path.Combine(GPIOPath, string.Format("gpio{0}", pin));
				using (StreamWriter writer = new StreamWriter(new FileStream(
					Path.Combine(pinPath, GPIOValue), FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite)))
				{
					writer.Write(value == Value.Low ? 0 : 1);
				}
			}
			catch (IOException ex)
			{
				Debug.Print("Unable to write the pin value.", ex);
			}
		}

		public static Value? Read(int pin)
		{
			int pinValue = -1;

			try
			{
				string pinPath = Path.Combine(GPIOPath, string.Format("gpio{0}", pin));
				using (StreamReader reader = new StreamReader(new FileStream(
					Path.Combine(pinPath, GPIOValue), FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
				{
					int.TryParse(reader.ReadToEnd(), out pinValue);
				}
			}
			catch (IOException ex)
			{
				Debug.Print("Unable to read the pin value.", ex);
			}

			if (pinValue != 0 && pinValue != 1)
			{
				return null;
			}

			return (Value) pinValue;
		}
	}

}

