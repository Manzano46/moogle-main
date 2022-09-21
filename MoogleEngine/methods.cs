namespace MoogleEngine;

//clase de metodos (funciones)
public class methods{

    //read : toma un fichero y devuelve un string con el contenido del archivo
    public static string read(string path){
        return File.ReadAllText(path,System.Text.Encoding.UTF8);
    }

    // titles : en la direccion dada devuelve los nombres de los ficheros
    public static List<string> titles(){
        string [] aux = Directory.GetFiles("../Content","*.*",SearchOption.AllDirectories);
        List<string> titles = new List<string>();
        for(int i=0;i<aux.Count();i++)
        titles.Add(aux[i]);
        return titles;
    }

    // WORDS : recibe como entrada un string lo convierte a letras minusculas y remueve todas las ocurrencias
    // de caracteres 'basura', ademas quita las tildes

    public static List<string> WORDS(string s){
        List<string> words = new List<string>();
        char [] trash = {',','.',';',':','\n',' ','<','>','(',')','}','{','!','?','$','#','\t'};
        s = s.ToLower();
        string [] news = s.Split(trash,StringSplitOptions.RemoveEmptyEntries);

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
            if(aux == "")
            continue;
            words.Add(aux);
        }

        return words;
    }

    //recibe un vector con palabras ya divididas, nuestro diccionario global
    // (para saber en la cantidad de documentos donde aparece una palabra) y
    // nuestra variable MAX para saver la cantidad de veces que aparece la mas
    // frecuente
    public static Dictionary<string,int> times(List<string> words, ref Dictionary<string,int> global,ref int MAX, ref Dictionary<string,int> global2){
        Dictionary<string,int> fre = new Dictionary<string, int>();
        MAX = 1;
        foreach(string s in words){
            if(global2.ContainsKey(s))
            global2[s]++;
            else
            global2.Add(s,1);
            if(global.ContainsKey(s) && !fre.ContainsKey(s))
            global[s]++;
            if(!global.ContainsKey(s))
            global.Add(s,1);
            if(fre.ContainsKey(s))
            {
                fre[s]++;
                if(MAX<fre[s])
                MAX = fre[s];
            }
            else
            fre.Add(s,1);
        }
        return fre;
    }

    //computar las posiciones de las palabras en el texto
    public static Dictionary<string,List<int>> dist(List<string> x){
        Dictionary<string,List<int>> sol = new Dictionary<string, List<int>>();
        for(int i=0;i<x.Count();i++){
            string wd = x[i];

            if(sol.ContainsKey(wd)){
                sol[wd].Add(i); 
            }
            else{
                List<int> a = new List<int>();
                a.Add(i);
                sol.Add(wd,a);
            }
                
        }

        return sol;
    }

    // edit_distance : con ayuda de una dp en dos dimensiones vamos a computar
    // la distancia(cantidad de cambios minimos) de una palabra a otra
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

    // findtf_idf : utilizando un poco de algebra le daremos d forma logica una 
    // puntuacion razonable a cada palabra, para tener una prioridad entre ellas

    public static Dictionary<string,double> findtf_idf(vector x,Dictionary<string,int> global,int n,Dictionary<string,int> global2){
        Dictionary<string,double> sol = new Dictionary<string, double>();
        n++;
        for(int i=0;i<x.words.Count();i++){
            if(!sol.ContainsKey(x.words[i]) && x.freq.ContainsKey(x.words[i])){
               
                double tf = (double)((double)(x.freq[x.words[i]])/(double)(global2[x.words[i]]));
                double X = (double)(n)/(double)(global[x.words[i]]);
                double idf = (double)(Math.Log2(X));
               double point = tf * idf;
                sol.Add(x.words[i],point);      
             }
        }
        return sol;
    }

    // dot : para calcular el producto dot (sumatoria de la multiplicacion de las
    // palabras que aparezcan en ambas strings)
    public static double dot(Dictionary<string,double> a,Dictionary<string,double> b){
        double ans = 0.0f;
        foreach(var pair in a){  
            if(b.ContainsKey(pair.Key)){
                 ans += (double)(pair.Value * b[pair.Key]);
            }
        }
        return ans;
    }
    
    // snipet : tomemos un pedazo del texto en donde se encuentre la palabra de mas
    // relevancia
    public static string snipet(vector x, string word){
        string ans = "";
        
        if(word.Length <= 2 || x.freq.ContainsKey(word) == false)
        return "";
        
        for(int i=0;i<Math.Min(4,x.dist[word].Count());i++){
            string t = "...";

            int pos = x.dist[word][i];
            for(int j = Math.Max(0,pos-20);j<Math.Min(pos+20,x.words.Count());j++){
                t += " " + x.words[j];
            }
            t += "...";

            ans += t;
        }
        
 
        return ans;
    }

    // metodo para tomar los documentos de mas relevancia y encontrar los snipet ademas de trabajar con los 
    // comparadores
    public static void findsnipet(ref List<string> s, ref List<int> s2, string best,string best2, ref List<vector> data,List<string> si,List<string> no){
        for (int i = 0; i < 10; i++)
        {
            string x = methods.snipet(data[i], best);
            
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }
            
            x = methods.snipet(data[i], best + 's');
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }
            x = methods.snipet(data[i], best + "es");
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }
            x = methods.snipet(data[i], best.Substring(0,Math.Max(0,best.Length-1)));
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }
            x = methods.snipet(data[i], best.Substring(0,Math.Max(0,best.Length-2)));
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }

            x = methods.snipet(data[i], best2);
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }
            x = methods.snipet(data[i], best2 + 's');
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }
            x = methods.snipet(data[i], best2 + "es");
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }
            x = methods.snipet(data[i], best2.Substring(0,Math.Max(0,best2.Length-1)));
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }
            x = methods.snipet(data[i], best2.Substring(0,Math.Max(0,best2.Length-2)));
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }
            
            while(s2.Count >=2 && s2[s2.Count() - 1] == s2[s2.Count() - 2]){
                s2.RemoveAt(s2.Count()-1);
                s[s.Count()-2] += " .......... " + s[s.Count()-1];
                s.RemoveAt(s.Count()-1);
            }
        }
        for(int i=0;i<no.Count();i++){
            string wd = no[i];
            for(int j=0;j<s.Count();j++){
                if(data[s2[j]].freq.ContainsKey(wd) && data[s2[j]].freq[wd] > 0){
                    s.RemoveAt(j);
                    s2.RemoveAt(j);
                    j--;
                }
            }
        }
        for(int i=0;i<si.Count();i++){
            string wd = si[i];
            for(int j=0;j<s.Count();j++){
                if(!data[s2[j]].freq.ContainsKey(wd)){
                    s.RemoveAt(j);
                    s2.RemoveAt(j);
                    j--;
                }
            }
        }
    }
    // operador de importancia 
    public static void sum(ref Dictionary<string,double> a,List<string> imp){
        double sum = 0.0f;
        foreach(var pair in a)
        sum += pair.Value*pair.Value*10;
        
        for(int i=0;i<imp.Count();i++){
            a[imp[i]] += sum;
        }
    }

    public static int caldist(string L,string R,vector x){
        int sol = int.MaxValue;
        if(x.freq.ContainsKey(L) == false || x.freq.ContainsKey(R) == false)
        return sol;
        
        List<int> l = x.dist[L];
        List<int> r = x.dist[R];

        int i=0,j=0;

        while(i < l.Count() &&  j < r.Count()){
           sol = Math.Min(sol, Math.Abs(l[i] - r[j]));
           if(l[i] < r[j])
           i++;
           else
           j++;
        }

        while(i == l.Count() && j != r.Count()){
            sol = Math.Min(sol, Math.Abs(l[i-1] - r[j++]));
        }

        while(i != l.Count() && j == r.Count()){
            sol = Math.Min(sol, Math.Abs(l[i++] - r[j-1]));
        }

        return sol;
    }
}
