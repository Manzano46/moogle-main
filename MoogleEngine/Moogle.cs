namespace MoogleEngine;


public static class Moogle
{
    public static SearchResult Query(string query) {

        List<string> titles = methods.titles();
        List<vector> data = new List<vector>();
        Dictionary<string,int> global = new Dictionary<string, int>();

      for(int i=0;i<titles.Count();i++){
            data.Add(new vector());
            data[i].text = methods.read(titles[i]);
            data[i].title = titles[i];
            data[i].words = methods.WORDS(data[i].text);
            data[i].freq =  methods.times(data[i].words,ref global,ref data[i].MAX);
       }

        vector QUERY = new vector();
        QUERY.text = query;
        QUERY.words = methods.WORDS(query);
        int trash = 0;
        QUERY.freq = methods.times(QUERY.words,ref global, ref trash);
        

       for(int i=0;i<titles.Count();i++){
           data[i].tf_idf = methods.findtf_idf(data[i],global);
       }
       QUERY.tf_idf = methods.findtf_idf(QUERY,global);
     
        for(int i=0;i<QUERY.words.Count();i++){
            string qw = QUERY.words[i];
            int min = int.MaxValue;
            string sol = qw;
            for(int j=0;j<data.Count();j++){
                for(int k=0;k<data[j].words.Count();k++){
                    int x = methods.edit_distance(qw,data[j].words[k]);
                    if(x < min){
                        min = x;
                        sol = data[j].words[k];
                    }
                }
            }
            QUERY.words[i] = sol;
        }

        string aux = "";
        int it = 0;
        System.Console.WriteLine(query);
        for(int i=0;i<QUERY.words.Count();i++)
        {
            aux += QUERY.words[i];
            while(it < query.Length && query[it] >= 'a' && query[it] <= 'z')
            it++;
            if(it < query.Length)
            aux += query[it++];
            
            aux += ' ';
        }

        query = aux;

        SearchItem[] items = new SearchItem[3] {
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.9f),
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.5f),
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.1f),
        };

        return new SearchResult(items, query);
    }
}
