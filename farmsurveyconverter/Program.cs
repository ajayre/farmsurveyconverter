using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using NDesk.Options;

namespace farmsurveyconverter
{
    class Program
    {
        enum InputTypes
        {
            Multiplane
        }

        enum OutputTypes
        {
            Xyz,
            Obj
        }

        static void Main(string[] args)
        {
            Console.WriteLine(Program.GetProductName() + " " + Program.GetVersion());
            Console.WriteLine(Program.GetCopyright());
            Console.WriteLine();

            string InputFile = null;
            string OutputFile = null;
            InputTypes InputType = InputTypes.Multiplane;
            OutputTypes OutputType = OutputTypes.Obj;

            bool ShowHelp = false;

            try
            {
                OptionSet Options = new OptionSet()
                    .Add("h|help", delegate (string v) { ShowHelp = v != null; })
                    .Add("i|input=", delegate (string v) { InputFile = ParseFileName(v); })
                    .Add("o|output=", delegate (string v) { OutputFile = ParseFileName(v); })
                    .Add("intype=", delegate (string v) { InputType = ParseInputType(v); })
                    .Add("outtype=", delegate (string v) { OutputType = ParseOutputType(v); });
                Options.Parse(args);
            }
            catch (Exception Exc)
            {
                Console.WriteLine("ERROR: " + Exc.Message);
                return;
            }

            if (ShowHelp)
            {
                Console.WriteLine("General:");
                Console.WriteLine("  --help (synonyms: h)                    : show this help");
                Console.WriteLine("  --input=\"<filename>\" (synonyms: i)      : Input file");
                Console.WriteLine("  --output=\"<filename>\" (synonyms: o)     : Output file");
                Console.WriteLine("  --intype type                           : Type of input file, default is multiplane");
                Console.WriteLine("    Supported types:");
                Console.WriteLine("      multiplane");
                Console.WriteLine("  --outtype type                          : Type of output file, default is obj");
                Console.WriteLine("    Supported types:");
                Console.WriteLine("      obj");
                Console.WriteLine("      xyz");

                return;
            }

            if (InputFile == null)
            {
                Console.WriteLine("ERROR: No input file specified");
                return;
            }

            if (OutputFile == null)
            {
                Console.WriteLine("ERROR: No output file specified");
                return;
            }

            if (!File.Exists(InputFile))
            {
                Console.WriteLine(string.Format("ERROR: Input file '{0}' not found", InputFile));
                return;
            }

            Survey Surv;

            Console.WriteLine("Processing " + InputFile + "...");

            try
            {
                IImporter Importer = null;

                switch (InputType)
                {
                    case InputTypes.Multiplane:
                        Importer = new MultiplaneImporter();
                        break;
                }

                if (Importer == null)
                {
                    Console.WriteLine("ERROR: No importer found");
                    return;
                }

                Surv = Importer.Import(InputFile);
            }
            catch (Exception Exc)
            {
                Console.WriteLine("ERROR: Failed to import, " + Exc.Message);
                return;
            }

            try
            {
                IProcessor Processor = new HeightScalingProcessor() { ScaleFactor = 20.0 };
                Processor.Process(Surv);
            }
            catch (Exception Exc)
            {
                Console.WriteLine("ERROR: Failed to process, " + Exc.Message);
            }

            try
            {
                IExporter Exporter = null;

                switch (OutputType)
                {
                    case OutputTypes.Obj:
                        Exporter = new ObjExporter();
                        break;
                    case OutputTypes.Xyz:
                        Exporter = new XyzExporter();
                        break;
                }

                if (Exporter == null)
                {
                    Console.WriteLine("ERROR: No exporter found");
                    return;
                }

                Exporter.Export(Surv, OutputFile);
            }
            catch (Exception Exc)
            {
                Console.WriteLine("ERROR: Failed to export, " + Exc.Message);
                return;
            }

            Console.WriteLine("Generated " + OutputFile);
        }

        /// <summary>
        /// Parses the input type
        /// </summary>
        /// <param name="Text">Text to parse</param>
        /// <returns>Parsed input type</returns>
        private static InputTypes ParseInputType
            (
            string Text
            )
        {
            Text = Text.Trim().ToLower();

            if (Text == "multiplane") return InputTypes.Multiplane;

            throw new Exception(string.Format("Unknown input type '{0}'", Text));
        }

        /// <summary>
        /// Parses the output type
        /// </summary>
        /// <param name="Text">Text to parse</param>
        /// <returns>Parsed output type</returns>
        private static OutputTypes ParseOutputType
            (
            string Text
            )
        {
            Text = Text.Trim().ToLower();

            if (Text == "obj") return OutputTypes.Obj;
            else if (Text == "xyz") return OutputTypes.Xyz;

            throw new Exception(string.Format("Unknown output type '{0}'", Text));
        }

        /// <summary>
        /// Parses a string containing a relative or absolute filename
        /// </summary>
        /// <param name="FileName">Relative or absolute filename to parse</param>
        /// <returns>Absolute filename</returns>
        private static string ParseFileName
            (
            string FileName
            )
        {
            return GetAbsolutePath(FileName, Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Gets the absolute path to a file, which may be relative or absolute
        /// </summary>
        /// <param name="FileName">Relative or absolute path to a file, and file name</param>
        /// <param name="BaseFileName">If relative then this folder or file is the base location</param>
        /// <returns>Absolute file path and name</returns>
        public static string GetAbsolutePath
            (
            string FileName,
            string BaseFileName
            )
        {
            // if no folder specified then assume folder of base file name
            string Folder = Path.GetDirectoryName(FileName);
            if (Folder.Length == 0)
            {
                FileName = Path.GetDirectoryName(BaseFileName) + Path.DirectorySeparatorChar + FileName;
            }
            // if relative path specified then assume relative to folder of executable
            else if (!Path.IsPathRooted(FileName))
            {
                Uri FileUri = new Uri(FileName, UriKind.Relative);
                Uri FileAbs = new Uri(new Uri(Path.GetDirectoryName(BaseFileName) + Path.DirectorySeparatorChar), FileUri);
                FileName = FileAbs.LocalPath;
            }

            return FileName;
        }

        // gets the complete version string
        public static String GetVersion
            (
            )
        {
            string Version = String.Format("{0}.{1}",
                Assembly.GetExecutingAssembly().GetName().Version.Major.ToString(),
                Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("D2"));

            return Version;
        }

        // gets the complete copyright string
        public static String GetCopyright
            (
            )
        {
            // get all copyright attributes on this assembly
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

            // if there aren't any copyright attributes, return an empty string
            if (attributes.Length == 0) return "";

            // if there is a copyright attribute, return its value
            return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
        }

        // gets the product name
        public static String GetProductName
            (
            )
        {
            // get all product name attributes on this assembly
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);

            // if there aren't any product name attributes, return <unknown>
            if (attributes.Length == 0) return "<unknown>";

            // if there is a product name attribute, return its value
            return ((AssemblyProductAttribute)attributes[0]).Product;
        }
    }
}
