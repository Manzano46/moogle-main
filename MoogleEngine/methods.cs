namespace MoogleEngine;

//clase de metodos (funciones)
public class methods{

    //read : toma un fichero y devuelve un string con el contenido del archivo
    public static string Read_Text(string path){
        return File.ReadAllText(path,System.Text.Encoding.UTF8);
    }

    // titles : en la direccion dada devuelve los nombres de los ficheros
    public static List<string> Read_Titles(){
        string [] aux = Directory.GetFiles("../Content","*.*",SearchOption.AllDirectories);
        List<string> titles = new List<string>();
        for(int i=0;i<aux.Count();i++)
        titles.Add(aux[i]);
        return titles;
    }

    // WORDS : recibe como entrada un string lo convierte a letras minusculas y remueve todas las ocurrencias
    // de caracteres 'basura', ademas quita las tildes

    public static List<string> Get_Words(string s){
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
    public static Dictionary<string,int> Fill_Frec(List<string> words, ref Dictionary<string,int> Documents, ref Dictionary<string,int> Frec_total){
        Dictionary<string,int> fre = new Dictionary<string, int>();
        foreach(string s in words){
            if(Frec_total.ContainsKey(s))
            Frec_total[s]++;
            else
            Frec_total.Add(s,1);
            if(Documents.ContainsKey(s) && !fre.ContainsKey(s))
            Documents[s]++;
            if(!Documents.ContainsKey(s))
            Documents.Add(s,1);
            if(fre.ContainsKey(s))
            {
                fre[s]++;
            }
            else
            fre.Add(s,1);
        }
        return fre;
    }

    //computar las posiciones de las palabras en el texto
    public static Dictionary<string,List<int>> Positions(List<string> x){
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
    public static int Edit_Distance(string a,string b){
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

    public static Dictionary<string,double> Calc_TfIdf(vector x,Dictionary<string,int> Documents,int n,Dictionary<string,int> Frec_total){
        Dictionary<string,double> sol = new Dictionary<string, double>();
        n++;
        for(int i=0;i<x.Words.Count();i++){
            if(!sol.ContainsKey(x.Words[i]) && x.Freq.ContainsKey(x.Words[i])){

                double tf = (double)(x.Freq[x.Words[i]])/(double)(Frec_total[x.Words[i]]);
                double X = (double)(n)/(double)(Documents[x.Words[i]]);
                double idf = (double)(Math.Log2(X));
               double point = tf * idf;
                sol.Add(x.Words[i],point);
             }
        }
        return sol;
    }

    // dot : para calcular el producto dot (sumatoria de la multiplicacion de las
    // palabras que aparezcan en ambas strings)
    public static double Dot_Product(Dictionary<string,double> a,Dictionary<string,double> b){
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
    public static string Get_Snipet(vector x, string word){
        string ans = "";

        if(word.Length <= 2 || x.Freq.ContainsKey(word) == false)
        return "";

        for(int i=0;i<Math.Min(5,x.Positions[word].Count());i++){
            string t = "...";

            int pos = x.Positions[word][i];
            for(int j = Math.Max(0,pos-50);j<Math.Min(pos+50,x.Words.Count());j++){
                t += " " + x.Words[j];
            }
            t += "...";

            ans += t;
        }


        return ans;
    }

    // metodo para tomar los documentos de mas relevancia y encontrar los snipet ademas de trabajar con los
    // comparadores
    public static void Find_Snipets(ref List<string> s, ref List<int> s2, string best,string best2, ref List<vector> data){
        for (int i = 0; i < 10; i++)
        {
            string x = methods.Get_Snipet(data[i], best);

            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }

            x = methods.Get_Snipet(data[i], best + 's');
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }
            x = methods.Get_Snipet(data[i], best + "es");
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }
            x = methods.Get_Snipet(data[i], best.Substring(0,Math.Max(0,best.Length-1)));
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }
            x = methods.Get_Snipet(data[i], best.Substring(0,Math.Max(0,best.Length-2)));
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }

            x = methods.Get_Snipet(data[i], best2);
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }
            x = methods.Get_Snipet(data[i], best2 + 's');
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }
            x = methods.Get_Snipet(data[i], best2 + "es");
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }
            x = methods.Get_Snipet(data[i], best2.Substring(0,Math.Max(0,best2.Length-1)));
            if (x.Length > 0)
            {
                s.Add(x);
                s2.Add(i);
            }
            x = methods.Get_Snipet(data[i], best2.Substring(0,Math.Max(0,best2.Length-2)));
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
    }

    public static void No(ref List<vector> BD, List<string> No){
        for(int i=0;i<BD.Count();i++){
            for(int j=0;j<No.Count();j++){
                string wd = No[j];
                if(BD[i].Freq.ContainsKey(wd))
                BD[i].Angle = 100000.0f;
            }
        }
    }


        public static void Yes(ref List<vector> BD, List<string> Yes){
        for(int i=0;i<BD.Count();i++){
            for(int j=0;j<Yes.Count();j++){
                string wd = Yes[j];
                if(!BD[i].Freq.ContainsKey(wd))
                BD[i].Angle = 100000.0f;
            }
        }
    }


    // operador de importancia
    public static void Priority(ref List<vector> BD ,List<string> Priority){
        for(int i=0;i<BD.Count();i++){
            for(int j=0;j<Priority.Count();j++){
                string wd = Priority[j];
                if(BD[i].Freq.ContainsKey(wd)){
                    BD[i].Angle/=BD[i].Freq[wd] + 1;
                }
            }
        }
    }

    public static int Cal_Dist(string L,string R,vector x){
        int sol = int.MaxValue;
        if(x.Freq.ContainsKey(L) == false || x.Freq.ContainsKey(R) == false)
        return sol;

        List<int> l = x.Positions[L];
        List<int> r = x.Positions[R];

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
