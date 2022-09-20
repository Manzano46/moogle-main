namespace MoogleEngine;
public static class Moogle
{
    // esta es la parte principal donde vamos a utilizar los metodos creados
    public static SearchResult Query(string query)
    {
        // DateTime start = DateTime.Now;
        // estructura para la querie 
        vector QUERY = new vector();
        QUERY.text = query;
        QUERY.words = methods.WORDS(query);

        // utilizando edit distance 
        for (int i = 0; i < QUERY.words.Count(); i++)
        {
            string qw = QUERY.words[i];
            int min = int.MaxValue;
            string sol = qw;

                foreach(var wd in BD.global){    
                    int x = methods.edit_distance(qw, wd.Key);
                    if (x < min)
                    {
                        min = x;
                        sol = wd.Key;
                    }
                }
            
            QUERY.words[i] = sol;
        }
        string aux = "";
        int it = 0;
        
        // estructuras para los operadores 
        List<string> imp = new List<string>();
        List<string> no = new List<string>(); 
        List<string> si = new List<string>();
        string L = "", R = "";

        // corrigiendo la querie y entrando los operadores 
        for (int i = 0; i < QUERY.words.Count(); i++)
        {
            // operador de importancia 
            while(query[it] == '*'){
                imp.Add(QUERY.words[i]);
                it++;
            }
            // negacion
            if(query[it] == '!'){
                no.Add(QUERY.words[i]);
                it++;
            }
            // afirmacion
            if(query[it] == '^'){
                si.Add(QUERY.words[i]);
                it++;
            }

            if(query[it] == '~'){
                L = QUERY.words[i - 1];
                R = QUERY.words[i];
                it++;
            }

            aux += QUERY.words[i];
            while (it < query.Length && ((query[it] >= 'A' && query[it] <= 'Z') || (query[it] >= 'a' && query[it] <= 'z')))
                it++;
            if (it < query.Length && (query[it] == '.' || query[it] == ','))
                aux += query[it++];
            it++;
            if(i < QUERY.words.Count()-1)
            aux += ' ';
        }        
        // llenando la estructura de la querie
        QUERY.text = aux;
        QUERY.freq = methods.times(QUERY.words, ref BD.global, ref QUERY.MAX, ref BD.global2);
        QUERY.tf_idf = methods.findtf_idf(QUERY, BD.global, BD.titles.Count()+1, BD.global2);
       
        methods.sum(ref QUERY.tf_idf,imp);
      
        // tomando las palabras de importancia para los snippets
        string best = "";
        string best2 = "";
        double p = 0.0f;
        double p2 = 0.0f;

        for (int i = 0; i < QUERY.words.Count(); i++)
        {
            
            if (QUERY.tf_idf[QUERY.words[i]] > p)
            {
                p = QUERY.tf_idf[QUERY.words[i]];
                best = QUERY.words[i];
            }
        }
        for (int i = 0; i < QUERY.words.Count(); i++)
        {
            if (QUERY.tf_idf[QUERY.words[i]] > p2 && QUERY.words[i] != best)
            {
                p2 = QUERY.tf_idf[QUERY.words[i]];
                best2 = QUERY.words[i];
            }
        }
      
        // llevando a cabo el producto dot entre los tf-idf de la querie y los documentos 
        for (int i = 0; i < BD.data.Count(); i++)
        {
            double A = methods.dot(QUERY.tf_idf, QUERY.tf_idf);
            double B = methods.dot(BD.data[i].tf_idf, BD.data[i].tf_idf);
            double C = methods.dot(QUERY.tf_idf, BD.data[i].tf_idf);

            int dist = int.MaxValue;
            dist = methods.caldist(L,R,BD.data[i]);

            // calculando el angulo
            BD.data[i].angle = Math.Acos(C / (A * B));
            if (BD.data[i].angle == double.NaN)
                BD.data[i].angle = 10000000.0;
            if(dist != int.MaxValue)
            BD.data[i].angle *= 1/dist;          
        }

        // ordenando los documentos segun el angulo
        BD.data.Sort(delegate (vector a, vector b) { if (a.angle < b.angle) return -1; else return 1; });

        // creando estructura para los snippets
        List<string> s = new List<string>();
        List<int> ind = new List<int>();
        methods.findsnipet(ref s,ref ind,best,best2, ref BD.data,si,no);
        SearchItem[] items = new SearchItem[Math.Max(1,s.Count())];

        if(s.Count == 0)
        items[0] = new SearchItem("Lo siento","No hay resultados para su busqueda",666f);
        else
        for (int i = 0; i < s.Count(); i++)
            items[i] = new SearchItem(BD.data[ind[i]].title.Substring(11),s[i], (float)(10.0f - BD.data[ind[i]].angle));

     // DateTime end = DateTime.Now;
     // System.Console.WriteLine(end-start);

        return new SearchResult(items,QUERY.text);
    }
}