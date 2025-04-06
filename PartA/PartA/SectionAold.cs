

using System;
using System.Collections;


namespace PartA;

public class SectionA
{
    const string filePath = @"C:\Lea\Home exercise\files\logs.txt";
    const int linesNumbers = 100_000;
    //לחלק את הקובץ לתוך מערכים חלוקה לכל 100,000 שורות ואז ריצה במקביליות
    //ואז עבור כל חלק פירת השכיחיות

    public static async Task findIncidence2(int N)
    {
        Hashtable errors;
        List<Hashtable> hashtables = new List<Hashtable>();
        long fileSize = new FileInfo(filePath).Length;
        int numOfThreads = 13;
        long chunkSize = fileSize / numOfThreads;
        Console.WriteLine($" fileSize: {fileSize} chunckSize:{fileSize / numOfThreads}");
        Console.WriteLine(fileSize);
        Task[] tasks = new Task[numOfThreads];
        for (int i = 0; i < numOfThreads; i++)
        {
            long start = i * chunkSize;
            long end = (i == numOfThreads - 1) ? fileSize : start + chunkSize;
            //lock{
            //errors = new Hashtable();
            tasks[i] = Task.Run(() => ReadFileChunk(start, end, new Hashtable(), hashtables));
        }
        await Task.WhenAll(tasks);
        SortedList<string, int> incidence = new SortedList<string, int>();
        while (hashtables.Count != 0)
        {
            errors = hashtables.First();
            hashtables.Remove(errors);
            List<string> keysToRemove = new List<string>();
            foreach (var error in errors.Keys)
            {
                if (!incidence.TryAdd((string)error, (int)errors[error]))
                {
                    incidence[(string)error] += (int)errors[error];
                    //incidence[itemValue].key = (int)errors[error];
                }
                foreach (var table in hashtables)
                {
                    if (table.Contains(error))
                    {
                        incidence[(string)error] += (int)table[error];
                    }
                }
                //errors.Remove(error);
                keysToRemove.Add((string)error);

            }
            foreach (var key in keysToRemove)
            {
                errors.Remove(key);
            }
        }
        var sortedIncidence = incidence.OrderBy(incidence => incidence.Value);
        for (int i = 0; i < N && sortedIncidence.Count() > i; i++)
        {
            Console.WriteLine(sortedIncidence.ElementAt(i).Key + " " + sortedIncidence.ElementAt(i).Value);
        }

    }
    public static void ReadFileChunk(long start, long end, Hashtable errors ,List<Hashtable> hashtables)
    {
        //try
        //{
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found");
                return;
            }
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            fs.Seek(start, SeekOrigin.Begin);
            using (StreamReader sw = new StreamReader(fs))
            {
                string subLine;
                int num;
                while (fs.Position < end)
                {
                    string line = sw.ReadLine(); 
                //Console.WriteLine(line);
                Console.WriteLine($"[Thread {Task.CurrentId}] {line}");
                subLine = line?.Substring(39);
                //subLine = line;
                    if (subLine != null)
                    {
                        //Console.WriteLine(j);
                        if (errors.ContainsKey(subLine))
                        {
                            num = (int)errors[subLine] + 1;
                            errors.Remove(subLine);
                            errors.Add(subLine, value: num);
                        }
                        else
                        {
                            errors.Add(subLine, 1);
                        }
                    }
                }
                hashtables.Add(errors);
            }
        //}
        //catch (Exception e)
        //{
        //    Console.WriteLine(e.StackTrace);
        //    Console.WriteLine(e.Message);
        //}
    }
    public static void findIncidence(int N)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found");
                return;
            }
            Hashtable errors;
            List<Hashtable> hashtables = new List<Hashtable>();

            using (StreamReader sw = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read)))
            {
                //begin to read the file

                string line, subLine;
                int num;
                int j = 0;
                //read every line from the file and take the error number and insert it into hashtable that each key in this hashtable is
                //error code and the value is the number of times that this error contain in this 100000 lines
                //every 100000 lines has their own hashtable
                //and insert all those hashtables into a list
                while (!sw.EndOfStream)
                {

                    errors = new Hashtable();

                    for (int i = 0; i < linesNumbers; i++)
                    {
                        j++;
                        line = sw.ReadLine();
                        //Console.WriteLine(line[39]);
                        subLine = line?.Substring(39);
                        if (subLine != null)
                        {
                            Console.WriteLine(j);
                            if (errors.ContainsKey(subLine))
                            {
                                num = (int)errors[subLine] + 1;
                                errors.Remove(subLine);
                                errors.Add(subLine, value: num);
                            }
                            else
                            {
                                errors.Add(subLine, 1);
                            }
                        }
                        //Console.WriteLine();
                        //Console.WriteLine(line.Split(",")[1].Split(":")[1].Substring(1));
                    }
                    hashtables.Add(errors);
                }
            }
            //for every item in 
            SortedList<string, int> incidence = new SortedList<string, int>();
            string itemKey;
            int itemValue = 0;
            while (hashtables.Count != 0)
            {
                errors = hashtables.First();
                hashtables.Remove(errors);
                List<string> keysToRemove = new List<string>();
                foreach (var error in errors.Keys)
                {
                    if (!incidence.TryAdd((string)error, (int)errors[error]))
                    {
                        incidence[(string)error] += (int)errors[error];
                        //incidence[itemValue].key = (int)errors[error];
                    }
                    foreach (var table in hashtables)
                    {
                        if (table.Contains(error))
                        {
                            incidence[(string)error] += (int)table[error];
                        }
                    }
                    //errors.Remove(error);
                    keysToRemove.Add((string)error);

                }
                foreach (var key in keysToRemove)
                {
                    errors.Remove(key);
                }
            }
            var sortedIncidence = incidence.OrderBy(incidence => incidence.Value);
            for (int i = 0; i < N && sortedIncidence.Count() > i; i++)
            {
                Console.WriteLine(sortedIncidence.ElementAt(i).Key + " " + sortedIncidence.ElementAt(i).Value);
            }
        }
        catch (Exception e)
        {

            Console.WriteLine(e.StackTrace);
            Console.WriteLine(e.Message);
        }
    }
    public static async Task Main(string[] args)
    {
        System.Console.WriteLine("Hello World!!!!!!");
        await findIncidence2(3);
    }
}
