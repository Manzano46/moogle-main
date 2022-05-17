namespace MoogleEngine;
class methods{
    public static string read(string path){
        return File.ReadAllText(path,System.Text.Encoding.UTF8);
    }

    public static List<string> titles(){
        string [] aux = Directory.GetFiles("../Content","*.*",SearchOption.AllDirectories);
        List<string> titles = new List<string>();
        for(int i=0;i<aux.Count();i++)
        titles.Add(aux[i]);
        return titles;
    }

    public static List<string> WORDS(string s){
        List<string> words = new List<string>();
        char [] trash = {',','.',';',':','\n',' ','<','>','(',')','}','{','!','?','$','#','\t'};
        s = s.ToLower();
        string [] news = s.Split(trash);

        for(int i=0;i<news.Count();i++){
            
            string aux = "";
            string aux2 = news[i];
            for(int j=0;j<aux2.Length;j++){
                if(aux2[j] == 'á')
                aux += 'a';
                else if(aux2[j] == 'ú')
                aux += 'u';
                else if(aux2[j] == 'ó')
                aux += 'o';
                else if(aux2[j] == 'í')
                aux += 'i';
                else if(aux2[j] == 'é')
                aux += 'e';
                else if(aux2[j] >= 'a' && aux2[j] <= 'z')
                aux += aux2[j];
                else if(aux2[j] >= '0' && aux2[j] <= '9')
                aux += aux2[j];
            }
            words.Add(aux);
        }

        return words;
    }

    public static Dictionary<string,int> times(List<string> words, ref Dictionary<string,int> global){
        Dictionary<string,int> fre = new Dictionary<string, int>();
        foreach(string s in words){
            if(global.ContainsKey(s) && !fre.ContainsKey(s))
            global[s]++;
            if(!global.ContainsKey(s))
            global.Add(s,1);
            if(fre.ContainsKey(s))
            fre[s]++;
            else
            fre.Add(s,1);
        }
        return fre;
    }

    public static int edit_distance(string a,string b){
        int n = a.Length, m = b.Length;
        int[,] dp = new int [n+1,m+1];
        for(int i=0;i<=n;i++){
            for(int j=0;j<=m;j++){
                if(i == 0)
                dp[i,j] = j;
                else if(j == 0)
                dp[i,j] = i;
                else if(a[i-1] == b[j-1])
                dp[i,j] = dp[i-1,j-1];
                else
                dp[i,j] = Math.Min(dp[i,j-1],Math.Min(dp[i-1,j-1],dp[i-1,j])) + 1;
            }
        }
    return dp[n,m];
    }

    public static Dictionary<string,double> findtf_idf(vector x,Dictionary<string,int> global){
        Dictionary<string,double> sol = new Dictionary<string, double>();
        for(int i=0;i<x.words.Count();i++){
            if(!sol.ContainsKey(x.words[i]) && x.freq.ContainsKey(x.words[i])){
                double point = (double)(x.freq[x.words[i]]) * (double)(Math.Log2(global.Count()/global[x.words[i]]));
                sol.Add(x.words[i],point);
             }
        }
        return sol;
    }
}