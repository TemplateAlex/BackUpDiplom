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
        private static readonly string mainRoute = "C:\\Users\\MohenTohen\\Desktop\\ZhukovPuchkov\\Diplomka\\wwwroot\\PythonScripts\\";

        public static void CallPythonFile()
        {
            InsertIntoTablesAVGAndMINValues();
            CreatePredictionByLinearRegression();
        }

        public static async Task CallPythonFileAsync()
        {
            await Task.Run(() =>
            {
                InsertIntoTablesAVGAndMINValues();
                CreatePredictionByLinearRegression();
            });
        }

        private static void InsertIntoTablesAVGAndMINValues()
        {
            Process process = new Process();
            process.StartInfo.FileName = "python.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = mainRoute + "\\GetAvgAndMinOrders.py";
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();
            process.WaitForExit();
        }

        private static void CreatePredictionByLinearRegression()
        {
            Process process = new Process();
            process.StartInfo.FileName = "python.exe";
            process.StartInfo.UseShellExecute=false;
            process.StartInfo.Arguments = mainRoute + "\\GetPredictions.py";
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();
            process.WaitForExit();
        }
    }
}
