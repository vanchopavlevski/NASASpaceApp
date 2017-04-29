using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace SyncFood.Python
{
    public class Python
    {
        public string ProcessImage(string imageFilePath,string script,string classifierPath)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine(@"cd " + classifierPath);//C:\Users\Vancho\Desktop\NASA Space App\Python\Food Classifier\");
            cmd.StandardInput.WriteLine("activate tensorflow");
            cmd.StandardInput.WriteLine("python " + script + " " + imageFilePath);

            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();

            string output = cmd.StandardOutput.ReadToEnd();
            string productClass = String.Empty;
            string[] lines = output.Split('\n');
            foreach(string line in lines)
            {
                if (line.Contains("Python: "))
                {
                    string[] array = line.Split(' ');
                    productClass = array[1];
                    productClass = productClass.Remove(productClass.Length - 1, 1);
                    break;
                }
            }
            return productClass;
        }
    }
}