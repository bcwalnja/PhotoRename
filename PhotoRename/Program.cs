namespace PhotoRename
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //welcome the user to the Rename Program
            Console.WriteLine("Welcome to the Rename Program");
            //request folder directory
            Console.WriteLine("Please enter the folder directory");
            //store folder directory
            string folderDirectory = Console.ReadLine() ?? string.Empty;
            //verify directory is valid
            while (!Directory.Exists(folderDirectory))
            {
                //request folder directory
                Console.WriteLine("Please enter the folder directory");
                //store folder directory
                folderDirectory = Console.ReadLine() ?? string.Empty;
            }

            //if the file count in the directory is 0, exit the program
            if (Directory.GetFiles(folderDirectory).Length == 0)
            {
                Console.WriteLine("There are no files in the directory. Press enter to end the program.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Directory is Valid. Proceed? (y/n)");
            if (Console.ReadLine()?.ToLower() != "y")
            {
                return;
            }

            //get the first file in the directory that is a .jpg file
            string[] files = Directory.GetFiles(folderDirectory, "*.jpg");

            //create a string array to hold the new file names
            var newFileNames = new string[files.Length];
            //loop through the files and create the new file names based on the date created "YYYY-MM-dd HH.mm.ss"
            for (int i = 0; i < files.Length; i++)
            {
                //set dateCreated to the file creation time
                var file = files[i];
                var dateCreated = GetDate(file);

                //if dateCreated == default, set newFileNames[i] to the file name
                if (dateCreated == default)
                {
                    newFileNames[i] = Path.GetFileName(file);
                    continue;
                }

                var extension = Path.GetExtension(file);

                newFileNames[i] = dateCreated.ToString("yyyy-MM-dd-HH-mm-ss") + extension;
            }

            //for formatting, find the old filename that is the longest
            var longestFileName = files.Max(x => Path.GetFileName(x).Length);

            //show the filenames to the user
            Console.WriteLine("The following files will be renamed:");
            for (int i = 0; i < files.Length; i++)
            {
                //get the file name without the directory
                var fileName = Path.GetFileName(files[i]);
                fileName = fileName.PadRight(longestFileName);
                Console.WriteLine($"{fileName} will be renamed to {newFileNames[i]}");
            }

            //ask the user if they want to proceed
            Console.WriteLine("Proceed? (y/n)");
            if (Console.ReadLine()?.ToLower() != "y")
            {
                Console.WriteLine("No files were renamed. Exiting.");
                return;
            }

            //make sure the previous access to the files is closed before renaming them
            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            //loop through the files and rename them
            for (int i = 0; i < files.Length; i++)
            {
                File.Move(files[i], Path.Combine(folderDirectory, newFileNames[i]));
            }
        }

        private static DateTime GetDate(string file)
        {
            DateTime dateCreated = default;

            if (Path.GetExtension(file)?.Contains(".jpg") == true )
            {
                var reader = new ExifLib.ExifReader(file);
                if (reader.GetTagValue(ExifLib.ExifTags.DateTimeOriginal, out DateTime dateTaken))
                {
                    dateCreated = dateTaken;
                }
            }
            if (dateCreated == default)
            {
                dateCreated = File.GetCreationTime(file);
            }
            if (dateCreated == default)
            {
                dateCreated = File.GetLastWriteTime(file);
            }

            return dateCreated;
        }
    }
}