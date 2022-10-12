namespace PerformanceInit.App.Services;
using System.Diagnostics;

public interface IBaseService
{
	int Number { get; }
	void WriteName();
}
