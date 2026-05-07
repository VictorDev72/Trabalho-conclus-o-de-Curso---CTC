

public class EspecieQuimica
{
    private Dictionary<int, int> ParteA { get; set; }
    private Dictionary<int, int> ParteB { get; set; }

    private readonly int CargaA;
    private readonly int CargaB; 

    public EspecieQuimica(Dictionary<int, int> parteA, Dictionary<int, int> parteB)
    {
        if (ParteA == null || ParteB == null)
            throw new ArgumentException("Não podem ser nulos");

        ParteA = parteA;
        ParteB = parteB;
        
    }

    public Dictionary<int,int> ToDictionary()
    {
     
        var res = new Dictionary<int, int>();

        foreach (var x in ParteA)
        {
            res[x.Key] = x.Value;
        }

        foreach (var x in ParteB)
        {
            if (res.ContainsKey(x.Key))
            {
                res[x.Key] += x.Value;
            }
            else
            {
                res[x.Key] = x.Value;
            }
        }
        return res;
    }
    /*
        Dictionary<int,int> reag1, Dictionary<int,int> reag2.        return new Dictionary<int,int>(reag1,reag2);


    */
}