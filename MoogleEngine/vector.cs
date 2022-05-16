namespace MoogleEngine;

public class vector{
    public string text,title;
    public Dictionary<string,int> freq;
    public List<string> words;
    public vector(){
        this.text = "";
        this.title = "";
        this.freq = new Dictionary<string,int>();
        this.words = new List<string>();
    }

    public vector(string a,string b,Dictionary<string,int> c,List<string> d){
        this.text = a;
        this.title = b;
        this.freq = c;
        this.words = d;
    }
}