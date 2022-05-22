using PiommodoreBASIC;

/*
	thyoften's BASIC interpreter
	
	See README.md for details
*/

Console.Title = "pbasic";
Console.Clear();

Console.WriteLine($"Welcome to Piommodore BASIC\nMemory size: {(Scratchpad.Size * sizeof(double)) / 1024}k bytes\n");
if (args.Length > 0)
{
    string file = args[0];

    if (File.Exists(file))
    {
        try
        {
            string[] program = File.ReadAllLines(file);

            Interpreter interpreter = new Interpreter(new Parser(program));
            interpreter.Run();
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

