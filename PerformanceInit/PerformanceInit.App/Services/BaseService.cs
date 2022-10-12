namespace PerformanceInit.App.Services;
using System.Diagnostics;

public abstract class BaseService
{
	public int Number { get; set; }



	public void WriteName()
	{
		System.Diagnostics.Debug.WriteLine($"Este es mi nombre: {GetType()}.");
	}
}
