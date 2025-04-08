

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace PartA;

public class SectionA
{
    const int MAX_LINES_NUMBER = 100000;
    const int BEFFER_SIZE = 1048576; //1MB 
    const string ERROR_PATTERN = "Error: ";
    const string TIMESTAMP_PATTERN = "Timestamp: "; //2023-10-01 12:00:00
    const bool USE_DEFAULT_PARAMS = false; //if false, use the params from the command line

    public static async Task Main(string[] args)
    {
        var filePath = ""; 
        var topErrorsNumberStr = "";

        if (USE_DEFAULT_PARAMS)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Wrong number of arguments");
                return;
            }
            filePath = args[0];
            topErrorsNumberStr = args[1];
        }
        else
        {
            Console.WriteLine("Enter file path:");
            filePath = Console.ReadLine();            

            Console.WriteLine("Enter top errors number:");
            topErrorsNumberStr = Console.ReadLine();
        }

        filePath = filePath != null ? filePath.Trim() : "";
        if (!File.Exists(filePath))
        {
            Console.WriteLine("File " + filePath + " doesn't exists");
            return;
        }
        topErrorsNumberStr = topErrorsNumberStr != null ? topErrorsNumberStr.Trim() : "";
        var topErrorsNumber = -1;
        if (!int.TryParse(topErrorsNumberStr, out topErrorsNumber))
        {
            Console.WriteLine("Errors number should be of integer type");
            return;
        }

        DateTime start = DateTime.Now;
        Console.WriteLine("Start by threading...");
        await ReadFile(filePath, topErrorsNumber);

        DateTime end = DateTime.Now;
        TimeSpan diff = (end - start);
        Console.WriteLine("By threading " +
            "total processing time... {0:00}:{1:00}:{2:00}.{3}", diff.Hours, diff.Minutes, diff.Seconds, diff.Milliseconds);

    }

    private static async Task ReadFileByLine(string filePath)
    {
        //var fileSize = new FileInfo(filePath).Length;

        //var countBulks = fileSize / BEFFER_SIZE + 1;
                
        List<Hashtable> allErrors = new();

        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var sr = new StreamReader(fs);

        var lineCount = 0;

        var errors = new Hashtable();
        while (!sr.EndOfStream)
        {
            lineCount++;
            if (lineCount > MAX_LINES_NUMBER)
            {
                allErrors.Add(errors);
                errors = new Hashtable();
                lineCount = 0;
            }

            string line = sr.ReadLine();
            var error = GetError(line);
            AddError(error, errors);
        }
    }


    private static async Task ReadFile(string filePath, int topErrorsNumber)
    {        
        var fileSize = new FileInfo(filePath).Length;

        var countBulks = fileSize / BEFFER_SIZE + 1;

        Task[] tasks = new Task[countBulks];
        List<Hashtable> allErrors = new();
        Hashtable brokenLinesByTask = new();
       
        for (int i = 0; i < countBulks; i++)
        {
            var start = i * BEFFER_SIZE;
            var taskIndex = i + 1;           
            tasks[i] = Task.Run(() => ReadChunk(filePath, start, (int)Math.Min(BEFFER_SIZE, fileSize - start), allErrors, brokenLinesByTask, taskIndex));
        }

        await Task.WhenAll(tasks);
        //handle broken lines
        if (brokenLinesByTask.Count > 0)
        {
            Hashtable errors = new();

            var length = countBulks - 1;
            for (int i = 0; i < length; i++)
            {
                var taskIndex = i + 1;
                if (brokenLinesByTask.ContainsKey(taskIndex))
                {
                    var brokenLines1 = (List<string>)brokenLinesByTask[taskIndex];
                    var taskIndexNext = taskIndex + 1;
                    var brokenLines2 = (List<string>)brokenLinesByTask[taskIndexNext];

                    var error = GetError(brokenLines1.Last() + brokenLines2.First());
                    AddError(error, errors);
                    brokenLinesByTask.Remove(taskIndex);
                    if (brokenLines2.Count > 1)
                    {
                        brokenLines2.RemoveAt(0);
                        brokenLinesByTask[taskIndexNext] = brokenLines2;
                    }
                    else
                    {
                        brokenLinesByTask.Remove(taskIndexNext);
                        i++;
                    }
                }
            }

            if (errors.Count > 0)
            {
                allErrors.Add(errors);
            }                
        }

        var sortedErrors = new SortedList<string, int>();

        var totalErrors = new HashSet<string>();
        var totalCounts = 0;
        foreach (var errors in allErrors)
        {            
            foreach (string key in errors.Keys)
            {
                if (!totalErrors.Contains(key))
                {
                    totalErrors.Add(key);
                }
                var count = (int)errors[key];
                if (sortedErrors.ContainsKey(key))
                {
                    sortedErrors[key] += count;
                }
                else
                {
                    sortedErrors.Add(key, count);
                }

                totalCounts += count;
            }
        }
        
        var topErrors = sortedErrors.OrderByDescending(x => x.Value).Take(topErrorsNumber).Select(x => x.Key + ": " + x.Value).ToArray();
        Console.WriteLine("Top " + topErrorsNumber + " errors of total " + totalErrors.Count + ":" + Environment.NewLine
            + string.Join(Environment.NewLine, topErrors) + Environment.NewLine + "Total lines: " + totalCounts);
    }

    public static void ReadChunk(string filePath, int start, int count, List<Hashtable> allErrors, Hashtable brokenLinesByTask, int taskIndex)
    {
        //reading the cuurent chunk
        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        fs.Seek(start, SeekOrigin.Begin);
        using var sr = new StreamReader(fs);
        var buffer = new byte[count];        
        fs.Read(buffer);

        var buffreStr = System.Text.Encoding.UTF8.GetString(buffer);
        var lines = buffreStr.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Hashtable errors = new();

        var index = 0;
        var totalLines = lines.Length;
        var brokenLines = new List<string>();
        foreach (var line in lines)
        {
            index++;            
            var error = GetError(line);

            if (index == 1 || index == totalLines)
            {
                var splitError = error.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                if (!line.ToUpper().StartsWith(TIMESTAMP_PATTERN.ToUpper()) || splitError.Length != 2 || splitError[1].Length != 3)
                {                   
                    brokenLines.Add(line);
                    continue;
                }
            }

            AddError(error, errors);           
        }

        allErrors.Add(errors);
        if (brokenLines.Count > 0)
        {
            brokenLinesByTask.Add(taskIndex, brokenLines);
        }        

    }

    private static string GetError(string line)
    {
        var startIndex = line.ToUpper().IndexOf(ERROR_PATTERN.ToUpper());
        if (startIndex == -1)
        {
            return line;
        }
       
        return line.Substring(startIndex + ERROR_PATTERN.Length);
    }

    private static void AddError(string error, Hashtable errors)
    {
        if (errors.ContainsKey(error))
        {
            errors[error] = (int)errors[error] + 1;
        }
        else
        {
            errors.Add(error, 1);
        }
    }
}
