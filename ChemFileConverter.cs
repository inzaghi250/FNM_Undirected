using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FNM_Undirected
{
    class ChemFileConverter
    {
        public static void Convert(TextReader sr, StreamWriter sw, int offSet)
        {
            string line = "";
            while (line.Length == 0)
                line = sr.ReadLine();
            string[] parts = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            int n = int.Parse(parts[0]),
                m = int.Parse(parts[1]);
            for (int i = 0; i < n; i++)
            {
                parts = sr.ReadLine().Split(" ".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                switch (parts[3])
                {
                    case "H":
                        sw.WriteLine(i + offSet + "\tisa\t0");
                        break;
                    case "O":
                        sw.WriteLine(i + offSet + "\tisa\t1");
                        break;
                    case "N":
                        sw.WriteLine(i + offSet + "\tisa\t2");
                        break;
                }
            }
            while (m-- > 0)
            {
                parts = sr.ReadLine().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int v1 = int.Parse(parts[0]),
                    v2 = int.Parse(parts[1]);
                sw.WriteLine((v1 - 1 + offSet) + "\t" + (v2 - 1 + offSet) + "\t" + parts[2]);
            }
        }
    }
}
