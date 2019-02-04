using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Subtitle_Word_Extractor
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("In the next step, please select the file you want to load.\nPress Enter to continue...");
            Console.ReadLine();
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() != DialogResult.OK || !File.Exists(open.FileName))
            {
                Console.WriteLine("No file has been selected. Please restart and select a valid file.");
                Console.ReadLine();
                return;
            }
            string content = Regex.Replace(File.ReadAllText(open.FileName), @"<.*?>", string.Empty); //load and remove all html tags
            var allWords = new List<string>();

            Match res = Regex.Match(content, @"[A-Za-z]+'*[A-Za-z]*"); //match all words including those with ' in like don't
            if(!res.Success)
            {
                Console.WriteLine("There was no match");
                Console.ReadLine();
                return;
            }

            while(res.Success)
            {
                string alreadyThere = allWords.Find(word => res.Value.ToLower().Equals(word.ToLower()));
                if(alreadyThere == null)
                    allWords.Add(res.Value);
                res = res.NextMatch();
            }
            Console.WriteLine(allWords.Count + " words have been found. Do you want to sort them? [Y]");
            string input = Console.ReadLine();
            if(input.Length < 1) input = "y";
            bool sort = input.ToLower()[0] != 'n';

            if (sort)
            {
                Console.WriteLine("Sorting elements...");
                allWords.Sort();
                Console.WriteLine("Sorting done.");
            }

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Text File|*.txt";
            if(save.ShowDialog() != DialogResult.OK || !CanWrite(save.FileName))
            {
                Console.WriteLine("Invalid directory");
                allWords.Clear();
                Console.ReadLine();
                return;
            }

            FileStream fs = new FileStream(save.FileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            for(int i = 0; i < allWords.Count; i++)
                sw.WriteLine(allWords[i]);
            sw.Close();
            fs.Close();

            Console.WriteLine("File has been written.");
            Console.ReadLine();

        }

        static bool CanWrite(string file)
        {
            try
            {
                DirectorySecurity ds = Directory.GetAccessControl(Path.GetDirectoryName(file));
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
