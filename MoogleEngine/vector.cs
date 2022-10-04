namespace MoogleEngine;

public class vector{
    public string Text,Title;
    public Dictionary<string,int> Freq;
    public List<string> Words;
    public double Angle;

    public Dictionary<string,List<int>> Positions;
    public Dictionary<string,double> tf_idf;
    public vector(){
        this.Text = "";
        this.Title = "";
        this.Freq = new Dictionary<string,int>();
        this.Words = new List<string>();
        this.tf_idf = new Dictionary<string, double>();
        this.Angle = 0.0;
        this.Positions = new Dictionary<string, List<int>>();
    }

    public vector(string a,string b,Dictionary<string,int> c,List<string> d,Dictionary<string,double> e,double g,Dictionary<string,List<int>> h){
        this.Text = a;
        this.Title = b;
        this.Freq = c;
        this.Words = d;
        this.tf_idf = e;
        this.Angle = g;
        this.Positions = h;
    }
}