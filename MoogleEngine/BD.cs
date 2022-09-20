namespace MoogleEngine;

public static class BD{
        public static List<string> titles = methods.titles();
        public static List<vector> data = new List<vector>();//info de cada documento
        public static Dictionary<string, int> global = new Dictionary<string, int>();
        public static Dictionary<string, int> global2 = new Dictionary<string, int>();
        
        public static void fill(){
             
            for (int i = 0; i < titles.Count(); i++)
            {
                data.Add(new vector());
                data[i].text = methods.read(titles[i]);
                data[i].title = titles[i];
                data[i].words = methods.WORDS(data[i].text);
                data[i].freq = methods.times(data[i].words, ref global, ref data[i].MAX, ref global2);
                data[i].dist = methods.dist(data[i].words);
            }

            for (int i = 0; i < titles.Count(); i++)
            {
                data[i].tf_idf = methods.findtf_idf(data[i], global, titles.Count()+1, global2);
            }
        }
}