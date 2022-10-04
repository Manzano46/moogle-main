namespace MoogleEngine;

public static class BD{
        public static List<string> titles = methods.Read_Titles();
        public static List<vector> data = new List<vector>();//info de cada documento
        public static Dictionary<string, int> Documents = new Dictionary<string, int>();
        public static Dictionary<string, int> Frec_total = new Dictionary<string, int>();

        public static void fill(){

            for (int i = 0; i < titles.Count(); i++)
            {
                data.Add(new vector());
                data[i].Text = methods.Read_Text(titles[i]);
                data[i].Title = titles[i];
                data[i].Words = methods.Get_Words(data[i].Text);
                data[i].Freq = methods.Fill_Frec(data[i].Words, ref Documents, ref Frec_total);
                data[i].Positions = methods.Positions(data[i].Words);
            }

            for (int i = 0; i < titles.Count(); i++)
            {
                data[i].tf_idf = methods.Calc_TfIdf(data[i], Documents, titles.Count()+1, Frec_total);
            }
        }
}