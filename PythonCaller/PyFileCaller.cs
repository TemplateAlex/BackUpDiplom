using System;
using System.Threading.Tasks;
using IronPython.Hosting;
using System.IO;
using System.Diagnostics;
using Microsoft.Scripting.Hosting;
using System.Runtime.CompilerServices;

namespace PythonCaller
{
    public class PyFileCaller
    {
        /*public static async Task CallPythonFileAsync(string path)
        {
            await Task.Run(() =>
            {
                string programmToRun = "D:\\Diplomka\\Diplomka\\Diplomka\\wwwroot\\PythonScripts\\Test.py";

                char[] splitter = { 'r' };

                Process process = new Process();
                process.StartInfo.FileName = "python.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.Arguments = "D:\\Diplomka\\Diplomka\\Diplomka\\wwwroot\\PythonScripts\\Test.py";
                process.StartInfo.RedirectStandardOutput = true;

                process.Start();

                process.WaitForExit();
            });
        }*/

        public static void CallPythonFile(string path)
        {
            ScriptEngine scriptEngine = Python.CreateEngine();

            scriptEngine.Execute(path);
        }

        public static async Task CallPythonFileAsync()
        {
            await Task.Run(() =>
            {
                string mainRoute = "D:\\Diplomka\\Diplomka\\Diplomka\\wwwroot\\PythonScripts";

                InsertIntoTablesAVGAndMINValues(mainRoute);

                CreateMinPredictionByLinearRegression(mainRoute);
                CreateAvgPredictionByLinearRegression(mainRoute);
                
            });
        }

        private static void InsertIntoTablesAVGAndMINValues(string mainRoute)
        {
            Process process = new Process();
            process.StartInfo.FileName = "python.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = mainRoute + "\\TestGetAvgAndMinValues.py";
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();
            process.WaitForExit();
        }

        private static void CreateMinPredictionByLinearRegression(string mainRoute)
        {
            Process process = new Process();
            process.StartInfo.FileName = "python.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = mainRoute + "\\TestPredictionPointMinimum.py";
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();
            process.WaitForExit();
        }

        private static void CreateAvgPredictionByLinearRegression(string mainRoute)
        {
            Process process = new Process();
            process.StartInfo.FileName = "python.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = mainRoute + "\\TestPredictionPointAverage.py";
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();
            process.WaitForExit();
        }
    }
}
