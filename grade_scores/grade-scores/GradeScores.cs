using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace grade_scores
{
    public class GradeTextFile
    {
        public void ReadRecordsFromTextFile(string _fileName)
        {
            try
            {
                using (StreamReader reader = File.OpenText(_fileName))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if(string.Compare(Regex.Replace(line, @"\s+", ""), string.Empty) != 0) // Skips any empty strings found
                        {
                            raw_records_list.Add(line);
                        }
                    }
                }
            }
            catch(IOException ex)
            {
                throw ex;
            }
        }

        public void VerifyAndSortRecords()
        {
            string rec_pattern = "^[A-Za-z]+,[A-Za-z]+,[0-9]{1,3}$";

            foreach (string line in raw_records_list)
            {
                try
                {
                    Match _match1;
                    _match1 = Regex.Match(line, rec_pattern,
                                    RegexOptions.IgnoreCase | RegexOptions.Compiled,
                                    TimeSpan.FromSeconds(1));
                    if (!_match1.Success)
                    {
                        throw new Exception("Invalid record found");
                    }
                }
                catch (RegexMatchTimeoutException ex)
                {
                    throw new Exception("Invalid record found" + ex.Message);
                }

                string[] buff = line.Split(',');
                if (buff.Length != 3)
                {
                    throw new Exception("Invalid record found");
                }
                try
                {
                    sorted_records_list.Add(new KeyValuePair<KeyValuePair<string, string>, int>(new KeyValuePair<string, string>(buff[0], buff[1]), int.Parse(buff[2])));
                }
                catch(Exception)
                {
                    throw new Exception("Invalid record found");
                }                
            }

            SortRecords();
        }

        public void WriteRecordsToTextFile(string _fileName)
        {
            try
            {
                using (StreamWriter writer = File.CreateText(_fileName))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (KeyValuePair<KeyValuePair<string, string>, int> s_line in sorted_records_list)
                    {
                        sb.AppendFormat("{0},{1},{2}\n", s_line.Key.Key, s_line.Key.Value, s_line.Value);
                        writer.WriteLine(sb.ToString());
                        sb.Clear();
                    }
                    writer.Flush();
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        public void WriteRecordsToConsole()
        {
            
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<KeyValuePair<string, string>, int> s_line in sorted_records_list)
            {
                sb.AppendFormat("{0},{1},{2}\n", s_line.Key.Key, s_line.Key.Value, s_line.Value);
                Console.Out.WriteLine(sb.ToString());
                sb.Clear();
            }       
        }

        public int RecordsCount
        {
            get { return raw_records_list.Count; }
        } 

        private void SortRecords()
        {
            sorted_records_list.Sort(
                delegate (KeyValuePair<KeyValuePair<string, string>, int> pair1,
                          KeyValuePair<KeyValuePair<string, string>, int> pair2)
                {
                    int comp = pair2.Value.CompareTo(pair1.Value);
                    if (0 == comp)
                    {
                        int str_cmp = pair2.Key.Value.CompareTo(pair1.Key.Value);
                        if (0 == str_cmp)
                        {
                            return pair2.Key.Key.CompareTo(pair1.Key.Key);
                        }
                        else
                        {
                            return str_cmp;
                        }
                    }
                    else
                    {
                        return comp;
                    }
                }
            );
        }

        private List<string> raw_records_list = new List<string>();
        private List<KeyValuePair<KeyValuePair<string, string>, int>> sorted_records_list = new List<KeyValuePair<KeyValuePair<string, string>, int>>();

    }

    class GradeScores
    {
        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.Out.WriteLine("usage: grade-scores [input_file]");
                return;
            }

            Console.Out.WriteLine("grade-scores {0}", args[0]);

            GradeTextFile gtf = new GradeTextFile();
            try
            {
                gtf.ReadRecordsFromTextFile(args[0]);
            }
            catch(IOException ex)
            {
                Console.Out.WriteLine("Error reading file" + ex.Message);
            }

            gtf.VerifyAndSortRecords();
            string[] fn = args[0].Split('.');
            StringBuilder sb = new StringBuilder();
            string out_file_name = (sb.AppendFormat("{0}-graded.{1}", fn[0], fn[1])).ToString();
            gtf.WriteRecordsToTextFile(out_file_name);
            gtf.WriteRecordsToConsole();
            Console.Out.WriteLine("Finished: created {0}", out_file_name);
        }
    }
}
