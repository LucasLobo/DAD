using System;
using System.Collections.Generic;
using System.Linq;
using Client.Commands;

namespace Client
{

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("ERROR: Expected a script name but received none.");
                return;
            }

            else if (args.Length > 1)
            {
                Console.WriteLine("WARNING: Expected 1 argument but received " + (args.Length - 1) + ".");
                Console.WriteLine();
            }

            String filename = args[0];

            string[] lines;
            try
            {
                lines = System.IO.File.ReadAllLines(filename);
            } catch (System.IO.FileNotFoundException e)
            {
                Console.WriteLine("ERROR: File " + filename + " not found in current directory.");
                Console.WriteLine(e);
                return;
            }


            try
            {
                List<string> preprocessed = CommandPreprocessor.Preprocess(lines);
            }
            catch (PreprocessingException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
