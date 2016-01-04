using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramScoreGenerator
{
    public class Data
    {
        public int Q_ID;
        public int Q_length;
        //TODO: Remove Query Text
        //public string Q_text;
        public Data ()
        {
            Q_ID = 0;
            Q_length = 0;
            //Q_text = string.Empty;
        }
        public Data (int id,int length)
        {
            Q_ID = id;
            Q_length = length;
            //Q_text = text;
        }
        public Data (Data copyin)
        {
            Q_ID = copyin.Q_ID;
            Q_length = copyin.Q_length;
        }
        public int compare(Data rhs)
        {
            if (this.Q_ID != rhs.Q_ID) return 0;
            if (this.Q_length != rhs.Q_length) return 0;
            return 1;
        }
        public static List<Data> intersect (List<Data> L1, List<Data> L2)
        {
            List<Data> new_list = new List<Data>();
            int i =0, j = 0;
            while(i<L1.Count && j < L2.Count)
            {
                if (L1[i].Q_ID == L2[j].Q_ID)
                {
                    new_list.Add(L1[i]);
                    ++i;
                    ++j;
                }
                else if (L1[i].Q_ID < L2[j].Q_ID)
                    ++i;
                else
                    ++j;
            }
            return new_list;
        }

        public static void print_list (List<Data> L1)
        {
            foreach (var item in L1)
            {
                Console.WriteLine(item.Q_ID);
            }
        }
    }


}
