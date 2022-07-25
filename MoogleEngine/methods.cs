namespace MoogleEngine;

//clase de metodos (funciones)
class methods{

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
        for(int i=0;i<x.words.Count();i++){
            if(!sol.ContainsKey(x.words[i]) && x.freq.ContainsKey(x.words[i])){
               double point = (double)((double)(x.freq[x.words[i]])/(double)(global2[x.words[i]])) * (double)(Math.Log((double)(n)/(double)(global[x.words[i]])));
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
    
    //snipet : tomemos un pedazo del texto en donde se encuentre la palabra de mas
    // relevancia
    public static string snipet(string txt, string word){
        string ans = "";

        for(int i=0;i<txt.Length;i++){
            
            string aux = "";
            while(i<txt.Length){
                 if(txt[i] == 'á')
                aux += 'a';
                else if(txt[i] == 'ú')
                aux += 'u';
                else if(txt[i] == 'ó')
                aux += 'o';
                else if(txt[i] == 'í')
                aux += 'i';
                else if(txt[i] == 'é')
                aux += 'e';
                else
                aux += txt[i];
                if('a' <= aux[aux.Length-1] && aux[aux.Length-1] <= 'z')
                i++;
                else
                {
                    aux = aux.Substring(0,aux.Length-1);
                    break;
                }
            }
            aux = aux.ToLower();
            if(aux == word){
                int pos = i - word.Length -50;
                pos = Math.Max(0,pos);
                int cant = 500;
                if(pos + cant >= txt.Length)
                cant = txt.Length-pos;
                while(txt[pos] != ' ')
                pos++;
                while(txt[pos+cant-1] != ' ')
                cant--;
                ans = txt.Substring(pos,cant);
            }
        }
        
        return ans;
    }
}
