#define ARGS
#define TIME

using PiommodoreBASIC;

Console.Title = "pbasic";
Console.Clear();

#if ARGS
Console.WriteLine($"Welcome to Piommodore BASIC\nMemory size: {(Scratchpad.Size * sizeof(double)) / 1024}k bytes\n");
if (args.Length > 0)
{
    string file = args[0];

    if (File.Exists(file))
    {
        try
        {
            string[] program = File.ReadAllLines(file);
#if TIME
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
#endif
            Interpreter interpreter = new Interpreter(new Parser(program));
            interpreter.Run();
#if TIME
            stopwatch.Stop();
            Console.WriteLine($"Elapsed time: {stopwatch.Elapsed.TotalSeconds} seconds");
#endif
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Console.ReadLine();
        }
    }
    else
    {
        Console.WriteLine($"No such file: {file}\nUsage: basic.exe [program]");
    }
}
else
{
    Console.WriteLine("No program file, exiting.\nUsage: basic.exe [program]");
}
#else
List<string> program = new List<string>();

string input = "";
while (true)
{
    input = Console.ReadLine();
    if (input.Length > 0 && input != "RUN")
        program.Add(input);
    else
    {
        Interpreter interpreter = new Interpreter(new Parser(program.ToArray()));
        interpreter.Run();

        return;
    }
}

#endif