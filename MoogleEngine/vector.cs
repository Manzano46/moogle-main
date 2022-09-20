namespace MoogleEngine;

public class vector{
    public string text,title;
    public Dictionary<string,int> freq;
    public List<string> words;
    public double angle;

    public Dictionary<string,List<int>> dist;
    public int MAX;
    public Dictionary<string,double> tf_idf;
    public vector(){
        this.text = "";
        this.title = "";
        this.freq = new Dictionary<string,int>();
        this.words = new List<string>();
        this.tf_idf = new Dictionary<string, double>();
        this.MAX = 0;
        this.angle = 0.0;
        this.dist = new Dictionary<string, List<int>>();
    }

    public vector(string a,string b,Dictionary<string,int> c,List<string> d,Dictionary<string,double> e,int f,double g,Dictionary<string,List<int>> h){
        this.text = a;
        this.title = b;
        this.freq = c;
        this.words = d;
        this.tf_idf = e;
        this.MAX = f;
        this.angle = g;
        this.dist = h;
    }
}