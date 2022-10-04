namespace MoogleEngine;
public static class Moogle
{
    // esta es la parte principal donde vamos a utilizar los metodos creados
    public static SearchResult Query(string query)
    {
        // DateTime start = DateTime.Now;
        // estructura para la querie
        vector QUERY = new vector();
        QUERY.Text = query;
        QUERY.Words = methods.Get_Words(query);

        // utilizando edit distance
        for (int i = 0; i < QUERY.Words.Count(); i++)
        {
            string qw = QUERY.Words[i];
            int min = int.MaxValue;
            string sol = qw;

            if(BD.Frec_total.ContainsKey(qw))
            continue;

            foreach (var wd in BD.Frec_total)
            {
                int x = methods.Edit_Distance(qw, wd.Key);
                if (x < min)
                {
                    min = x;
                    sol = wd.Key;
                }
            }

            QUERY.Words[i] = sol;
        }
        string aux = "";
        int it = 0;

        // estructuras para los operadores
        List<string> Priority = new List<string>();
        List<string> No = new List<string>();
        List<string> Yes = new List<string>();
        string L = "", R = "";

        // corrigiendo la querie y entrando los operadores
        for (int i = 0; i < QUERY.Words.Count(); i++)
        {
            // operador de importancia
            while(query[it] == '*'){
                Priority.Add(QUERY.Words[i]);
                it++;
            }
            // negacion
            if(query[it] == '!'){
                No.Add(QUERY.Words[i]);
                it++;
            }
            // afirmacion
            if(query[it] == '^'){
                Yes.Add(QUERY.Words[i]);
                it++;
            }

            if(query[it] == '~'){
                L = QUERY.Words[i - 1];
                R = QUERY.Words[i];
                it++;
            }

            aux += QUERY.Words[i];
            while (it < query.Length && ((query[it] >= 'A' && query[it] <= 'Z') || (query[it] >= 'a' && query[it] <= 'z')))
                it++;
            if (it < query.Length && (query[it] == '.' || query[it] == ','))
                aux += query[it++];
            it++;
            if(i < QUERY.Words.Count()-1)
            aux += ' ';
        }
        // llenando la estructura de la querie
        QUERY.Text = aux;
        QUERY.Freq = methods.Fill_Frec(QUERY.Words, ref BD.Documents, ref BD.Frec_total);
        QUERY.tf_idf = methods.Calc_TfIdf(QUERY, BD.Documents, BD.titles.Count()+1, BD.Frec_total);

        // tomando las palabras de importancia para los snippets
        string best = "";
        string best2 = "";
        double p = 0.0f;
        double p2 = 0.0f;

        for (int i = 0; i < QUERY.Words.Count(); i++)
        {

            if (QUERY.tf_idf[QUERY.Words[i]] > p)
            {
                p = QUERY.tf_idf[QUERY.Words[i]];
                best = QUERY.Words[i];
            }
        }
        for (int i = 0; i < QUERY.Words.Count(); i++)
        {
            if (QUERY.tf_idf[QUERY.Words[i]] > p2 && QUERY.Words[i] != best)
            {
                p2 = QUERY.tf_idf[QUERY.Words[i]];
                best2 = QUERY.Words[i];
            }
        }

        // llevando a cabo el producto dot entre los tf-idf de la querie y los documentos
        for (int i = 0; i < BD.data.Count(); i++)
        {
            double A = methods.Dot_Product(QUERY.tf_idf, QUERY.tf_idf);
            double B = methods.Dot_Product(BD.data[i].tf_idf, BD.data[i].tf_idf);
            double C = methods.Dot_Product(QUERY.tf_idf, BD.data[i].tf_idf);

            int dist = int.MaxValue;
            dist = methods.Cal_Dist(L,R,BD.data[i]);

            // calculando el angulo
            BD.data[i].Angle = Math.Acos(C / (A * B));
            if (BD.data[i].Angle == double.NaN)
                BD.data[i].Angle = 10000000.0;
            if(dist != int.MaxValue)
            BD.data[i].Angle *= 1/dist;
        }

        methods.Priority(ref BD.data,Priority);
        if(Yes.Count() > 0)
        methods.Yes(ref BD.data,Yes);
        if(No.Count() > 0)
        methods.No(ref BD.data,No);

        // ordenando los documentos segun el angulo
        BD.data.Sort(delegate (vector a, vector b) { if (a.Angle < b.Angle) return -1;  return 1; });

        // creando estructura para los snippets
        List<string> s = new List<string>();
        List<int> ind = new List<int>();
        methods.Find_Snipets(ref s,ref ind,best,best2, ref BD.data);
        SearchItem[] items = new SearchItem[Math.Max(1,s.Count())];

        if(s.Count == 0)
        items[0] = new SearchItem("Lo siento","No hay resultados para su busqueda",666f);
            else
        for (int i = 0; i < s.Count(); i++)
            items[i] = new SearchItem(BD.data[ind[i]].Title.Substring(11),s[i], (float)(10.0f - BD.data[ind[i]].Angle));

     // DateTime end = DateTime.Now;
     // System.Console.WriteLine(end-start);
        if(QUERY.Text == query)
        QUERY.Text = "";
        return new SearchResult(items,QUERY.Text);
    }
}