namespace MoogleEngine;

public class vector{
    public string text,title;
    public Dictionary<string,int> freq;
    public List<string> words;

    public Dictionary<string,double> tf_idf;
    public vector(){
        this.text = "";
        this.title = "";
        this.freq = new Dictionary<string,int>();
        this.words = new List<string>();
        this.tf_idf = new Dictionary<string, double>();
    }

    public vector(string a,string b,Dictionary<string,int> c,List<string> d,Dictionary<string,double> e){
        this.text = a;
        this.title = b;
        this.freq = c;
        this.words = d;
        this.tf_idf = e;
    }
}