namespace MoogleEngine;
public static class Moogle
{
    // esta es la parte principal donde vamos a utilizar los metodos creados
    public static SearchResult Query(string query)
    {
        List<string> titles = methods.titles();
        List<vector> data = new List<vector>();//info de cada documento
        Dictionary<string, int> global = new Dictionary<string, int>();
        Dictionary<string, int> global2 = new Dictionary<string, int>();
        //construyendo nuestra BD
        for (int i = 0; i < titles.Count(); i++)
        {
            data.Add(new vector());
            data[i].text = methods.read(titles[i]);
            data[i].title = titles[i];
            data[i].words = methods.WORDS(data[i].text);
            data[i].freq = methods.times(data[i].words, ref global, ref data[i].MAX, ref global2);
        }
        vector QUERY = new vector();
        QUERY.text = query;
        QUERY.words = methods.WORDS(query);
        for (int i = 0; i < titles.Count(); i++)
        {
            data[i].tf_idf = methods.findtf_idf(data[i], global, titles.Count(), global2);
        }
        //utilizando edit distance
        for (int i = 0; i < QUERY.words.Count(); i++)
        {
            string qw = QUERY.words[i];
            int min = int.MaxValue;
            string sol = qw;
            for (int j = 0; j < data.Count(); j++)
            {
                for (int k = 0; k < data[j].words.Count(); k++)
                {
                    int x = methods.edit_distance(qw, data[j].words[k]);
                    if (x < min)
                    {
                        min = x;
                        sol = data[j].words[k];
                    }
                }
            }
            QUERY.words[i] = sol;
        }
        string aux = "";
        int it = 0;
        for (int i = 0; i < QUERY.words.Count(); i++)
        {
            aux += QUERY.words[i];
            while (it < query.Length && ((query[it] >= 'A' && query[it] <= 'Z') || (query[it] >= 'a' && query[it] <= 'z')))
                it++;
            if (it < query.Length && (query[it] == '.' || query[it] == ','))
                aux += query[it++];
            it++;
            aux += ' ';
        }
        QUERY.text = aux;
        QUERY.freq = methods.times(QUERY.words, ref global, ref QUERY.MAX, ref global2);
        QUERY.tf_idf = methods.findtf_idf(QUERY, global, titles.Count(), global2);
        string best = "";
        double p = 0f;

        for (int i = 0; i < QUERY.words.Count(); i++)
        {
            if (QUERY.tf_idf[QUERY.words[i]] > p)
            {
                p = QUERY.tf_idf[QUERY.words[i]];
                best = QUERY.words[i];
            }
        }

        for (int i = 0; i < data.Count(); i++)
        {
            double A = methods.dot(QUERY.tf_idf, QUERY.tf_idf);
            double B = methods.dot(data[i].tf_idf, data[i].tf_idf);
            double C = methods.dot(data[i].tf_idf, QUERY.tf_idf);

            data[i].angle = Math.Acos(C / (A * B));
            if (data[i].angle == double.NaN)
                data[i].angle = 10000000.0;
            
        }

        data.Sort(delegate (vector a, vector b) { if (a.angle < b.angle) return -1; else return 1; });

        List<string> s = new List<string>();
        for (int i = 0; i < titles.Count(); i++)
        {
            string x = methods.snipet(data[i].text, best);
            if (x.Length > 0)
            {
                s.Add(x);
            }
            x = methods.snipet(data[i].text, best + 's');
            if (x.Length > 0)
            {
                s.Add(x);
            }
            x = methods.snipet(data[i].text, best + "es");
            if (x.Length > 0)
            {
                s.Add(x);
            }
            x = methods.snipet(data[i].text, best.Substring(0,best.Length-1));
            if (x.Length > 0)
            {
                s.Add(x);
            }
            x = methods.snipet(data[i].text, best.Substring(0,best.Length-2));
            if (x.Length > 0)
            {
                s.Add(x);
            }
        }

        SearchItem[] items = new SearchItem[s.Count()];

        for (int i = 0; i < s.Count(); i++)
        {
            items[i] = new SearchItem(data[i].title.Substring(11), s[i], (float)(data[i].angle));
        }

        return new SearchResult(items, QUERY.text);
    }
}
