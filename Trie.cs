using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramScoreGenerator
{
    class Trie
    {
        public TrieNode root;
        public int TotalNodes;
        public Trie ()
        {
            root = new TrieNode();
            TotalNodes = 1;
            root.frequency = 100;
        }
        public bool wordFound(string word)
        {
            TrieNode p = this.root;
            int i = 0;
            char c;
            while (i < word.Length)
			{
                if(p!=null)
                {
                    c = word[i];
                    if (i == word.Length - 1)
                    {
                        if (p.letter == c && p.endOfWord)
                            return true;
                        else if ((p.letter != c && p.sibling == null) || (p.letter == c && !p.endOfWord))
                            return false;
                        else
                            p = p.sibling;
                    }

                    if (p.letter == c && p.daughter != null)
                    {
                        p = p.daughter;
                        ++i;
                        continue;
                    }
                    else if (p.letter != c && p.sibling != null)
                    {
                        p = p.sibling;
                        continue;
                    }
                    else
                        return false;
                }
			}
            return false;
        }

        public void insert (string word, Data new_data, int num_words) //num_words tells whether it is trigram or bigram. Remember each word in a bigram must be separated from other by " "
        {
            TrieNode p = this.root;
            int i = 0;
            while (i < word.Length -1)
            {
                if (p == null)
                    break;
                char test = word[i];
                char test2 = word[i + 1];
                char pLetter = p.letter;
                if ( p.letter == word[i]) 
                {
                    if (p.daughter != null)
                        p = p.daughter;
                    else
                    {
                        p.daughter = new TrieNode(word[i + 1]);
                        p = p.daughter;
                        this.TotalNodes++;
                    }
                    //go to next letter in the word   
                    ++i;
                }
                else
                {
                    if (p.sibling != null)
                        p = p.sibling;
                    else
                    {
                        p.sibling = new TrieNode(word[i]);
                        p = p.sibling;
                        this.TotalNodes++;
                    }
                }
            }

            //handling the last character as a special case
            while (i < word.Length)
            {
                if (p == null)
                    break;
                if (p.letter == word[i])
                    ++i;
                else if (p.letter != word[i] && p.sibling != null)
                {
                    p = p.sibling;
                    continue;
                }
                else if (p.letter != word[i]) // and we have no sibling
                {
                    p.sibling = new TrieNode(word[i]);
                    this.TotalNodes++;
                    p = p.sibling;
                }
                
                p.endOfWord = true;
                p.frequency++;
                p.NumOfWords = num_words;
                if (num_words == 1)
                {
                    p.Query_list.Add(new_data);
                }
            }
        }
        public void printTrie ()
        {
            using (StreamWriter outfile = new StreamWriter("F:\\Scripts\\Victor\\Trie.txt"))
            {
                Stack<char> s = new Stack<char>();
                printTrie(this.root, s, outfile);
            }
            
        }
        public void printTrie (TrieNode p, Stack<char> s, StreamWriter outfile)
        {
            if(p!=null)
            {
                s.Push(p.letter);
                if(p.endOfWord)
                {
                    string st = string.Join("", s.ToArray().Reverse());
                    if (p.Hoefding_score != 0)
                        outfile.Write(st + " | " + p.Hoefding_score);
                    else
                        outfile.Write(st + " | " + p.frequency);
                }
                if (p.daughter == null)
                {
                    outfile.WriteLine();
                }
                else
                    printTrie(p.daughter, s,outfile);

                s.Pop();
                printTrie(p.sibling, s, outfile);
                return;
            }
        }
        public int factorial(int n , int t)
        {
            if (n < 0 || t < 0)
                throw new Exception("Incorrect parameters for factorial");
            int prod=1;
            for (int i = n;i>n-t+1;i--)
            {
                prod*=i;
            }
            return prod;
        }
        public TrieNode FindWord(string word)
        {
            TrieNode p = this.root;
            int i = 0;
            char c;
            while (i < word.Length)
            {
                if (p != null)
                {
                    c = word[i];
                    if (i == word.Length - 1)
                    {
                        if (p.letter == c && p.endOfWord)
                            return p;
                        else if ((p.letter != c && p.sibling == null) || (p.letter == c && !p.endOfWord))
                            return null;
                        else
                            p = p.sibling;
                    }

                    if (p.letter == c && p.daughter != null)
                    {
                        p = p.daughter;
                        ++i;
                        continue;
                    }
                    else if (p.letter != c && p.sibling != null)
                    {
                        p = p.sibling;
                        continue;
                    }
                    else
                        return null;
                }
            }
            return null;
        }
        public void FindExpectation(string outfilePath)
        {
            using (StreamWriter outfile = new StreamWriter(outfilePath))
            {
                Stack<char> stack_list = new Stack<char>();
                Expectation(this.root, stack_list, outfile);
            }
            
        }
        public void Expectation (TrieNode p, Stack<char> stack_list, StreamWriter outfile)
        {
            if (p != null)
            {
                stack_list.Push(p.letter);
                List<Data> data_list = new List<Data>();
                if((p.endOfWord) && (p.NumOfWords > 1) && (p.frequency >= 20)) //unigrams need not be considered: see paper for reason and we want bi/trigram to occur atleast 10 times to be considered
                {
                    TrieNode new_p;
                    string temp = string.Join("", stack_list.ToArray().Reverse());
                    temp = temp.Trim();
                    string[] words = temp.Split(new string[]{" "},StringSplitOptions.RemoveEmptyEntries);
                    string word;
                    ///**************************************************** Finding the intersecting LISTS ***************/
                    for (int i = 0; i < words.Length; i++)
                    {
                        word = " " + words[i];//root has " " to begin with
                        new_p = this.FindWord(word);
                        if (new_p == null)
                            continue;// we coudl have removed that word during pruning
                            //throw new Exception("You are trying to get expectation value for a words whose unigram is not present");
                        if (i == 0)
                            data_list = new_p.Query_list;
                        else if (data_list != null)
                            data_list = Data.intersect(data_list, new_p.Query_list); // this gives the number of queies which contain all the 3 words
                        else
                            throw new Exception("Expectation calculation:Unigram is stored without a query id");
                    }
                    p.Set_frequency = data_list.Count;
                    /* *************************************************** Finding the Hoefding's ************** */
                    p.Set_expectation = 0;
                    for (int i = 0; i < data_list.Count; i++)
                    {
                        int number = this.factorial(data_list[i].Q_length, p.NumOfWords);
                        float num_inverse = 0;
                        if (number < 10000000 && number != 0)
                        {
                            num_inverse = 1 / (( float )number);
                            p.Set_expectation += num_inverse;
                        }
                    }

                    /*
                        Hoefding's inequality: F-E > sqrt(-K*log(s)/2)
                        let hoefding'score -log(s)=((F-E)^2)*2/k;
                        */
                    if (p.frequency > p.Set_expectation)
                    {
                        p.Hoefding_score = (((p.frequency - p.Set_expectation) * (p.frequency - p.Set_expectation)) * 2) / 2 * p.Set_frequency;
                        outfile.WriteLine(p.Hoefding_score+"\t"+ temp);
                    }

                }
                string test = String.Join("", stack_list.ToArray().Reverse());
                Expectation(p.daughter, stack_list, outfile);
                stack_list.Pop();
                Expectation(p.sibling, stack_list, outfile);
                
            }
            
            return;
        }
        // The pruning is always done at a particular level ie. while doing it for trigrams we dont consider pruning for bigrams and unigrams, considering that they have already been pruned.
        public void prune_trie(int threshold)
        {
            prune(this.root, threshold);
        }
        public void prune(TrieNode p , int threshold)
        {
            if(p==null)
                return;
            if(p.daughter !=null)
            {
                prune(p.daughter, threshold);
                if(p.daughter.to_be_deleted == true)
                {
                    TrieNode temp = p.daughter;
                    p.daughter = temp.sibling;
                    --TotalNodes;
                }
            }
            if(p.sibling !=null)
            {
                prune(p.sibling, threshold);
                if(p.sibling.to_be_deleted==true)
                {
                    TrieNode temp = p.sibling;
                    p.sibling = temp.sibling;
                    --TotalNodes;
                }
            }
            //TODO:See why old code has l.push_front(p->letters);
            if ((p.endOfWord == true && p.frequency < threshold)|| (!p.endOfWord))
            {
                if (p.daughter == null)
                    p.to_be_deleted = true;//if it is nor the end of any word and has no daughters delete it. Also it is less than threshold delete it
                else if(p.endOfWord) // if it has daughters but is also the end of a word that does not meet the threshold , just clear the query list
                {
                    p.Query_list.Clear();
                    p.Query_list.TrimExcess();
                    p.endOfWord = false;
                }
            }

        }

    }
}
