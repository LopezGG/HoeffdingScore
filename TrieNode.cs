using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramScoreGenerator
{
    class TrieNode
    {
        public bool endOfWord;  //endofword=true only entry by entry
        public char letter;
        public int frequency; // Frequency of the words occuring toegether in the query
        public int NumOfWords;
        public List<Data> Query_list;// The list of queries in which the word occcurs
        public TrieNode daughter ;
        public TrieNode sibling ;
        //Obtained in 2nd run; While parsing the TRIE
        public float Set_expectation = 0;// Expectated value of words(seperated by \w) occuring(may not be together)
        public int Set_frequency = 0;// Number of queries in which the words(seperated by \w) occur 
        public float Hoefding_score = 0;
        //public bool is_significant_gram;
        public bool to_be_deleted;

        public TrieNode ()
        {
            letter = ' ';
            endOfWord = false;
            frequency = 0;
            NumOfWords = 0;
            to_be_deleted = false;
            Query_list = new List<Data>();
        }

        public TrieNode (char ch)
        {
            letter = ch;
            NumOfWords = 0;
            endOfWord = false;
            to_be_deleted = false;
            Query_list = new List<Data>();
        }
    }
}
