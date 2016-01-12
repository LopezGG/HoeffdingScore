using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramScoreGenerator
{
    class Program
    {
        static void Main (string[] args)
        {
            string inputFilename = args[0];
            string outfile = args[1];
            int maxGramSize = 4;

            int Qid = 1;

            Trie Trie_tree = new Trie();
            using (StreamReader Sr = new StreamReader(inputFilename))
            {
                string line;
                while((line=Sr.ReadLine())!=null)
                {
                    if (String.IsNullOrWhiteSpace(line))
                        continue;
                    string[] words = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (Qid == 2100000)
                        break;
                    Data Q_data = new Data(Qid++, words.Length);
                    for (int i = 0; i < words.Length; i++)
                    {
                        string result = String.Empty;
                        for (int gramsize = 0; gramsize < maxGramSize; gramsize++)
                        {
                            if (i + gramsize >= words.Length)
                                break;
                            result = result + " " + words[i + gramsize];
                            Trie_tree.insert(result, Q_data, gramsize + 1);

                        }
                    } 
                }
            }
            // The trie is constructed level-wise. 
            // ie first all the unigrams in the query list are inserted.Then it is pruned. then all the bigrams in the query list are inserted, again it is pruned and so on.
            // itr keeps track of the level. It is inititalized to level 1(unigrams).
            
            int threshold = 15, threshold_dec_Step = 5;
            for (int gramsize = 0; gramsize < maxGramSize; gramsize++)
            {
                Trie_tree.prune_trie(threshold);
                threshold -= threshold_dec_Step;
            }
            Trie_tree.printTrie();
            Trie_tree.FindExpectation(outfile);
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
